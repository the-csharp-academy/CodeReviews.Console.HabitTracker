using Spectre.Console;
using HabitsLogger.Database;

namespace HabitsLogger.UI.Table;

/// <summary>
/// Base class to display and deal with a table on console. 
/// Contains all functionality to display tabular data and to navigate throug it
/// </summary>
internal abstract class DataTable
{
   public enum ProcesMode
   {
      View,
      Add,
      Delete,
      Update
   }

   protected Spectre.Console.Table? _table;
   protected ProcesMode _mode;
   protected List<DataEntry>? _entries;
   
   int _selectedLine;
   int _selectedColumn = 1;

   public DataTable (ProcesMode mode = ProcesMode.View)
   {
      _mode = mode;
   }

   protected abstract void Init ();

   protected virtual void InformNoData ()
   {
      Gui.Info ("There is no data in table");
   }

   protected virtual void ShowTableTitle ()
   {
   }

   protected void FillTable ()
   {
      if (_table == null || _entries == null || _entries.Count <= 0)
         return;

      for (int i = 0; i < _entries.Count; ++i)
         _entries [i].AppendToTable (i + 1, _table);
   }

   public void Show ()
   {
      // The data will be read from database only when the table will be shown for the first time
      if (_entries == null)
      {
         Init ();
         FillTable ();
      }

      if (_entries == null || _table == null)
      {
         Gui.Exception ("The table was not properly initialized");
         return;
      }

      if (_entries.Count <= 0)
      {
         InformNoData ();
         return;
      }

      if (_mode == ProcesMode.View)
         ShowTableData ();
      else
         Select (); // Show table and selecr first record

      ProcessInput ();
   }

   void ShowTableData ()
   {
      ShowTableTitle ();

      if (_table != null)
         AnsiConsole.Write (_table);

      ShowFooter ();
   }

   void ShowFooter ()
   {
      if (_mode == ProcesMode.View)
      {
         AnsiConsole.MarkupLine ("Press [blue]Esc[/] to go back to main menu");
         return;
      }

      Console.WriteLine ();

      var footer = new Spectre.Console.Table ().HideHeaders ().Border (TableBorder.None);
      footer.AddColumn ("Key1", col => col.RightAligned ());
      footer.AddColumn ("Desc1");
      footer.AddColumn ("Key2", col => col.RightAligned ());
      footer.AddColumn ("Desc2");

      if (_mode == ProcesMode.Delete)
      {
         footer.AddRow ("[blue]Up[/]", "- move up", "[blue]Del[/]", "- delete record");
         footer.AddRow ("[blue]Down[/]", "- move down", "[blue]Esc[/]", "- back to main menu");
         footer.AddRow ("[blue]PgUp[/]", "- move to first record", "", "");
         footer.AddRow ("[blue]PgDown[/]", "- move to last record", "", "");
      }
      else
      {
         footer.AddRow ("[blue]Up[/]", "- move up", "[blue]Down[/]", "- move down");
         footer.AddRow ("[blue]Left[/]", "- move left", "[blue]Right[/]", "- move right");
         footer.AddRow ("[blue]PgUp[/]", "- move to first record in column", "[blue]PgDown[/]", "- move to last record in column");
         footer.AddRow ("[blue]Home[/]", "- move to first record in line", "[blue]End[/]", "- move to last record in line");
         footer.AddRow ("[blue]Enter[/]", "- update record", "[blue]Esc[/]", "- back to main menu");
      }

      AnsiConsole.Write (footer);
      Console.CursorVisible = false;
   }

   void ProcessInput ()
   {
      Console.CursorVisible = false;

      for (; ; )
      {
         var keyCode = Console.ReadKey (true).Key;
         if (keyCode == ConsoleKey.Escape)
            break;

         // Navigation is off, if the table shown in "read-only" mode
         if (_mode == ProcesMode.View)
            continue;

         switch (keyCode)
         {
            case ConsoleKey.UpArrow:
               MoveVertical (moveUp: true);
               break;

            case ConsoleKey.DownArrow:
               MoveVertical (moveUp: false);
               break;

            case ConsoleKey.LeftArrow:
               MoveHorizontal (moveLeft: true);
               break;

            case ConsoleKey.RightArrow:
               MoveHorizontal (moveLeft: false);
               break;

            case ConsoleKey.PageDown:
               MoveVertical (moveUp: false, moveToLimit: true);
               break;

            case ConsoleKey.PageUp:
               MoveVertical (moveUp: true, moveToLimit: true);
               break;

            case ConsoleKey.Home:
               MoveHorizontal (moveLeft: true, moveToLimit: true);
               break;

            case ConsoleKey.End:
               MoveHorizontal (moveLeft: false, moveToLimit: true);
               break;

            case ConsoleKey.Delete:
               if (!DeleteCurrentLine ())
                  return;

               break;

            case ConsoleKey.Enter:
               Update (); 
               break;
         }
      }
   }

