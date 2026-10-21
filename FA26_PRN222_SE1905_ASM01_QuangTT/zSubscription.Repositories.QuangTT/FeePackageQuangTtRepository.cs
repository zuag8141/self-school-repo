using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT.Base;
using zSubscription.Repositories.QuangTT.DBContext;

namespace zSubscription.Repositories.QuangTT;

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
