using HelpDesk.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDesk.Web.ViewModels;

public class TicketDetailsViewModel
{
    public Ticket Ticket { get; set; } = null!;
    public IEnumerable<SelectListItem> StatusItems { get; set; } = new List<SelectListItem>();
    public bool CanManage { get; set; }
}
