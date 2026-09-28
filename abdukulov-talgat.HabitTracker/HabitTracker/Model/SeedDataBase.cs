using Microsoft.Data.Sqlite;

namespace HabitTracker.Model;

public class SeedDataBase
{
    private readonly List<( string Name, int MinQty, int MaxQty)> _habitValueSuppliers =
    [
        ("Water", 1, 3),
        ("Walk", 3000, 12000),
        ("Read", 15, 60),
        ("Watch TV", 10, 30),
    ];

    public void Seed(string connectionString)
    {
        using SqliteConnection con = new(connectionString);
        con.Open();
        SqliteTransaction transaction = con.BeginTransaction();
        SqliteCommand cmd = con.CreateCommand();
        cmd.Transaction = transaction;

        cmd.CommandText = """
                            INSERT INTO Habits (Name, Quantity, OccurrenceAt)
                            VALUES 
                            (@name, @quantity, @occurrenceAt);
                          """;

        cmd.Parameters.Add("@name", SqliteType.Text);
        cmd.Parameters.Add("@quantity", SqliteType.Real);
        cmd.Parameters.Add("@occurrenceAt", SqliteType.Text);

        for (int i = 0; i < 200; i++)
        {
            Habit habit = GetRandomHabit();
            cmd.Parameters["@name"].Value = habit.Name;
            cmd.Parameters["@quantity"].Value = habit.Quantity;
            cmd.Parameters["@occurrenceAt"].Value = habit.OccurrenceAt;

            cmd.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private Habit GetRandomHabit()
    {
        DateTime time = GetRandomDateTime();
        Random random = new Random();
        var supplier = _habitValueSuppliers[random.Next(0, _habitValueSuppliers.Count)];

        return new Habit(
            -1,
            supplier.Name,
            random.Next(supplier.MinQty, supplier.MaxQty + 1),
            time);
    }

    private DateTime GetRandomDateTime()
    {
        Random random = new();

        DateTime startTime = DateTime.Now.AddDays(-random.Next(1, 60));
        int hours = random.Next(7, 23);
        int minutes = random.Next(0, 60);

        return startTime + new TimeSpan(hours, minutes, 0);
    }
}