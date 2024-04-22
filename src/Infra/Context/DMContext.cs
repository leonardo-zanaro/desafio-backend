using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infra.Context;

public class DmContext : IdentityDbContext<User>
{
    public DmContext(DbContextOptions<DmContext> options): base(options) { }
    
    public override DbSet<User> Users { get; set; }
    public DbSet<Deliverer> Deliverers { get; set; }
    public DbSet<Motorcycle> Motorcycles { get; set; }
    public DbSet<RentalPeriod> RentalPeriods { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Deliverer>(b =>
        {
            b.HasIndex(e => e.PrimaryDocument).IsUnique();
            b.HasIndex(e => e.Cnh).IsUnique();
        });
    }
}