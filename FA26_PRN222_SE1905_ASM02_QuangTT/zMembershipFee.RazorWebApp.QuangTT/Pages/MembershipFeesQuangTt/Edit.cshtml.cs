using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

[Authorize]
public class EditModel : MembershipFeeFormPageModel
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;
    private readonly IFeePackageQuangTtService _feePackageService;

    public EditModel(
        IMembershipFeeQuangTtService membershipFeeService,
        IFeePackageQuangTtService feePackageService)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var item = await _membershipFeeService.GetByIdAsync(id.Value);
        if (item is null)
        {
            return NotFound();
        }

        MembershipFee = item;
        await LoadPackagesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync();
            return Page();
        }

        var updated = await _membershipFeeService.UpdateAsync(MembershipFee);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Đã cập nhật khoản phí #{MembershipFee.MembershipFeeQuangTtid}.";
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
