using Microsoft.Data.Sqlite;
using System.Globalization;
using HabitLogger.Habit;

string dbFileName = "habit.db";

CreateTableIfNoneExist();
HandleMenuUserInput();

void CreateTableIfNoneExist()
{
    using var connection = new SqliteConnection($"Data Source={dbFileName}");
    connection.Open();

    var command = connection.CreateCommand();
    try
    {
        command.CommandText = @"SELECT * FROM habit;";
        command.ExecuteNonQuery();
    }
    catch
    {
        Console.WriteLine("No table found.");
        try
        {
            Console.WriteLine("Creating Table.");
            command.CommandText = @"CREATE TABLE habit(HabitName TEXT, QuantityPerDay INT, Date DATE);";
            command.ExecuteNonQuery();
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
        }
    }
    finally
    {
        command.Dispose();
        connection.Dispose();
    }
}

void HandleMenuUserInput()
{
    Console.WriteLine("Wellcome to the Habit Logger App!");
    Console.WriteLine("\nWhat would you like to do?\n");
    Console.WriteLine("\t1 - Create new habit");
    Console.WriteLine("\t2 - Get existing habits");
    Console.WriteLine("\t3 - Update existing habit");
    Console.WriteLine("\t4 - Delete existing habit");
    Console.WriteLine("\t5 - Exit Application");

    int userInput;
    while(!int.TryParse(Console.ReadLine(), out userInput) || userInput <= 0 || userInput > 5)
    {
        Console.WriteLine("Please Enter a valid option");
    }

    switch (userInput)
    {
        case 1:
            Console.Clear();
            Console.WriteLine("Creating new habit");
            CreateHabit();
            break;
        case 2:
            Console.Clear();
            Console.WriteLine("Getting existing habits");
            GetHabits();
            break;
        case 3:
            Console.WriteLine("Updating existing habit");
            UpdateHabit();
            break;
        case 4:
            Console.WriteLine("Deleting existing habit");
            DeleteHabit();
            break;
        case 5:
            Console.WriteLine("Exitting the Application");
            break;
    }
}

void HandleUserConfirmation()
{
    Console.WriteLine("\nPress any key to continue");
    Console.ReadKey();
}

void CreateHabit()
{
    Console.WriteLine("\nPlease enter the name of the habit");
    string? name = $"{Console.ReadLine()}";

    Console.WriteLine("\nPlease enter the number of times it was done");
    int userInput;
    while (!int.TryParse(Console.ReadLine(), out userInput) || userInput <= 0)
    {
        Console.WriteLine("Please enter a positive number greater than 0");
    }

    Console.WriteLine("\nPlease enter the date of occurrence (Format dd-MM)");
    DateOnly date;
    while (!DateOnly.TryParseExact(Console.ReadLine(), "dd-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
    {
        Console.WriteLine("Please enter a valid date in dd-MM format");
    }

    string dateTime = $"{date:dd-MM}";
    var habit = new Habit(name, userInput, dateTime);

    using var connection = new SqliteConnection($"Data Source={dbFileName}");
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText ="INSERT INTO habit(HabitName, QuantityPerDay, Date) VALUES($name, $quantity, $date);";
    command.Parameters.AddWithValue("$name", habit.habitName);
    command.Parameters.AddWithValue("$quantity", habit.quantityPerDay);
    command.Parameters.AddWithValue("$date", habit.dateTime);

    try
    {
        command.ExecuteNonQuery();
    }
    catch(SqliteException e)
    {
        Console.WriteLine(e.Message);
    }
    finally
    {
        command.Dispose();
        connection.Dispose();
    }

    HandleUserConfirmation();
    Console.Clear();
    HandleMenuUserInput();
}

void GetHabits()
{
    using var connection = new SqliteConnection($"Data Source={dbFileName}");
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM habit;";
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"\t\n{reader.GetString(0)} | {reader.GetInt32(1)} | {reader.GetString(2)}");
    }

    HandleUserConfirmation();
    Console.Clear();
    HandleMenuUserInput();
}

