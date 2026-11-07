namespace HelpDesk.Web.Models;

/// <summary>Stored as INT in SQL Server. Keep the numbers in sync with database.sql.</summary>
public enum TicketStatus
{
    New = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3
}
