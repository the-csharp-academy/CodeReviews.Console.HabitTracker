using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitTracker.minhquang932004;

public class Program
{
    static readonly string connectionString = @"Data Source=Habbit-Tracker.db";
    static void Main(string[] args)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS habit_tracker (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Date TEXT,
                    Quantity INTEGER,
                    MeasurementUnit TEXT
                    )";

            tableCmd.ExecuteNonQuery();

            SeedDataIfEmpty(connection);
        }

        GetUserInput();
    }

    static void GetUserInput()
    {
        bool closeApp = false;
        while (closeApp == false)
        {
            Console.Clear();
            Console.WriteLine("\n\nMAIN MENU");
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("\nType 0 to Close Application");
            Console.WriteLine("Type 1 to View All Records");
            Console.WriteLine("Type 2 to Insert New Record");
            Console.WriteLine("Type 3 to Update Existed Record");
            Console.WriteLine("Type 4 to Delete A Record");
            Console.WriteLine("---------------------------\n");

            string? userInput = Console.ReadLine();

            switch (userInput)
            {
                case "0":
                    Console.WriteLine("");
                    closeApp = true;
                    break;
                case "1":
                    GetAllRecords();
                    PauseConsole();
                    break;
                case "2":
                    Insert();
                    PauseConsole();
                    break;
                case "3":
                    Update();
                    PauseConsole();
                    break;
                case "4":
                    Delete();
                    PauseConsole();
                    break;
                default:
                    Console.WriteLine("\nInvalid input, try again!");
                    break;
            }
        }
    }

    private static void Insert()
    {
        Console.Clear();

        string? name = GetNameInput();
        if (name == null)
            return;

        string? date = GetDateInput();
        if (date == null)
            return;

        int? quantity = GetNumberInput("\n\nPlease insert the number (quantity) for your habit. No decimal is allowed!");
        if (quantity == null)
            return;

        string? measurementUnit = GetMeasurementUnitInput();
        if (measurementUnit == null)
            return;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
                $"INSERT INTO habit_tracker(name, date, quantity, measurementUnit) VALUES(@Name, @Date, @Quantity, @MeasurementUnit)";

            // Simplicity approach of Parameterized queries
            tableCmd.Parameters.AddWithValue("@Name", name);
            tableCmd.Parameters.AddWithValue("@Date", date);
            tableCmd.Parameters.AddWithValue("@Quantity", quantity);
            tableCmd.Parameters.AddWithValue("@MeasurementUnit", measurementUnit);

            tableCmd.ExecuteNonQuery();

        }
        Console.WriteLine($"\n>>>This is your new habbit record: {name} - {date} - {quantity} - {measurementUnit}<<<\n");
    }

    private static void GetAllRecords()
    {
        Console.Clear();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = $"SELECT * FROM habit_tracker ";

            List<Habit> tableData = [];

            SqliteDataReader reader = tableCmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    tableData.Add(
                        new Habit
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Date = DateTime.ParseExact(reader.GetString(2), "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None),
                            Quantity = reader.GetInt32(3),
                            MeasurementUnit = reader.GetString(4)
                        });
                }
            }
            else
                Console.WriteLine("No rows found!");

            Console.WriteLine("List of habits: \n");
            Console.WriteLine("--------------------------");
            foreach (var habbit in tableData)
            {
                Console.WriteLine($"Id: {habbit.Id} - {habbit.Name} - {habbit.Date:dd-MM-yyyy} - Quantity: {habbit.Quantity} - {habbit.MeasurementUnit}");
            }
            Console.WriteLine("--------------------------\n");
        }
    }

    private static void Delete()
    {
        Console.Clear();
        GetAllRecords();

        var recordId = GetNumberInput("\n\nPlease type the Id of record you want to delete! \n");

        if (recordId == null)
            return;

        if (!ConfirmDelete(recordId.Value))
            return;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = $"DELETE FROM habit_tracker WHERE Id = @RecordId";

            tableCmd.Parameters.AddWithValue("@RecordId", recordId);

            int rowCount = tableCmd.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.");
                PauseConsole();
                Delete();
            }
        }
        Console.WriteLine($"\n>>>Record with Id {recordId} was deleted.<<<\n");
    }

    private static void Update()
    {
        Console.Clear();
        GetAllRecords();

        var recordId = GetNumberInput("\n\nPlease type the Id of record you want to update!");

        if (recordId == null)
            return;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM habit_tracker WHERE Id = {recordId})";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                Console.WriteLine($"\n\n>>>Record with Id {recordId} doesn't exist.<<<\n");
                Update();
            }

            string? name = GetNameInput();
            if (name == null)
                return;

            string? date = GetDateInput();
            if (date == null)
                return;

            int? quantity = GetNumberInput("Please insert the number (quantity) for your habit. No decimal is allowed!");
            if (quantity == null)
                return;

            string? measurementUnit = GetMeasurementUnitInput();
            if (measurementUnit == null)
                return;

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"UPDATE habit_tracker SET name = @Name, date = @Date, quantity = @Quantity, measurementUnit = @MeasurementUnit WHERE Id = @RecordId";

            tableCmd.Parameters.AddWithValue("@Name", name);
            tableCmd.Parameters.AddWithValue("@Date", date);
            tableCmd.Parameters.AddWithValue("@Quantity", quantity);
            tableCmd.Parameters.AddWithValue("@MeasurementUnit", measurementUnit);
            tableCmd.Parameters.AddWithValue("@RecordId", recordId);

            tableCmd.ExecuteNonQuery();

            Console.WriteLine($"\n>>>The record with Id {recordId} has been updated succesfully!<<<\n");
        }
    }

    public static int? GetNumberInput(string message)
    {
        Console.WriteLine(message);
        Console.Write("Enter the number (Or type 0 to return): ");

        string? numberInput = Console.ReadLine();
        int finalInput;

        if (numberInput == "0")
            return null;

        while (!int.TryParse(numberInput, out finalInput) || finalInput < 0)
        {
            Console.WriteLine("\n!!!!---Invalid number---!!!!\n");
            Console.Write("Type again (Or exit with 0): ");
            numberInput = Console.ReadLine();

            if (numberInput == "0")
                return null;
        }
        return finalInput;
    }

    public static string? GetDateInput()
    {
        Console.WriteLine("\nPlease insert the date: (Format: dd-mm-yy).");

        Console.Write("Enter the date (Or type 0 to return): \n");
        string? dateInput = Console.ReadLine();

        if (dateInput == "0")
            return null;

        while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            Console.WriteLine("\n!!!!---Invalid Format!---!!!!\n");
            Console.Write("Type again (Or exit with 0): ");
            dateInput = Console.ReadLine();

            if (dateInput == "0")
                return null;
        }
        return dateInput;
    }

    public static string? GetNameInput()
    {
        Console.WriteLine("\nPlease insert the name of habbit.");
        Console.Write("Enter the name (Or type 0 to return): \n");


        string? nameInput = Console.ReadLine();

        if (nameInput == "0")
            return null;

        while (IsNotValidString(nameInput))
        {
            Console.WriteLine("\n!!!!---Invalid Format (Maybe your name was empty, or you had numbers inside)!---!!!!\n");
            Console.Write("Type again (Or exit with 0): ");
            nameInput = Console.ReadLine();

            if (nameInput == "0")
                return null;
        }
        return nameInput;
    }

    public static string? GetMeasurementUnitInput()
    {
        while (true)
        {
            Console.WriteLine("\nPick measurement unit for the habbit: ");
            Console.WriteLine("1. times");
            Console.WriteLine("2. glasses");
            Console.WriteLine("3. pages");
            Console.WriteLine("4. custom");
            Console.WriteLine("0. back to main menu\n");

            Console.Write("Enter your desire input: ");
            string? userInput = Console.ReadLine();

            switch (userInput)
            {
                case "0":
                    return null;
                case "1":
                    return "times";
                case "2":
                    return "glasses";
                case "3":
                    return "pages";
                case "4":
                    Console.Write("\nEnter the custom measurement unit: ");
                    string? custom = Console.ReadLine();

                    if (IsNotValidString(custom) == true)
                    {
                        Console.WriteLine("\n!!!!---Measurement unit cannot be empty or contains numbers.---!!!!\n");
                        continue;
                    }
                    return custom.Trim();
                default:
                    Console.WriteLine("\n!!!!---Unrecognized number! Try again.---!!!!\n");
                    break;
            }
        }
    }

    internal static void PauseConsole()
    {
        Console.WriteLine("\n>>>Enter to contiune<<<\n");
        string? input = Console.ReadLine();
    }

    internal static void SeedDataIfEmpty(SqliteConnection connection)
    {
        using (var checkCmd = connection.CreateCommand())
        {
            checkCmd.CommandText = "SELECT COUNT(*) FROM habit_tracker";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                using (var seedCmd = connection.CreateCommand())
                {
                    seedCmd.CommandText =
                    @"INSERT INTO habit_tracker(name, date, quantity, measurementUnit) VALUES (@Name, @Date, @Quantity, @Unit)";

                    seedCmd.Parameters.Add("@Name", SqliteType.Text);
                    seedCmd.Parameters.Add("@Date", SqliteType.Text);
                    seedCmd.Parameters.Add("@Quantity", SqliteType.Integer);
                    seedCmd.Parameters.Add("@Unit", SqliteType.Text);

                    // Use tuples for seed data (Easier vs Using List<Habit>)
                    // Also Implementing the Advanced Pattern in Handle Multiple Records
                    var habits = new[]
                    {
                        (Name: "Running", Date: "15-07-26", Quantity: 5, Unit: "times"),
                        (Name: "Reading", Date: "16-07-26", Quantity: 8, Unit: "pages"),
                        (Name: "Meditation", Date: "17-07-26", Quantity: 1, Unit: "times")
                    };

                    foreach (var seedData in habits)
                    {
                        seedCmd.Parameters["@Name"].Value = seedData.Name;
                        seedCmd.Parameters["@Date"].Value = seedData.Date;
                        seedCmd.Parameters["@Quantity"].Value = seedData.Quantity;
                        seedCmd.Parameters["@Unit"].Value = seedData.Unit;

                        seedCmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine(">>>Database seeded with initial data!<<<\n");
                PauseConsole();
            }
        }
    }

    public static bool ConfirmDelete(int id)
    {
        while (true)
        {
            Console.Write($"\nAre you sure about delete record with Id {id}? Enter here (y/n): ");
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (input == "y")
            {
                Console.WriteLine($"\n>>>You have processed to delete of the Record with Id {id}<<<");
                return true;
            }
            if (input == "n")
            {
                Console.WriteLine($"\n>>>You have processed not to delete the Record with Id {id}<<<");
                return false;
            }
            Console.WriteLine("Please type y or n only.");
        }
    }

    public static bool IsNotValidString(string? input)
    {
        return string.IsNullOrWhiteSpace(input) || input.Any(char.IsDigit);
    }
}

public class Habit
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public required string MeasurementUnit { get; set; }
}