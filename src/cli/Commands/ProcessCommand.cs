using Arma.Demo.Core.Processing;
using System.CommandLine;

namespace CloudCli.Commands;
public class ProcessCommand : CliCommand
{
    public ProcessCommand() : base(
        "process",
        "Demonstrate cross-service coordination",
        new Func<string, Intent, Task>(Call),
        new()
        {
            new Option<Intent>(
                new[] { "--intent", "-i" },
                getDefaultValue: () => Intent.Acquire,
                description: "The package to submit to the API"
            )
        }
    ) { }

    static Task Call(string server, Intent intent)
    {
        Console.WriteLine($"Submitting package for {intent.ToActionString()}");
        return Task.CompletedTask;
    }
}