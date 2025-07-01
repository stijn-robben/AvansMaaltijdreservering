using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AvansMaaltijdreservering.Infrastructure.Data;
using AvansMaaltijdreservering.Core.Domain.Interfaces;
using AvansMaaltijdreservering.Infrastructure.Repositories;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using AvansMaaltijdreservering.Core.DomainService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework DbContexts (separate databases as required)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
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

// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
