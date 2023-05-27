using Commerce.Security.Database;
using Commerce.Security.Interfaces;
using Commerce.Security.Repository;
using Commerce.Security.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Commerce.IoC;

public static class Container
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(
            configuration.GetSection("MongoDatabaseConfig"));

        services.AddScoped<ISecurityRepository, SecurityRepository>();
        
        services.AddAutoMapper(typeof(AuthMapper));
        return services;
    }
}