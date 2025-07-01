using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AvansMaaltijdreservering.Infrastructure.Data;

public class ApplicationIdentityDbContext : IdentityDbContext
{
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Custom Identity configuration can be added here if needed
    }
}
