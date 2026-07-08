
using HabitsLogger.Database;
using HabitsLogger.UI;
using Spectre.Console;

namespace HabitsLogger;

internal class App
{
   public void Run ()
   {
      if (DbManager.CreateEmptyDatabase ())
      {
         AnsiConsole.WriteLine ();
         AnsiConsole.Write ("  ");
         if (Gui.Ask ("The database has no data. Would you like to populate it with some random records?"))
            DbManager.FillWithRandomData ();
      }

      while (!Gui.ShowMainMenu ())
      {
      }
   }
}
