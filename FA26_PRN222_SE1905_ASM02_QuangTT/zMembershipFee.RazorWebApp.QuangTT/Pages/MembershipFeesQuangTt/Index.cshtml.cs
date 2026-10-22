using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

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
