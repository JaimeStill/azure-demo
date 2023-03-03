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
        new ProcessCommand().Build()
    };

    public static RootCommand BuildRootCommand(this List<Command> commands)
    {
        var root = new RootCommand("Processor Test CLI");

        commands.ForEach(root.AddCommand);
        return root;
    }
}