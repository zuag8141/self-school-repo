using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace zSubscription.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

[Authorize]
public class SignalRModel : PageModel
{
    public void OnGet()
    {
    }
}
