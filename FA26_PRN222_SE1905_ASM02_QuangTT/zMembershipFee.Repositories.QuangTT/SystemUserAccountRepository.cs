using Microsoft.EntityFrameworkCore;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Repositories.QuangTT.Base;
using zMembershipFee.Repositories.QuangTT.DBContext;

namespace zMembershipFee.Repositories.QuangTT;

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
