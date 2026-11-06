using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Repositories.QuangTT;
using Microsoft.EntityFrameworkCore;
using zMembershipFee.Repositories.QuangTT.DBContext;

namespace zMembershipFee.Services.QuangTT;

public class FeePackageQuangTtService : IFeePackageQuangTtService
{
    private readonly IDbContextFactory<PRN222Context> _contextFactory;

    public FeePackageQuangTtService(IDbContextFactory<PRN222Context> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<FeePackageQuangTt>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await new FeePackageQuangTtRepository(context).GetAllAsync();
    }
}
