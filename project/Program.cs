using System.IO;
using Microsoft.Data.Sqlite;
using SQLitePCL;

void Main() {
    bool play = true;

    CreateDatabase();
    Console.Clear();
    
    while (play) 
    {
        switch (Options())
        {
            case 1: // create
                CreateInput();
                break;
            case 2: // read
                ReadTable();
                break;
            case 3: // update
                string idStr = "";
                int idHabit = 0;
                while(!int.TryParse(idStr, out idHabit))
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine("Input Habit ID to check: ");
                    idStr = Console.ReadLine();
                    System.Console.WriteLine();
                }
                CheckRow(idHabit);
                System.Console.WriteLine();
                System.Console.WriteLine("Is that correct? (y/n)");
                string answer = Console.ReadLine().Trim().ToLower();
                System.Console.WriteLine();
                if (answer == "y")
                {
                    updateRow(idHabit);
                    System.Console.WriteLine();
                    System.Console.WriteLine("Row updated!");
                    System.Console.WriteLine("Press Enter to continue.");
                    Console.ReadKey();
                }

                break;
            case 4: // delete
                break;
            case 5: // exit
                play = false;
                break;

        }
        System.Console.WriteLine();
    }

    System.Console.WriteLine("Exiting now. See you later!");
    System.Console.WriteLine();
    Environment.Exit(0);
}

int Options()
{
    string userAnswer = "";
    int option = 0;
    
    System.Console.WriteLine("**** Options ****");
    System.Console.WriteLine("1) Create ........"); 
    System.Console.WriteLine("2) Read .........."); 
    System.Console.WriteLine("3) Update ........"); 
    System.Console.WriteLine("4) Delete ........"); 
    System.Console.WriteLine("5) Exit .........."); 
    System.Console.WriteLine();

    while (!int.TryParse(userAnswer, out option) && option < 1 || option > 5)
    {
        userAnswer = Console.ReadLine();
        System.Console.WriteLine();
    }

    return option;
}

void CreateDatabase()
{
    string connectionStr = $"Data Source=habits.db";

    Batteries.Init();
    using var connection = new SqliteConnection(connectionStr); 
    connection.Open();

    var createTable = @"
        CREATE TABLE IF NOT EXISTS Habits(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Habit TEXT NOT NULL,
            Quantity INTEGER NOT NULL,
            Date TEXT
        );";
    
    using (var command = new SqliteCommand(createTable, connection))
    {
        command.ExecuteNonQuery();
    }

    connection.Close();
}

void CreateRow(string habit, int quantity, DateOnly date)
{
    string connectionStr = $"Data Source=habits.db";

    Batteries.Init();
    using var connection = new SqliteConnection(connectionStr); 
    connection.Open();

    var createInput = $"""
        INSERT OR IGNORE INTO Habits(Habit, Quantity, Date)
        VALUES ('{habit}', '{quantity}', '{date.ToString()}');
        """;
    
    using (var command = new SqliteCommand(createInput, connection))
    {
        command.ExecuteNonQuery();
    }

    connection.Close();
}

void CheckRow(int id)
{
    string connectionStr = $"Data Source=habits.db";

    Batteries.Init();
    using var connection = new SqliteConnection(connectionStr); 
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = $"""
        SELECT Id, 
              Habit, 
              Quantity, 
              Date
        FROM Habits
        WHERE Id = {id};
        """;
    using var reader = command.ExecuteReader();

    System.Console.WriteLine("** You Selected: **");
    while (reader.Read())
    {
        var habitId = reader.GetString(0);
        var habitName = reader.GetString(1);
        var habitQuantity = reader.GetString(2);
        var habitDate = reader.GetString(3);

        Console.WriteLine($"ID: {habitId} - HABIT: {habitName} - QUANTITY: {habitQuantity} - DATE: {habitDate}");
    }

    connection.Close();
}

