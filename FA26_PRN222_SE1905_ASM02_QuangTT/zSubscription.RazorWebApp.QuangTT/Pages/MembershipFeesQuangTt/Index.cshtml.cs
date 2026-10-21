using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Services.QuangTT;

namespace zSubscription.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;

    public IndexModel(IMembershipFeeQuangTtService membershipFeeService)
    {
        _membershipFeeService = membershipFeeService;
    }

    public IList<MembershipFeeQuangTt> MembershipFees { get; set; } = new List<MembershipFeeQuangTt>();

    [BindProperty(SupportsGet = true)]
    public string? MemberName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PaymentMethod { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PackageName { get; set; }

    public async Task OnGetAsync()
    {
        MembershipFees = await _membershipFeeService.SearchAsync(MemberName, PaymentMethod, PackageName);
    }
}
