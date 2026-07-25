using System.Globalization;
using Microsoft.Data.Sqlite;

string connectionString = @"Data Source=HabitLogger.db";

List<string>? extraColumns = new List<string>();

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    var cmd = connection.CreateCommand();
    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Habit_Logger (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT,
    WaterQuantity INTEGER,
    StepsQuantity INTEGER
    )";
    cmd.ExecuteNonQuery();
    connection.Close();
}

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    var cmd = connection.CreateCommand();
    cmd.CommandText = "PRAGMA table_info(Habit_Logger)";
    SqliteDataReader reader = cmd.ExecuteReader();


    while (reader.Read())
    {
        string? columnName = reader.GetValue(1).ToString() ?? "";
        if (columnName != "id" && columnName != "Date" && columnName != "WaterQuantity" && columnName != "StepsQuantity")
        {
            extraColumns.Add(columnName);
        }
    }
    connection.Close();
}

FillTable(connectionString);
Menu(connectionString, extraColumns);

static void Menu(string connectionString, List<string>? extraColumns)
{
    bool wantToExit = false;


    do
    {
        Console.WriteLine("\n\t MENU\n");
        Console.WriteLine("Choose an option: \n");
        Console.WriteLine("1. View all entries");
        Console.WriteLine("2. Add an entry");
        Console.WriteLine("3. Delete an entry");
        Console.WriteLine("4. Update an entry");
        Console.WriteLine("5. Add an habit that you wish to track");
        Console.WriteLine("0. Exit");


        string? userInput = Console.ReadLine();
        Console.WriteLine();

        try
        {
            switch (userInput)
            {
                case "0":
                    Console.WriteLine("Goodbye!");
                    wantToExit = true;
                    break;

                case "1":

                    ViewAllEntries(connectionString, extraColumns);
                    break;

                case "2":

                    AddEntry(connectionString, extraColumns);
                    break;

                case "3":

                    DeleteEntry(connectionString, extraColumns);
                    break;

                case "4":
                    UpdateEntry(connectionString, extraColumns);
                    break;

                case "5":
                    AddHabit(connectionString, extraColumns);
                    break;

                default:
                    Console.WriteLine("Invalid Input.");
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            continue;
        }

    } while (wantToExit == false);
}

static void ViewAllEntries(string connectionString, List<string>? extraColumns)
{


    Console.WriteLine("\n\tTable entries: \n");


    using (var connection = new SqliteConnection(connectionString))
    {

        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT * FROM Habit_Logger";
        SqliteDataReader reader = cmd.ExecuteReader();


        if (reader.HasRows)
        {
            while (reader.Read())
            {
                string extra = "";

                int id = reader.GetInt32(0);
                string date = reader.GetString(1);
                int water = reader.GetInt32(2);
                int steps = reader.GetInt32(3);


                for (int i = 4; i < reader.FieldCount; i++)
                {
                    string? colName = reader.GetName(i);
                    string value = reader.IsDBNull(i) ? "N/A" : reader.GetValue(i).ToString() ?? "N/A";


                    extra += $", {colName}: {value}";
                }

                Console.Write($"{id} - {date}: Glasses of water: {water}, steps: {steps}");
                Console.WriteLine(extra);
            }
        }

        connection.Close();
    }
}

static void AddEntry(string connectionString, List<string>? extraColumns)
{
    List<int> extraValues = new List<int>();
    string fields = "Date, WaterQuantity, StepsQuantity";
    string values = "@date, @water, @steps";

    string date = GetUserDate("\nInsert the date (format: yyyy-MM-dd)");
    int water = GetUserInt("\nInsert number of water glasses you drank");
    int steps = GetUserInt("\nInsert number of steps taken");

    for (int i = 0; i < extraColumns?.Count; i++)
    {
        fields += $", {extraColumns[i]}";
        extraValues.Add(GetUserInt($"\ninsert a value for: {extraColumns[i]}"));
        values += $", @extra{i}";
    }

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @$"INSERT INTO Habit_Logger({fields}) VALUES({values})";
        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@water", water);
        cmd.Parameters.AddWithValue("@steps", steps);
        for (int i = 0; i < extraColumns?.Count; i++)
        {
            cmd.Parameters.AddWithValue($"@extra{i}", extraValues[i]);
        }
        cmd.ExecuteNonQuery();
        connection.Close();
    }
    Console.WriteLine("\nEntry added successfully!\n");
}

