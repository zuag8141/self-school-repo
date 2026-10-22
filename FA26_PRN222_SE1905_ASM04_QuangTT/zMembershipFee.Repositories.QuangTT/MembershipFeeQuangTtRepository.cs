using Microsoft.EntityFrameworkCore;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Repositories.QuangTT.Base;
using zMembershipFee.Repositories.QuangTT.DBContext;

namespace zMembershipFee.Repositories.QuangTT;

public class MembershipFeeQuangTtRepository : GenericRepository<MembershipFeeQuangTt>
{
    public MembershipFeeQuangTtRepository(PRN222Context context) : base(context)
    {
    }

    public override async Task<List<MembershipFeeQuangTt>> GetAllAsync()
    {
        return await _context.MembershipFeeQuangTts
            .Include(x => x.FeePackageQuangTt)
            .OrderByDescending(x => x.MembershipFeeQuangTtid)
            .ToListAsync();
    }

    public override async Task<MembershipFeeQuangTt?> GetByIdAsync(int id)
    {
        return await _context.MembershipFeeQuangTts
            .Include(x => x.FeePackageQuangTt)
            .FirstOrDefaultAsync(x => x.MembershipFeeQuangTtid == id);
    }

    /// <summary>
    /// Tìm kiếm theo 3 tiêu chí: tên hội viên, phương thức thanh toán, tên gói.
    /// </summary>
    public async Task<List<MembershipFeeQuangTt>> SearchAsync(
        string? memberName,
        string? paymentMethod,
        string? packageName)
    {
        var query = _context.MembershipFeeQuangTts
            .Include(x => x.FeePackageQuangTt)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(memberName))
        {
            var keyword = memberName.Trim();
            query = query.Where(x => x.MemberName.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(paymentMethod))
        {
            var keyword = paymentMethod.Trim();
            query = query.Where(x => x.PaymentMethod != null && x.PaymentMethod.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(packageName))
        {
            var keyword = packageName.Trim();
            query = query.Where(x =>
                x.FeePackageQuangTt != null && x.FeePackageQuangTt.PackageName.Contains(keyword));
        }

        return await query
            .OrderByDescending(x => x.MembershipFeeQuangTtid)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy các khoản phí còn Active nhưng đã quá EndDate (dùng cho Worker).
    /// </summary>
    public async Task<List<MembershipFeeQuangTt>> GetExpiredActiveAsync(DateTime asOf)
    {
        return await _context.MembershipFeeQuangTts
            .AsTracking()
            .Where(x => x.IsActive && x.EndDate != null && x.EndDate < asOf)
            .ToListAsync();
    }

    public async Task<MembershipFeeSummary> GetSummaryAsync()
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        return new MembershipFeeSummary
        {
            TotalFees = await _context.MembershipFeeQuangTts.CountAsync(),
            ActiveFees = await _context.MembershipFeeQuangTts.CountAsync(x => x.IsActive),
            InactiveFees = await _context.MembershipFeeQuangTts.CountAsync(x => !x.IsActive),
            ExpiredFees = await _context.MembershipFeeQuangTts
                .CountAsync(x => x.EndDate != null && x.EndDate < today),
            TotalRevenue = await _context.MembershipFeeQuangTts
                .SumAsync(x => (decimal?)x.Amount) ?? 0m,
            RevenueThisMonth = await _context.MembershipFeeQuangTts
                .Where(x => x.PaymentDate >= firstDayOfMonth)
                .SumAsync(x => (decimal?)x.Amount) ?? 0m,
            PackageCount = await _context.FeePackageQuangTts.CountAsync()
        };
    }
}
