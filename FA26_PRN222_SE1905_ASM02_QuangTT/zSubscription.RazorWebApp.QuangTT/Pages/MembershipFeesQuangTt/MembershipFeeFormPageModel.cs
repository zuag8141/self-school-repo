using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

/// <summary>
/// Base cho Create/Edit để dùng chung partial _FormFields.
/// </summary>
public abstract class MembershipFeeFormPageModel : PageModel
{
    [BindProperty]
    public MembershipFeeQuangTt MembershipFee { get; set; } = new();
}
