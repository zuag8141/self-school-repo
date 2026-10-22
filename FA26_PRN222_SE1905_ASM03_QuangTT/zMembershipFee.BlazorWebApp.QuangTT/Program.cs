using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using zMembershipFee.BlazorWebApp.QuangTT.Components;
using zMembershipFee.Repositories.QuangTT;
using zMembershipFee.Repositories.QuangTT.DBContext;
using zMembershipFee.Services.QuangTT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "zMembershipFee.Blazor.Session";
        options.LoginPath = "/login";
        options.LogoutPath = "/account/logout";
        options.AccessDeniedPath = "/login";
        options.ReturnUrlParameter = "returnUrl";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

builder.Services.AddDbContext<PRN222Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MembershipFeeQuangTtRepository>();
builder.Services.AddScoped<FeePackageQuangTtRepository>();
builder.Services.AddScoped<SystemUserAccountRepository>();
builder.Services.AddScoped<IMembershipFeeQuangTtService, MembershipFeeQuangTtService>();
builder.Services.AddScoped<IFeePackageQuangTtService, FeePackageQuangTtService>();
builder.Services.AddScoped<ISystemUserAccountService, SystemUserAccountService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/account/login", async (
    HttpContext context,
    ISystemUserAccountService userAccountService,
    IAntiforgery antiforgery) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(context);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.Redirect("/login?error=session");
    }

    var form = await context.Request.ReadFormAsync();
    var userName = form["userName"].ToString().Trim();
    var password = form["password"].ToString();
    var returnUrl = form["returnUrl"].ToString();

    if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect(BuildLoginErrorUrl("missing", returnUrl));
    }

    var account = await userAccountService.GetUserAccountAsync(userName, password);
    if (account is null)
    {
        return Results.Redirect(BuildLoginErrorUrl("invalid", returnUrl));
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, account.UserAccountId.ToString()),
        new(ClaimTypes.Name, account.FullName),
        new(ClaimTypes.Email, account.Email),
        new(ClaimTypes.Role, account.RoleId.ToString())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = false });

    return Results.Redirect(IsLocalUrl(returnUrl) ? returnUrl : "/membership-fees");
}).DisableAntiforgery();

app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static string BuildLoginErrorUrl(string error, string? returnUrl)
{
    var result = "/login?error=" + Uri.EscapeDataString(error);
    if (!string.IsNullOrWhiteSpace(returnUrl))
    {
        result += "&returnUrl=" + Uri.EscapeDataString(returnUrl);
    }

    return result;
}

static bool IsLocalUrl(string? returnUrl)
    => !string.IsNullOrWhiteSpace(returnUrl)
       && returnUrl.StartsWith('/')
       && !returnUrl.StartsWith("//")
       && !returnUrl.StartsWith("/\\");
