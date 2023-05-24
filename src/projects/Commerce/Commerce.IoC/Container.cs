using Commerce.Security.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Commerce.IoC;

public static class Container
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(AuthMapper));
        return services;
    }
}