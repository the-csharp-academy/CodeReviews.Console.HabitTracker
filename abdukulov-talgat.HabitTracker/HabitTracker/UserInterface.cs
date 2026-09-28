using HabitTracker.Model;
using HabitTracker.Model.Db;
using Spectre.Console;

namespace HabitTracker;

public class UserInterface(SqliteRepository repository)
{
    private static readonly Dictionary<MenuAction, string> HabitActionToStringMap = new()
    {
        { MenuAction.Create, "Add habit" },
        { MenuAction.Read, "Show all habits" },
        { MenuAction.Update, "Edit Habit" },
        { MenuAction.Delete, "Delete Habit" },
        { MenuAction.Exit, "Exit" }
    };

    public void Run()
    {
        while (true)
        {
            try
            {
                AnsiConsole.Clear();
                MenuAction choice = AnsiConsole.Prompt(new SelectionPrompt<MenuAction>()
                    .AddChoices(Enum.GetValues<MenuAction>())
                    .UseConverter(action => HabitActionToStringMap[action]));

                switch (choice)
                {
                    case MenuAction.Create:
                        CreateHabit();
                        break;
                    case MenuAction.Read:
                        ShowAllHabits();
                        break;
                    case MenuAction.Update:
                        UpdateHabit();
                        break;
                    case MenuAction.Delete:
                        DeleteHabit();
                        break;
                    case MenuAction.Exit:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurs: {ex.Message}[/]");
            }
        }
    }

    private void CreateHabit()
    {
        (string name, int quantity, DateTime occurence) = AskHabitData();
        repository.CreateHabit(new Habit(-1, name, quantity, occurence));
        PrintSuccessMessage("New Habit was created!");
        WaitForUserInput();
    }

    private void ShowAllHabits()
    {
        Table table = new();

        table.AddColumns("Id", "Name", "Quantity", "Occurence At");

        foreach (Habit habit in repository.GetHabits())
        {
            table.AddRow(
                habit.Id.ToString(),
                habit.Name,
                habit.Quantity.ToString("F2"),
                habit.OccurrenceAt.ToString("F"));
        }

        AnsiConsole.Write(table);
        WaitForUserInput();
    }

    private void UpdateHabit()
    {
        // ****** Visual Artifacts *******
        // AnsiConsole.Prompt(new SelectionPrompt<Habit>()
        //     .WrapAround()
        //     .AddChoices(Repository.GetHabits())
        //     .UseConverter(h => $"Id: {h.Id}\tName: {h.Name}")
        //     .EnableSearch()
        //     .SearchPlaceholderText("Search ( Have some visual glitches ): "));

        Habit habit = AskTillSuccess("Which habit you want edit (Enter Id)?");
        (string name, int quantity, DateTime occurence) = AskHabitData();
        int count = repository.UpdateHabit(new Habit(habit.Id, name, quantity, occurence));
        PrintSuccessMessage($"Habit was updated. {count} habit was updated");
        WaitForUserInput();
    }

    private void DeleteHabit()
    {
        Habit toDelete = AskTillSuccess("Which habit you want delete (Enter Id)?");
        int count = repository.DeleteHabit(toDelete.Id);
        PrintSuccessMessage($"Habit was deleted. {count} habit was deleted");
        WaitForUserInput();
    }

    private void WaitForUserInput()
    {
        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void PrintSuccessMessage(string msg)
    {
        AnsiConsole.MarkupLine($"[green]{msg}[/]");
    }

    private Habit AskTillSuccess(string question)
    {
        Habit? habit;
        while (true)
        {
            int id = AnsiConsole.Ask<int>(question);
            habit = repository.GetHabits(id).FirstOrDefault();
            if (habit == null)
            {
                AnsiConsole.MarkupLine($"[red]Habit with id {id} doesn't exist[/]");
                continue;
            }

            break;
        }

        return habit;
    }

    private (string Name, int Quantity, DateTime OccurenceAt) AskHabitData()
    {
        string name = AnsiConsole.Ask<string>("Enter a new habit name: ");
        int quantity = AnsiConsole.Ask<int>("How many? ");
        DateTime occurence =
            AnsiConsole.Ask<DateTime>("When it happens, date and/or time? (YYYY-MM-DD HH:MM:SS for example): ");
        return (name, quantity, occurence);
    }
}