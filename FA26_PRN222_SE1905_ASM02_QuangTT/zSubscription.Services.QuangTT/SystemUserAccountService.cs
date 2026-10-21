using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT;

namespace zSubscription.Services.QuangTT;

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
