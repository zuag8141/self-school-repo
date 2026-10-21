using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT
{
    public class FeePackageQuangTtService : IFeePackageQuangTtService
    {
        private readonly FeePackageQuangTtService _repository;

        public async Task<List<FeePackageQuangTt>> GetAllAsync()
        {
            try 
            {
                return await _repository.GetAllAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error in GetAllAsync: " + ex);
            }
            //throw new NotImplementedException();
        }
    }
}
