using Habit_Logger;

public class Program
{
    static void Main()
    {
        var dbManager = new DatabaseManager();
        dbManager.InitializeDatabase();

        var menu = new Menu(dbManager);
        menu.CallMenu();
    }
}