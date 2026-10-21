using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.RazorWebApp.QuangTT.Hubs;
using zSubscription.Services.QuangTT;

namespace zSubscription.RazorWebApp.QuangTT.Pages.MembershipFeesQuangTt;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;
    private readonly IHubContext<MembershipFeeHub> _hubContext;

    public DeleteModel(
        IMembershipFeeQuangTtService membershipFeeService,
        IHubContext<MembershipFeeHub> hubContext)
    {
        _membershipFeeService = membershipFeeService;
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
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var item = await _membershipFeeService.GetByIdAsync(id.Value);
        if (item != null)
        {
            var memberName = item.MemberName;
            await _membershipFeeService.DeleteAsync(id.Value);
            await _hubContext.Clients.All.SendAsync("MembershipFeeChanged", new
            {
                Action = "deleted",
                MemberName = memberName,
                ChangedAt = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
            });
        }

        return RedirectToPage("./Index");
    }
}
