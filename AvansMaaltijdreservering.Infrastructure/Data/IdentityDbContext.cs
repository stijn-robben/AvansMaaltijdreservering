using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AvansMaaltijdreservering.Infrastructure.Identity;

namespace AvansMaaltijdreservering.Infrastructure.Data;

public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Seed roles
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = IdentityRoles.Student, NormalizedName = IdentityRoles.Student.ToUpper() },
            new IdentityRole { Id = "2", Name = IdentityRoles.CanteenEmployee, NormalizedName = IdentityRoles.CanteenEmployee.ToUpper() }
        );
    }
}
