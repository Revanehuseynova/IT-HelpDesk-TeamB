namespace HelpDesk.Web.Models;

/// <summary>Stored as INT in SQL Server. Keep the numbers in sync with database.sql.</summary>
public enum TicketPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}
