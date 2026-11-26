using HabitTracker.Database;
using HabitTracker.Exceptions;
using HabitTracker.Models;
using HabitTracker.Repositories;
using Spectre.Console;

namespace HabitTracker.Controllers;

public class HabitLogController
{
    private readonly HabitLogRepository _habitLogRepository = new();
    private readonly HabitRepository _habitRepository = new();

    public void AddHabitLog()
    {
        var habitLog = new HabitLog
        {
            HabitId = AnsiConsole.Ask<int>("Enter [green]habit id[/]: ")
        };

        if (_habitLogRepository.Read(habitLog.HabitId) is null)
            throw new NotFoundException(nameof(Habit), habitLog.HabitId);
        
        habitLog.Occurrences = AnsiConsole.Ask<int>("Enter [green]occurence count[/]: ");
        habitLog.OccurrenceDate = AnsiConsole.Ask<DateOnly>("Enter [green]date[/] int format 'yyyy-mm-dd': ");
        
        if(habitLog.OccurrenceDate.Year != DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(
                nameof(habitLog.OccurrenceDate), 
                habitLog.OccurrenceDate, 
                $"The date must be in the current year ({DateTime.Now.Year}).");

        _habitLogRepository.Create(habitLog);
    }

    public void UpdateHabitLog()
    {
        var habitLog = new HabitLog
        {
            Id = AnsiConsole.Ask<int>("Enter [green]id[/]: ")
        };

        if (_habitLogRepository.Read(habitLog.Id) is null)
            throw new NotFoundException(nameof(HabitLog), habitLog.Id);
        
        habitLog.HabitId = AnsiConsole.Ask<int>("Enter [green]habit id[/]: ");
        
        if (_habitRepository.Read(habitLog.HabitId) is null)
            throw new NotFoundException(nameof(Habit), habitLog.HabitId);
        
        habitLog.Occurrences = AnsiConsole.Ask<int>("Enter [green]occurence count[/]: ");
        
        var date = habitLog.OccurrenceDate = AnsiConsole.Ask<DateOnly>("Enter [green]date[/] int format 'yyyy-mm-dd': ");
        var now = DateTime.Now;
        if(date.Year != now.Year)
            throw new ArgumentOutOfRangeException(
                nameof(habitLog.OccurrenceDate), 
                habitLog.OccurrenceDate, 
                $"The date must be in the current year ({DateTime.Now.Year}).");

        _habitLogRepository.Update(habitLog);
    }

    public void DeleteHabitLog()
    {
        var id = AnsiConsole.Ask<int>("Enter [green]log id[/]: ");

        if (_habitLogRepository.Read(id) is null)
            throw new NotFoundException(nameof(HabitLog), id);

        _habitLogRepository.Delete(id);
    }

    public void ListHabitLogs()
    {
        var habitLogs = _habitLogRepository.Read();
        
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("HabitId");
        table.AddColumn("Occurrences");
        table.AddColumn("OccurenceDate");

        foreach (var habitLog in habitLogs)
        {
            table.AddRow(habitLog.Id.ToString(), habitLog.HabitId.ToString(), habitLog.Occurrences.ToString(),
                habitLog.OccurrenceDate.ToString());
        }
        
        AnsiConsole.Write(table);
    }
}