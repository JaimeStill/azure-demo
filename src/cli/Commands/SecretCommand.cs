namespace CloudCli.Commands;
public class SecretCommand : CliCommand
{
    public SecretCommand() : base(
        "secret",
        "Demonstrate retrieving a Azure Key Vault secret",
        new Func<string, Task>(
            async (server) => await Call(server)
        )
    ) { }

    static async Task Call(string server)
    {
        try
        {
            string endpoint = $"{server}secret";
            Console.WriteLine($"Sending request to {endpoint}");
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(endpoint);
            Console.WriteLine(await response.Content.ReadAsStringAsync());        
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}