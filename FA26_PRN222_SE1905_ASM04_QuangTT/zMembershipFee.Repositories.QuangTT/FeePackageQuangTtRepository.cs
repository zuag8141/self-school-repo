using Microsoft.EntityFrameworkCore;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Repositories.QuangTT.Base;
using zMembershipFee.Repositories.QuangTT.DBContext;

namespace zMembershipFee.Repositories.QuangTT;

public class FeePackageQuangTtRepository : GenericRepository<FeePackageQuangTt>
{
    public FeePackageQuangTtRepository(PRN222Context context) : base(context)
    {
    }

    public override async Task<List<FeePackageQuangTt>> GetAllAsync()
    {
        return await _context.FeePackageQuangTts
            .OrderBy(x => x.PackageName)
            .ToListAsync();
    }
}
