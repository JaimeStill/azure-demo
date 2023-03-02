using CoreCli.Commands;
using System.CommandLine;

namespace CoreCli;
public static class CommandApp
{
    public static RootCommand Initialize() =>
        BuildCommands()
            .BuildRootCommand();

    static List<Command> BuildCommands() => new()
    {
        new TestProcessorCommand().Build()
    };

    public static RootCommand BuildRootCommand(this List<Command> commands)
    {
        var root = new RootCommand("Processor Test CLI");

        root.AddGlobalOption(new Option<string>(
            new[] { "--root", "-r" },
            getDefaultValue: () => "http://localhost:5000/",
            description: "The root API endpoint"
        ));

        commands.ForEach(root.AddCommand);
        return root;
    }
}