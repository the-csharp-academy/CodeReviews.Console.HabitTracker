using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Habit_Logger
{
    internal class DatabaseManager
    {
        private const string DataSource = "Data Source=habits.db";
        public void InitializeDatabase()
        {
            using var connection = new SqliteConnection(DataSource);

            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS habits (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                HabitName TEXT,
                Date TEXT,
                Quantity INTEGER
            )
            ";

            command.ExecuteNonQuery();
        }

        public void InsertHabit(Habit habit)
        {
            using var connection = new SqliteConnection(DataSource);

            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO habits (HabitName, Date, Quantity)
                VALUES (@HabitName, @Date, @Quantity)
            ";

            command.Parameters.AddWithValue("@HabitName", habit.HabitName);
            command.Parameters.AddWithValue("@Date", habit.Date.ToString("dd-MM-yyyy"));
            command.Parameters.AddWithValue("@Quantity", habit.Quantity);

            command.ExecuteNonQuery();
        }

        public List<Habit> GetHabits()
        {
            var habits = new List<Habit>();

            using var connection = new SqliteConnection(DataSource);

            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, HabitName, Date, Quantity FROM habits
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                habits.Add(new Habit(reader.GetInt32(0), reader.GetString(1), DateOnly.ParseExact(reader.GetString(2), "dd-MM-yyyy", CultureInfo.InvariantCulture), reader.GetInt32(3)));
            }

            return habits;
        }

        public int EraseHabit(int deleteId)
        {
            using var connection = new SqliteConnection(DataSource);

            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM habits WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", deleteId);

            return command.ExecuteNonQuery();
        }

        //public bool CheckIdExistence(int id)
        //{
        //    using var connection = new SqliteConnection(DataSource);

        //    connection.Open();

        //    using var command = connection.CreateCommand();
        //    command.CommandText = @"
        //        SELECT COUNT(1) FROM habits WHERE Id = @Id
        //    ";
        //    command.Parameters.AddWithValue("@Id", id);

        //    long count = (long)command.ExecuteScalar()!;

        //    return count > 0;
        //}

        public bool UpdateHabit(int id, string name, DateOnly date, int quantity)
        {
            using var connection = new SqliteConnection(DataSource);

            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE habits
                SET HabitName = @HabitName, Date = @Date, Quantity = @Quantity
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@HabitName", name);
            command.Parameters.AddWithValue("@Date", date.ToString("dd-MM-yyyy"));
            command.Parameters.AddWithValue("@Quantity", quantity);

            bool isUpadted = command.ExecuteNonQuery() > 0;

            return isUpadted;
        }
    }
}
