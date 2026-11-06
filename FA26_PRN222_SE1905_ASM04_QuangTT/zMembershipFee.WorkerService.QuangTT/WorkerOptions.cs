namespace zMembershipFee.WorkerService.QuangTT;

public class WorkerOptions
{
    public const string SectionName = "Worker";

    /// <summary>Chu kỳ chạy job (giây).</summary>
    public int IntervalSeconds { get; set; } = 5;
    public string FilePath { get; set; } = "QuangTT_SE1905_WS.txt";

    /// <summary>Tự động chuyển các khoản phí quá hạn sang Inactive.</summary>
    public bool DeactivateExpiredFees { get; set; } = false;
}
