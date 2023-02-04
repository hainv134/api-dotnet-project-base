using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ConfigureInfrastructure
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Out-going services call
            services.AddScoped<HttpClientService>();

            // Database context injection
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<AppDbContextInitialiser>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SQLServer"),
                    b => b.MigrationsAssembly("WebApi"));
            });

            // Repostitory pattern injection
            services.AddScoped<UnitOfWork>();          

            return services;
        }
    }
}