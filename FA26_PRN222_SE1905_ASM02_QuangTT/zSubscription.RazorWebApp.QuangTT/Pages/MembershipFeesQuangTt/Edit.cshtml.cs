using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.RazorWebApp.QuangTT.Hubs;
using zSubscription.Services.QuangTT;

namespace zSubscription.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

[Authorize]
public class EditModel : PageModel
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;
    private readonly IFeePackageQuangTtService _feePackageService;
    private readonly IHubContext<MembershipFeeHub> _hubContext;

    public EditModel(
        IMembershipFeeQuangTtService membershipFeeService,
        IFeePackageQuangTtService feePackageService,
        IHubContext<MembershipFeeHub> hubContext)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
        _hubContext = hubContext;
    }

    [BindProperty]
    public MembershipFeeQuangTt MembershipFee { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var item = await _membershipFeeService.GetByIdAsync(id.Value);
        if (item == null)
        {
            return NotFound();
        }

        MembershipFee = item;
        await LoadPackagesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("MembershipFee.FeePackageQuangTt");
        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync();
            return Page();
        }

        await _membershipFeeService.UpdateAsync(MembershipFee);
        await _hubContext.Clients.All.SendAsync("MembershipFeeChanged", new
        {
            Action = "updated",
            MemberName = MembershipFee.MemberName,
            ChangedAt = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
        });

        return RedirectToPage("./Index");
    }

    private async Task LoadPackagesAsync()
    {
        var packages = await _feePackageService.GetAllAsync();
        ViewData["FeePackageQuangTtid"] = new SelectList(
            packages,
            "FeePackageQuangTtid",
            "PackageName",
            MembershipFee.FeePackageQuangTtid);
    }
}
