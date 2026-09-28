using Microsoft.Data.Sqlite;
using TCSA.HabitTracker.Models;
using TCSA.HabitTracker.UserInteraction;

namespace TCSA.HabitTracker.DataAccessObjects
{
    internal class HabitDao : BaseDao, IDao<Habit>,IHabitUnitReader
    {
        public HabitDao(string connectionString) : base(connectionString) 
        {
        }

        public List<Habit> GetAll()
        {
            List<Habit> habits = [];
            using var connection = CreateConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, HabitUnit, HabitDescription FROM Habit";
            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var habit = new Habit()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Unit = reader.GetString(reader.GetOrdinal("HabitUnit")),
                        Description = reader.GetString(reader.GetOrdinal("HabitDescription"))
                    };
                    habits.Add(habit);
                }

            }

            return habits;
        }

        public Habit? GetByUnit(string unit)
        {
            using var connection = CreateConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, HabitUnit, HabitDescription FROM Habit WHERE HabitUnit = @unit";
            command.Parameters.AddWithValue("@unit", unit);
            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                var habit = new Habit()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Unit = reader.GetString(reader.GetOrdinal("HabitUnit")),
                    Description = reader.GetString(reader.GetOrdinal("HabitDescription"))
                };
                return habit;
            }
            return null;
        }

        public void Insert(Habit habit)
        {
            using var connection = CreateConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Habit (HabitUnit, HabitDescription) VALUES (@unit, @description)";
            command.Parameters.AddWithValue("@unit", habit.Unit.ToLower());
            command.Parameters.AddWithValue("@description", habit.Description.ToLower());

            command.ExecuteNonQuery();
        }
        public void UpdateByUnit(string oldHabitUnit, Habit updatedHabit)
        {
            using var connection = CreateConnection();
            connection.Open();

            string sql = "UPDATE Habit SET HabitUnit = @newUnit, HabitDescription = @newDescription WHERE HabitUnit = @oldHabitUnit";

            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("@newUnit", updatedHabit.Unit.ToLower());
            command.Parameters.AddWithValue("@newDescription", updatedHabit.Description.ToLower());
            command.Parameters.AddWithValue("@oldHabitUnit", oldHabitUnit);
            command.ExecuteNonQuery();
        }

        public void DeleteByUnit(string habitUnit)
        {
            using var connection = CreateConnection();
            connection.Open();

            string sql = "DELETE FROM Habit WHERE HabitUnit = @habitUnit"; 
            
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("@habitUnit", habitUnit);
            command.ExecuteNonQuery();
        }

        public List<string> GetAllUnits()
        {
            using var connection = CreateConnection();
            connection.Open();

            string sql = "SELECT HabitUnit FROM Habit ORDER BY HabitUnit";

            var command = connection.CreateCommand();
            command.CommandText = sql;

            var units = new List<string>();
            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    units.Add(reader.GetString(reader.GetOrdinal("HabitUnit")));
                }
            }

            return units;
        }

        public int GetUnitId(string habitUnit)
        {
            using var connection = CreateConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id FROM Habit WHERE HabitUnit = @habitUnit";
            command.Parameters.AddWithValue("@habitUnit", habitUnit.ToLower());

            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(reader.GetOrdinal("Id"));
            }
            return -1;
        }

        public void InsertMany(List<Habit> items)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO Habit (HabitUnit, HabitDescription) VALUES (@unit, @description)";

            var unitParam = command.CreateParameter();
            unitParam.ParameterName = "@unit";
            command.Parameters.Add(unitParam);

            var descriptionParam = command.CreateParameter();
            descriptionParam.ParameterName = "@description";
            command.Parameters.Add(descriptionParam);

            foreach (Habit habit in items)
            {
                unitParam.Value = habit.Unit.ToLower();
                descriptionParam.Value = habit.Description.ToLower();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException e)
                {
                    UserMessages.Warning(e.Message);
                    transaction.Rollback();
                    UserMessages.NoChangesMessage();
                    return;
                }
                
            }

            transaction.Commit();
        }
    }
}
