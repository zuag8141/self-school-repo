using zMembershipFee.Entities.QuangTT.Models;

namespace zMembershipFee.Services.QuangTT;

public interface IFeePackageQuangTtService
{
    Task<List<FeePackageQuangTt>> GetAllAsync();
}
