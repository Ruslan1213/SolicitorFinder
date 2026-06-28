using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data;

public sealed class SolicitorDbContext : DbContext
{
    public SolicitorDbContext()
    {
    }
    public SolicitorDbContext(DbContextOptions<SolicitorDbContext> options) : base(options)
    {
    }

    public DbSet<Solicitor> Solicitors { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<AreaEntity> Areas { get; set; }

    public DbSet<SolicitorArea> SolicitorAreas { get; set; }

    public DbSet<SolicitorLocation> SolicitorLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SolicitorDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SolicitorFinder"))
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
