using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT;

namespace zSubscription.Services.QuangTT
{
    public class FeePackageQuangTtService : IFeePackageQuangTtService
    {
        private readonly FeePackageQuangTtRepository _repository;

        public FeePackageQuangTtService(FeePackageQuangTtRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FeePackageQuangTt>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAllAsync", ex);
            }
        }
    }
}
