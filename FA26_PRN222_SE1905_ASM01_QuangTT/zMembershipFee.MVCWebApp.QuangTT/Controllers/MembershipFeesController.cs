using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.MVCWebApp.QuangTT.Controllers;

[Authorize]
public class MembershipFeesController : Controller
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;
    private readonly IFeePackageQuangTtService _feePackageService;

    public MembershipFeesController(
        IMembershipFeeQuangTtService membershipFeeService,
        IFeePackageQuangTtService feePackageService)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
    }

    // GET: /MembershipFees?memberName=...&paymentMethod=...&packageName=...
    public async Task<IActionResult> Index(string? memberName, string? paymentMethod, string? packageName)
    {
        ViewData["MemberName"] = memberName;
        ViewData["PaymentMethod"] = paymentMethod;
        ViewData["PackageName"] = packageName;

        var items = await _membershipFeeService.SearchAsync(memberName, paymentMethod, packageName);
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        return item is null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        await LoadPackagesAsync();
        return View(new MembershipFeeQuangTt
        {
            PaymentDate = DateTime.Today,
            PublishDate = DateTime.Today,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MembershipFeeQuangTt membershipFee)
    {
        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync(membershipFee.FeePackageQuangTtid);
            return View(membershipFee);
        }

        await _membershipFeeService.CreateAsync(membershipFee);
        TempData["Success"] = $"Đã tạo khoản phí cho \"{membershipFee.MemberName}\".";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        await LoadPackagesAsync(item.FeePackageQuangTtid);
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MembershipFeeQuangTt membershipFee)
    {
        if (id != membershipFee.MembershipFeeQuangTtid)
        {
            return BadRequest();
        }

        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync(membershipFee.FeePackageQuangTtid);
            return View(membershipFee);
        }

        var updated = await _membershipFeeService.UpdateAsync(membershipFee);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Đã cập nhật khoản phí #{membershipFee.MembershipFeeQuangTtid}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _membershipFeeService.DeleteAsync(id);
        TempData[deleted ? "Success" : "Error"] = deleted
            ? $"Đã xoá khoản phí #{id}."
            : $"Không tìm thấy khoản phí #{id}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadPackagesAsync(int? selectedId = null)
    {
        var packages = await _feePackageService.GetAllAsync();
        ViewBag.FeePackages = new SelectList(
            packages,
            nameof(FeePackageQuangTt.FeePackageQuangTtid),
            nameof(FeePackageQuangTt.PackageName),
            selectedId);
    }

    /// <summary>
    /// Bỏ qua validate của navigation property vì form chỉ post khoá ngoại.
    /// </summary>
    private void RemoveNavigationErrors()
    {
        var keys = ModelState.Keys
            .Where(k => k.StartsWith(nameof(MembershipFeeQuangTt.FeePackageQuangTt), StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keys)
        {
            ModelState.Remove(key);
        }
    }
}
