using HabitLogger.Entities;
using HabitLogger.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace HabitLogger.UI
{
    internal class ManageLogEntriesUI
    {
        private readonly HabitRepository _habitRepository;
        private List<Habit> _habits;
        private List<HabitLogEntry> _habitLogs;

        public ManageLogEntriesUI(HabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
            RefreshHabitLogs();
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();

                ViewHabitLogTable(_habitLogs);

                var choise = AnsiConsole.Prompt(new SelectionPrompt<LogMenuOptions>()
                    .Title("What do you want to do?")
                    .UseConverter(x => x switch
                    {
                        LogMenuOptions.CreateLog => "Create a new log Entry",
                        LogMenuOptions.EditLog => "Edit a log entry",
                        LogMenuOptions.DeleteLog => "Delete a log entry",
                        LogMenuOptions.Back => "Back to previous menu..."
                    })
                    .AddChoices(Enum.GetValues<LogMenuOptions>()));

                switch (choise)
                {
                    case LogMenuOptions.CreateLog:
                        CreateHabitLog();
                        break;
                    case LogMenuOptions.EditLog:
                        EditHabitLog();
                        break;
                    case LogMenuOptions.DeleteLog:
                        DeleteHabitLog();
                        break;
                    case LogMenuOptions.Back:
                        Console.Clear();
                        return;
                }
            }
        }
        public void CreateHabitLog()
        {
            Console.Clear();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<Habit>().Title("Please select a habit:")
                .UseConverter(habit => $"{habit.Name} - {habit.Measurement}")
                .AddChoices(_habits));

            var quantity = AnsiConsole.Prompt(
                new TextPrompt<int>($"Please enter the quantiy in [yellow]{choice.Measurement}:[/]")
                .ValidationErrorMessage("[red]Please enter a valid number[/]"));


            var time = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("When did you complete the habit? [yellow](mm/dd/yyyy HH:mm)[/] or Leave blank to use the current ate and time")
                .ValidationErrorMessage("[red]Please enter a valid date and time.[/]")
                .AllowEmpty());

            while (time > DateTime.Now)
            {
                time = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("The log date cannot be in the future, please select a date and time equal or less than the current time [yellow](mm/dd/yyyy HH:mm)[/] or Leave blank to use the current ate and time")
                    .ValidationErrorMessage("[red]Please enter a valid date and time.[/]")
                    .AllowEmpty());
            }

            if (time == DateTime.MinValue)
            {
                time = DateTime.Now;
            }

            _habitRepository.CreateHabitLogEntry(choice, quantity, time);
            RefreshHabitLogs();
        }


        public void ViewHabitLogTable(List<HabitLogEntry> habitLogs)
        {
            RefreshHabitLogs();

            var table = new Table()
                .AddColumn("Id")
                .AddColumn("Habit")
                .AddColumn("Quantity")
                .AddColumn("Measurement")
                .AddColumn("TimeStamp");


            habitLogs.ForEach(x =>
            {
                table.AddRow(x.Id.ToString(), x.Habit.Name, x.Quantity.ToString(), x.Habit.Measurement, x.TimeEntry.ToString());
            });

            AnsiConsole.Write(table);
        }

        public void EditHabitLog()
        {
            var choice = AnsiConsole.Prompt(new SelectionPrompt<HabitLogEntry>()
                .Title("Which Log entry would you like to edit?")
                .UseConverter(x => $"{x.Id} - {x.Habit.Name} - {x.Habit.Measurement} - {x.Quantity} - {x.TimeEntry}")
                .AddChoices(_habitLogs));

            var newHabit = AnsiConsole.Prompt(new SelectionPrompt<Habit>()
                .Title("Please select a habit")
                .UseConverter(x => $"{x.Name} - {x.Measurement}")
                .AddChoices(_habits));

            var newQuantityPrompt = AnsiConsole.Prompt(new TextPrompt<string>
                ("Please enter a new quantity or leave blank to keep the current value")
                .AllowEmpty()
                .Validate(x =>
                {
                    if (string.IsNullOrWhiteSpace(x))
                        return ValidationResult.Success();

                    return int.TryParse(x, out int quantity) && quantity >= 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Please enter a valid qunatity (o or greater).");
                }));

            var newTimeStamp = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Please enter a new Date and Time or leave blacnk to keep the current values:")
                .AllowEmpty());

            _habitRepository.EditHabitLogEntry(choice, newHabit, newQuantityPrompt, newTimeStamp);
            RefreshHabitLogs();
        }
        public void DeleteHabitLog()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<HabitLogEntry>()
                    .Title("Please select a log entry to delete")
                    .UseConverter(x => $"{x.Id} - {x.Habit.Name} - {x.Habit.Measurement} - {x.Quantity} - {x.TimeEntry}")
                    .AddChoices(_habitLogs));

                var confirmation = AnsiConsole.Prompt(
                    new SelectionPrompt<DeleteConfirmation>()
                    .Title($"Are you sure you want to delete {choice.Habit.Name} with time entry; {choice.TimeEntry.ToString()}")
                    .AddChoices(Enum.GetValues<DeleteConfirmation>()));


                switch (confirmation)
                {
                    case DeleteConfirmation.No:
                        break;
                    case DeleteConfirmation.Yes:
                        _habitRepository.DeleteLogEntry(choice);
                        RefreshHabitLogs();
                        return;
                }
            }
        }

        public void RefreshHabitLogs()
        {
            _habits = _habitRepository.GetHabits();
            _habitLogs = _habitRepository.GetHabitLogEntries();
        }
    }

    enum LogMenuOptions
    {
        CreateLog,
        EditLog,
        DeleteLog,
        Back
    }

    enum DeleteConfirmation
    {
        Yes,
        No
    }
}
