using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT.DBContext;

namespace zSubscription.Repositories.QuangTT
{
    public class FeePackageQuangTtRepository : GenericRepository<FeePackageQuangTt>
    {
        public FeePackageQuangTtRepository() { }
        public FeePackageQuangTtRepository(PRN222Context context) : base(context) { }
    }
}
