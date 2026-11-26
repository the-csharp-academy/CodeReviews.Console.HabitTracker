using HabitTracker.Database;
using Microsoft.Data.Sqlite;

namespace HabitTracker.Repositories;

public class RepositoryBase : DatabaseConfiguration
{
    public void ExecuteCommand(string sql, (string Name, object Value)[] parameters)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = new SqliteCommand(sql, connection);

        foreach (var parameter in parameters)
            command.Parameters.AddWithValue(parameter.Name, parameter.Value);

        command.ExecuteNonQuery();
    }

    public void ExecuteBulkCommand(string sql, (string Name, object Value)[][] parameters)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();

        command.Transaction = transaction;
        command.CommandText = sql;

        foreach (var parameter in parameters)
        {
            command.Parameters.Clear();

            foreach (var (name, value) in parameter)
            {
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }

            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public List<T> ExecuteQuery<T>(string sql, Func<SqliteDataReader, T> map, (string Name, object Value)[]? parameters = null)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = new SqliteCommand(sql, connection);
        
        if(parameters is not null)
            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);
        
        using var reader = command.ExecuteReader();

        List<T> entities = new List<T>();
        if(reader.HasRows)
            while (reader.Read())
            {
                entities.Add(map(reader));
            }

        return entities;
    }

    public T? ExecuteQueryReturnSingle<T>(string sql, Func<SqliteDataReader, T> map,
        (string Name, object Value)[]? parameters = null)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = new SqliteCommand(sql, connection);
        
        if(parameters is not null)
            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);
        
        using var reader = command.ExecuteReader();
        
        if(reader.HasRows && reader.Read())
        {
            var entity = map(reader);
            return entity;
        }

        return default;
    }
}