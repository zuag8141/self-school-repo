using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT;

public interface IFeePackageQuangTtService
{
    Task<List<FeePackageQuangTt>> GetAllAsync();
}
