using Microsoft.EntityFrameworkCore;
using MobileNumLogin.Models;
using Service.AccountService;
using Service.MasterService;
using Service.SMSHelperService;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext registration
builder.Services.AddDbContext<ChristellDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ISMSHelperRepository, SMSHelperServices> ();
builder.Services.AddScoped<IAccountRepository, AccountServices> ();
builder.Services.AddScoped<IMasterRepository, MasterService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
