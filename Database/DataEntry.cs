
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace HabitsLogger.Database;

/// <summary>
/// Abstract class with common function of each DB-record
/// </summary>
internal abstract class DataEntry
{
   public int Id { get; set; }

   // Commands to insert, delete and update data in individual tables
   public DbManager.SqlCmd? cmdAdd = null;
   public DbManager.SqlCmd? cmdDelete = null;
   public DbManager.SqlCmd? cmdUpdate = null;

   public abstract void GetData (SqliteDataReader reader);
   public abstract void AppendToTable (int id, Table table);

   public abstract void UpdateLine (int currentLine, Table? table, bool showSelected);

   public abstract void UpdateCell (int currentLine, int currentColumn, Table? table, bool showSelected);

   public abstract Tuple<string, string> GetProperty (int index);

   public abstract string SetProperty (int index, string value);

   public abstract bool CheckIfUnique (int index, string value, List<DataEntry> entries);

   /// <summary>
   /// Used to simulate selection / deselection of text on screen
   /// </summary>
   /// <param name="text"></param>
   /// <param name="selected"></param>
   /// <returns></returns>
   protected string ConvertText (string text, bool selected)
   {
      if (!selected)
         return text;

      return $"[white on red]{text}[/]";
   }
}
