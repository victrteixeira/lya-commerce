namespace Commerce.Security.Helpers;

public static class EnvironmentVariable
{
    public static string GetRequiredEnvironmentVariable(string name)
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