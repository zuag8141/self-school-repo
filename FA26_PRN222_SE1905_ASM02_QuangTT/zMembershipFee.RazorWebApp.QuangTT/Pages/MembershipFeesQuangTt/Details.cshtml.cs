using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;

    public DetailsModel(IMembershipFeeQuangTtService membershipFeeService)
    {
        _membershipFeeService = membershipFeeService;
    }

    public MembershipFeeQuangTt MembershipFee { get; set; } = new();

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
        return Page();
    }
}
