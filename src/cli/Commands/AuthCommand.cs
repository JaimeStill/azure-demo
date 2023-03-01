using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace CloudCli.Commands;
class AuthSettings
{
    public string? ClientId { get; set; }
    public string? TenantId { get; set; }
    public string[]? Scopes { get; set; }
}

public class AuthCommand : CliCommand
{
    public AuthCommand() : base(
        "auth",
        "Demonstrate Azure AD API Authentication / Authorization",
        new Func<string, Task>(Call)
    ) { }

    static async Task Call(string server)
    {
        Console.WriteLine("Retrieving Auth Settings...");
        IConfiguration config = BuildConfiguration();

        AuthSettings? settings = config
            .GetSection("AuthSettings")
            .Get<AuthSettings>();

        if (settings is null)
            throw new Exception("AuthSettings is missing from configuration");

        Console.WriteLine("Initializing Public Client Application...");
        IPublicClientApplication app = BuildClientApp(settings);

        Console.WriteLine("Authenticating...");

        AuthenticationResult result = await Authenticate(app, settings);

        Console.WriteLine("Calling Restricted API Endpoint...");
        HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new("Bearer", result.AccessToken);

        HttpResponseMessage response = await client.GetAsync($"{server}auth");
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

    static async Task<AuthenticationResult> Authenticate(IPublicClientApplication app, AuthSettings settings)
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
            return await AuthenticateViaDevCode(app, settings);
        }
    }

    static async Task<AuthenticationResult> AuthenticateViaDevCode(IPublicClientApplication app, AuthSettings settings)
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