using HabitTracker.Database.Fakers;
using HabitTracker.Repositories;
using Microsoft.Data.Sqlite;

namespace HabitTracker.Database;

public class DatabaseInitialization : DatabaseConfiguration
{
    public void CreateDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        string createHabitsTableSql = """
                                      CREATE TABLE IF NOT EXISTS Habits (
                                      Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                      Name TEXT NOT NULL,
                                      Unit TEXT NOT NULL
                                      );
                                      """;
        
        string createHabitLogsTableSql = """
                                         CREATE TABLE IF NOT EXISTS HabitLogs (
                                         Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                         HabitId INTEGER NOT NULL,
                                         Occurrences INTEGER NOT NULL,
                                         OccurrenceDate TEXT NOT NULL,
                                         FOREIGN KEY (HabitId) REFERENCES Habits(Id)
                                         );
                                         """;
        
        using var createHabitsTableCommand = new SqliteCommand(createHabitsTableSql, connection);
        createHabitsTableCommand.ExecuteNonQuery();
        
        using var createHabitLogsTableCommand = new SqliteCommand(createHabitLogsTableSql, connection);
        createHabitLogsTableCommand.ExecuteNonQuery();
        
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var habitRepository = new HabitRepository();

        if (habitRepository.Read().Count > 0)
            return;

        var generatedHabits = new HabitFaker().Generate(10);
        habitRepository.BulkCreate(generatedHabits);

        var habits = habitRepository.Read();
        var habitLogs = new HabitLogFaker(habits).Generate(100);

        new HabitLogRepository().BulkCreate(habitLogs);
    }
}
