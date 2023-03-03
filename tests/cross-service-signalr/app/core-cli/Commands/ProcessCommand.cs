using Arma.Demo.Core.Processing;
using System.CommandLine;
using CoreCli.Clients;
using System.Net.Http.Json;

namespace CoreCli.Commands;
public class ProcessCommand : CliCommand
{
    public ProcessCommand() : base(
        "process",
        "Simulate processing a package across API services",
        new Func<string, string, Intent, Task>(Call),
        new()
        {
            new Option<string>(
                new string[] { "--api", "-a" },
                getDefaultValue: () => "http://localhost:5001/api/process",
                description: "The Core API process endpoint"
            ),
            new Option<string>(
                new string[] { "--hub", "-h" },
                getDefaultValue: () => "http://localhost:5000/processor",
                description: "The processor API hub endpoint"
            ),
            new Option<Intent>(
                new string[] { "--intent", "-i" },
                getDefaultValue: () => Intent.Approve,
                description: "The intent of the simulated package"
            )
        }
    ) { }

    static async Task Call(string api, string hub, Intent intent)
    {
        Console.WriteLine($"Generating {intent.ToActionString()} Package");
        Package package = GeneratePackage(intent);

        await using ProcessorClient processor = new(package.Key, hub, Output);
        await processor.Initialize();

        string endpoint = $"{api}";
        Console.WriteLine($"Sending package {package.Name} to {endpoint}");
        HttpClient client = new();
        HttpResponseMessage response = await client.PostAsJsonAsync(endpoint, package);
        Console.WriteLine($"Package processing execution {(response.IsSuccessStatusCode ? "succeeded" : "failed")}");
    }

    static void Output(string message) => Console.WriteLine(message);

    static Package GeneratePackage(Intent intent) => intent switch
    {
        Intent.Approve => Approval(),
        Intent.Acquire => Acquisition(),
        Intent.Transfer => Transfer(),
        Intent.Destroy => Destruction(),
        _ => throw new ArgumentOutOfRangeException(
            nameof(intent),
            intent,
            "An unexpected intent was provided and no associated package could be found"
        )
    };

    static Package Approval() =>
        new()
        {
            Key = Guid.NewGuid(),
            Name = "Approval Package",
            Resources = GenerateResources()
        };

    static Package Acquisition() =>
        new()
        {
            Key = Guid.NewGuid(),
            Name = "Acquisition Package",
            Resources = GenerateResources()
        };

    static Package Transfer() =>
        new()
        {
            Key = Guid.NewGuid(),
            Name = "Transfer Package",
            Resources = GenerateResources()
        };

    static Package Destruction() =>
        new()
        {
            Key = Guid.NewGuid(),
            Name = "Destruction Package",
            Resources = GenerateResources()
        };

    static List<Resource> GenerateResources() => new()
    {
        new() {
            Key = Guid.NewGuid(),
            Name = "Training Plan"
        },
        new() {
            Key = Guid.NewGuid(),
            Name = "R&D Proposal"
        }
    };
}