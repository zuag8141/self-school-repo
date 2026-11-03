using System.ComponentModel.DataAnnotations;

namespace zMembershipFee.Entities.QuangTT.Models;

public partial class SystemUserAccount
{
    public int UserAccountId { get; set; }

    [Required, StringLength(100)]
    [RegularExpression(@"^[A-Za-z0-9._-]+$", ErrorMessage = "Username can contain only letters, numbers, dots, underscores, and hyphens.")]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿĀ-ſƠơƯưẠ-ỹ0-9\u0300-\u036f]+(?: +[A-Za-zÀ-ÖØ-öø-ÿĀ-ſƠơƯưẠ-ỹ0-9\u0300-\u036f]+)*$", ErrorMessage = "Full name can contain only letters, numbers, and spaces.")]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(150), EmailAddress(ErrorMessage = "Email must be in a valid format, for example name@example.com.")]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(11)]
    [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Phone must start with 0 and contain 10 to 11 digits only.")]
    public string Phone { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "Employee code can contain only letters, numbers, and hyphens.")]
    public string EmployeeCode { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public string? RequestCode { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ApplicationCode { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool IsActive { get; set; }
}
