using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitLogger.Services
{
    internal class InitializeTable
    {
        private SqliteConnection _connection;
        public InitializeTable(SqliteConnection connetion)
        {
            _connection = connetion;
        }
        public void InitializeTables()
        {
            Console.WriteLine("starting InitializeTable.cs");

            try
            {
                _connection.Open();

                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Habits (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Measurement TEXT
                )";

                using var createTableCommand2 = new SqliteCommand(createTableQuery, _connection);
                createTableCommand2.ExecuteNonQuery();
                Console.WriteLine("Habit Table initialized successfully.");

                string createTableQuery2 = @"
                    CREATE TABLE IF NOT EXISTS HabitLogEntry (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Habit TEXT NOT NULL,
                        Quantity INTEGER NOT NULL,
                        TimeStamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY(Habit) References Habits(id) ON DELETE CASCADE
                    );
                ";
                using var createTableCommand = new SqliteCommand(createTableQuery2, _connection);
                createTableCommand.ExecuteNonQuery();
                Console.WriteLine("Habit Log Table initialized successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Problem initializing one or more tables: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

        }

        public void SeedTable()
        {

            try
            {
                _connection.Open();
                Console.WriteLine("Checking Seed Data...");

                string seedTableQuery = @"
                    INSERT OR IGNORE INTO Habits (Name, Measurement)
                    VALUES ('Water', 'Glass(es)'),
                           ('Running', 'KM(S)'),
                           ('Reading', 'Pages'),
                           ('Coding', 'Line(s)');
                ";

                using var SeeTableCommand = new SqliteCommand(seedTableQuery, _connection);

                Console.WriteLine($"Table seeded: Inserted {SeeTableCommand.ExecuteNonQuery()} Habits, Press any key to continue...");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error seeding Habits table: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }
        }
             
    }
}
