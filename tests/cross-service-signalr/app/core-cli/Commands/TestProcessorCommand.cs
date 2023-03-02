using Arma.Demo.Core.Processing;
using System.CommandLine;

namespace CoreCli.Commands;
public class TestProcessorCommand : CliCommand
{
    public TestProcessorCommand() : base(
        "test",
        "Test SignalR connection to processor API",
        new Func<string, Task>(Call)
    ) { }

    static async Task Call(string root)
    {
        await using ProcessorClient processor = new($"{root}processor", Output);
        await processor.Initialize();

        string endpoint = $"{root}api/ping/{processor.Key}";
        Console.WriteLine($"Sending request to {endpoint}");
        HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(endpoint);
        Console.WriteLine($"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
    }

    static void Output(string message) => Console.WriteLine(message);
}