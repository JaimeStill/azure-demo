using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;
using CoreCli.Clients;
using System.CommandLine;

namespace CoreCli.Commands;
public class SyncCommand : CliCommand
{
    public SyncCommand(): base(
        "sync",
        "Establish a SyncServer session for testing",
        new Func<string, Guid, bool, Task>(Call),
        new()
        {
            new Option<string>(
                new string[] { "--endpoint", "-e" },
                getDefaultValue: () => "http://localhost:5100/processor",
                description: "The SyncServer processor endpoint"
            ),
            new Option<Guid>(
                new string[] { "--key", "-k" },
                getDefaultValue: Guid.NewGuid,
                description: "The SyncServer group to connect to"
            ),
            new Option<bool>(
                new string[] { "--listen", "-l" },
                getDefaultValue: () => false,
                description: "Indicates whether the CLI process is a passive listener, or actively sends Sync data"
            )
        }
    ) { }

    static async Task Call(string endpoint, Guid key, bool listen)
    {
        bool exit = false;
        await using SyncClient sync = new(endpoint);

        static Task Output(SyncMessage<Package> message)
        {
            Console.WriteLine(message.Message);
            return Task.CompletedTask;
        }

        sync.OnInitialize
            = sync.OnNotify
            = Output;

        sync.OnComplete = async (SyncMessage<Package> message) =>
        {
            Console.WriteLine(message.Message);
            await sync.Leave(key);
            exit = true;
        };

        await sync.Connect();
        await sync.Join(key);

        if (!listen)
        {
            Package package = new()
            {
                Key = key,
                Name = "Sync Package",
                Intent = Intent.Approve,
                Resources = new()
                {
                    new() { Key = Guid.NewGuid(), Name = "Resource A" }
                }
            };

            SyncMessage<Package> message = new()
            {
                Id = Guid.NewGuid(),
                Key = key,
                Message = $"Initializing package {package.Name}",
                Data = package
            };

            await sync.Initialize(message);
            
            message.Message = $"Package {package.Name} has been reviewed and is pending approval";

            await sync.Notify(message);

            message.Message = $"Package {package.Name} has been approved and is now complete";

            await sync.Complete(message);
        }
        else
        {
            sync.OnComplete = async (SyncMessage<Package> message) =>
            {
                await sync.Leave(key);
                exit = true;
            };

            Console.WriteLine("Listening for SyncServer events.");

            while (!exit) { }
        }
    }
}