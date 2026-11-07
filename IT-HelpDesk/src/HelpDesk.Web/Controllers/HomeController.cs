using HelpDesk.Web.Data;
using HelpDesk.Web.Helpers;
using HelpDesk.Web.Models;
using HelpDesk.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            IsAuthenticated = User.Identity?.IsAuthenticated == true
        };

        // Statistics: IT specialists see all tickets, other users only their own.
        if (vm.IsAuthenticated)
        {
            vm.SeesAllTickets = User.IsInRole(Roles.ITSpecialist);
            var query = _db.Tickets.AsNoTracking().AsQueryable();
            if (!vm.SeesAllTickets)
            {
                var userId = User.GetUserId();
                query = query.Where(t => t.CreatedById == userId);
            }

            vm.Total = await query.CountAsync();
            var groups = await query
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var status in Enum.GetValues<TicketStatus>())
            {
                vm.ByStatus[status] = groups.FirstOrDefault(g => g.Status == status)?.Count ?? 0;
            }
        }

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
