using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using zMembershipFee.Repositories.QuangTT.DBContext;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Repositories.QuangTT;

namespace zMembershipFee.Services.QuangTT;

public class MembershipFeeQuangTtService : IMembershipFeeQuangTtService
{
    private readonly IDbContextFactory<PRN222Context> _contextFactory;
    public MembershipFeeQuangTtService(IDbContextFactory<PRN222Context> contextFactory) => _contextFactory = contextFactory;
    public Task<List<MembershipFeeQuangTt>> GetAllAsync() => SearchAsync(null, null, null);
    public async Task<MembershipFeeQuangTt?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await new MembershipFeeQuangTtRepository(context).GetByIdAsync(id);
    }
    public async Task<List<MembershipFeeQuangTt>> SearchAsync(string? memberName, string? paymentMethod, string? packageName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await new MembershipFeeQuangTtRepository(context).SearchAsync(memberName, paymentMethod, packageName);
    }
    public async Task<MembershipFeeQuangTt> CreateAsync(MembershipFeeQuangTt membershipFee)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var repository = new MembershipFeeQuangTtRepository(context);
        ArgumentNullException.ThrowIfNull(membershipFee);
        Validator.ValidateObject(membershipFee, new ValidationContext(membershipFee), true);

        // Không đính kèm navigation khi thêm mới, tránh EF cố insert cả FeePackage.
        membershipFee.FeePackageQuangTt = null;
        membershipFee.MembershipFeeQuangTtid = 0;
        membershipFee.PaymentDate = membershipFee.PaymentDate == default
            ? DateTime.Today
            : membershipFee.PaymentDate;
        membershipFee.PublishDate ??= DateTime.Today;
        membershipFee.UpdateAt = DateTime.Now;

        return await repository.CreateAsync(membershipFee);
    }

    public async Task<bool> UpdateAsync(MembershipFeeQuangTt membershipFee)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var repository = new MembershipFeeQuangTtRepository(context);
        ArgumentNullException.ThrowIfNull(membershipFee);
        Validator.ValidateObject(membershipFee, new ValidationContext(membershipFee), true);

        var existing = await repository.GetByIdAsync(membershipFee.MembershipFeeQuangTtid);
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

        return await repository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var repository = new MembershipFeeQuangTtRepository(context);
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        return await repository.DeleteAsync(existing);
    }
}
