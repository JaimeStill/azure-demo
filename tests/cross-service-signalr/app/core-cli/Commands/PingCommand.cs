using System.CommandLine;
using CoreCli.Clients;

namespace CoreCli.Commands;
public class PingCommand : CliCommand
{
    public PingCommand() : base(
        "ping",
        "Ping SignalR connection to processor API",
        new Func<string, Task>(Call),
        new()
        {
            new Option<string>(
                new string[] { "--root", "-r" },
                getDefaultValue: () => "http://localhost:5000/",
                description: "The root processor API endpoint"                
            )
        }
    ) { }

    static async Task Call(string root)
    {
        await using PingClient ping = new($"{root}processor", Output);
        await ping.Initialize();

        string endpoint = $"{root}api/ping/{ping.Key}";
        Console.WriteLine($"Sending request to {endpoint}");
        HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(endpoint);
        Console.WriteLine($"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
    }

    static void Output(string message) => Console.WriteLine(message);
}