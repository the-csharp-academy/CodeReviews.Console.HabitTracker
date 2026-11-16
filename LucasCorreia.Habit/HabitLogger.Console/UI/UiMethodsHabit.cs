using HabitLogger.Console.Queries;

namespace HabitLogger.Console.UI;

public class UiMethodsHabit
{
    private readonly InputUser _inputUser = new InputUser();
    private readonly HabitQueries _habitQueries = new HabitQueries();
    
    public void InsertHabit()
    {
            System.Console.WriteLine("Insert a New Habit\n");
        
            System.Console.WriteLine("Write a name:");
            string? name = System.Console.ReadLine();
            System.Console.WriteLine("Write a unit of measure:");
            string? unit = System.Console.ReadLine();

            _habitQueries.Create(name!, unit!);
        
            System.Console.WriteLine("Habit created");
    }

    public void UpdateHabit()
    {
        try
        {
            System.Console.WriteLine("Update a Habit\n");
            System.Console.WriteLine("Write a id to Update");
            long id = _inputUser.InputLong(System.Console.ReadLine()!);
            System.Console.WriteLine("Write a name:");
            string? name = System.Console.ReadLine();
            System.Console.WriteLine("Write a unit of measure:");
            string? unit = System.Console.ReadLine();

            if (_habitQueries.Update(id, name!, unit!))
            { 
                System.Console.WriteLine("Habit Updated");
            }
            else
            {
                System.Console.WriteLine("Not Updated");
            };
        }
        catch (FormatException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
    }

    public void DeleteHabit()
    {
        try
        {
            System.Console.WriteLine("Delete a Habit\n");
            System.Console.WriteLine("Write a id to Delete");
            long id = _inputUser.InputLong(System.Console.ReadLine()!);

            if (_habitQueries.Delete(id))
            {
                System.Console.WriteLine("Deleted");
            }
            else
            {
                System.Console.WriteLine("Not Deleted");
            };
        }
        catch (FormatException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
    }

    public void ListsHabits()
    {
        System.Console.WriteLine("List of Habits\n");
        var list = _habitQueries.GetAll();

        foreach (var habit in list)
        {
            System.Console.WriteLine($"Id: {habit.Id} - Name: {habit.Name} - Unit of Measure: {habit.Unit}");
        }
    }
}