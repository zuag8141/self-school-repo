using System.ComponentModel.DataAnnotations;

namespace zMembershipFee.Entities.QuangTT.Models;

public partial class MembershipFeeQuangTt
{
    public int MembershipFeeQuangTtid { get; set; }

    [Required(ErrorMessage = "Member name is required.")]
    [StringLength(250)]
    [Display(Name = "Member name")]
    public string MemberName { get; set; } = string.Empty;

    [Range(0.01, 999999999, ErrorMessage = "Amount must be greater than 0.")]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Required]
    [Display(Name = "Payment date")]
    public DateTime PaymentDate { get; set; } = DateTime.Today;

    [StringLength(100)]
    [Display(Name = "Payment method")]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    [Display(Name = "Transaction code")]
    public string? TransactionCode { get; set; }

    [Display(Name = "Start date")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End date")]
    public DateTime? EndDate { get; set; }

    public string? Notes { get; set; }

    public int? Status { get; set; }

    [Display(Name = "Publish date")]
    public DateTime? PublishDate { get; set; } = DateTime.Today;

    [Display(Name = "Updated at")]
    public DateTime? UpdateAt { get; set; }

    [Display(Name = "Fee package")]
    public int? FeePackageQuangTtid { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public virtual FeePackageQuangTt? FeePackageQuangTt { get; set; }
}
