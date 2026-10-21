using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zSubscription.Services.QuangTT
{
    public interface IFeePackageQuangTtService
    {
        Task<List<Entities.QuangTT.Models.FeePackageQuangTt>> GetAllAsync();
    }
}