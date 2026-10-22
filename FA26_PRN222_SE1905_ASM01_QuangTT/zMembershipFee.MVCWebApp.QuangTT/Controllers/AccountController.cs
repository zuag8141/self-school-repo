using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zMembershipFee.MVCWebApp.QuangTT.Models;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.MVCWebApp.QuangTT.Controllers;

public class AccountController : Controller
{
    private readonly ISystemUserAccountService _userAccountService;

    public AccountController(ISystemUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "MembershipFees");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var account = await _userAccountService.GetUserAccountAsync(model.UserName, model.Password);
        if (account is null)
        {
            ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.UserAccountId.ToString()),
            new(ClaimTypes.Name, account.FullName),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.Role, account.RoleId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "MembershipFees");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
