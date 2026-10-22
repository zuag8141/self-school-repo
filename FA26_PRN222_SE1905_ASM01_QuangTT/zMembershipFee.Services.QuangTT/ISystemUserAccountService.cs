using zMembershipFee.Entities.QuangTT.Models;

namespace zMembershipFee.Services.QuangTT;

public interface ISystemUserAccountService
{
    Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password);
}
