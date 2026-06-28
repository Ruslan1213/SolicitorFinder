using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolicitorFinder.Data.HostedServices;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Repositories;

namespace SolicitorFinder.Data.Extensions;

public static class BuilderExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<SolicitorDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddHostedService<DatabaseMigrationService>();

        services.AddScoped(typeof(IReadRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<IConfigRepository, ConfigRepository>();
        services.AddScoped<ISolicitorRepository, SolicitorRepository>();
        services.AddScoped<IReportingRepository, ReportingRepository>();

        return services;
    }
}
