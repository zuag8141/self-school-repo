using Microsoft.EntityFrameworkCore;
using zSubscription.Repositories.QuangTT;
using zSubscription.Repositories.QuangTT.DBContext;
using zSubscription.Services.QuangTT;
using zSubscription.WorkerService.QuangTT;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PRN222Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MembershipFeeQuangTtRepository>();
builder.Services.AddScoped<FeePackageQuangTtRepository>();
builder.Services.AddScoped<SystemUserAccountRepository>();
builder.Services.AddScoped<IMembershipFeeQuangTtService, MembershipFeeQuangTtService>();
builder.Services.AddScoped<IFeePackageQuangTtService, FeePackageQuangTtService>();
builder.Services.AddScoped<IMembershipFeeMaintenanceService, MembershipFeeMaintenanceService>();

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddHostedService<MembershipFeeWorker>();

var host = builder.Build();
host.Run();
