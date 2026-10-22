using System.ComponentModel.DataAnnotations;

namespace zMembershipFee.RazorWebApp.QuangTT.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Email or username")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
