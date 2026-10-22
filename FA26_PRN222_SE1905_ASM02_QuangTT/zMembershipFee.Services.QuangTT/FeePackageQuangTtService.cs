using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Repositories.QuangTT;

namespace zMembershipFee.Services.QuangTT;

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
