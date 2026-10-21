using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT;

namespace zSubscription.Services.QuangTT;

public class MembershipFeeQuangTtService : IMembershipFeeQuangTtService
{
    private readonly MembershipFeeQuangTtRepository _repository;

    public MembershipFeeQuangTtService(MembershipFeeQuangTtRepository repository)
    {
        _repository = repository;
    }

    public Task<List<MembershipFeeQuangTt>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<MembershipFeeQuangTt?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public Task<List<MembershipFeeQuangTt>> SearchAsync(string? memberName, string? paymentMethod, string? packageName)
        => _repository.SearchAsync(memberName, paymentMethod, packageName);

    public async Task<MembershipFeeQuangTt> CreateAsync(MembershipFeeQuangTt membershipFee)
    {
        ArgumentNullException.ThrowIfNull(membershipFee);

        // Không đính kèm navigation khi thêm mới, tránh EF cố insert cả FeePackage.
        membershipFee.FeePackageQuangTt = null;
        membershipFee.MembershipFeeQuangTtid = 0;
        membershipFee.PaymentDate = membershipFee.PaymentDate == default
            ? DateTime.Today
            : membershipFee.PaymentDate;
        membershipFee.PublishDate ??= DateTime.Today;
        membershipFee.UpdateAt = DateTime.Now;

        return await _repository.CreateAsync(membershipFee);
    }

    public async Task<bool> UpdateAsync(MembershipFeeQuangTt membershipFee)
    {
        ArgumentNullException.ThrowIfNull(membershipFee);

        var existing = await _repository.GetByIdAsync(membershipFee.MembershipFeeQuangTtid);
        if (existing is null)
        {
            return false;
        }

        // Cập nhật trực tiếp trên entity đang được DbContext theo dõi để tránh lỗi
        // "another instance with the same key value is already being tracked".
        // Đặc biệt quan trọng với Blazor Server vì DbContext sống theo circuit.
        existing.MemberName = membershipFee.MemberName;
        existing.Amount = membershipFee.Amount;
        existing.PaymentDate = membershipFee.PaymentDate;
        existing.PaymentMethod = membershipFee.PaymentMethod;
        existing.TransactionCode = membershipFee.TransactionCode;
        existing.StartDate = membershipFee.StartDate;
        existing.EndDate = membershipFee.EndDate;
        existing.Notes = membershipFee.Notes;
        existing.Status = membershipFee.Status;
        existing.PublishDate = membershipFee.PublishDate ?? existing.PublishDate ?? DateTime.Today;
        existing.FeePackageQuangTtid = membershipFee.FeePackageQuangTtid;
        existing.IsActive = membershipFee.IsActive;
        existing.UpdateAt = DateTime.Now;

        // Không đính kèm navigation để tránh EF cập nhật lan sang FeePackage.
        existing.FeePackageQuangTt = null;

        return await _repository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        return await _repository.DeleteAsync(existing);
    }
}
