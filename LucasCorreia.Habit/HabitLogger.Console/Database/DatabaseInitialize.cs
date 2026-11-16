using Microsoft.Data.Sqlite;

namespace HabitLogger.Console.Database;

public class DatabaseInitialize : DatabaseConfig
{
    public static void Initialize()
    {
        using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                              CREATE TABLE IF NOT EXISTS habits (
                                  id INTEGER PRIMARY KEY AUTOINCREMENT,
                                  name TEXT NOT NULL,
                                  unit TEXT NOT NULL
                              );

                              CREATE TABLE IF NOT EXISTS record_habits (
                                  id INTEGER PRIMARY KEY AUTOINCREMENT,
                                  date TEXT NOT NULL,
                                  quantity INTEGER NOT NULL,
                                  habit_id INTEGER NOT NULL,
                                  FOREIGN KEY (habit_id) REFERENCES habits (id) ON DELETE CASCADE
                              );
                              """;
        command.ExecuteNonQuery();
    }
}