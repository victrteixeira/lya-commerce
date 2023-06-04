using System.Text;
using Commerce.Core.Interfaces;
using Commerce.Infrastructure.Database;
using Commerce.Infrastructure.Repositories;
using Commerce.Security.Database;
using Commerce.Security.Helpers;
using Commerce.Security.Interfaces;
using Commerce.Security.Repository;
using Commerce.Security.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddHostedService<SeedInitial>();

        var key = EnvironmentVariable.GetRequiredEnvironmentVariable("JWT_KEY");
        var audience = EnvironmentVariable.GetRequiredEnvironmentVariable("JWT_AUDIENCE");
        var issuer = EnvironmentVariable.GetRequiredEnvironmentVariable("JWT_ISSUER");
        
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwt =>
        {
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        
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

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authentication for Commerce API"
        };

        var securityRequirements = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        };

        var contactInfo = new OpenApiContact
        {
            Name = "Cat Moon MakeUp",
            Email = "lyandrapetrato@outlook.com", 
            Url = new Uri("https://commerce.com/contato") // TODO -> Change it later
        };

        var license = new OpenApiLicense
        {
            Name = "Free License" // TODO -> Change it later
        };

        var info = new OpenApiInfo
        {
            Version = "v1",
            Title = "Back-End Api for Cat Moon MakeUp Commerce",
            Contact = contactInfo,
            License = license
        };

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", info);
            c.AddSecurityDefinition("Bearer", securityScheme);
            c.AddSecurityRequirement(securityRequirements);
        });

        return services;
    }
}