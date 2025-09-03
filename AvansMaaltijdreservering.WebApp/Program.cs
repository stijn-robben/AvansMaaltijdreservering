using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AvansMaaltijdreservering.Infrastructure.Data;
using AvansMaaltijdreservering.Infrastructure.Identity;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Repositories;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Services;
using AvansMaaltijdreservering.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework DbContexts (separate databases as required)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// Add Identity services with roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

// Register Repository interfaces with implementations
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICanteenRepository, CanteenRepository>();
builder.Services.AddScoped<ICanteenEmployeeRepository, CanteenEmployeeRepository>();

// Register Domain Service interfaces with implementations
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// Register thread-safety services
builder.Services.AddSingleton<IPackageLockService, PackageLockService>();

// Register Authorization Service
builder.Services.AddScoped<AvansMaaltijdreservering.Infrastructure.Identity.IUserAuthorizationService, AvansMaaltijdreservering.Infrastructure.Identity.UserAuthorizationService>();

// Register Logging Service
builder.Services.AddScoped<ILoggerService, LoggerService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure culture for comma as decimal separator
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "nl-NL", "nl-BE" }; // Dutch cultures use comma as decimal separator
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("nl-NL");
    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
