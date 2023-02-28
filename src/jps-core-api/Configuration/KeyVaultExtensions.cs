using Azure.Identity;

namespace Core.Configuration;

public static class KeyVaultExtensions
{
    public static void InitializeKeyVault(this WebApplicationBuilder builder, string vaultKey = "VaultName")
    {
        Uri vaultUri = new($"https://{builder.Configuration[vaultKey]}.vault.azure.net/");

        builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
    }
}