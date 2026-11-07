using HelpDesk.Web.Data;
using HelpDesk.Web.Helpers;
using HelpDesk.Web.Models;
using HelpDesk.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Web.Controllers;

[Authorize]
public class TicketController : Controller
{
    private readonly AppDbContext _db;

    public TicketController(AppDbContext db) => _db = db;

    private int CurrentUserId => User.GetUserId();
    private bool IsItSpecialist => User.IsInRole(Roles.ITSpecialist);

    /// <summary>Tickets the current user is allowed to see (all for IT, own for others).</summary>
    private IQueryable<Ticket> VisibleTickets()
    {
        var query = _db.Tickets.AsQueryable();
        if (!IsItSpecialist)
        {
            var userId = CurrentUserId;
            query = query.Where(t => t.CreatedById == userId);
        }
        return query;
    }

    // GET /Ticket?categoryId=1&status=InProgress&search=wifi
    public async Task<IActionResult> Index(int? categoryId, TicketStatus? status, string? search)
    {
        var query = VisibleTickets().AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t => t.Title.Contains(term));
        }

        var vm = new TicketListViewModel
        {
            Tickets = await query.OrderByDescending(t => t.CreatedAt).ToListAsync(),
            Categories = await CategoryItemsAsync(categoryId),
            Statuses = StatusItems(status),
            CategoryId = categoryId,
            Status = status,
            Search = search,
            SeesAllTickets = IsItSpecialist
        };
        return View(vm);
    }

    // GET /Ticket/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var ticket = await VisibleTickets().AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id);

        // 404 for missing AND for foreign tickets, so ticket ids cannot be probed.
        if (ticket == null) return NotFound();

        return View(new TicketDetailsViewModel
        {
            Ticket = ticket,
            StatusItems = StatusItems(ticket.Status),
            CanManage = IsItSpecialist
        });
    }

    // GET /Ticket/Create
    public async Task<IActionResult> Create()
    {
        var vm = new TicketCreateViewModel();
        await FillListsAsync(vm);
        return View(vm);
    }

    // POST /Ticket/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TicketCreateViewModel model)
    {
        if (model.CategoryId.HasValue &&
            !await _db.Categories.AnyAsync(c => c.Id == model.CategoryId.Value))
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Yanlış kateqoriya seçilib.");
        }

        if (!Enum.IsDefined(typeof(TicketPriority), model.Priority))
        {
            ModelState.AddModelError(nameof(model.Priority), "Yanlış prioritet seçilib.");
        }

        if (!ModelState.IsValid)
        {
            await FillListsAsync(model);
            return View(model);
        }

        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            CategoryId = model.CategoryId!.Value,
            Priority = model.Priority,
            Status = TicketStatus.New,
            CreatedById = CurrentUserId,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Müraciət uğurla yaradıldı.";
        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    // POST /Ticket/Accept/5  -> IT specialist takes the ticket (New -> In Progress)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.ITSpecialist)]
    public async Task<IActionResult> Accept(int id)
    {
        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        if (ticket.Status != TicketStatus.New)
        {
            TempData["Error"] = "Yalnız 'New' statuslu müraciət qəbul edilə bilər.";
            return RedirectToAction(nameof(Details), new { id });
        }

        ticket.Status = TicketStatus.InProgress;
        ticket.AssignedToId = CurrentUserId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Müraciət qəbul edildi və sizə təyin olundu.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /Ticket/ChangeStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.ITSpecialist)]
    public async Task<IActionResult> ChangeStatus(int id, TicketStatus status)
    {
        if (!Enum.IsDefined(typeof(TicketStatus), status))
        {
            TempData["Error"] = "Yanlış status.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        ticket.Status = status;
        if (status != TicketStatus.New && ticket.AssignedToId == null)
        {
            ticket.AssignedToId = CurrentUserId;
        }
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Status '{status.Label()}' olaraq dəyişdirildi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ----------------------------------------------------------------- helpers
    private async Task<List<SelectListItem>> CategoryItemsAsync(int? selected = null)
    {
        var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        return categories
            .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == selected))
            .ToList();
    }

    private static List<SelectListItem> StatusItems(TicketStatus? selected = null) =>
        Enum.GetValues<TicketStatus>()
            .Select(s => new SelectListItem(s.Label(), ((int)s).ToString(), s == selected))
            .ToList();

    private static List<SelectListItem> PriorityItems() =>
        Enum.GetValues<TicketPriority>()
            .Select(p => new SelectListItem(p.Label(), ((int)p).ToString()))
            .ToList();

    private async Task FillListsAsync(TicketCreateViewModel vm)
    {
        vm.Categories = await CategoryItemsAsync(vm.CategoryId);
        vm.Priorities = PriorityItems();
    }
}
