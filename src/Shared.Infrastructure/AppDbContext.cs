using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Shared.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<Instrument> Instruments => Set<Instrument>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Price> Prices => Set<Price>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mp");

        modelBuilder.Entity<Portfolio>(b =>
        {
            b.ToTable("portfolios");
            b.HasKey(x => x.Id);
            
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.CreatedAtUtc).IsRequired();
                
            b.HasMany(x => x.Positions)
             .WithOne()
             .HasForeignKey(p => p.PortfolioId)
             .OnDelete(DeleteBehavior.Cascade);
            
            b.Navigation(x=> x.Positions)
                .HasField("_positions")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Instrument>(b =>
        {
            b.ToTable("instruments");
            b.HasKey(x => x.Id);
            b.Property(x => x.Symbol).HasMaxLength(32).IsRequired();
            b.HasIndex(x => x.Symbol).IsUnique();
            b.Property(x => x.Description).HasMaxLength(512);
            b.Property(x => x.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<Position>(b =>
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
            b.HasKey(x => new { x.InstrumentId, x.TsUtc });

            b.Property(x => x.Px).HasColumnType("numeric(30,10)").IsRequired();
            b.Property(x => x.TsUtc).IsRequired();

            b.HasIndex(x => x.TsUtc);
        });
    }
}
