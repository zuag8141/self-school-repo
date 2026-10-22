using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

[Authorize]
public class CreateModel : MembershipFeeFormPageModel
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;
    private readonly IFeePackageQuangTtService _feePackageService;

    public CreateModel(
        IMembershipFeeQuangTtService membershipFeeService,
        IFeePackageQuangTtService feePackageService)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
    }

    public async Task OnGetAsync()
    {
        MembershipFee = new MembershipFeeQuangTt
        {
            PaymentDate = DateTime.Today,
            PublishDate = DateTime.Today,
            IsActive = true
        };

        await LoadPackagesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync();
            return Page();
        }

        await _membershipFeeService.CreateAsync(MembershipFee);
        TempData["Success"] = $"Đã tạo khoản phí cho \"{MembershipFee.MemberName}\".";
        return RedirectToPage("./Index");
    }

    private async Task LoadPackagesAsync()
    {
        var packages = await _feePackageService.GetAllAsync();
        ViewData["FeePackageQuangTtid"] = new SelectList(
            packages,
            nameof(FeePackageQuangTt.FeePackageQuangTtid),
            nameof(FeePackageQuangTt.PackageName),
            MembershipFee.FeePackageQuangTtid);
    }

    private void RemoveNavigationErrors()
    {
        var keys = ModelState.Keys
            .Where(k => k.Contains("FeePackageQuangTt", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keys)
        {
            ModelState.Remove(key);
        }
    }
}
