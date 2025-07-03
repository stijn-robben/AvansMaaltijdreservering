using Microsoft.EntityFrameworkCore;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Canteen> Canteens { get; set; }
    public DbSet<CanteenEmployee> CanteenEmployees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.StudentNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Package configuration
        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasOne(e => e.ReservedByStudent)
                  .WithMany(s => s.Reservations)
                  .HasForeignKey(e => e.ReservedByStudentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
        });

        // Many-to-many relationship between Package and Product
        modelBuilder.Entity<Package>()
            .HasMany(p => p.Products)
            .WithMany(p => p.Packages)
            .UsingEntity(j => j.ToTable("PackageProducts"));

        // Canteen configuration
        modelBuilder.Entity<Canteen>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Location).IsUnique();
        });

        // CanteenEmployee configuration
        modelBuilder.Entity<CanteenEmployee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.HasOne(e => e.Canteen)
                  .WithMany(c => c.Employees)
                  .HasForeignKey(e => e.CanteenId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data for Canteens
        modelBuilder.Entity<Canteen>().HasData(
            new Canteen { Id = 1, Location = CanteenLocation.BREDA_LA_BUILDING, City = City.BREDA, ServesWarmMeals = true },
            new Canteen { Id = 2, Location = CanteenLocation.BREDA_LD_BUILDING, City = City.BREDA, ServesWarmMeals = false },
            new Canteen { Id = 3, Location = CanteenLocation.BREDA_LD_BUILDING, City = City.BREDA, ServesWarmMeals = false },
            new Canteen { Id = 4, Location = CanteenLocation.BREDA_LA_BUILDING, City = City.BREDA, ServesWarmMeals = true },
            new Canteen { Id = 5, Location = CanteenLocation.DENBOSCH_BUILDING, City = City.DENBOSCH, ServesWarmMeals = true },
            new Canteen { Id = 6, Location = CanteenLocation.TILBURG_BUILDING, City = City.TILBURG, ServesWarmMeals = true }
        );
    }
}
