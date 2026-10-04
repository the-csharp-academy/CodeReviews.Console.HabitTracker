using Microsoft.Data.Sqlite;
namespace HabitLogger.Paddymcn.Database;


public class DatabaseHandler
{
    private readonly string connectionString = 
        $"Data Source={Path.Combine(AppContext.BaseDirectory, "biscuits.db")}";
    
    public void EnsureCreated()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = 
        @"
                CREATE TABLE IF NOT EXISTS Biscuits 
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BrandName TEXT NOT NULL,
                Description TEXT NOT NULL,
                CaloriesPerItem INTEGER NOT NULL,
                NumberEaten INTEGER NOT NULL,
                TotalCalories INTEGER NOT NULL,
                DateEaten TEXT NOT NULL
            );
        ";
        createTableCmd.ExecuteNonQuery();
    }

    public void SaveToDatabase(Biscuit biscuit)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText =
            @"
                INSERT INTO Biscuits (BrandName, Description, CaloriesPerItem, NumberEaten, TotalCalories, DateEaten)
                    VALUES ($brandName, $description, $caloriesPerItem, $numberEaten,  $totalCalories, $dateEaten);
            ";
        insertCmd.Parameters.AddWithValue("$brandName", biscuit.BrandName);
        insertCmd.Parameters.AddWithValue("$description", biscuit.Description);
        insertCmd.Parameters.AddWithValue("$caloriesPerItem", biscuit.CaloriesPerItem);
        insertCmd.Parameters.AddWithValue("$numberEaten", biscuit.NumberEaten);
        insertCmd.Parameters.AddWithValue("$totalCalories", biscuit.TotalCalories); 
        insertCmd.Parameters.AddWithValue("$dateEaten", biscuit.DateEaten);   
        insertCmd.ExecuteNonQuery();
    }
    
    public void Delete(int id)
    {
        if (!checkIdExists(id))
        {
            Console.WriteLine($"Id {id} does not exist");
            return;
        }
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Biscuits WHERE ID = $id;";
        deleteCmd.Parameters.AddWithValue("$id", id);
        deleteCmd.ExecuteNonQuery(); 
        Console.WriteLine($"Id {id} has been deleted");
    }

    public void GetById(int id)
    {
        if (!checkIdExists(id))
        {
            Console.WriteLine($"Id {id} does not exist");
            return;
        }
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT * FROM Biscuits WHERE ID = $id;";
        checkCmd.Parameters.AddWithValue("$id", id);
        using var reader = checkCmd.ExecuteReader();
        while (reader.Read())
        {
            int idValue = reader.GetInt32(0);
            string brandName = reader.GetString(1);
            string description = reader.GetString(2);
            int caloriesPerItem = reader.GetInt32(3);
            int numberEaten = reader.GetInt32(4);
            int totalCalories = reader.GetInt32(5);
            DateTime dateEaten = reader.GetDateTime(6);

            Console.WriteLine($"{idValue}: {brandName}, {description}, {caloriesPerItem} cal/item, {numberEaten} eaten, {totalCalories} total cal, logged {dateEaten.ToString("dd-MM-yy")}");
        }
    }
    
    public void GetAll()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT * FROM Biscuits";
        using var reader = checkCmd.ExecuteReader();
        while (reader.Read())
        {
            int idValue = reader.GetInt32(0);
            string brandName = reader.GetString(1);
            string description = reader.GetString(2);
            int caloriesPerItem = reader.GetInt32(3);
            int numberEaten = reader.GetInt32(4);
            int totalCalories = reader.GetInt32(5);
            DateTime dateEaten = reader.GetDateTime(6);

            Console.WriteLine($"{idValue}: {brandName}, {description}, {caloriesPerItem} cal/item, {numberEaten} eaten, {totalCalories} total cal, logged {dateEaten.ToString("dd-MM-yy")}");
        }
    }

    public void UpdateEntry(int id, Biscuit biscuit)
    {
        
        if (!checkIdExists(id))
        {
            Console.WriteLine($"Id {id} does not exist");
            return;
        }

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText =
            @"
                UPDATE Biscuits
                SET BrandName = $brandName, Description = $description, CaloriesPerItem = $caloriesPerItem, NumberEaten = $numberEaten, TotalCalories = $totalCalories, DateEaten = $dateEaten
                WHERE Id = $id;
            ";
        updateCmd.Parameters.AddWithValue("$id", id);
        updateCmd.Parameters.AddWithValue("$brandName", biscuit.BrandName);
        updateCmd.Parameters.AddWithValue("$description", biscuit.Description);
        updateCmd.Parameters.AddWithValue("$caloriesPerItem", biscuit.CaloriesPerItem);
        updateCmd.Parameters.AddWithValue("$numberEaten", biscuit.NumberEaten);
        updateCmd.Parameters.AddWithValue("$totalCalories", biscuit.TotalCalories); 
        updateCmd.Parameters.AddWithValue("$dateEaten", biscuit.DateEaten);   
        updateCmd.ExecuteNonQuery();
    }

    public bool checkIdExists(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM Biscuits WHERE ID = $id;";
        checkCmd.Parameters.AddWithValue("$id", id);
        int result = Convert.ToInt32(checkCmd.ExecuteScalar());
        bool output = (result != 0);
        return output;
    }
}