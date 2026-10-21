using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using zSubscription.Entities.QuangTT.Models;
using zSubscription.Services.QuangTT;

namespace zSubscription.RazorWebApp.QuangTT.Hubs;

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

    public async Task<object> GetPackages()
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

    public async Task<object> GetAll(string? memberName = null, string? paymentMethod = null, string? packageName = null)
    {
        var items = await _membershipFeeService.SearchAsync(memberName, paymentMethod, packageName);
        return items.Select(MapItem);
    }

    public async Task<object> GetById(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        return item == null ? null! : MapItem(item);
    }

    public async Task<object> Create(MembershipFeeDto dto)
    {
        var entity = MapDto(dto);
        entity.IsActive = true;
        if (entity.PaymentDate == default)
        {
            entity.PaymentDate = DateTime.Today;
        }

        await _membershipFeeService.CreateAsync(entity);
        await BroadcastChangedAsync("created", entity.MemberName);
        var items = await _membershipFeeService.GetAllAsync();
        return items.Select(MapItem);
    }

    public async Task<object> Update(MembershipFeeDto dto)
    {
        var existing = await _membershipFeeService.GetByIdAsync(dto.MembershipFeeQuangTtid);
        if (existing == null)
        {
            throw new HubException("Membership fee not found.");
        }

        var entity = MapDto(dto);
        entity.IsActive = dto.IsActive;
        await _membershipFeeService.UpdateAsync(entity);
        await BroadcastChangedAsync("updated", entity.MemberName);
        var items = await _membershipFeeService.GetAllAsync();
        return items.Select(MapItem);
    }

    public async Task<object> Delete(int id)
    {
        var existing = await _membershipFeeService.GetByIdAsync(id);
        if (existing == null)
        {
            throw new HubException("Membership fee not found.");
        }

        var memberName = existing.MemberName;
        await _membershipFeeService.DeleteAsync(id);
        await BroadcastChangedAsync("deleted", memberName);
        var items = await _membershipFeeService.GetAllAsync();
        return items.Select(MapItem);
    }

    private async Task BroadcastChangedAsync(string action, string? memberName)
    {
        await Clients.All.SendAsync("MembershipFeeChanged", new
        {
            Action = action,
            MemberName = memberName,
            ChangedAt = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
        });
    }

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
        MemberName = dto.MemberName,
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
    public string MemberName { get; set; } = string.Empty;
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
