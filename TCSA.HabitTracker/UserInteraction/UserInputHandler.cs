using Spectre.Console;
using System.Globalization;
using System.Numerics;

namespace TCSA.HabitTracker.UserInput
{
    internal static class UserInputHandler
    {
        internal static string GetDate(string? defaultDate = null)
        {

            string dateFormat = "dd-MM-yyyy";

            if (string.IsNullOrWhiteSpace(defaultDate))
            {
                defaultDate = DateTime.Now.ToString(dateFormat);
            }
            else if (!IsDateFormatted(defaultDate, dateFormat)) 
            {
                throw new ArgumentException("Wrong Default Date Format");
            }


            string userInput = string.Empty;
            AnsiConsole.MarkupLine("[bold yellow]Press Enter to insert the [bold green](Default Date)[/][/]");
            do
            {
                userInput = AnsiConsole.Ask($"[yellow]Insert a date in the format [bold blue]{dateFormat}[/][/]", defaultDate);


            } while (!IsDateFormatted(userInput, dateFormat));

            return userInput;
        }

        private static bool IsDateFormatted(string date, string targetFormat) => DateTime.TryParseExact(date, targetFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

        internal static T ListSelectionPrompt<T>(string title, List<T> values) where T : notnull
        {
            if (values is null) throw new ArgumentNullException(nameof(values));
            if (values.Count == 0) throw new ArgumentException(nameof(values.Count));

            return AnsiConsole.Prompt(
                new SelectionPrompt<T>()
                .Title(title)
                .AddChoices(values)
                );
        }

        internal static T GetNonZeroPositiveNumber<T>(string prompt = "Enter Positive Number") where T : INumber<T>
        {
            T userInput = T.Zero;
            do
            {
                userInput = AnsiConsole.Ask<T>(prompt);
                if (userInput <= T.Zero)
                    AnsiConsole.MarkupLine("[red]Enter only [bold]Positive[/] numbers[/]");

            } while (userInput <= T.Zero);

            return userInput;
        }
        internal static T GetNonZeroPositiveNumber<T>(T defaultValue, string prompt = "Enter Positive Number") where T : INumber<T>
        {
            T userInput = T.Zero;
            do
            {
                userInput = AnsiConsole.Ask<T>(prompt, defaultValue);
                if (userInput <= T.Zero)
                    AnsiConsole.MarkupLine("[red]Enter only [bold]Positive[/] numbers[/]");

            } while (userInput <= T.Zero);

            return userInput;
        }

        
    }
}
