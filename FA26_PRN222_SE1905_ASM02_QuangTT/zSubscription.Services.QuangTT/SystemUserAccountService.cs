using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT;

namespace zSubscription.Services.QuangTT
{
    public class SystemUserAccountService : ISystemUserAccountService
    {
        private readonly SystemUserAccountRepository _repository;

        public SystemUserAccountService(SystemUserAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<SystemUserAccount?> GetUserAccount(string userName, string password)
        {
            try
            {
                return await _repository.GetByUserNameAsync(userName, password);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserAccount", ex);
            }
        }
    }
}
