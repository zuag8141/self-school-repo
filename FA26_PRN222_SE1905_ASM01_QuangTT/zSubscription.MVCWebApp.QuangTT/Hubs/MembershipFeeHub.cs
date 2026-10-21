using Microsoft.AspNetCore.SignalR;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Services.QuangTT;

namespace zSubscription.MVCWebApp.QuangTT.Hubs;

/// <summary>
/// Hub cho phép Create / Read / Update / Delete ngay trên trang Index
/// (các nút "SignalR Create/Edit/Delete" nằm cạnh nút CRUD thường).
/// </summary>
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

        var created = await _membershipFeeService.CreateAsync(entity);
        await BroadcastChangedAsync("created", created.MemberName);
        return created.MembershipFeeQuangTtid;
    }

    public async Task<bool> Update(MembershipFeeDto dto)
    {
        var entity = MapDto(dto);
        var updated = await _membershipFeeService.UpdateAsync(entity);

        if (updated)
        {
            await BroadcastChangedAsync("updated", entity.MemberName);
        }

        return updated;
    }

    public async Task<bool> Delete(int id)
    {
        var existing = await _membershipFeeService.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        var deleted = await _membershipFeeService.DeleteAsync(id);
        if (deleted)
        {
            await BroadcastChangedAsync("deleted", existing.MemberName);
        }

        return deleted;
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
        PublishDate = dto.PublishDate,
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
