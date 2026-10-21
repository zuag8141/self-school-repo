using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT;

namespace zSubscription.Services.QuangTT;

public class MembershipFeeMaintenanceService : IMembershipFeeMaintenanceService
{
    private readonly MembershipFeeQuangTtRepository _repository;

    public MembershipFeeMaintenanceService(MembershipFeeQuangTtRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> DeactivateExpiredAsync(DateTime asOf)
    {
        var expiredItems = await _repository.GetExpiredActiveAsync(asOf);
        if (expiredItems.Count == 0)
        {
            return 0;
        }

        foreach (var item in expiredItems)
        {
            item.IsActive = false;
            item.Status = 0; // 0 = hết hạn
            item.UpdateAt = DateTime.Now;
        }

        // Các entity đang được tracking bởi GetExpiredActiveAsync (.AsTracking()).
        await _repository.UpdateRangeAsync(expiredItems);
        return expiredItems.Count;
    }

    public Task<MembershipFeeSummary> GetSummaryAsync()
        => _repository.GetSummaryAsync();
}
