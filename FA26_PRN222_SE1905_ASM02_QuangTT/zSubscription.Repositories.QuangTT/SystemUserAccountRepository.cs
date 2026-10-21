using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT.DBContext;

namespace zSubscription.Repositories.QuangTT
{
    public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
    {
        public SystemUserAccountRepository() { }
        public SystemUserAccountRepository(PRN222Context context) : base(context) { }

        public async Task<SystemUserAccount?> GetByUserNameAsync(string userName, string password)
        {
            var keyword = userName.Trim();
            return await _context.SystemUserAccounts.FirstOrDefaultAsync(u =>
                u.IsActive
                && u.Password == password
                && (u.Email == keyword || u.UserName == keyword));
        }
    }
}
