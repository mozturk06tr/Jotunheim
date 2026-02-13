using System.CommandLine;
using Spectre.Console.Cli;
using Shared.Domain;
using Shared.Infrastructure;
internal sealed class Cli
{
    internal static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Jotunheim CLI");
        // Add subcommands here, e.g.:
        // rootCommand.AddCommand(new CreatePortfolioCommand());
        // rootCommand.AddCommand(new AddPositionCommand());
        await rootCommand.InvokeAsync(args);
    }
}