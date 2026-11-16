using HabitLogger.Console.Database;
using HabitLogger.Console.Model;

namespace HabitLogger.Console.Queries;

public class HabitQueries
{
    public void Create(string name, string unit)
    {
        string sql = "INSERT INTO habits (name, unit) VALUES ($name, $unit);";
        
        DatabaseCommands.ExecuteQuery(sql, ("$name",name), ("$unit",unit));
    }

    public bool Update(long id, string name, string unit)
    {
        string sql = "UPDATE habits SET name = $name, unit = $unit WHERE id = $id";

        var row = DatabaseCommands.ExecuteQueryReturn(sql, ("$name", name), ("$unit", unit), ("id",id));

        return row > 0;
    }

    public bool Delete(long id)
    {
        string sql = "DELETE FROM habits WHERE id = $id";

        var row = DatabaseCommands.ExecuteQueryReturn(sql, ($"id", id));

        return row > 0;
    }


    public List<Habit> GetAll()
    {
        string sql = "SELECT * FROM habits";

        var habits = DatabaseCommands.Get(sql, reader => new Habit
        (
            id: reader.GetInt64(0),
            name: reader.GetString(1),
            unit: reader.GetString(2)
        ));

        return habits;
    }
}