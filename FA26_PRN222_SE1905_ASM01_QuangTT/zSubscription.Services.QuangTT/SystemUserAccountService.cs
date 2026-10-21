using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT
{
    public class SystemUserAccountService : ISystemUserAccountService
    {
        private readonly SystemUserAccountService _repository;
        public async Task<SystemUserAccount> GetUserAccount(string userName, string password)
        {
            try 
            { 
                return await _repository.GetUserAccount(userName, password);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetUserAccount: ", ex);
            }
            //throw new NotImplementedException();
        }
    }
}
