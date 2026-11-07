using HelpDesk.Web.Models;

namespace HelpDesk.Web.ViewModels;

public class HomeViewModel
{
    public bool IsAuthenticated { get; set; }
    public bool SeesAllTickets { get; set; }
    public int Total { get; set; }
    public Dictionary<TicketStatus, int> ByStatus { get; set; } = new();
}
