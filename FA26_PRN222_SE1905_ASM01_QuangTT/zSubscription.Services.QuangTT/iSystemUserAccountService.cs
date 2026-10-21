using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT
{
    public  interface ISystemUserAccountService
    {
        Task<SystemUserAccount> GetUserAccount(string userName, string password);
    }
}
