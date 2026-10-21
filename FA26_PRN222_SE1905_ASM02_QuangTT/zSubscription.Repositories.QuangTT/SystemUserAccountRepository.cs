using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT.Base;
using zSubscription.Repositories.QuangTT.DBContext;

namespace zSubscription.Repositories.QuangTT;

public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
{
    public SystemUserAccountRepository(PRN222Context context) : base(context)
    {
    }

    public async Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password)
    {
        return await _context.SystemUserAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.UserName == userName &&
                x.Password == password &&
                x.IsActive);
    }
}
