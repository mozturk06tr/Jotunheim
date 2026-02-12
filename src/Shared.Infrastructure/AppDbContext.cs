using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Shared.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Portfolio> Portfolios  => Set<Portfolio>();
    public DbSet<Instrument> Instrument => Set<Instrument>();
    public DbSet<Position> Positions => Set<Position>();    
    public DbSet<Price> Prices => Set<Price>();
    
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mp");
        
        modelBuilder.Entity<Portfolio>(b =>
        {
            b.ToTable("Portfolio");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.CreatedAtUtc).IsRequired();
            
            b.HasMany<Position>("_positions")
                
        })
    }
}