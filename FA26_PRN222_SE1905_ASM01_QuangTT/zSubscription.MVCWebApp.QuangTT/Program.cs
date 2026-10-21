using Microsoft.EntityFrameworkCore;
using zSubscription.MVCWebApp.QuangTT.Hubs;
using zSubscription.Repositories.QuangTT;
using zSubscription.Repositories.QuangTT.DBContext;
using zSubscription.Services.QuangTT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddDbContext<PRN222Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MembershipFeeQuangTtRepository>();
builder.Services.AddScoped<FeePackageQuangTtRepository>();
builder.Services.AddScoped<SystemUserAccountRepository>();
builder.Services.AddScoped<IMembershipFeeQuangTtService, MembershipFeeQuangTtService>();
builder.Services.AddScoped<IFeePackageQuangTtService, FeePackageQuangTtService>();
builder.Services.AddScoped<ISystemUserAccountService, SystemUserAccountService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<MembershipFeeHub>("/membershipFeeHub");

app.Run();
