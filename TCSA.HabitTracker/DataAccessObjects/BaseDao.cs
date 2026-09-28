using Microsoft.Data.Sqlite;

namespace TCSA.HabitTracker.DataAccessObjects
{
    internal class BaseDao
    {
        protected readonly string _connectionString;
        public BaseDao(string connectionString)
        {
            _connectionString = connectionString;
        }


        protected SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
