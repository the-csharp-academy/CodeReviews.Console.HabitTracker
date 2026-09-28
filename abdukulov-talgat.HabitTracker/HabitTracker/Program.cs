using System.Diagnostics.CodeAnalysis;
using HabitTracker.Model;
using HabitTracker.Model.Db;
using Microsoft.Data.Sqlite;
using Spectre.Console;


namespace HabitTracker;

public static class Program
{
    private static readonly string ConnectionString =
        $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"HabitTracker.db")}";

    public static void Main()
    {
        SqliteRepository repository;
        try
        {
            repository = new(new SqliteConnectionStringBuilder(ConnectionString));
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An error occurred while creating the database. Further execution is pointless. {ex.Message}");
            throw ex;
        }

        UserInterface ui = new UserInterface(repository);
        ui.Run();
    }
}