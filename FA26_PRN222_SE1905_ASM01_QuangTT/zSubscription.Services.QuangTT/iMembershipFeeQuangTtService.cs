using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT
{
    public interface IMembershipFeeQuangTtService
    {
        Task<List<MembershipFeeQuangTt>> GetAllAsync();
        Task<MembershipFeeQuangTt> GetByIdAsync(int id);
        Task<List<MembershipFeeQuangTt>> SearchAsync(string transCode, decimal amount, string payMethod);
        Task<int> CreateAsync(MembershipFeeQuangTt membershipFee);
        Task<int> UpdateAsync(MembershipFeeQuangTt membershipFee);
        Task<bool> DeleteAsync(Guid id);
    }
}
