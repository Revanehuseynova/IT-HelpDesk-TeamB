using HelpDesk.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDesk.Web.ViewModels;

public class TicketListViewModel
{
    public IReadOnlyList<Ticket> Tickets { get; set; } = new List<Ticket>();
    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();

    public int? CategoryId { get; set; }
    public TicketStatus? Status { get; set; }
    public string? Search { get; set; }

    public bool SeesAllTickets { get; set; }
    public bool HasFilter => CategoryId.HasValue || Status.HasValue || !string.IsNullOrWhiteSpace(Search);
}
