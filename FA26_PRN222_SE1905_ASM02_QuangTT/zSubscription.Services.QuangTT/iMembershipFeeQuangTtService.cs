using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT;

public interface IMembershipFeeQuangTtService
{
    Task<List<MembershipFeeQuangTt>> GetAllAsync();
    Task<MembershipFeeQuangTt?> GetByIdAsync(int id);
    Task<List<MembershipFeeQuangTt>> SearchAsync(string? memberName, string? paymentMethod, string? packageName);
    Task<MembershipFeeQuangTt> CreateAsync(MembershipFeeQuangTt membershipFee);
    Task<bool> UpdateAsync(MembershipFeeQuangTt membershipFee);
    Task<bool> DeleteAsync(int id);
}
