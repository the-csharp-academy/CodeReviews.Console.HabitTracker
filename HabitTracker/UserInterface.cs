using HabitTracker.Controllers;
using HabitTracker.Enums;
using HabitTracker.Exceptions;
using Spectre.Console;

namespace HabitTracker;

public class UserInterface
{
    private readonly HabitController _habitController = new();
    private readonly HabitLogController _habitLogController = new();

    public void MainMenu()
    {
        do
        {
            Console.Clear();

            var entityChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Entity>()
                    .Title("[yellow]Choose entity to operate on[/]")
                    .AddChoices(Enum.GetValues<Entity>()));

            var operationChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Operation>()
                    .Title("[yellow]Choose the operation[/]")
                    .AddChoices(Enum.GetValues<Operation>()));

            switch (operationChoice)
            {
                case Operation.Create:
                    CreateEntity(entityChoice);
                    break;
                case Operation.Read:
                    ReadEntity(entityChoice);
                    break;
                case Operation.Update:
                    UpdateEntity(entityChoice);
                    break;
                case Operation.Delete:
                    DeleteEntity(entityChoice);
                    break;
            }
            
            Console.Clear();
            
        } while (!Exit());
    }

    private bool Exit()
    {
        var exit = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Choose continue or exit[/]")
                .AddChoices("Continue", "Exit"));
        return exit == "Exit";
    }

    private void CreateEntity(Entity entity)
    {
        ExecuteEntityAction(entity, _habitController.AddHabit, _habitLogController.AddHabitLog);
    }
    
    private void ReadEntity(Entity entity)
    {
        ExecuteEntityAction(entity, _habitController.ListHabits, _habitLogController.ListHabitLogs);
        Console.Write("Press any key to continue...");
        Console.ReadKey();
    }
    
    private void UpdateEntity(Entity entity)
    {
        ExecuteEntityAction(entity, _habitController.UpdateHabit, _habitLogController.UpdateHabitLog);

    }
    
    private void DeleteEntity(Entity entity)
    {
        ExecuteEntityAction(entity, _habitController.DeleteHabit, _habitLogController.DeleteHabitLog);
    }

    private void ExecuteEntityAction(Entity entity, Action habitAction, Action habitLogAction)
    {
        switch (entity)
        {
            case Entity.Habit:
                ExecuteOperation(habitAction);
                break;
            case Entity.HabitLog:
                ExecuteOperation(habitLogAction);
                break;
        }
    }

    private void ExecuteOperation(Action operation)
    {
        try
        {
            operation();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.NoStackTrace);
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}