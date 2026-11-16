using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace HabitLogger.Console.Database;

public class DatabaseCommands : DatabaseConfig
{
    public static void ExecuteQuery(string sql, params (string name, object value)[] parameters)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        command.ExecuteNonQuery();
    }

    public static int ExecuteQueryReturn(string sql, params (string name, object value)[] parameters)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        return command.ExecuteNonQuery();
    }

    public static List<T> Get<T>(string sql, Func<SqliteDataReader, T> map, params (string name, object? value)[] parameters)
    {
        var list = new List<T>();
        
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(map(reader));
        }
        
        return list;
    }
}