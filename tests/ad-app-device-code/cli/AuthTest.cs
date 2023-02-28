using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace DevCodeCli;
class AuthSettings
{
    public string? ClientId { get; set; }
    public string? TenantId { get; set; }
    public string[]? Scopes { get; set; }
}

public class AuthTest
{
    static string endpoint = "https://jps-devcode-api.azurewebsites.net/api/auth/";

    public static async Task Execute()
    {
        IConfiguration config = BuildConfiguration();

        AuthSettings? settings = config
            .GetSection("AuthSettings")
            .Get<AuthSettings>();

        if (settings is null)
            throw new Exception("AuthSettings is missing from configuration");

        IPublicClientApplication app = BuildClientApp(settings);

        AuthenticationResult result = await Acquire(app, settings);

        HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new ("Bearer", result.AccessToken);

        HttpResponseMessage response = await client.GetAsync(endpoint);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

    static IPublicClientApplication BuildClientApp(AuthSettings settings) =>
        PublicClientApplicationBuilder
            .Create(settings.ClientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
            .WithDefaultRedirectUri()
            .Build();

    static async Task<AuthenticationResult> Acquire(IPublicClientApplication app, AuthSettings settings)
    {
        try
        {
            var accounts = await app.GetAccountsAsync();

            return await app
                .AcquireTokenSilent(settings.Scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            return await AcquireViaDevCode(app, settings);
        }
    }

    static async Task<AuthenticationResult> AcquireViaDevCode(IPublicClientApplication app, AuthSettings settings)
    {
        try
        {
            AuthenticationResult result = await app
                .AcquireTokenWithDeviceCode(
                    settings.Scopes,
                    deviceCodeResult =>
                    {
                        Console.WriteLine(deviceCodeResult.Message);
                        return Task.FromResult(0);
                    }
                )
                .ExecuteAsync();

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}