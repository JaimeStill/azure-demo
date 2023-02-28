using CloudCli.Commands;
using System.CommandLine;

namespace CloudCli;
public static class CommandApp
{
    public static RootCommand Initialize() =>
        BuildCommands()
            .BuildRootCommand();

    static List<Command> BuildCommands() => new()
    {
        new AuthCommand().Build(),
        new ErrorCommand().Build(),
        new SecretCommand().Build()
    };

    public static RootCommand BuildRootCommand(this List<Command> commands)
    {
        var root = new RootCommand("Cloud CLI");

        root.AddGlobalOption(new Option<string>(
            new[] { "--server", "-s" },
            getDefaultValue: () => "https://jps-core-api.azurewebsites.net/api/",
            description: "The root of the API server"
        ));

        commands.ForEach(root.AddCommand);
        return root;
    }
}