static void DeleteEntry(string connectionString, List<string>? extraColumns)
{
    int id = GetID(connectionString, extraColumns, "\nEnter the ID of the entry you want to delete:");
    if(id == -1)
    {
        return;
    }

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = $"DELETE FROM Habit_Logger WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
        Console.WriteLine("\nEntry deleted successfully!\n");
        connection.Close();
    }
}

static void UpdateEntry(string connectionString, List<string>? extraColumns)
{
    int id = GetID(connectionString, extraColumns, "\nEnter the ID of the entry you want to update:");
    if(id == -1)
    {
        return;
    }

    string variables = "WaterQuantity = @water, StepsQuantity = @steps";
    List<int> extraValues = new List<int>();


    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var cmd = connection.CreateCommand();

        int water = GetUserInt("\nInsert new number of water glasses you drank");
        int steps = GetUserInt("\nInsert new number of steps taken");

        for (int i = 0; i < extraColumns?.Count; i++)
        {
            extraValues.Add(GetUserInt($"\nInsert new value for: {extraColumns[i]}"));
            variables += $", {extraColumns[i]} = @{extraColumns[i]}";
        }

        cmd.CommandText = $"UPDATE Habit_Logger SET {variables} WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@water", water);
        cmd.Parameters.AddWithValue("@steps", steps);
        for (int i = 0; i < extraColumns?.Count; i++)
        {
            cmd.Parameters.AddWithValue($"@{extraColumns[i]}", extraValues[i]);
        }
        cmd.ExecuteNonQuery();
        Console.WriteLine("Entry updated successfully!\n");
        connection.Close();
    }
}

static void AddHabit(string connectionString, List<string>? extraColumns)
{
    Console.WriteLine("Insert name for the new habit: (Can only track numeric habit)");
    string? newHabit = Console.ReadLine()?.Trim();


    while (string.IsNullOrWhiteSpace(newHabit) || !newHabit.All(char.IsLetterOrDigit))
    {
        Console.WriteLine("Invalid input, enter the name again: ");
        newHabit = Console.ReadLine()?.Trim();
    }


    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = $"ALTER TABLE Habit_Logger ADD COLUMN {newHabit} INTEGER DEFAULT 0";
        cmd.ExecuteNonQuery();
        connection.Close();
    }
    Console.WriteLine("\nNew habit added successfully!\n");
    extraColumns?.Add(newHabit);
}

static int GetID(string connectionString, List<string>? extraColumns, string message)
{
    ViewAllEntries(connectionString, extraColumns);
    int id = GetUserInt(message);

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var cmd = connection.CreateCommand();


        cmd.Parameters.AddWithValue("@id", id);
        cmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM Habit_Logger WHERE id = @id)";
        int exists = Convert.ToInt32(cmd.ExecuteScalar());
        if (exists == 0)
        {
            Console.WriteLine("\nRecord does not exists.");
            return -1;
        }
        connection.Close();
        return id;
    }
}

static int GetUserInt(string message)
{
    int value = 0;


    Console.WriteLine(message);
    string? userInput = Console.ReadLine()?.Trim();


    while (!int.TryParse(userInput, out value))
    {
        Console.WriteLine("Invalid Input.");
        userInput = Console.ReadLine()?.Trim();
    }
    return value;
}

static string GetUserDate(string message)
{
    string? date;
    bool validDate = false;
    do
    {

        Console.WriteLine(message);
        date = Console.ReadLine();
        validDate = DateTime.TryParseExact(date, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out _);

        if (!validDate)
        {
            Console.WriteLine("Invalid input. Make sure the format is -> \"yyyy-MM-dd\" ");
        }

    } while (!validDate);


    return date!;
}

//METHOD TO FILL THE TABLE WITH 50 ENTRIES
static void FillTable(string connectionString)
{
    Random random = new Random();
    DateTime currentDate = DateTime.Today;
    DateTime startDate = new DateTime(2026, 1, 1);
    int dateRange = (currentDate - startDate).Days;


    int filler = 50;


    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();




        while (filler > 0)
        {
            int water = random.Next(1, 20);
            int steps = random.Next(1, 10000);
            string date = startDate.AddDays(random.Next(0, dateRange)).ToString("yyyy-MM-dd");


            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Habit_Logger(Date, WaterQuantity, StepsQuantity)
                                    VALUES(@date, @water, @steps)";
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@water", water);
            cmd.Parameters.AddWithValue("@steps", steps);
            cmd.ExecuteNonQuery();
            filler -= 1;
        }
        connection.Close();
    }
}
