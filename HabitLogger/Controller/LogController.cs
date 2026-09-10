using HabitLogger.Models;
using HabitLogger.SQLlite;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HabitLogger.Controller;
public class LogController : BaseController
{
    private readonly LogRepository _logRepository = new();

    public void InitializeConnection()
    {
        _logRepository.InitializeBd();
    }

    public void GetAllLogs()
    {

        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Habit[/]");
        table.AddColumn("[yellow]Date[/]");
        table.AddColumn("[yellow]Quantity[/]");

        // Filtering only items of the book type
        var logs = _logRepository.GetAllLogs();

        foreach (var log in logs)
        {
            table.AddRow(
                log.Id.ToString(),
                $"[cyan]{log.Habit}[/]",
                $"[cyan]{log.Date}[/]",
                $"[cyan]{log.Quantity}[/]"
                );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("Press Any Key to Continue.");
        Console.ReadKey();

    }

    public void InsertLog()
    {

        var habit = AnsiConsole.Ask<string>("Enter the [green]habit[/] name");
        var input = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]date[/] (yyyy-MM-dd, or 't' for today):")
                .DefaultValue("t")
                .Validate(value =>
                {
                    var v = value.Trim();
                    if (v.Equals("t", StringComparison.OrdinalIgnoreCase))
                        return ValidationResult.Success();

                    return DateTime.TryParseExact(v, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid date — use yyyy-MM-dd or 't'[/]");
                }));

        DateTime date = input.Trim().Equals("t", StringComparison.OrdinalIgnoreCase)
            ? DateTime.Today
            : DateTime.ParseExact(input.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var quantity = AnsiConsole.Ask<int>("Enter the [green]quantity[/] of times you did it");

        var newLog = new Log{Id= 0,Habit= habit, Date = date.ToString("yyyy-MM-dd"), Quantity = quantity};


        _logRepository.InsertLog(newLog);
    }


    public void DeleteLog()
    {
        var logs = _logRepository.GetAllLogs();

        if(logs.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No logs available to delete.[/]");
        }

        var logToDelete = AnsiConsole.Prompt(
            new SelectionPrompt<Log>()
                .Title("Select a [red]log[/] to delete:")
                .UseConverter(b => $"Habit: {b.Habit},Date: {b.Date}, Quantity: {b.Quantity}")
                .AddChoices(logs));

        if (ConfirmDeletion(logToDelete.Date))
        {
            _logRepository.DeleteItem(logToDelete);
        }
    }


    public void UpdateLog()
    {
        var logs = _logRepository.GetAllLogs();

        if (logs.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No logs available to update.[/]");
        }

        var logToUpdate = AnsiConsole.Prompt(
            new SelectionPrompt<Log>()
                .Title("Select a [blue]log[/] to update:")
                .UseConverter(b => $"Habit: {b.Habit},Date: {b.Date}, Quantity: {b.Quantity}")
                .AddChoices(logs));

        var habit = AnsiConsole.Ask<string>("Enter the [green]habit[/] name");
        var input = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter the [green]date[/] (yyyy-MM-dd, or 't' for today):")
            .DefaultValue("t")
            .Validate(value =>
            {
                var v = value.Trim();
                if (v.Equals("t", StringComparison.OrdinalIgnoreCase))
                    return ValidationResult.Success();

                return DateTime.TryParseExact(v, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Invalid date — use yyyy-MM-dd or 't'[/]");
            }));
        DateTime date = input.Trim().Equals("t", StringComparison.OrdinalIgnoreCase)
            ? DateTime.Today
            : DateTime.ParseExact(input.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var quantity = AnsiConsole.Ask<int>("Enter the [green]quantity[/] of times you did it");

        logToUpdate.Date = date.ToString("yyyy-MM-dd");
        logToUpdate.Quantity = quantity;
        logToUpdate.Habit = habit;


        _logRepository.UpdateItem(logToUpdate);
    }

}

