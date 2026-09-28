using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Data;
using System.Globalization;
using TCSA.HabitTracker.DataAccessObjects;
using TCSA.HabitTracker.Models;
using TCSA.HabitTracker.UserInput;
using TCSA.HabitTracker.UserInteraction;

namespace TCSA.HabitTracker.Services
{
    internal class HabitEntryService
    {
        private readonly HabitEntryDao _habitEntryDao;
        private readonly IHabitUnitReader _habitUnitReader;
        public HabitEntryService(HabitEntryDao habitEntryDao, IHabitUnitReader habitUnitReader)
        {
            _habitEntryDao = habitEntryDao;
            _habitUnitReader = habitUnitReader;
        }

        public void ViewAllEntries()
        {
            List<HabitEntry> entries = _habitEntryDao.GetAll();
            if (entries.Count == 0)
            {
                UserMessages.Warning("No Records available");
                return;
            }
            var sortedByDateEntries = entries.OrderByDescending(entry => DateTime.ParseExact(entry.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture));

            var table = new Table();
            table.AddColumns("Id", "Value", "Unit", "Date");
            foreach (HabitEntry entry in sortedByDateEntries)
            {
                table.AddRow(entry.Id.ToString(), entry.Value.ToString(), entry.HabitUnit, entry.Date);
            }

            AnsiConsole.Write(table);
        }

        public void InsertHabitEntry()
        {
            List<string> habitUnitOptions = _habitUnitReader.GetAllUnits();
            habitUnitOptions.Add("Cancel Insertion");

            var promptTitle = "[bold yellow]Select the unit of the Entry[/]";
            string selectedUnit = UserInputHandler.ListSelectionPrompt(promptTitle, habitUnitOptions);

            if (selectedUnit == "Cancel Insertion")
            {
                UserMessages.NoChangesMessage();
                return;
            }

            int selectedUnitId = _habitUnitReader.GetUnitId(selectedUnit);
            if (selectedUnitId == -1)
            {
                UserMessages.Warning("Error: Unit's Id could not be found\nCheck if Unit exists");
                return;
            }


            double value = UserInputHandler.GetNonZeroPositiveNumber<double>("[bold yellow]Enter the amount of units[/]");
            string date = UserInputHandler.GetDate();

            var habitEntry = new HabitEntry()
            {
                HabitId = selectedUnitId,
                Value = value,
                Date = date,
                HabitUnit = selectedUnit,
            };

            _habitEntryDao.Insert(habitEntry);
        }

        public void DeleteEntryById()
        {
            ViewAllEntries();
            int selectedId = GetUserEntryId();

            try
            {
                HabitEntry? currentEntry = _habitEntryDao.GetById(selectedId);
                if (currentEntry is null)
                {
                    UserMessages.Warning($"Error: Could not find Entry with Id {selectedId}");
                    UserMessages.NoChangesMessage();
                    return;
                }

                var table = new Table();
                table.AddColumns("Id", "Value", "Unit", "Date");
                table.AddRow(currentEntry.Id.ToString(), currentEntry.Value.ToString(), currentEntry.HabitUnit, currentEntry.Date);

                UserMessages.Warning("Warning: The following Entry is going to be permanently [bold]Deleted[/]");
                AnsiConsole.Write(table);

                bool confirmDeletion = AnsiConsole.Confirm("[yellow]Are you sure you want to Delete this Entry?[/]", false);
                if (confirmDeletion)
                {
                    _habitEntryDao.DeleteById(selectedId);
                    AnsiConsole.MarkupLine("[green]Entry [bold]Deleted[/] successfully.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[green]Deletion Cancelled[/].");
                    UserMessages.NoChangesMessage();
                }
            } 
            catch (SqliteException ex)
            {
                UserMessages.Warning($"Database Error {ex.Message}");
                UserMessages.NoChangesMessage();
            }
        }

