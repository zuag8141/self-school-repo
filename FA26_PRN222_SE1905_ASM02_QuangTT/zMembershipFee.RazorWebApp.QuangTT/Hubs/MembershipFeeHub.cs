using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using zMembershipFee.Entities.QuangTT.Models;
using zMembershipFee.Services.QuangTT;

namespace zMembershipFee.RazorWebApp.QuangTT.Hubs;

/// <summary>
/// SignalR CRUD hub for the ASM02 Membership Fees page.
/// The page keeps its normal Razor Pages buttons and adds SignalR buttons next to them.
/// </summary>
[Authorize]
public class MembershipFeeHub : Hub
{
    private readonly IMembershipFeeQuangTtService _membershipFeeService;
    private readonly IFeePackageQuangTtService _feePackageService;

    public MembershipFeeHub(
        IMembershipFeeQuangTtService membershipFeeService,
        IFeePackageQuangTtService feePackageService)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
    }

    public async Task<IEnumerable<object>> GetAll(string? memberName = null, string? paymentMethod = null, string? packageName = null)
    {
        var items = await _membershipFeeService.SearchAsync(memberName, paymentMethod, packageName);
        return items.Select(MapItem);
    }

    public async Task<object?> GetById(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        return item is null ? null : MapItem(item);
    }

    public async Task<IEnumerable<object>> GetPackages()
    {
        var packages = await _feePackageService.GetAllAsync();
        return packages.Select(p => new
        {
            p.FeePackageQuangTtid,
            p.PackageName,
            p.Price,
            p.DurationInDays
        });
    }

    public async Task<int> Create(MembershipFeeDto dto)
    {
        var entity = MapDto(dto);
        entity.MembershipFeeQuangTtid = 0;
        Validate(entity);

        var created = await _membershipFeeService.CreateAsync(entity);
        await BroadcastChangedAsync("created", created.MemberName);
        return created.MembershipFeeQuangTtid;
    }

    public async Task<bool> Update(MembershipFeeDto dto)
    {
        if (dto.MembershipFeeQuangTtid <= 0)
        {
            throw new HubException("Invalid membership fee id.");
        }

        var entity = MapDto(dto);
        Validate(entity);

        var updated = await _membershipFeeService.UpdateAsync(entity);
        if (!updated)
        {
            throw new HubException("Membership fee was not found.");
        }

        await BroadcastChangedAsync("updated", entity.MemberName);
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var existing = await _membershipFeeService.GetByIdAsync(id);
        if (existing is null)
        {
            throw new HubException("Membership fee was not found.");
        }

        var deleted = await _membershipFeeService.DeleteAsync(id);
        if (!deleted)
        {
            throw new HubException("Could not delete membership fee.");
        }

        await BroadcastChangedAsync("deleted", existing.MemberName);
        return true;
    }

    private static void Validate(MembershipFeeQuangTt entity)
    {
        if (string.IsNullOrWhiteSpace(entity.MemberName))
        {
            throw new HubException("Member name is required.");
        }

        if (entity.Amount <= 0)
        {
            throw new HubException("Amount must be greater than 0.");
        }
    }

    private Task BroadcastChangedAsync(string action, string? memberName)
        => Clients.All.SendAsync("MembershipFeeChanged", new
        {
            Action = action,
            MemberName = memberName,
            ChangedAt = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
        });

    private static object MapItem(MembershipFeeQuangTt item) => new
    {
        item.MembershipFeeQuangTtid,
        item.MemberName,
        item.Amount,
        PaymentDate = item.PaymentDate.ToString("yyyy-MM-dd"),
        item.PaymentMethod,
        item.TransactionCode,
        StartDate = item.StartDate?.ToString("yyyy-MM-dd"),
        EndDate = item.EndDate?.ToString("yyyy-MM-dd"),
        item.Notes,
        item.Status,
        PublishDate = item.PublishDate?.ToString("yyyy-MM-dd"),
        UpdateAt = item.UpdateAt?.ToString("yyyy-MM-dd HH:mm"),
        item.FeePackageQuangTtid,
        PackageName = item.FeePackageQuangTt?.PackageName,
        item.IsActive
    };

    private static MembershipFeeQuangTt MapDto(MembershipFeeDto dto) => new()
    {
        MembershipFeeQuangTtid = dto.MembershipFeeQuangTtid,
        MemberName = dto.MemberName?.Trim() ?? string.Empty,
        Amount = dto.Amount,
        PaymentDate = dto.PaymentDate ?? DateTime.Today,
        PaymentMethod = dto.PaymentMethod,
        TransactionCode = dto.TransactionCode,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Notes = dto.Notes,
        Status = dto.Status,
        PublishDate = dto.PublishDate ?? DateTime.Today,
        FeePackageQuangTtid = dto.FeePackageQuangTtid,
        IsActive = dto.IsActive
    };
}

public class MembershipFeeDto
{
    public int MembershipFeeQuangTtid { get; set; }
    public string? MemberName { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public int? Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? FeePackageQuangTtid { get; set; }
    public bool IsActive { get; set; } = true;
}
