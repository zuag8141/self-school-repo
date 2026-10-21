using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT;

public interface ISystemUserAccountService
{
    Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password);
}
