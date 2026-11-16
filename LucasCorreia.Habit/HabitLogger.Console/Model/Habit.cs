namespace HabitLogger.Console.Model;

public class Habit
{
    public long Id { get;  }
    public string Name { get;  }
    public string Unit { get; }

    public Habit(long id, string name, string unit)
    {
        Id = id;
        Name = name;
        Unit = unit;
    }
}