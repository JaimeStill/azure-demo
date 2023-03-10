using Arma.Demo.Core.Processing;
using System.CommandLine;
using CloudCli.Clients;
using System.Net.Http.Json;
using Arma.Demo.Core.Sync;

namespace CloudCli.Commands;
public class ProcessCommand : CliCommand
{
    public ProcessCommand() : base(
        "process",
        "Simulate processing a package across API services",
        new Func<string, string, string, Intent, Task>(Call),
        new()
        {
            new Option<string>(
                new string[] { "--api", "-a" },
                getDefaultValue: () => "https://jps-core-api.azurewebsites.net/api/process",
                description: "The Core API process endpoint"
            ),
            new Option<string>(
                new string[] { "--sync", "-s" },
                getDefaultValue: () => "https://jps-sync.azurewebsites.net/processor",
                description: "The SyncServer processor endpoint"
            ),
            new Option<Intent>(
                new string[] { "--intent", "-i" },
                getDefaultValue: () => Intent.Approve,
                description: "The intent of the simulated package"
            )
        }
    ) { }

    static async Task Call(string server, string api, string sync, Intent intent)
    {
        bool exit = false;
        await using ProcessorClient processor = new(sync);

        processor.OnComplete.Set(async (SyncMessage<Package> message) =>
        {
            Console.WriteLine(message.Message);
            await processor.Leave(message.Key);
            exit = true;
        });

        processor.OnReject.Set(async (SyncMessage<Package> message) =>
        {
            Console.WriteLine(message.Message);
            await processor.Leave(message.Key);
            exit = true;
        });

        Console.WriteLine($"Generating {intent.ToActionString()} Package");
        Package package = GeneratePackage(intent);

        await processor.Connect();
        await processor.Join(package.Key);

        string endpoint = $"{api}";
        Console.WriteLine($"Sending package {package.Name} to {endpoint}");
        HttpClient client = new();
        HttpResponseMessage response = await client.PostAsJsonAsync(endpoint, package);
        Console.WriteLine($"Package processing execution {(response.IsSuccessStatusCode ? "succeeded" : "failed")}");

        while (!exit) { }
    }

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
            Name = "CLI Approval Package",
            Intent = Intent.Approve,
            Resources = GenerateResources()
        };

    static Package Acquisition() =>
        new()
        {
            Key = Guid.NewGuid(),
            Name = "CLI Acquisition Package",
            Intent = Intent.Acquire,
            Resources = GenerateResources()
        };

    static Package Transfer() =>
        new()
        {
            Key = Guid.NewGuid(),
            Name = "CLI Transfer Package",
            Intent = Intent.Transfer,
            Resources = GenerateResources()
        };

    static Package Destruction() =>
        new()
        {
            Key = Guid.NewGuid(),
            Name = "CLI Destruction Package",
            Intent = Intent.Destroy,
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