using Commerce.Core.Interfaces;
using Commerce.Infrastructure.Database;
using Commerce.Infrastructure.Repositories;
using Commerce.Security.Database;
using Commerce.Security.Interfaces;
using Commerce.Security.Repository;
using Commerce.Security.Services;
using Commerce.Security.Utils;
using Microsoft.EntityFrameworkCore;
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
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ISecurityService, SecurityService>();
        
        services.AddAutoMapper(typeof(AuthMapper));
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("MySqlDatabaseConfig");
        services.AddDbContext<CommerceContext>(opt => opt.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}