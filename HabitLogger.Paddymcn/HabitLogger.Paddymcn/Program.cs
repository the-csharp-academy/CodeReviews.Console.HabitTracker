using HabitLogger.Paddymcn.Logic;
using HabitLogger.Paddymcn.Database;
using Microsoft.Data.Sqlite;

var databaseHandler = new DatabaseHandler();
try
{
    databaseHandler.EnsureCreated();
}
catch (SqliteException ex)
{
    Console.WriteLine($"Couldn't set up the database: {ex.Message}");
    return;
}
var logic = new HabitLoggerLogic();



DisplayMenu();
void DisplayMenu()
{
    string menuChoice="";
    while (menuChoice != "6")
    {

        Console.WriteLine("Menu\n====");
        Console.WriteLine("1. Add Entry");
        Console.WriteLine("2. Read All Entries");
        Console.WriteLine("3. Read Entry by ID");
        Console.WriteLine("4. Update Entry");
        Console.WriteLine("5. Delete Entry by ID");
        Console.WriteLine("6. Exit");
        menuChoice = Console.ReadLine() ?? "6";

        try
        {
            MenuChoice(menuChoice);
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Database problem: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong: {ex.Message}");
        }
    }
}

void MenuChoice(string menuChoice)
{
        switch (menuChoice)
        {
            case "1":
                databaseHandler.SaveToDatabase(logic.PromptForBiscuitDetails());
                break;
            case "2":
                databaseHandler.GetAll();
                break;
            case "3":
                databaseHandler.GetById(logic.CheckEntry());
                break;
            case "4":
                int id = logic.CheckEntry();
                if (!databaseHandler.checkIdExists(id))
                {
                    Console.WriteLine($"Id {id} does not exist");
                    break;
                }
                databaseHandler.UpdateEntry(id, logic.PromptForBiscuitDetails());
                break;
            case "5":
                databaseHandler.Delete(logic.CheckEntry());
                break;
            case "6":
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid choice, try again.");
                break;
        }
}