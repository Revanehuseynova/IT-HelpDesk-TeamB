using System.Security.Claims;
using HelpDesk.Web.Data;
using HelpDesk.Web.Models;
using HelpDesk.Web.Security;
using HelpDesk.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUser = HelpDesk.Web.Models.User;

namespace HelpDesk.Web.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPasswordService _passwords;

    public AccountController(AppDbContext db, IPasswordService passwords)
    {
        _db = db;
        _passwords = passwords;
    }

    // ------------------------------------------------------------------ Login
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

        // Same generic message for "unknown e-mail" and "wrong password" (no user enumeration).
        if (user == null || !_passwords.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "E-poçt və ya parol yanlışdır.");
            return View(model);
        }

        await SignInAsync(user, model.RememberMe);

        // Only redirect to local URLs (prevents open-redirect attacks).
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    // --------------------------------------------------------------- Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "Bu e-poçt artıq qeydiyyatdan keçib.");
            return View(model);
        }

        var user = new AppUser
        {
            FullName = model.FullName.Trim(),
            Email = email,
            PasswordHash = _passwords.Hash(model.Password),
            Role = model.Role, // validated by regex: Student | Employee only
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // e.g. unique index violation caused by two simultaneous registrations
            ModelState.AddModelError(nameof(model.Email), "Bu e-poçt artıq qeydiyyatdan keçib.");
            return View(model);
        }

        TempData["Success"] = "Qeydiyyat tamamlandı. İndi daxil ola bilərsiniz.";
        return RedirectToAction(nameof(Login));
    }

    // ----------------------------------------------------------------- Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    // ---------------------------------------------------------------- helpers
    private async Task SignInAsync(AppUser user, bool persistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = persistent });
    }
}
