using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using zMembershipFee.Repositories.QuangTT;
using zMembershipFee.Repositories.QuangTT.DBContext;
using zMembershipFee.Services.QuangTT;
using zMembershipFee.WorkerService.QuangTT;

var builder = Host.CreateApplicationBuilder(args);

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

if (!await TryLoginAsync(host))
{
    Environment.ExitCode = 1;
    return;
}

await host.RunAsync();

static async Task<bool> TryLoginAsync(IHost host)
{
    using var scope = host.Services.CreateScope();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<WorkerLoginOptions>>().Value;
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("WorkerLogin");

    if (!options.RequireLogin)
    {
        logger.LogWarning("Worker login is disabled (Worker:Login:RequireLogin = false).");
        return true;
    }

    if (string.IsNullOrWhiteSpace(options.UserName) || string.IsNullOrWhiteSpace(options.Password))
    {
        logger.LogError("Worker login failed: UserName or Password is empty.");
        return false;
    }

    var userAccountService = scope.ServiceProvider.GetRequiredService<ISystemUserAccountService>();
    var account = await userAccountService.GetUserAccountAsync(options.UserName, options.Password);
    if (account is null)
    {
        logger.LogError("Worker login failed for user {UserName}.", options.UserName);
        return false;
    }

    logger.LogInformation(
        "Worker login successful: {UserName} ({FullName}).",
        account.UserName,
        account.FullName);
    return true;
}