void updateRow(int id)
{
    string connectionStr = $"Data Source=habits.db";

    Batteries.Init();
    using var connection = new SqliteConnection(connectionStr); 
    connection.Open();

    string answer = "";
    int answerInt = 0;
    string column = "";
    string value = "";

    string quantityStr = "";
    int quantity = 0;
    DateOnly date;
    string dateStr = "";
    

    while (!int.TryParse(answer, out answerInt) || answerInt < 1 || answerInt > 3) 
    {
        System.Console.WriteLine("Which value would you like to update?");
        System.Console.WriteLine("1) Habit name ....................");
        System.Console.WriteLine("2) Habit quantity ................");
        System.Console.WriteLine("3) Habit date ....................");
        System.Console.WriteLine();
        answer = Console.ReadLine();
        int.TryParse(answer, out answerInt);

        System.Console.WriteLine();
    }

    switch (answerInt)
    {
        case 1:
            column = "Habit";
            System.Console.WriteLine("Input the new name");
            System.Console.WriteLine();
            value = Console.ReadLine();
            break;
        case 2:
            column = "Quantity";
            while (!int.TryParse(quantityStr, out quantity)){
                System.Console.WriteLine("Input the new quantity");
                System.Console.WriteLine();
                quantityStr = Console.ReadLine();
            }
            value = quantity.ToString();
            break;
        case 3:
            column = "Date";
            while (!DateOnly.TryParse(dateStr, out date) || dateStr.ToLower().Trim() != "today" ){
                System.Console.WriteLine("Input the new date");
                System.Console.WriteLine();
                dateStr = Console.ReadLine();
                if (dateStr.ToLower().Trim() == "today") {
                    dateStr = DateTime.Today.ToString();
                    break;
                }
            }
            value = date.ToString();
            break;
    }


    var createInput = $"""
            UPDATE Habits
            SET {column} = {value}
            WHERE Id = {id};
        """;
    
    using (var command = new SqliteCommand(createInput, connection))
    {
        command.ExecuteNonQuery();
    }

    connection.Close();
}

void ReadTable()
{
    System.Console.WriteLine("** YOUR HABITS **");
    
    using var connection = new SqliteConnection("Data Source=habits.db"); 
    connection.Open();
    
    using var command = connection.CreateCommand();
    command.CommandText = """
        SELECT Id, 
              Habit, 
              Quantity, 
              Date
        FROM Habits
    """;
    using var reader = command.ExecuteReader();

    while (reader.Read())
    {
        var habitId = reader.GetString(0);
        var habitName = reader.GetString(1);
        var habitQuantity = reader.GetString(2);
        var habitDate = reader.GetString(3);

        Console.WriteLine($"ID: {habitId} - HABIT: {habitName} - QUANTITY: {habitQuantity} - DATE: {habitDate}");
    }

    connection.Close();

    System.Console.WriteLine();
    System.Console.WriteLine("Press Enter to continue.");
    Console.ReadKey();
}



void CreateInput()
{
    int habitQuantity = 0;
    DateOnly habitDate;

    System.Console.WriteLine("Habit Name:");
    string habitName = Console.ReadLine().Trim();
    System.Console.WriteLine();
    
    System.Console.WriteLine("Quantity:");
    string quantityStr = Console.ReadLine();
    while (!int.TryParse(quantityStr, out habitQuantity))
    {
        System.Console.WriteLine("Invalid quantity. Only integers are accepted!"); 
        System.Console.WriteLine("Quantity:");
        quantityStr = Console.ReadLine();
    }
    System.Console.WriteLine();

    System.Console.WriteLine("Date: (Tip: use 'Today' for today's date!)");
    string dateStr = Console.ReadLine().Trim().ToLower();
    if (dateStr != "today"){
        while (!DateOnly.TryParse(dateStr, out habitDate))
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Invalid date. Only dates are accepted!"); 
            System.Console.WriteLine("Date: (Tip: use 'Today' for today's date!)");
            dateStr = Console.ReadLine();
        }
    }
    else
    {
        habitDate = DateOnly.Parse(DateTime.Today.ToShortDateString());
    }

    System.Console.WriteLine();
    System.Console.WriteLine("** Your Input: **");
    System.Console.WriteLine($"Habit: {habitName}");
    System.Console.WriteLine($"Quantity: {habitQuantity}");
    System.Console.WriteLine($"Date: {habitDate}");
    System.Console.WriteLine();

    System.Console.WriteLine("Is that correct? (y/n)");
    string userAnswer = Console.ReadLine().Trim().ToLower();
    System.Console.WriteLine();

    if (userAnswer == "y")
    {
        System.Console.WriteLine("Saving habit......");
        CreateRow(habitName, habitQuantity, habitDate);
        System.Console.WriteLine("Saved!");
        System.Console.WriteLine();
        System.Console.WriteLine("Press Enter to continue.");
        Console.ReadKey();
    }
    else if (userAnswer == "n")
    {
        System.Console.WriteLine("Habit input canceled!");
    }
    else
    {
        while (userAnswer != "y" || userAnswer != "n")
        {
            System.Console.WriteLine("Please input 'y' for yes and 'n' for no.");
            userAnswer = Console.ReadLine().Trim().ToLower(); 
        }
    }
    System.Console.WriteLine();
}

Main();