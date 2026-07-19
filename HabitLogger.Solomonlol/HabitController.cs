using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace STUDY.HabitLogger
{
    static class HabitController
    {
        private static string connectionString = "Data Source=HabitLogger.db";
        
        public static void CreatingDbTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var commandSQL = connection.CreateCommand();

                commandSQL.CommandText = @"CREATE TABLE IF NOT EXISTS Habits" +
                    "(Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Habit TEXT NOT NULL UNIQUE," +
                    "Quantity INTEGER NOT NULL," +
                    "Date TEXT)";
                commandSQL.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static void AddNewDbData()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                Console.Clear();
                GetDbData(false);

                HabitInput("Write habit name:", out string habit);
                NumberInput("Write habit quantity:", out int quantity);
                DateInput("Write a date in 'dd-MM-yyyy' format:", out DateTime date);

                connection.Open();
                var commandSQL = connection.CreateCommand();

                commandSQL.CommandText = @$"INSERT INTO Habits (Habit, Quantity, Date) VALUES ('{habit}', {quantity}, '{date}') ON CONFLICT (Habit) DO NOTHING";

                int checkSQL = commandSQL.ExecuteNonQuery();

                if (checkSQL == 0)
                {
                    Console.WriteLine($"Record whith habit name='{habit}' already exist.\nPress any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"Record whith habit name='{habit}' was created.\nPress any key to continue...");
                    Console.ReadKey();
                }
                connection.Close();
            }
        }

        public static void UpdateDbData()
        {
            GetDbData(false);
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var commandSQL = connection.CreateCommand();
                NumberInput("Write record ID to update", out int id);
                NumberInput("Set new quantity", out int quantity);
                DateInput("Set new date", out DateTime date);

                commandSQL.CommandText = $@"UPDATE Habits SET Quantity='{quantity}', Date='{date}' WHERE Id='{id}'";
                int checkSQL = commandSQL.ExecuteNonQuery();

                if (checkSQL == 0)
                {
                    Console.WriteLine($"Record whit ID='{id}' dosn't exist.\nPress any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"Record whith ID='{id}' was updated.\nPress any key to continue...");
                    Console.ReadKey();
                }
                connection.Close();
            }
        }
        public static void GetDbData(bool isMainMenu=true)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var commandSQL = connection.CreateCommand();

                List<Habit> habitsList = new();

                commandSQL.CommandText = "SELECT * FROM Habits";
                using (var reader = commandSQL.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("===================================================");
                        Console.WriteLine("ID\tHabit Name\tQuantity\tDate");
                        while (reader.Read())
                        {
                            habitsList.Add
                                (new Habit
                                    (reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetInt32(2),
                                    DateOnly.FromDateTime(DateTime.ParseExact(reader.GetString(3), "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture))
                                    ));
                        }
                        Console.WriteLine("===================================================");
                    }
                    else Console.WriteLine("Not found any records in database.");
                    reader.Close();
                }
                connection.Close();

                foreach (var habit in habitsList)
                {
                    habit.Print();
                }
                Console.WriteLine("===================================================");
                if (isMainMenu)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
        public static void DeleteDbData()
        {
            GetDbData(false);
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                NumberInput("Write record ID to delete", out int id);
                
                var commandSQL = connection.CreateCommand();


                commandSQL.CommandText = $"DELETE FROM Habits WHERE Id='{id}'";
                int checkSQL = commandSQL.ExecuteNonQuery();

                if (checkSQL == 0)
                {
                    Console.WriteLine($"Record whit ID='{id}' dosn't exist.\nPress any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"Record whith ID='{id}' was deleted.\nPress any key to continue...");
                    Console.ReadKey();
                }
                connection.Close();
            }
        }


        private static void NumberInput(string s, out int quantity)
        {
            Console.WriteLine(s);
            while (!int.TryParse(Console.ReadLine(), out quantity) || quantity<0)
            {
                Console.WriteLine("Wrong data format. The string must contain only digits equal or above zero. Try again.");
            }
        }

        private static void DateInput(string s, out DateTime date)
        {
            Console.WriteLine(s);
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine("Wrong data format. Write it like 'dd.MM.yyyy' and try again.");
            }
        }

        private static void HabitInput(string s, out string habit)
        {
            Console.WriteLine(s);
            while(!Regex.IsMatch(habit = Console.ReadLine(), @"^[a-zA-Zа-яА-Я]+$"))
            {
                Console.WriteLine("Wrong data format. The string must contain only characters from the Russian or Latin alphabets. Try again.");
            }
        }
    }
}
