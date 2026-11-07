using HelpDesk.Web.Models;

namespace HelpDesk.Web.Helpers;

/// <summary>Human readable labels and Bootstrap badge classes for enums and roles.</summary>
public static class DisplayExtensions
{
    public static string Label(this TicketStatus status) => status switch
    {
        TicketStatus.InProgress => "In Progress",
        _ => status.ToString()
    };

    public static string BadgeClass(this TicketStatus status) => status switch
    {
        TicketStatus.New => "text-bg-primary",
        TicketStatus.InProgress => "text-bg-warning",
        TicketStatus.Resolved => "text-bg-success",
        TicketStatus.Closed => "text-bg-secondary",
        _ => "text-bg-light"
    };

    public static string Label(this TicketPriority priority) => priority.ToString();

    public static string BadgeClass(this TicketPriority priority) => priority switch
    {
        TicketPriority.Low => "text-bg-info",
        TicketPriority.Medium => "text-bg-primary",
        TicketPriority.High => "text-bg-warning",
        TicketPriority.Critical => "text-bg-danger",
        _ => "text-bg-light"
    };

    public static string RoleLabel(this string role) => role switch
    {
        Roles.ITSpecialist => "IT Specialist",
        _ => role
    };
}
