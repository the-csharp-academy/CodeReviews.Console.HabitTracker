using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HabitLogger.Console.Database;
using HabitLogger.Console.UI;

DatabaseInitialize.Initialize();

int option = -1;
do
{
    try
    {
        Console.Clear();
        option = Menu.MenuInit();
        Menu.MenuAction(option); 
    }
    catch(FormatException e)
    {
        Console.WriteLine("ERROR: " + e.Message);
        Console.ReadLine();
    }
    catch(ArgumentException e)
    {
        Console.WriteLine("ERROR: " + e.Message);
        Console.ReadLine();
    }
    
}while (option != 0);