        public void UpdateEntryById()
        {
            ViewAllEntries();
            Console.WriteLine();

            int selectedId = GetUserEntryId();
            HabitEntry? currentEntry = _habitEntryDao.GetById(selectedId);
            if (currentEntry is null)
            {
                UserMessages.Warning($"Habit Entry with the Id: {selectedId} could not be found");
                UserMessages.NoChangesMessage();
                return;
            }

            AnsiConsole.MarkupLine("[bold yellow]Press Enter to use the [bold green](Default Value)[/][/]");
            double newValue = UserInputHandler.GetNonZeroPositiveNumber(currentEntry.Value, "Enter new Value");
            string newDate = UserInputHandler.GetDate(currentEntry.Date);

            bool doUpdateUnit = AnsiConsole.Confirm("[bold yellow]Update Unit?[/]", false);
            string? newHabitUnit;
            if (!doUpdateUnit) 
            {
                newHabitUnit = currentEntry.HabitUnit;
            }
            else
            {
                newHabitUnit = PromptForUnit(currentEntry.HabitUnit);
                if (newHabitUnit is null)
                {
                    UserMessages.Warning("Error: Unit is not available");
                    UserMessages.NoChangesMessage();
                    return;
                }
            }

            int newHabitUnitId = _habitUnitReader.GetUnitId(newHabitUnit);

            HabitEntry newEntry = new HabitEntry()
            {
                Value = newValue,
                Date = newDate,
                HabitId = newHabitUnitId,
                HabitUnit = newHabitUnit
            };

            PresentUpdateDifferencesTables(currentEntry, newEntry);

            bool confirmUpdate = AnsiConsole.Confirm("[bold yellow]Apply Update?[/]", false);
            if (confirmUpdate)
            {
                _habitEntryDao.UpdateById(selectedId, newEntry);
                AnsiConsole.MarkupLine("[bold green]Entry Successfully Updated[/]");
            }
            else
                UserMessages.NoChangesMessage();
        }

        private void PresentUpdateDifferencesTables(HabitEntry currentEntry, HabitEntry newEntry)
        {
            string[] tableHeader =
            [
                "Value",
                "HabitUnit",
                "Date"
            ];

            Console.WriteLine();

            var currentEntryTable = new Table().AddColumns(tableHeader).Title("[bold yellow]Current Entry[/]");
            currentEntryTable.AddRow(currentEntry.Value.ToString(), currentEntry.HabitUnit, currentEntry.Date);

            var newEntryTable = new Table().AddColumns(tableHeader).Title("[bold green]Updated Entry[/]");
            newEntryTable.AddRow(newEntry.Value.ToString(), newEntry.HabitUnit, newEntry.Date);

            AnsiConsole.Write(currentEntryTable);
            AnsiConsole.Write(newEntryTable);
        }

        private string? PromptForUnit(string? defaultUnit = null)
        {
            List<string> habitUnitOptions = _habitUnitReader
                .GetAllUnits()
                .OrderBy(unit => unit)
                .ToList();

            if (habitUnitOptions.Count == 0)
            {
                UserMessages.Warning("No Units available");
                return null;
            }

            if (defaultUnit is not null && habitUnitOptions.Contains(defaultUnit))
            {
                habitUnitOptions.Remove(defaultUnit);
                habitUnitOptions.Insert(0, defaultUnit);
            }

            habitUnitOptions.Add("Cancel Operation");

            var promptTitle = "[bold yellow]Select the unit of the Entry[/]";
            string selectedUnit = UserInputHandler.ListSelectionPrompt(promptTitle, habitUnitOptions);
            if (selectedUnit == "Cancel Operation")
            {
                AnsiConsole.MarkupLine("[green]Operation cancelled[/]");
                return null;
            }
            
            return selectedUnit;
        }

        private int GetUserEntryId()
        {
            return UserInputHandler.GetNonZeroPositiveNumber<int>("[yellow]Enter an Entry Id[/]");
        }
    }
}
