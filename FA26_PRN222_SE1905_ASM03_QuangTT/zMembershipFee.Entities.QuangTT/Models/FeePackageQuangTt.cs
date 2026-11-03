using System.ComponentModel.DataAnnotations;

namespace zMembershipFee.Entities.QuangTT.Models;

public partial class FeePackageQuangTt
{
    public int FeePackageQuangTtid { get; set; }

    [Required, StringLength(200)]
    [RegularExpression(@"^[\p{L}\p{M}0-9]+(?:[ .&()/'\-][\p{L}\p{M}0-9]+)*$", ErrorMessage = "Package name contains invalid characters.")]
    public string PackageName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(1, 3650)]
    public int? DurationInDays { get; set; }

    [Range(0, 999999999)]
    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public DateTime? PublishDate { get; set; }

    public virtual ICollection<MembershipFeeQuangTt> MembershipFeeQuangTts { get; set; } = new List<MembershipFeeQuangTt>();
}
