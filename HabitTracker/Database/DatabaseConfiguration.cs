using Microsoft.Data.Sqlite;

namespace HabitTracker.Database;

public class DatabaseConfiguration
{
    protected readonly string _connectionString;
    
    public DatabaseConfiguration()
    {
        _connectionString = "Data Source=HabitTracker.db; Foreign Keys=True";
    }
}