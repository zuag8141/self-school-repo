using Microsoft.Extensions.Options;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.WorkerService.QuangTT;

/// <summary>
/// Background job định kỳ:
///   1. Chuyển các khoản phí đã quá EndDate nhưng còn IsActive sang Inactive.
///   2. Ghi log thống kê doanh thu / số lượng khoản phí.
/// </summary>
public class MembershipFeeWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<WorkerOptions> _options;
    private readonly ILogger<MembershipFeeWorker> _logger;

    public MembershipFeeWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<WorkerOptions> options,
        ILogger<MembershipFeeWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = Math.Max(5, _options.Value.IntervalSeconds);
        _logger.LogInformation(
            "MembershipFeeWorker started. Interval = {Interval}s, DeactivateExpiredFees = {Deactivate}.",
            intervalSeconds,
            _options.Value.DeactivateExpiredFees);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MembershipFeeWorker gặp lỗi trong chu kỳ xử lý.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("MembershipFeeWorker stopped.");
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var maintenance = scope.ServiceProvider.GetRequiredService<IMembershipFeeMaintenanceService>();

        var deactivated = 0;
        if (_options.Value.DeactivateExpiredFees)
        {
            deactivated = await maintenance.DeactivateExpiredAsync(DateTime.Now);
        }

        var summary = await maintenance.GetSummaryAsync();

        _logger.LogInformation(
            "Run at {RunAt:dd/MM/yyyy HH:mm:ss} | Deactivated: {Deactivated} | Total: {Total} | Active: {Active} | Inactive: {Inactive} | Expired: {Expired} | Revenue: {Revenue:N0} VND | This month: {RevenueThisMonth:N0} VND | Packages: {Packages}",
            DateTime.Now,
            deactivated,
            summary.TotalFees,
            summary.ActiveFees,
            summary.InactiveFees,
            summary.ExpiredFees,
            summary.TotalRevenue,
            summary.RevenueThisMonth,
            summary.PackageCount);

        if (deactivated > 0)
        {
            _logger.LogWarning(
                "Đã tự động chuyển {Count} khoản phí quá hạn sang trạng thái Inactive.",
                deactivated);
        }
    }
}
