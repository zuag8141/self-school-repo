namespace zMembershipFee.WorkerService.QuangTT;

public class WorkerOptions
{
    public const string SectionName = "Worker";

    /// <summary>Chu kỳ chạy job (giây).</summary>
    public int IntervalSeconds { get; set; } = 30;

    /// <summary>Tự động chuyển các khoản phí quá hạn sang Inactive.</summary>
    public bool DeactivateExpiredFees { get; set; } = true;
}
