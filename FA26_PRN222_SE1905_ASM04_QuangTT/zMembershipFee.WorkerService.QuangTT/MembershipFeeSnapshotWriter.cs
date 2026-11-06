using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using zMembershipFee.Entities.QuangTT.Models;
namespace zMembershipFee.WorkerService.QuangTT;

public sealed class MembershipFeeSnapshotWriter(IOptions<WorkerOptions> options)
{
    public string FilePath => Path.GetFullPath(options.Value.FilePath, AppContext.BaseDirectory);
    public async Task WriteAsync(List<MembershipFeeQuangTt> items, CancellationToken token)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions {
            WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
        var entry = $"{new string('=',72)}{Environment.NewLine}QUANG TT - MEMBERSHIP FEE WINDOWS SERVICE{Environment.NewLine}"
            + $"Thoi gian ghi log: {DateTimeOffset.Now:dd/MM/yyyy HH:mm:ss.fff zzz}{Environment.NewLine}"
            + $"So luong Membership Fee: {items.Count}{Environment.NewLine}{new string('-',72)}{Environment.NewLine}{json}{Environment.NewLine}{Environment.NewLine}";
        await File.AppendAllTextAsync(FilePath, entry, Encoding.UTF8, token);
    }
}
