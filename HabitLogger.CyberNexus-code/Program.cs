
using HabitLogger.Interfaces;
using HabitLogger.Services;
using Microsoft.Data.Sqlite;
using HabitLogger.UI;

string ConnectionString  = "Data Source=./db/database.db";

Console.WriteLine("starting Program.cs");

IInitializeDB database = new InitializeDB();
database.InitializeDatabase();

using var connection = new SqliteConnection(ConnectionString);


Console.WriteLine("Checking Tables...");

var table = new InitializeTable(connection);

table.InitializeTables();
table.SeedTable();


var repository = new HabitRepository(connection);
var manageHabitsUI = new ManageHabitsUI(repository);
var manageLogEntriesUI = new ManageLogEntriesUI(repository);
var optionsUI = new OptionsUI(manageHabitsUI, manageLogEntriesUI);

var mainMenu = new MainMenu();

while (true)
{
    Console.Clear();

    if (mainMenu.RenderMenu(optionsUI, manageLogEntriesUI, repository))
    {
        break;
    }
}








