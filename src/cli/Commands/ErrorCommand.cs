namespace CloudCli.Commands;
public class ErrorCommand : CliCommand
{
    public ErrorCommand() : base(
        "error",
        "Demonstrate JsonExceptionMiddleware error handling",
        new Action<string>(
            async (server) => await Call(server)
        )
    ) { }

    static async Task Call(string server)
    {
        try
        {
            string endpoint = $"{server}error";
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