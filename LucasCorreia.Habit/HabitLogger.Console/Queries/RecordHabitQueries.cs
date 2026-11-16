using HabitLogger.Console.Database;
using HabitLogger.Console.Model;

namespace HabitLogger.Console.Queries;

public class RecordHabitQueries
{
    public void Create(long habitId, DateTime date, int quantity)
    {
        string sql = "INSERT INTO record_habits (date, quantity, habit_id) VALUES ($date,$quantity,$habitId)";
        
        DatabaseCommands.ExecuteQuery(sql, ("$date", date), ("$quantity", quantity), ("$habitId", habitId));
    }

    public  bool Update(long id, DateTime date, int quantity)
    {
        string sql = "UPDATE record_habits SET date = $date, quantity = $quantity WHERE id = $id";

        var row = DatabaseCommands.ExecuteQueryReturn(sql, ("$date", date), ("quantity", quantity), ("$id", id));
        
        return row > 0;
    }

    public bool Delete(long id)
    {
        string sql = "DELETE FROM record_habits WHERE id = $id";

        var row = DatabaseCommands.ExecuteQueryReturn(sql, ("$id", id));

        return row > 0;
    }

    public  List<RecordHabit> GetAllById(long id)
    {
        string sql = "SELECT * FROM record_habits WHERE habit_id = $id";

        var recordsHabits = DatabaseCommands.Get(sql, reader => new RecordHabit
        (
            date: reader.GetDateTime(1),
            quantity: reader.GetInt32(2),
            id: reader.GetInt64(0)
        ), ("$id", id));

        return recordsHabits;
    }
}