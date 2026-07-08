using HabitsLogger.Database;
using HabitsLogger.UI.Table;
using Spectre.Console;
namespace HabitsLogger.UI.Menu;

/// <summary>
/// Menu of all habits
/// </summary>
internal class HabitsMenu : ConsoleMenu
{
   public HabitsMenu (string title, DataTable.ProcesMode mode)
      : base (title)
   {
      var habits = DbManager.GetAllHabits ();

      MenuAction action = mode switch
      {
         DataTable.ProcesMode.View => ViewLogs,
         DataTable.ProcesMode.Add => AddLog,
         DataTable.ProcesMode.Delete => DeleteLog,
         DataTable.ProcesMode.Update => UpdateLog,
         _ => throw new NotSupportedException ()
      };

      foreach (var habit in habits)
         AddItem (((HabitEntry) habit).Description, action);

      AddItem ("<Back to previous menu>", (item) => { return true; });
   }

   /// <summary>
   /// Display all logs of selected habit
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   bool ViewLogs (string item)
   {
      LogTable table = new (item);
      table.Show ();
      return false;
   }

   /// <summary>
   /// Append a new log to selected habit
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   bool AddLog (string item)
   {
      HashSet<string> names = new ();

      var habits = DbManager.GetAllHabits ();
      string habitName = item.Trim ().ToLower ();

      // Assign to new log ID of habit, which it belongs to
      LogEntry? newLog = null;
      foreach (var habit in habits)
      {
         HabitEntry entry = (HabitEntry) habit;
         if (entry.Description.Trim ().ToLower () == habitName)
         {
            newLog = new ();
            newLog.HabitId = entry.Id;
            newLog.QuantityDesc = entry.QuantityDesc;
            break;
         }
      }

      if (newLog == null)
         return false;


      string prevAnswer = string.Empty;

      // Get date of log
      for (; ; )
      {
         Console.Clear ();
         Gui.ShowMainTitel ();
         AnsiConsole.MarkupLine ($"Add new log to habit [green]'{item}'[/]\n");

         string input = Gui.GetUserInput ("Please enter a date of new log entry in format 'dd-mm-yy' or type 'now' to use actual date: ");
         string date = LogEntry.ParseDate (input);
         if (string.IsNullOrEmpty (date))
         {
            Gui.Exception ($"The parameter was entered in incorrect format. Expected format: dd-mm-yy");
            continue;
         }

         newLog.Date = date;

         prevAnswer = "Please enter a date of new log entry in format 'dd-mm-yy' or type 'now' to use actual date: " + input;

         break;
      }

      // Get quantity, which will be logged 
      for (; ; )
      {
         int quantity = LogEntry.ParseQuantity (Gui.GetUserInput ($"Please enter quantity in {newLog.QuantityDesc} (not negative number): "));
         if (quantity < 0)
         {
            Gui.Exception ($"The entered parameter is incorrect. Expected not negative number");
            
            Console.Clear ();
            Gui.ShowMainTitel ();
            AnsiConsole.MarkupLine ($"Add new log to habit [green]'{item}'[/]\n");
            AnsiConsole.WriteLine (prevAnswer);

            continue;
         }

         newLog.Quantity = quantity;
         break;
      }

      DbManager.ExecuteCommand (newLog.cmdAdd);
      
      Gui.Info ($"\n[green]The new log was appened to the database[/]");

      return false;
   }

   /// <summary>
   /// Show all logs of habit to delete from database
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   bool DeleteLog (string item)
   {
      LogTable table = new LogTable (item, DataTable.ProcesMode.Delete);
      table.Show ();

      return false;
   }

   /// <summary>
   /// Show all logs of habit to update their data
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   bool UpdateLog (string item)
   {
      LogTable table = new LogTable (item, DataTable.ProcesMode.Update);
      table.Show ();

      return false;
   }
}
