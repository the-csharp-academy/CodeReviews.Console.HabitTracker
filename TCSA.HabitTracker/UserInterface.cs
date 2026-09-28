using Spectre.Console;
using TCSA.HabitTracker.Services;
using static TCSA.HabitTracker.Enums.MainMenuEnums;
namespace TCSA.HabitTracker
{
    internal class UserInterface
    {
        private readonly HabitEntryService _recordService;
        private readonly HabitService _habitService;
        public UserInterface(HabitEntryService recordService, HabitService habitService)
        {
            _recordService = recordService;
            _habitService = habitService;
        }
        public void MainMenu()
        {
            bool isAppRunning = true;
            while (isAppRunning)
            {
                Console.Clear();
                DisplayAppFiglet();

                var mainMenuOption = AnsiConsole.Prompt
                    (
                        new SelectionPrompt<MainMenuOptions>()
                        .Title("What would you like to do?")
                        .AddChoices(Enum.GetValues<MainMenuOptions>())
                        .UseConverter(MenuOptionFormatter)
                    );

                switch (mainMenuOption)
                {
                    case MainMenuOptions.EntriesMenu:
                        EntriesSubMenu();
                        break;
                    case MainMenuOptions.HabitsMenu:
                        HabitSubMenu();
                        break;
                    case MainMenuOptions.Exit:
                        AnsiConsole.MarkupLine("[bold yellow]Leaving Habit Logger. See ya!![/]");
                        isAppRunning = false;
                        break;
                }

                WaitForAnyKey();
            }
        }

        private void HabitSubMenu()
        {
            var menuOption = AnsiConsole.Prompt(
                new SelectionPrompt<HabitMenuOptions>()
                    .Title("What would you like to do?")
                    .AddChoices(Enum.GetValues<HabitMenuOptions>())
                    .UseConverter(MenuOptionFormatter));

            switch (menuOption)
            {
                case HabitMenuOptions.ViewHabits:
                    _habitService.ViewAllHabits();
                    break;
                case HabitMenuOptions.InsertHabit:
                    _habitService.InsertHabit();
                    break;
                case HabitMenuOptions.UpdateHabit:
                    _habitService.UpdateHabit();
                    break;
                case HabitMenuOptions.DeleteHabit:
                    _habitService.DeleteHabit();
                    break;
                case HabitMenuOptions.Exit:
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold red]Invalid Input[/]");
                    break;
            }
        }

        private void EntriesSubMenu()
        {
            var menuOption = AnsiConsole.Prompt(
                new SelectionPrompt<EntryMenuOptions>()
                    .Title("What would you like to do?")
                    .AddChoices(Enum.GetValues<EntryMenuOptions>())
                    .UseConverter(MenuOptionFormatter));

            switch (menuOption)
            {
                case EntryMenuOptions.ViewAllEntries:
                    _recordService.ViewAllEntries();
                    break;
                case EntryMenuOptions.InsertEntry:
                    _recordService.InsertHabitEntry();
                    break;
                case EntryMenuOptions.UpdateEntry:
                    _recordService.UpdateEntryById();
                    break;
                case EntryMenuOptions.DeleteEntry:
                    _recordService.DeleteEntryById();
                    break;
                case EntryMenuOptions.Exit:
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold red]Invalid Input[/]");
                    break;
            }
        }

        private void DisplayAppFiglet()
        {
            var menuFiglet = new FigletText("Habit Tracker")
            {
                Color = Color.Blue,
                Justification = Justify.Center,
            };
            AnsiConsole.Write(menuFiglet);
        }

        private void WaitForAnyKey()
        {
            AnsiConsole.MarkupLine("[grey]Press Any Key To Continue...[/]");
            Console.ReadKey();
        }
    }
}
