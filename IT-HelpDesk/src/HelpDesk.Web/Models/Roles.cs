namespace HelpDesk.Web.Models;

/// <summary>Role names stored in Users.Role and used in [Authorize(Roles = ...)].</summary>
public static class Roles
{
    public const string Student = "Student";
    public const string Employee = "Employee";
    public const string ITSpecialist = "ITSpecialist";
}
