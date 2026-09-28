using Microsoft.Data.Sqlite;
using TCSA.HabitTracker.Models;

namespace TCSA.HabitTracker.DataAccessObjects
{
    internal class HabitEntryDao : BaseDao, IDao<HabitEntry>
    {
        public HabitEntryDao(string connectionString) : base(connectionString)
        {
        }
        
        public List<HabitEntry> GetAll()
        {
            using var connection = CreateConnection();
            connection.Open();

            List<HabitEntry> records = [];

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT HabitEntry.Id, HabitEntry.Value, Habit.HabitUnit, HabitEntry.Date " +
                                    "FROM HabitEntry " +
                                    "INNER JOIN Habit " +
                                    "ON Habit.Id = HabitEntry.HabitId; ";

            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                return records;
            }

            while (reader.Read())
            {
                var record = new HabitEntry()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Value = reader.GetDouble(reader.GetOrdinal("Value")),
                    HabitUnit = reader.GetString(reader.GetOrdinal("HabitUnit")),
                    Date = reader.GetString(reader.GetOrdinal("Date")),
                };
                records.Add(record);
            }
            return records;
        }


        public void Insert(HabitEntry habitEntry)
        {
            using var connection = CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO HabitEntry (HabitId, Date, Value) VALUES (@habitId, @date, @value)";
            command.Parameters.AddWithValue("@habitId", habitEntry.HabitId);
            command.Parameters.AddWithValue("@date", habitEntry.Date);
            command.Parameters.AddWithValue("@value", habitEntry.Value);

            command.ExecuteNonQuery();
        }

        public void DeleteById(int id)
        {
            using var connection = CreateConnection();
            connection.Open();

            var sql = "DELETE FROM HabitEntry WHERE Id = @id";

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        public void UpdateById(int oldHabitEntryid, HabitEntry newHabitEntry)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = "UPDATE HabitEntry SET HabitId = @newHabitId, Date = @newDate, Value = @newValue WHERE Id = @id";
            command.Parameters.AddWithValue("@id", oldHabitEntryid);
            command.Parameters.AddWithValue("@newHabitId", newHabitEntry.HabitId);
            command.Parameters.AddWithValue("@newDate", newHabitEntry.Date);
            command.Parameters.AddWithValue("@newValue", newHabitEntry.Value);

            command.ExecuteNonQuery();
        }

        public HabitEntry? GetById(int id)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT HabitEntry.Id, Date, Value, HabitUnit FROM HabitEntry INNER JOIN Habit ON HabitId = Habit.Id WHERE HabitEntry.Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new HabitEntry() 
                { 
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Value = reader.GetDouble(reader.GetOrdinal("Value")),
                    Date = reader.GetString(reader.GetOrdinal("Date")),
                    HabitUnit = reader.GetString(reader.GetOrdinal("HabitUnit")),
                };
            }

            return null;
        }

        public void InsertMany(List<HabitEntry> items)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO HabitEntry (HabitId, Date, Value) VALUES (@habitId, @date, @value)";

            var habitIdParam = command.CreateParameter();
            habitIdParam.ParameterName = "@habitId";
            command.Parameters.Add(habitIdParam);

            var dateParam = command.CreateParameter();
            dateParam.ParameterName = "@date";
            command.Parameters.Add(dateParam);

            var valueParam = command.CreateParameter();
            valueParam.ParameterName = "@value";
            command.Parameters.Add(valueParam);

            foreach (HabitEntry entry in items)
            {
                habitIdParam.Value = entry.HabitId;
                dateParam.Value = entry.Date;
                valueParam.Value = entry.Value;
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        }
    }
}
