namespace zMembershipFee.WorkerService.QuangTT;

public class WorkerLoginOptions
{
    public const string SectionName = "Worker:Login";

    /// <summary>Bật kiểm tra đăng nhập khi Worker khởi động.</summary>
    public bool RequireLogin { get; set; } = true;

    public string UserName { get; set; } = "admin";

    public string Password { get; set; } = "123456";
}
