using System.Globalization;
using Microsoft.Data.Sqlite;

namespace habit_tracker
{
    class Program
    {
        static string connectionString = @"Data Source=habit-Tracker.db";

        static void Main(string[] args)
        {

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                    )";

                tableCmd.ExecuteNonQuery();
                
                connection.Close();
            }

            GetUserInput();
        }

        static void GetUserInput()
        {
            bool closeApp = false;
            while(closeApp == false)
            {
                Console.WriteLine("\n\nMain Menu");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to close Application");
                Console.WriteLine("Type 1 to View All Records");
                Console.WriteLine("Type 2 to Add New Record");
                Console.WriteLine("Type 3 to Delete Record");
                Console.WriteLine("Type 4 to Update Record");
                Console.WriteLine("-----------------------------");

                string commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("Goodbye!");
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        AddNewRecord();
                        break;
                    case "3":
                        DeleteRecord();
                        break;
                    case "4":
                        UpdateRecord();
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please try again.");
                        break;
                }
            }
        }

        private static void GetAllRecords()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                                Quantity = reader.GetInt32(2)
                            }
                        );
                    }
                }
                else
                {
                    Console.WriteLine("No rows found!");
                }
                    
                connection.Close();

                Console.WriteLine("----------------\n");

                foreach(var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MMM-yyyy")} - Quantity: {dw.Quantity}");
                }
                Console.WriteLine("----------------\n");
            }
        }

        private static void AddNewRecord()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease enter the number of glasses or other measure of your choice (no decimals allowed)\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";
                    
                tableCmd.ExecuteNonQuery();
                
                connection.Close();
            }
        }

        private static void DeleteRecord()
        {

            GetAllRecords();

            var recordId = GetNumberInput("\n\nType in the record Id you want to delete");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"Delete from drinking_water WHERE Id = '{recordId}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"Record with Id {recordId} doesn't exist");
                    DeleteRecord();
                }

                Console.WriteLine($"Record with Id {recordId} was deleted");

                GetUserInput();
            }
        }

        private static void UpdateRecord()
        {
            GetAllRecords();

            var recordId = GetNumberInput("\n\nType in the record Id you want to update");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if(checkQuery == 0)
                {
                    Console.WriteLine("Record doesn't exist");
                    connection.Close();
                    UpdateRecord();
                }

                string date = GetDateInput();

                int quantity = GetNumberInput("\n\nPlease enter the number of glasses or other measure of your choice (no decimals allowed)\n\n");


                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE Id = {recordId}";

                int rowCount = tableCmd.ExecuteNonQuery();

                Console.WriteLine($"Record with Id {recordId} was updated");

                connection.Close();
            }

        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\nPlease enter the date in the format dd-mm-yy (or type 0 to return to the main menu):");
            string? dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
                {
                    Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main menu or try again:\n\n");
                    dateInput = Console.ReadLine();
                }

            return dateInput;
        }

        public static int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string? numberInput = Console.ReadLine();

            if (numberInput == "0") GetUserInput();

            while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
                {
                    Console.WriteLine("\n\nInvalid number. Try again.\n\n");
                    numberInput = Console.ReadLine();
                }

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }
    }

    public class DrinkingWater
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

    }
}