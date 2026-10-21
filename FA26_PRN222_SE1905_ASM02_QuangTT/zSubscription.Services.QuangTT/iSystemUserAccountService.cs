using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT
{
    public interface ISystemUserAccountService
    {
        Task<SystemUserAccount?> GetUserAccount(string userName, string password);
    }
}
