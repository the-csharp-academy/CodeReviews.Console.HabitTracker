using HabitLogger.Entities;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace HabitLogger.Services
{
    internal class HabitRepository
    {
        private SqliteConnection _connection;

        public HabitRepository(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void AddHabbit(string name, string measurement)
        {
            try
            {
                _connection.Open();


                var query = @"INSERT OR IGNORE INTO Habits (Name, Measurement) "
                            + $"VALUES(@name, @measurement)";

                var command = new SqliteCommand(query, _connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@measurement", measurement);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error inserting habbbit into Habits table: {ex.Message} [/]");
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<Habit> GetHabits()
        {
            List<Habit> habitList = new();

            try
            {
                _connection.Open();
                var query = @"SELECT * FROM Habits";
                var command = new SqliteCommand(query, _connection);

                var read = command.ExecuteReader();

                while (read.Read())
                {
                    Habit habit = new Habit(Int32.Parse(read.GetString(0)), read.GetString(1), read.GetString(2));
                    habitList.Add(habit);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Trouble getting Habits: {ex.Message}[/]");
            }
            finally
            {
                _connection.Close();
            }

            return habitList;
        }

        public void DeleteHabit(string name)
        {
            try
            {
                _connection.Open();

                var query = @"DELETE FROM Habits where Name = @name";
                var command = new SqliteCommand(query, _connection);

                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error deleting habit '{name}' from Habits table: {ex.Message}[/]\nPress any key to continue...");
            }
            finally
            {
                _connection.Close();
            }
        }

        public void EditHabit(Habit habit, string newName, string newMeasurement)
        {
            var selectedhabit = habit.Name;

            habit.Updatename(newName);
            habit.UpdateMeasurement(newMeasurement);

            try
            {
                _connection.Open();

                var query = @"UPDATE Habits set Name = @NewName, Measurement = @measurement WHERE name = @selectedHabit";
                var command = new SqliteCommand(query, _connection);

                command.Parameters.AddWithValue("@NewName", habit.Name);
                command.Parameters.AddWithValue("@measurement", habit.Measurement);
                command.Parameters.AddWithValue("@selectedHabit", selectedhabit);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("There was an error updating the habit: " + ex.Message + "\nPress any key to return to the menu...");
                Console.ReadKey();
            }
            finally
            {
                _connection.Close();
            }
        }

        public void CreateHabitLogEntry(Habit habit, int quantity, DateTime timeStamp)
        {
            try
            {
                _connection.Open();

                var query = @"INSERT OR IGNORE INTO HabitLogEntry (Habit, Quantity, TimeStamp) "
                            + "VALUES (@Habit, @Quantity, @TimeStamp)";

                var command = new SqliteCommand(query, _connection);
                command.Parameters.AddWithValue("@Habit", habit.Id);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@TimeStamp", timeStamp);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Problem creating new habit Log Entry: {ex.Message} [/]");
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<HabitLogEntry> GetHabitLogEntries()
        {
            List<HabitLogEntry> logEntryList = new();
            try
            {
                var habits = GetHabits();

                _connection.Open();

                var query = @"SELECT * FROM HabitLogEntry ORDER BY Timestamp DESC";

                var command = new SqliteCommand(query, _connection);

                var read = command.ExecuteReader();

                return logEntryList = BuildHabitLogFromReader(read, habits);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error getting habbit log entries {ex.Message} [/]");
                return logEntryList;
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<HabitLogEntry> GetLogEntryByDate(DateTime dateTime)
        {
            List<HabitLogEntry> logEntryList = new();
            try
            {
                var habits = GetHabits();

                _connection.Open();

                var query = @"SElECT * FROM HabitLogEntry WHERE Timestamp >= @dateTime ORDER BY Timestamp DESC";

                var command = new SqliteCommand(query, _connection);
                command.Parameters.AddWithValue("@dateTime", dateTime);
                var read = command.ExecuteReader();

                return logEntryList = BuildHabitLogFromReader(read, habits);
            }catch(Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]error getting habit log entries for the specified date: {dateTime.ToString()}, {ex.Message}[/]" +
                    $"\nPress any key to continue...");
                Console.ReadKey();
                return logEntryList;
            }
            finally
            {
                _connection.Close();
            }
        }
        public void EditHabitLogEntry(HabitLogEntry habitLogEntry, Habit newHabit, string newQuantity, DateTime newTimeStamp)
        {
            try
            {
                habitLogEntry.UpdateHabit(newHabit);
                habitLogEntry.UpdateQuantity(newQuantity);
                habitLogEntry.UpdateTimeStamp(newTimeStamp);

                _connection.Open();

                var query = @"UPDATE HabitLogEntry SET habit = @habit, quantity = @quantity, timestamp = @timestamp WHERE Id = @id";
                var command = new SqliteCommand(query, _connection);

                command.Parameters.AddWithValue("@habit", habitLogEntry.Habit.Id);
                command.Parameters.AddWithValue("@quantity", habitLogEntry.Quantity);
                command.Parameters.AddWithValue("@timestamp", habitLogEntry.TimeEntry);
                command.Parameters.AddWithValue("@id", habitLogEntry.Id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error updating the selecte log entry: ${ex.Message}[/]\n Press any key to continue...");
                Console.ReadKey();
            }
            finally
            {
                _connection.Close();
            }
        }

        public void DeleteLogEntry(HabitLogEntry logEntry)
        {
            try
            {
                _connection.Open();

                var query = @"DELETE FROM HabitLogEntry WHERE Id = @id";

                var command = new SqliteCommand(query, _connection);
                command.Parameters.AddWithValue("@id", logEntry.Id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to delete the log entry: {ex.Message}[red]");
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<HabitLogEntry> BuildHabitLogFromReader(SqliteDataReader reader, List<Habit> habits)
        {
            List<HabitLogEntry> logEntries = new();

            while (reader.Read())
            {
                var habit = habits.Where(x => x.Id.ToString() == reader.GetString(1)).First();
                var habitLog = new HabitLogEntry(Int32.Parse(reader.GetString(0)), habit, Int32.Parse(reader.GetString(2)), DateTime.Parse(reader.GetString(3)));
                logEntries.Add(habitLog);
            }

            return logEntries;
        }
    }
}
