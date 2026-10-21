using System.ComponentModel.DataAnnotations;

namespace zSubscription.Entities.QuangTT.Models;

public partial class FeePackageQuangTt
{
    public int FeePackageQuangTtid { get; set; }

    [Required, StringLength(200)]
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
