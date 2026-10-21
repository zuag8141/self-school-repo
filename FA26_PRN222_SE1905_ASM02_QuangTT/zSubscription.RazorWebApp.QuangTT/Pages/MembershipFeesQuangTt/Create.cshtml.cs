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
public class CreateModel : PageModel
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;
    private readonly IFeePackageQuangTtService _feePackageService;
    private readonly IHubContext<MembershipFeeHub> _hubContext;

    public CreateModel(
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
        ModelState.Remove("MembershipFee.FeePackageQuangTt");
        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync();
            return Page();
        }

        await _membershipFeeService.CreateAsync(MembershipFee);
        await _hubContext.Clients.All.SendAsync("MembershipFeeChanged", new
        {
            Action = "created",
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
