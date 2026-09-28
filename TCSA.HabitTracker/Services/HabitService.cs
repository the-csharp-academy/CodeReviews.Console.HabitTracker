using Microsoft.Data.Sqlite;
using Spectre.Console;
using TCSA.HabitTracker.DataAccessObjects;
using TCSA.HabitTracker.Models;
using TCSA.HabitTracker.UserInput;
using TCSA.HabitTracker.UserInteraction;

namespace TCSA.HabitTracker.Services
{
    internal class HabitService
    {
        private readonly HabitDao _habitDao;
        public HabitService(HabitDao habitDao)
        {
            _habitDao = habitDao;
        }

        public void ViewAllHabits()
        {
            List<Habit> habits = _habitDao.GetAll();
            if (habits.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No habits available[/]");
                return;
            }

            var table = new Table();
            table.AddColumns("Unit", "Description");
            foreach (Habit habit in habits)
            {
                table.AddRow(habit.Unit, habit.Description);
            }
            AnsiConsole.Write(table);
        }
        public void InsertHabit()
        {
            string habitUnit = AnsiConsole.Ask<string>("[bold yellow]Enter the Habit's [bold cyan]Unit[/][/]:");
            string habitDescription = AnsiConsole.Ask<string>("[bold yellow]Enter the Habit's [bold cyan]Description[/][/]:");
            var habit = new Habit()
            {
                Unit = habitUnit,
                Description = habitDescription
            };

            try
            {
                _habitDao.Insert(habit);
                AnsiConsole.MarkupLine("[green]Habit inserted Successfully[/]");
            }
            catch (SqliteException ex)
            {
                string errorMessage = ex.Message;
                if (ex.SqliteErrorCode == 19) errorMessage = "Unit Values must be unique.";

                AnsiConsole.MarkupLine($"[bold red]Error updating the database:\n\t[bold orange1]{errorMessage}[/][/]");
                UserMessages.NoChangesMessage();
            }
        }
        public void UpdateHabit()
        {
            List<string> habitUnitOptions = GenerateUnitSelectionOptions();

            string promptTitle = "[bold yellow]Select the unit form the Habit you wish to [bold orange1]Update[/][/]\n[bold cyan]Cancel Operation[/] will not change saved Data.";
            string selectedUnit = UserInputHandler.ListSelectionPrompt(promptTitle, habitUnitOptions);
            if (selectedUnit == "Cancel Operation")
            {
                UserMessages.NoChangesMessage();
                return;
            }

            Habit? currentHabit = _habitDao.GetByUnit(selectedUnit);
            if (currentHabit is null)
            {
                AnsiConsole.MarkupLine($"[bold red]Error: Habit with the Unit {selectedUnit} was not found");
                return;
            }

            AnsiConsole.MarkupLine($"[bold yellow]In case you don't want to change the Unit or Description\n[cyan]Press Enter[/]to use the original value[/]");

            string newUnit = AnsiConsole.Ask($"Enter [green]new Unit[/]:", currentHabit.Unit);
            string newDescription = AnsiConsole.Ask($"Enter [green]new Description[/]:", currentHabit.Description);

            if (newUnit == currentHabit.Unit &&  newDescription == currentHabit.Description)
            {
                AnsiConsole.MarkupLine("[green]No changes were made.[/]");
                return;
            }

            _habitDao.UpdateByUnit(currentHabit.Unit, new Habit { Unit = newUnit, Description = newDescription });
            AnsiConsole.MarkupLine($"[bold green]Habit {currentHabit.Unit} successfully Updated[/]");
        }

        
        public void DeleteHabit()
        {
            List<string> habitUnitOptions = GenerateUnitSelectionOptions();

            string promptTitle = "[bold yellow]Select the unit form the Habit you wish to [bold orange1]Delete[/][/]\n[bold cyan]Cancel Operation[/] will cancel the operation.";
            string selectedUnit = UserInputHandler.ListSelectionPrompt(promptTitle, habitUnitOptions);
            
            if (selectedUnit == "Cancel Operation")
            {
                UserMessages.NoChangesMessage();
                return;
            }

            AnsiConsole.MarkupLine("\n[bold red]Warning: Deleting this Habit will delete every Entry related to it[/]");
            bool confirmDeletion = AnsiConsole.Confirm($"[bold blue]Are you sure you want to [bold red]DELETE[/] {selectedUnit}?[/]", false);
            if (confirmDeletion) 
            {
                _habitDao.DeleteByUnit(selectedUnit);
                AnsiConsole.MarkupLine($"[bold green]{selectedUnit} deleted Successfully[/]");
            }
            else
            {
                UserMessages.NoChangesMessage();
            }
        }

        private List<string> GenerateUnitSelectionOptions()
        {
            List<string> habitUnits = _habitDao.GetAllUnits();
            habitUnits.Add("Cancel Operation");
            return habitUnits;
        }

    }
}
