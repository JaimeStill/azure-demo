using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace CloudCli.Commands;

public abstract class CliCommand
{
    readonly string name;
    readonly string description;
    readonly Delegate @delegate;
    readonly List<Option>? options;

    public CliCommand(string name, string description, Delegate @delegate, List<Option>? options = null)
    {
        this.name = name;
        this.description = description;
        this.@delegate = @delegate;
        this.options = options;
    }

    public Command Build()
    {
        Command command = new(name, description)
        {
            Handler = CommandHandler.Create(@delegate)
        };

        options?.ForEach(command.AddOption);

        return command;
    }
}