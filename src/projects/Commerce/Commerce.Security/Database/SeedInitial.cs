using Commerce.Security.Interfaces;
using Commerce.Security.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Commerce.Security.Database;

public class SeedInitial : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SeedInitial(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISecurityRepository>();
        var pwdService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        
        var emailEnv = GetRequiredEnvironmentVariable("DEV_EMAIL");
        var pwdEnv = GetRequiredEnvironmentVariable("DEV_PASSWORD");

        var encryptedPwd = await pwdService.EncryptPassword(pwdEnv);
        
        var devUser = new User("Root", "CatMoonMakeUp", emailEnv, encryptedPwd);
        devUser.UpdateRole("Admin");
        
        if (await repository.GetSingleUserByEmailAsync(emailEnv) == null)
        {
            await repository.AddUserAsync(devUser);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    private string GetRequiredEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            Console.Error.WriteLine($"Environment Variable {name} is not set and it's required. Aborting.");
            Environment.Exit(-1);
        }

        return value;
    }

    
}