using HabitLogger.Models;
using Microsoft.Data.Sqlite; //
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HabitLogger.SQLlite;
public class LogRepository
{
    readonly string DbPath = "habit_loger.db";
    readonly SqliteConnection Connection = new SqliteConnection($"Data Source=habit_loger.db");

    private static readonly string[] PossibleHabits = new[]
    {
        "Drink Water",
        "Read",
        "Exercise",
        "Meditate",
        "Study",
        "Sleep 8 Hours",
        "Walk",
        "Journal",
        "Stretch",
        "No Sugar"
    };


    internal LogRepository()
    {
        InitializeBd();
        SeedRandomHabits();
    }


    public void InitializeBd()
    {
        Connection.Open();

        //Create the main table
        string sqlCreateTable = @"
        CREATE TABLE IF NOT EXISTS Logs (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Habit TEXT,
        Date TEXT,
        Quantity INTEGER
        )
        ";

        using var command = new SqliteCommand(sqlCreateTable, Connection);

        command.ExecuteNonQuery();
        Connection.Close();
    }

    public void SeedRandomHabits(int count = 10)
    {

        var logs = GetAllLogs();

        if (logs.Count == 0)
        {

            Connection.Open();

            var random = new Random();
            using var transaction = Connection.BeginTransaction();

            string sqlInsert = @"
                INSERT INTO Logs (Habit, Date, Quantity)
                VALUES ($habit, $date, $quantity)
            ";

            for (int i = 0; i < count; i++)
            {
                using var command = new SqliteCommand(sqlInsert, Connection, transaction);

                string habit = PossibleHabits[random.Next(PossibleHabits.Length)];
                DateTime date = DateTime.Today.AddDays(-random.Next(0, 30));
                int quantity = random.Next(1, 10);

                command.Parameters.AddWithValue("$habit", habit);
                command.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$quantity", quantity);

                command.ExecuteNonQuery();
            }

            transaction.Commit();
            Connection.Close();
        }
    }


    public List<Log> GetAllLogs()
    {
        var logs = new List<Log>();

        Connection.Open();
        string sqlSelectItems = @"
            SELECT * 
            FROM Logs
        ";

        using var command = new SqliteCommand( sqlSelectItems, Connection);

        using var reader = command.ExecuteReader();

        while (reader.Read()) 
        {
            int id = reader.GetInt32(0);
            string habit = reader.GetString(1);
            string date = reader.GetString(2);
            int quantity = reader.GetInt32(3);

            logs.Add(new Log { 
                Id = id, 
                Habit = habit,
                Date = date, 
                Quantity = quantity
            });

        }

        Connection.Close();
        return logs;
    }

    public void InsertLog(Log log)
    {
        Connection.Open();

        // Use parameters (@Name, @Age) to prevent SQL Injection security risks
        string insertSql = "INSERT INTO Logs (Habit, Date, Quantity) VALUES (@Habit, @Date, @Quantity);";
        using var command = new SqliteCommand(insertSql, Connection);

        command.Parameters.AddWithValue("@Habit", log.Habit); //
        command.Parameters.AddWithValue("@Date", log.Date); //
        command.Parameters.AddWithValue("@Quantity", log.Quantity);

        command.ExecuteNonQuery();
        Connection.Close();
    }

    public void DeleteItem(Log log)
    {
        Connection.Open();

        string deleteSql = "DELETE FROM Logs WHERE Id = @Id";

        using var command = new SqliteCommand(deleteSql, Connection);

        command.Parameters.AddWithValue("@Id", log.Id);

        command.ExecuteNonQuery();
        Connection.Close();
    }

    public void UpdateItem(Log log)
    {
        Connection.Open();

        string updateSql = "UPDATE Logs SET Habit = @Habit, Date = @Date, Quantity = @Quantity WHERE Id = @Id";

        using var command = new SqliteCommand(updateSql, Connection);

        command.Parameters.AddWithValue("@Id", log.Id);
        command.Parameters.AddWithValue("@Habit", log.Habit); //
        command.Parameters.AddWithValue("@Date", log.Date); //
        command.Parameters.AddWithValue("@Quantity", log.Quantity);

        command.ExecuteNonQuery();
        Connection.Close();
    }
}

