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
                .WithOne()
                .HasForeignKey(p => p.PorfolioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Instrument>(b =>
        {
            b.ToTable("positions");
            b.HasKey(x => new { x.PortfolioId, x.InstrumentId });

            b.Property(x => x.Quantity).HasColumnType("numeric(30,10)").IsRequired();
            b.Property(x => x.CreatedAtUtc).IsRequired();

            b.Property(x => x.Version)
                .IsConcurrencyToken();

            b.HasIndex(x => x.InstrumentId);
        });
        modelBuilder.Entity<Price>(b =>
        {
            b.ToTable("prices");
            b.HasKey(x => new { x.PortfolioId, x.TsUtc });

            b.Property(x => x.Px).HasColumnType("numeric(30,10)").IsRequired();
            b.Property(x => x.TsUtc).IsRequired();

            b.HasIndex(x => x.PortfolioId);
        });
    }
}