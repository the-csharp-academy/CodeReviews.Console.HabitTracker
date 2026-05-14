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
                break;
            case 3: // update
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