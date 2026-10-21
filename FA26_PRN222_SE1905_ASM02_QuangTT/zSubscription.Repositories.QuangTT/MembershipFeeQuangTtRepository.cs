using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT.DBContext;

namespace zSubscription.Repositories.QuangTT
{
    public class MembershipFeeQuangTtRepository : GenericRepository<MembershipFeeQuangTt>
    {
        public MembershipFeeQuangTtRepository() { }
        public MembershipFeeQuangTtRepository(PRN222Context context) : base(context) { }

        public override async Task<List<MembershipFeeQuangTt>> GetAllAsync()
        {
            return await _context.MembershipFeeQuangTts
                .Include(c => c.FeePackageQuangTt)
                .OrderByDescending(c => c.MembershipFeeQuangTtid)
                .ToListAsync();
        }

        public override async Task<MembershipFeeQuangTt?> GetByIdAsync(int id)
        {
            return await _context.MembershipFeeQuangTts
                .Include(c => c.FeePackageQuangTt)
                .FirstOrDefaultAsync(c => c.MembershipFeeQuangTtid == id);
        }

        public async Task<List<MembershipFeeQuangTt>> SearchAsync(
            string? memberName,
            string? paymentMethod,
            string? packageName)
        {
            var query = _context.MembershipFeeQuangTts
                .Include(c => c.FeePackageQuangTt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(memberName))
            {
                var keyword = memberName.Trim();
                query = query.Where(c => c.MemberName != null && c.MemberName.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(paymentMethod))
            {
                var keyword = paymentMethod.Trim();
                query = query.Where(c => c.PaymentMethod != null && c.PaymentMethod.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(packageName))
            {
                var keyword = packageName.Trim();
                query = query.Where(c =>
                    c.FeePackageQuangTt != null && c.FeePackageQuangTt.PackageName.Contains(keyword));
            }

            return await query
                .OrderByDescending(c => c.MembershipFeeQuangTtid)
                .ToListAsync();
        }
    }
}
