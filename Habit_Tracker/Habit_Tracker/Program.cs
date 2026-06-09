using System.Globalization;
using Microsoft.Data.Sqlite;

internal class Program
{
    private static string connectionString = @"Data Source=habit-tracker.db";

    private static void CreateTables()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
            @"CREATE TABLE IF NOT EXISTS habits (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            unit TEXT NOT NULL
            );";
            tableCmd.ExecuteNonQuery();

            tableCmd.CommandText =
            @"CREATE TABLE IF NOT EXISTS habit_entries (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            habitId INTEGER NOT NULL,
            date TEXT NOT NULL,
            quantity INTEGER NOT NULL,
            FOREIGN KEY (habitId) REFERENCES habits(id)
            );";
            tableCmd.ExecuteNonQuery();
        }
    }

    private static void Main(string[] args)
    {
        CreateTables();
        SeedData();
        GetUserInput();
    }

    private static void GetUserInput()
    {
        Console.Clear();
        bool closeapp = false;
        while (closeapp == false)
        {
            Console.WriteLine("Welcome to the Habit Tracker!");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Add a new habit");
            Console.WriteLine("2. Delete a habit");
            Console.WriteLine("3. Update a habit");
            Console.WriteLine("4. View all habits");
            Console.WriteLine("5. Add new entry");
            Console.WriteLine("6. View entries");
            Console.WriteLine("7. View entries by habit");
            Console.WriteLine("8. Update entry");
            Console.WriteLine("9. Delete entry");
            Console.WriteLine("--------------------------------------");

            int choice;
            bool valid = int.TryParse(Console.ReadLine(), out choice);

            if (!valid)
            {
                Console.WriteLine("Enter number only.");
                continue;
            }

            switch (choice)
            {
                case 0:
                    Console.WriteLine("Goodbye!");
                    closeapp = true;
                    Environment.Exit(0);
                    break;

                case 1:
                    AddHabit();
                    break;

                case 2:
                    DeleteHabit();
                    break;

                case 3:
                    UpdateHabit();
                    break;

                case 4:
                    ViewHabits();
                    break;

                case 5:
                    AddEntry();
                    break;

                case 6:
                    ViewEntries();
                    break;

                case 7:
                    ViewEntriesByHabit();
                    break;

                case 8:
                    UpdateEntry();
                    break;

                case 9:
                    DeleteEntry();
                    break;

                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        }
    }

    private static void UpdateEntry()
    {
        Console.Clear();
        ViewEntries();

        int id = GetNumberInput("Enter entry Id to update:");

        string date = GetDateInput("New Date");

        int quantity = GetNumberInput("New quantity:");

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText =
           @"UPDATE habit_entries
           SET date = @date, quantity = @quantity
           WHERE id = @id";

        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@quantity", quantity);
        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();

        Console.WriteLine("Entry updated!");
    }

    private static void DeleteEntry()
    {
        Console.Clear();
        ViewEntries();

        int id = GetNumberInput("Enter entry Id to delete:");

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM habit_entries WHERE id = @id";

        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();

        Console.WriteLine("Entry deleted!");
    }

    private static void ViewEntriesByHabit()
    {
        Console.Clear();

        ViewHabits();

        int habitId = GetNumberInput("Select habit Id:");

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText =
        @"SELECT habit_entries.id, habits.name, habits.unit, habit_entries.date, habit_entries.quantity
      FROM habit_entries
      JOIN habits ON habits.id = habit_entries.habitId
      WHERE habit_entries.habitId = @id";

        cmd.Parameters.AddWithValue("@id", habitId);

        var reader = cmd.ExecuteReader();

        Console.WriteLine("ENTRIES FOR SELECTED HABIT:");
        Console.WriteLine("----------------------------");

        while (reader.Read())
        {
            Console.WriteLine(
                $"Id: {reader.GetInt32(0)} | " +
                $"{reader.GetString(1)} ({reader.GetString(2)}) | " +
                $"{reader.GetString(3)} | " +
                $"{reader.GetInt32(4)}");
        }

        Console.WriteLine("----------------------------");
    }

    private static void ViewEntries()
    {
        Console.Clear();

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText =
        @"SELECT habit_entries.id, habits.name, habits.unit, habit_entries.date, habit_entries.quantity
      FROM habit_entries
      JOIN habits ON habits.id = habit_entries.habitId";

        var reader = cmd.ExecuteReader();

        Console.WriteLine("ENTRIES:");
        Console.WriteLine("----------------------------");

        while (reader.Read())
        {
            Console.WriteLine(
                $"Id: {reader.GetInt32(0)} | " +
                $"{reader.GetString(1)} ({reader.GetString(2)}) | " +
                $"{reader.GetString(3)} | " +
                $"{reader.GetInt32(4)}");
        }

        Console.WriteLine("----------------------------");
    }

    private static void AddEntry()
    {
        Console.Clear();

        ViewHabits();

        int habitId = GetNumberInput("Select habit Id:");

        string date = GetDateInput();

        int quantity = GetNumberInput("Enter quantity:");

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText =
            @"INSERT INTO habit_entries (habitId, date, quantity)
          VALUES (@habitId, @date, @quantity)";

        cmd.Parameters.AddWithValue("@habitId", habitId);
        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@quantity", quantity);

        cmd.ExecuteNonQuery();

        Console.WriteLine("Entry added!");
    }

    private static void DeleteHabit()
    {
        Console.Clear();
        ViewHabits();

        int id = GetNumberInput("Enter habit Id to delete:");

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM habits WHERE id = @id";

        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();

        Console.WriteLine("Habit deleted!");
    }

    private static void AddHabit()
    {
        Console.Clear();

        string name = GetStringInput("Enter habit name:");

        string unit = GetStringInput("Enter unit (e.g. ml, km, pages):");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
            "INSERT INTO habits (name, unit) VALUES (@name, @unit)";

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@unit", unit);

            cmd.ExecuteNonQuery();
        }

        Console.WriteLine("Habit added!");
    }

    private static void UpdateHabit()
    {
        Console.Clear();
        ViewHabits();

        int id = GetNumberInput("Enter habit Id to update:");

        string name = GetStringInput("New name:");

        string unit = GetStringInput("New unit:");

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText =
            "UPDATE habits SET name = @name, unit = @unit WHERE id = @id";

        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@unit", unit);
        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();

        Console.WriteLine("Habit updated!");
    }

    internal static int GetNumberInput(string message)
    {
        Console.WriteLine(message);

        while (true)
        {
            string? input = Console.ReadLine();

            if (input == "0")
            {
                GetUserInput();
                return 0;
            }

            if (int.TryParse(input, out int value) && value >= 0)
            {
                return value;
            }

            Console.WriteLine("Invalid input. Try again.");
        }
    }

    private static string GetDateInput(string? prompt = null)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            Console.WriteLine(prompt + ":");
        }

        Console.WriteLine("Please insert the date in format dd-MM-yyyy.");
        Console.WriteLine("Or type 't' for today.");

        while (true)
        {
            string? dateInput = Console.ReadLine();

            if (dateInput == "0")
            {
                GetUserInput();
                return "";
            }

            if (dateInput?.Equals("t", StringComparison.OrdinalIgnoreCase) == true)
            {
                return DateTime.Now.ToString("dd-MM-yyyy");
            }

            if (DateTime.TryParseExact(
                dateInput,
                "dd-MM-yyyy",
                new CultureInfo("en-US"),
                DateTimeStyles.None,
                out _))
            {
                return dateInput!;
            }

            Console.WriteLine("Invalid date. Try again (dd-MM-yyyy or 't').");
        }
    }

    private static void ViewHabits()
    {
        Console.Clear();

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM habits";

        var reader = cmd.ExecuteReader();

        Console.WriteLine("HABITS:");
        Console.WriteLine("----------------------------");

        while (reader.Read())
        {
            Console.WriteLine($"Id: {reader.GetInt32(0)} | Name: {reader.GetString(1)} | Unit: {reader.GetString(2)}");
        }

        Console.WriteLine("----------------------------");
    }

    private static string GetStringInput(string message)
    {
        while (true)
        {
            Console.WriteLine(message);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine("Invalid input. Try again.");
        }
    }

    private static void SeedData()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();

        cmd.CommandText = "SELECT COUNT(*) FROM habits";
        long count = Convert.ToInt64(cmd.ExecuteScalar());
        if (count > 0) return;

        cmd.CommandText =
"INSERT INTO habits (name, unit) VALUES ('Water', 'glasses')";
        cmd.ExecuteNonQuery();

        cmd.CommandText =
        "INSERT INTO habits (name, unit) VALUES ('Reading', 'pages')";
        cmd.ExecuteNonQuery();
    }
}