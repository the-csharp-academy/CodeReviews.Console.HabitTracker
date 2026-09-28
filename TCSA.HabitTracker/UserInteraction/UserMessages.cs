using Spectre.Console;

namespace TCSA.HabitTracker.UserInteraction
{
    internal static class UserMessages
    {
        public static void NoChangesMessage() => AnsiConsole.MarkupLine("[bold green]No changes were made.[/]\n");
        public static void Warning(string message) => AnsiConsole.MarkupLine("[bold red]"+message+"[/]");

    }
}
