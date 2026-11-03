using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using zMembershipFee.Repositories.QuangTT;
using zMembershipFee.Repositories.QuangTT.DBContext;
using zMembershipFee.Services.QuangTT;
using zMembershipFee.WorkerService.QuangTT;

var builder = Host.CreateApplicationBuilder(args);

// Khi process được Service Control Manager khởi động, dùng Windows Service lifetime.
// Khi chạy bằng `dotnet run`, ứng dụng vẫn chạy bình thường như một console worker.
builder.Services.AddWindowsService(options => options.ServiceName = "QuangTT_SE1905_WS");

// Development: <project Worker Service>/Logs. Sau publish: <publish>/Logs.
var fileLogOptions = builder.Configuration
    .GetSection(FileLoggingOptions.SectionName)
    .Get<FileLoggingOptions>() ?? new FileLoggingOptions();
var logRoot = builder.Environment.IsDevelopment()
    ? builder.Environment.ContentRootPath
    : AppContext.BaseDirectory;
builder.Services.AddLogging(loggingBuilder =>
    loggingBuilder.AddProvider(new FileLoggerProvider(fileLogOptions, logRoot)));

builder.Services.AddDbContext<PRN222Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MembershipFeeQuangTtRepository>();
builder.Services.AddScoped<FeePackageQuangTtRepository>();
builder.Services.AddScoped<SystemUserAccountRepository>();
builder.Services.AddScoped<IMembershipFeeQuangTtService, MembershipFeeQuangTtService>();
builder.Services.AddScoped<IFeePackageQuangTtService, FeePackageQuangTtService>();
builder.Services.AddScoped<ISystemUserAccountService, SystemUserAccountService>();
builder.Services.AddScoped<IMembershipFeeMaintenanceService, MembershipFeeMaintenanceService>();

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.Configure<WorkerLoginOptions>(builder.Configuration.GetSection(WorkerLoginOptions.SectionName));
builder.Services.AddHostedService<MembershipFeeWorker>();

var host = builder.Build();

// Ghi dòng khởi động vào file log ngay lập tức (không phụ thuộc DB).
using (var startupScope = host.Services.CreateScope())
{
    var bootstrapLogger = startupScope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Bootstrap");
    bootstrapLogger.LogInformation(
        "ASM4 Worker Service đang khởi động. Log file: {LogPath}",
        fileLogOptions.GetFullPath(logRoot));
}

await host.RunAsync();
