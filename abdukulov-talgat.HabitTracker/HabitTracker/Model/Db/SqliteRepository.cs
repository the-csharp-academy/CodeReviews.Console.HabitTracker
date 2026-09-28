using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace HabitTracker.Model.Db;

public class SqliteRepository
{
    private readonly string _connectionString;

    public SqliteRepository(SqliteConnectionStringBuilder builder)
    {
        _connectionString = builder.ConnectionString;

        if (File.Exists(builder.DataSource)) return; //Already exists, no need to create and populate

        try
        {
            CreateTable();
            new SeedDataBase().Seed(_connectionString);    
        }
        catch(Exception ex)
        {
            AnsiConsole.WriteLine($"[red]An exception happen when creating repository.[/]");
        }
        
    }

    public IEnumerable<Habit> GetHabits(int? id = null)
    {
        string baseQuery = "SELECT Id, Name, Quantity, OccurrenceAt FROM Habits";

        using SqliteConnection con = new(_connectionString);
        con.Open();
        SqliteCommand cmd = con.CreateCommand();

        if (id != null)
        {
            baseQuery = baseQuery + $" WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
        }

        cmd.CommandText = baseQuery;
        using SqliteDataReader reader = cmd.ExecuteReader();

        List<Habit> list = [];
        if (!reader.HasRows) return list;

        while (reader.Read()) //No adapter implemented in package :(
        {
            Habit habit = new Habit(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetDateTime(3)
            );
            list.Add(habit);
        }

        return list;
    }

    public int CreateHabit(Habit habit)
    {
        using SqliteConnection con = new(_connectionString);
        con.Open();
        SqliteCommand cmd = con.CreateCommand();
        cmd.CommandText = """
                          INSERT INTO Habits(Name, Quantity, OccurrenceAt) 
                          VALUES (@name, @quantity, @occurrenceAt);
                          """;
        cmd.Parameters.AddWithValue("@name", habit.Name);
        cmd.Parameters.AddWithValue("@quantity", habit.Quantity);
        cmd.Parameters.AddWithValue("@occurrenceAt", habit.OccurrenceAt);
        return cmd.ExecuteNonQuery();
    }

    public int UpdateHabit(Habit habit)
    {
        using SqliteConnection con = new(_connectionString);
        con.Open();
        SqliteCommand cmd = con.CreateCommand();
        cmd.CommandText = """
                          UPDATE Habits
                          SET Name = @name, Quantity = @quantity, OccurrenceAt = @occurrenceAt
                          WHERE Id = @id;
                          """;
        cmd.Parameters.AddWithValue("@id", habit.Id);
        cmd.Parameters.AddWithValue("@name", habit.Name);
        cmd.Parameters.AddWithValue("@quantity", habit.Quantity);
        cmd.Parameters.AddWithValue("@occurrenceAt", habit.OccurrenceAt);
        return cmd.ExecuteNonQuery();
    }

    public int DeleteHabit(int id)
    {
        using SqliteConnection con = new(_connectionString);
        con.Open();
        SqliteCommand cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM Habits WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery();
    }

    private void CreateTable()
    {
        const string cmdText = """
                                 CREATE TABLE IF NOT EXISTS Habits
                                 (
                                     Id INTEGER PRIMARY KEY,
                                     Name TEXT NOT NULL,
                                     Quantity INTEGER NOT NULL,
                                     OccurrenceAt TEXT NOT NULL
                                 )
                               """;


        using SqliteConnection con = new(_connectionString);
        con.Open();
        SqliteCommand cmd = con.CreateCommand();
        cmd.CommandText = cmdText;
        cmd.ExecuteNonQuery();
    }
}