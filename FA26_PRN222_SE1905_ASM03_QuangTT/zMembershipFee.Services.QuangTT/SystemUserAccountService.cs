using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Repositories.QuangTT;

namespace zMembershipFee.Services.QuangTT;

public class SystemUserAccountService : ISystemUserAccountService
{
    private readonly SystemUserAccountRepository _repository;

    public SystemUserAccountService(SystemUserAccountRepository repository)
    {
        _repository = repository;
    }

    public Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password)
        => _repository.GetUserAccountAsync(userName, password);
}