void UpdateHabit()
{
    using var connection = new SqliteConnection($"Data Source={dbFileName}");
    connection.Open();

    Console.WriteLine("\nPlease enter the name of the habit you wish to modify");
    string? name = Console.ReadLine();

    Console.WriteLine("\nPlease enter the date of the habit you wish to modify");
    string? date = Console.ReadLine();

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM habit WHERE HabitName = $name AND Date = $date";
    command.Parameters.AddWithValue("$name", name);
    command.Parameters.AddWithValue("$date", date);

    int quantity = 0;

    try
    {
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                name = reader.GetString(0);
                quantity = reader.GetInt32(1);
                date = reader.GetString(2);
                Console.WriteLine($"\t\n{name} | {quantity} | {date}");
            }
        }
        else
        {
            Console.WriteLine("Habit not found");
            HandleUserConfirmation();
            Console.Clear();
            HandleMenuUserInput();
            return;
        }
    }
    catch (SqliteException e)
    {
        Console.WriteLine(e.Message);
        HandleUserConfirmation();
        Console.Clear();
        HandleMenuUserInput();
        return;
    }
    
    bool correctInput = false;
    Console.WriteLine("\nPlease enter what field you wish to update: n - Name | q - Quantity | d - Date");

    string? newName = name;
    int newQuantity = quantity;
    string? newDate = date;

    while (!correctInput)
    {
        string? userInput = Console.ReadLine();
        switch (userInput)
        {
            case "n":
                correctInput = true;
                Console.WriteLine("\nInput a new name for this habit");
                newName = Console.ReadLine();
                break;
            case "q":
                correctInput = true;
                Console.WriteLine("\nPlease enter the number of times it was done");
                int input;
                while (!int.TryParse(Console.ReadLine(), out input) || input <= 0)
                {
                    Console.WriteLine("Please enter a positive number greater than 0");
                }
                newQuantity = input;
                break;
            case "d":
                correctInput = true;
                Console.WriteLine("\nPlease enter the date of occurrence (Format dd-MM)");
                DateOnly dateOnly;
                while (!DateOnly.TryParseExact(Console.ReadLine(), "dd-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOnly))
                {
                    Console.WriteLine("Please enter a valid date in dd-MM format");
                }
                newDate = $"{dateOnly:dd-MM}";
                break;
        }

        if (!correctInput) Console.WriteLine("\nPlease input a valid option");
    }

    command.CommandText = $"UPDATE habit SET HabitName = $newName, QuantityPerDay = $newQuantity, Date = $newDate WHERE HabitName = $name AND QuantityPerDay = $quantity AND Date = $date;";
    command.Parameters.Clear();
    command.Parameters.AddWithValue("$name", name);
    command.Parameters.AddWithValue("$quantity", quantity);
    command.Parameters.AddWithValue("$date", date);
    command.Parameters.AddWithValue("$newName", newName);
    command.Parameters.AddWithValue("$newQuantity", newQuantity);
    command.Parameters.AddWithValue("$newDate", newDate);

    try
    {
        command.ExecuteNonQuery();
    }
    catch(Exception e)
    {
        Console.WriteLine(e.Message);
    }
    finally
    {
        command.Dispose();
        connection.Dispose();
    }

    HandleUserConfirmation();
    Console.Clear();
    HandleMenuUserInput();
}

void DeleteHabit()
{
    using var connection = new SqliteConnection($"Data Source={dbFileName}");
    connection.Open();

    Console.WriteLine("\nPlease enter the name of the habit you wish to delete");
    string? name = Console.ReadLine();

    Console.WriteLine("\nPlease enter the date of the habit you wish to delete");
    string? date = Console.ReadLine();

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM habit WHERE HabitName = $name AND Date = $date";
    command.Parameters.AddWithValue("$name", name);
    command.Parameters.AddWithValue("$date", date);

    int quantity = 0;

    try
    {
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                name = reader.GetString(0);
                quantity = reader.GetInt32(1);
                date = reader.GetString(2);
                Console.WriteLine($"\t\n{name} | {quantity} | {date}");
            }
        }
        else
        {
            Console.WriteLine("Habit not found");
            HandleUserConfirmation();
            Console.Clear();
            HandleMenuUserInput();
            return;
        }
    }
    catch (SqliteException e)
    {
        Console.WriteLine(e.Message);
        HandleUserConfirmation();
        Console.Clear();
        HandleMenuUserInput();
        return;
    }

    Console.WriteLine("Do you want to delete this habit? (y/n)");
    bool correctInput = false;
    while (!correctInput)
    {
        string? input = Console.ReadLine();
        switch (input)
        {
            case "y":
                correctInput = true;
                command.Parameters.Clear();
                command.CommandText = "DELETE FROM habit WHERE HabitName = $name AND QuantityPerDay = $quantity AND Date = $date";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$quantity", quantity);
                command.Parameters.AddWithValue("$date", date);

                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Deleting habit.");
                }
                catch (SqliteException e)
                {
                    Console.WriteLine(e.Message);
                }

                break;
            case "n":
                correctInput = true;
                break;
        }

        if (!correctInput) Console.WriteLine("\nPlease input a valid option");
    }

    command.Dispose();
    connection.Dispose();
    HandleUserConfirmation();
    Console.Clear();
    HandleMenuUserInput();
}