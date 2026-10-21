using zSubscription.Entities.QuangTT.Models;
using zSubscription.Repositories.QuangTT;

namespace zSubscription.Services.QuangTT;

public class FeePackageQuangTtService : IFeePackageQuangTtService
{
    private readonly FeePackageQuangTtRepository _repository;

    public FeePackageQuangTtService(FeePackageQuangTtRepository repository)
    {
        _repository = repository;
    }

    public Task<List<FeePackageQuangTt>> GetAllAsync()
        => _repository.GetAllAsync();
}
