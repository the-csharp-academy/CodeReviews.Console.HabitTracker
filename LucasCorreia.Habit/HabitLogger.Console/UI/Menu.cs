using HabitLogger.Console.Queries;

namespace HabitLogger.Console.UI;

public class Menu
{
    private static readonly InputUser _inputUser = new InputUser();
    private static readonly UiMethodsHabit _uiMethodsHabit = new UiMethodsHabit();

    private static readonly UiMethodsRecordHabit _uiMethodsRecordHabit = new UiMethodsRecordHabit();
    
    public static int MenuInit()
    {
        System.Console.WriteLine("""
                                 ***** Welcome to your HabitLogger *****
                                 
                                 Choose your option
                                 
                                 Options Habits:     |  Options Records:
                                 1 - Create a Habit   |  5 - Create a Record
                                 2 - Get all Habits   |  6 - Get full Record of a habit
                                 3 - Update a Habit   |  7 - Update a Record
                                 4- Delete a Habit    |  8 - Delete a record
                                 
                                 
                                 0 - Exit
                                 
                                 Your option:
                                 """);
        
        int option = _inputUser.InputInt(System.Console.ReadLine()!);

        return option;
    }

    public static void MenuAction(int option)
    {
        System.Console.Clear();
        switch (option)
        {
            case 1:
                _uiMethodsHabit.InsertHabit();
                break;
            case 2:
                _uiMethodsHabit.ListsHabits();
                break;
            case 3:
                _uiMethodsHabit.UpdateHabit();
                break;
            case 4:
                _uiMethodsHabit.DeleteHabit();
                break;
            case 5:
                _uiMethodsRecordHabit.InsertRecord();
                break;
            case 6:
                _uiMethodsRecordHabit.ListRecordByIdHabit();
                break;
            case 7:
                _uiMethodsRecordHabit.UpdateRecord();
                break;
            case 8:
                _uiMethodsRecordHabit.DeleteRecord();
                break;
            case 0:
                Environment.Exit(0);
                break;
            default:
                break;
        }
        System.Console.ReadLine();
        System.Console.Clear();
    }
}