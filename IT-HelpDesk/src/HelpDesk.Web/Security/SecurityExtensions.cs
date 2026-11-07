using Microsoft.AspNetCore.Authentication.Cookies;

namespace HelpDesk.Web.Security;

public static class SecurityExtensions
{
    /// <summary>Cookie authentication, authorization and password hashing.</summary>
    public static IServiceCollection AddHelpDeskSecurity(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordService, PasswordService>();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.Name = "HelpDesk.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

        services.AddAuthorization();
        return services;
    }

    /// <summary>Adds basic security headers. CSP is enabled outside Development only.</summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, bool includeCsp)
    {
        return app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            if (includeCsp)
            {
                headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "script-src 'self' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                    "style-src 'self' https://cdn.jsdelivr.net; " +
                    "img-src 'self' data:; " +
                    "font-src 'self'; " +
                    "frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
            }

            await next();
        });
    }
}
