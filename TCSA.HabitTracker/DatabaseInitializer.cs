using Microsoft.Data.Sqlite;

namespace TCSA.HabitTracker
{
    internal class DatabaseInitializer
    {
        private readonly string _connectionString;
        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            InitializeHabitTable();
            InitializeHabitEntryTable();
        }

        private void InitializeHabitTable()
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Habit (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    HabitUnit TEXT NOT NULL UNIQUE,
                                    HabitDescription TEXT
                                )";
                var command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        private void InitializeHabitEntryTable()
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS HabitEntry (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    HabitId INTEGER NOT NULL,
                                    Date TEXT NOT NULL,
                                    Value REAL NOT NULL,
                                    FOREIGN KEY (HabitId) REFERENCES Habit(Id) 
                                    ON DELETE CASCADE
                                )";
                var command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        public void InitializeDrinkingWaterTable()
        {
            using (var connection =CreateConnection())
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Date TEXT,
                                            Quantity INTEGER
                                            )";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
