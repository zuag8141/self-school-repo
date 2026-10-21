using System.ComponentModel.DataAnnotations;

namespace zSubscription.RazorWebApp.QuangTT.Models;

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