   /// <summary>
   /// Navigation - move actual selection from top to bottom or vise versa
   /// </summary>
   /// <param name="moveUp"></param>
   /// <param name="moveToLimit"></param>
   void MoveVertical (bool moveUp, bool moveToLimit = false)
   {
      Deselect ();

      if (moveUp)
      {
         if (moveToLimit) // "PgUp" pressed
            _selectedLine = 0;
         else // "Up arrow" pressed
         {
            --_selectedLine;
            if (_selectedLine < 0) // Wrap around
               _selectedLine = _entries?.Count - 1 ?? 0;
         }
      }
      else
      {
         if (moveToLimit) // "PgDown" pressed
            _selectedLine = _entries?.Count - 1 ?? 0;
         else // "Down arrow" pressed
         {
            ++_selectedLine;
            if (_selectedLine >= _entries?.Count) // Wrap around
               _selectedLine = 0;
         }
      }

      Select ();
   }

   /// <summary>
   /// Navigation - move actual selection from left to right or vise versa
   /// </summary>
   /// <param name="moveLeft"></param>
   /// <param name="moveToLimit"></param>
   void MoveHorizontal (bool moveLeft, bool moveToLimit = false)
   {
      if (_mode != ProcesMode.Update)
         return;

      Deselect ();

      if (moveLeft)
      {
         if (moveToLimit) // "Home" pressed
            _selectedColumn = 1;
         else // "Left arrow" pressed
         {
            --_selectedColumn;
            if (_selectedColumn < 1) // Wrap around
               _selectedColumn = _table?.Columns.Count - 1 ?? 0;
        }
      }
      else
      {
         if (moveToLimit) // "End" pressed
            _selectedColumn = _table?.Columns.Count - 1 ?? 0;
         else // "Right arrow" pressed
         {
            ++_selectedColumn;
            if (_selectedColumn >= _table?.Columns.Count) // Wrap around
               _selectedColumn = 1;
         }
      }

      Select ();
   }

   /// <summary>
   /// Shows actial line or cell as selected
   /// </summary>
   void Select ()
   {
      if (_entries == null)
         return;

      // In mode "delete" will be selected entire line of table.
      // In mode "update" will be selected only one cell
      var entry = _entries [_selectedLine];
      if (_mode == ProcesMode.Delete)
         entry.UpdateLine (_selectedLine, _table, showSelected: true);
      else
         entry.UpdateCell (_selectedLine, _selectedColumn, _table, showSelected: true);

      Gui.ShowMainTitel ();
      ShowTableData ();
   }

   void Deselect ()
   {
      if (_entries == null)
         return;

      var entry = _entries [_selectedLine];
      if (_mode == ProcesMode.Delete)
         entry.UpdateLine (_selectedLine, _table, showSelected: false);
      else
         entry.UpdateCell (_selectedLine, _selectedColumn, _table, showSelected: false);
   }

   /// <summary>
   /// Deletes actual selected record from table and from database
   /// </summary>
   /// <returns></returns>
   bool DeleteCurrentLine ()
   {
      if (_mode != ProcesMode.Delete || _entries == null || _table == null)
         return true;

      if (_entries.Count <= 0)
         return false;

      // Delete record from database
      DbManager.ExecuteCommand (_entries [_selectedLine].cmdDelete);

      // Recreate table - first delete all records from it and then fill it with existed records from database
      for (int i = _entries.Count - 1; i >= 0; --i)
         _table.RemoveRow (i);

      _entries.RemoveAt (_selectedLine);
      if (_entries.Count <= 0)
         return false;

      if (_selectedLine >= _entries.Count)
         _selectedLine = _entries.Count - 1;

      FillTable ();
      Select ();

      return true;
   }

   /// <summary>
   /// Support for updating data in selected cell
   /// </summary>
   void Update ()
   {
      if (_mode != ProcesMode.Update || _entries == null || _entries.Count <= 0)
         return;

      DataEntry entry = _entries [_selectedLine];
      var (name, value) = entry.GetProperty (_selectedColumn);

      // Ask a new data until all inputs will be correct  
      for (; ; )
      {
         Console.Clear ();
         Gui.ShowMainTitel ();

         AnsiConsole.WriteLine ();
         AnsiConsole.Markup ($"Actual value of property [green]'{name}'[/]: {value}\n");

         string newValue = Gui.GetUserInput ("Please enter a new value ('Enter' to make no changes): ");
         if (string.IsNullOrEmpty (newValue))
            break;

         // Some values must be unique. We check, if it is so for newly entered data
         if (!entry.CheckIfUnique (_selectedColumn, newValue, _entries))
         {
            Gui.Exception ("There is already data with this value. Please enter another value for this property.");
            continue;
         }

         // Transfer entered data in actual record
         string exception = entry.SetProperty (_selectedColumn, newValue);
         if (!string.IsNullOrEmpty (exception))
         {
            Gui.Exception (exception);
            continue;
         }

         // Update data in database
         DbManager.ExecuteCommand (entry.cmdUpdate);
         
         break;
      }

      Select ();
   }
}
