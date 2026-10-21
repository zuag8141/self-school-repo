using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT.DBContext;

namespace zSubscription.Repositories.QuangTT
{
    class FeePackageQuangTtRepository : GenericRepository<FeePackageQuangTt>
    {
        public FeePackageQuangTtRepository() => _context ??= new PRN222Context();
        public FeePackageQuangTtRepository(PRN222Context context) => _context = context;
    }
}
