namespace zSubscription.Entities.QuangTT.Models;

/// <summary>
/// Số liệu tổng hợp dùng cho Worker Service (ASM4) và dashboard.
/// </summary>
public class MembershipFeeSummary
{
    public int TotalFees { get; set; }
    public int ActiveFees { get; set; }
    public int InactiveFees { get; set; }
    public int ExpiredFees { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int PackageCount { get; set; }
}
