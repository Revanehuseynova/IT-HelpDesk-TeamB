using System.Security.Claims;

namespace HelpDesk.Web.Helpers;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Returns the database Id of the signed-in user (stored in the NameIdentifier claim).</summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : 0;
    }
}
