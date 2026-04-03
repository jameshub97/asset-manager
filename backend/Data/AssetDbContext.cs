using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AssetDbContext : DbContext
{
    public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Vendor> Vendors => Set<Vendor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired();
            entity.Property(a => a.Description).IsRequired();
            entity.Property(a => a.CreatedAt).IsRequired(false);
            entity.Property(a => a.Price).HasPrecision(18, 2).IsRequired(false);
            entity.Property(a => a.UserId).IsRequired(false);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).ValueGeneratedOnAdd();
            entity.Property(v => v.Name).IsRequired();
            entity.Property(v => v.Email).IsRequired();
        });
    }
}
