using HelpDesk.Web.Data;
using HelpDesk.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Every POST/PUT/DELETE must carry a valid anti-forgery token (CSRF protection).
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    // Non-nullable properties of view models are not treated as [Required] automatically.
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication (cookie), authorization, password hashing -> Security Developer
builder.Services.AddHelpDeskSecurity();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSecurityHeaders(includeCsp: !app.Environment.IsDevelopment());
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
