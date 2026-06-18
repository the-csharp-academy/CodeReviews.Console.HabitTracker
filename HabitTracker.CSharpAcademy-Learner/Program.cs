using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Data.Sqlite;

namespace habit_tracker
{
    class Program
    {
        static string connectionString = @"Data Source=habit-tracker.db";

        static void Main(string[] args)
        {
            // Code
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
            Console.Clear();
            bool closeApp = false;

            while (!closeApp)
            {
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View All Records.");
                Console.WriteLine("Type 2 to Insert Record.");
                Console.WriteLine("Type 3 to Delete Record.");
                Console.WriteLine("Type 4 to Update Record.");
                Console.WriteLine("------------------------------------------\n");

                string? commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;
                    default:
                        Console.WriteLine("\nInvalid command. Please type a number from 0 to 4.\n");
                        break;
                }
            }
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-MM-yy). Type 0 to return to the main menu. Type 'Today' (without quotes) to use today's date.\n");
            string? dateInput = Console.ReadLine();

            if (dateInput == "0")
            {
                GetUserInput();
            }

            if (dateInput != null && dateInput.Trim().ToLower() == "today")
            {
                dateInput = DateTime.Now.ToString("dd-MM-yy");
            }

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-MM-yy). Type 0 to return to the main menu or try again:\n");
                dateInput = Console.ReadLine();
                if (dateInput == "0")
                {
                    GetUserInput();
                }
            }

            return dateInput;
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string? numberInput = Console.ReadLine();

            if (numberInput == "0")
            {
                GetUserInput();
            }

            while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("\n\nInvalid number. Try again.\n");
                numberInput = Console.ReadLine();
                if (numberInput == "0")
                {
                    GetUserInput();
                }
            }

            int finalInput = Convert.ToInt32(numberInput);
            return finalInput;
        }

        private static void Insert()
        {
            string date = GetDateInput();
            int quantity = GetNumberInput("\n\nPlease insert a number of glasses (no decimals allowed):\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO drinking_water(date, quantity) VALUES(@Date, @Quantity)";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private static void GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new List<DrinkingWater>();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        });
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                Console.WriteLine("------------------------------------------\n");

                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MM-yyyy")} - Quantity: {dw.Quantity} glasses");
                }

                Console.WriteLine("------------------------------------------\n");
            }
        }

        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to delete or type 0 to go back to the main menu:\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE from drinking_water WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", recordId);

                    int rowCount = command.ExecuteNonQuery();

                    if (rowCount == 0)
                    {
                        Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. Try again.\n");
                        connection.Close();
                        Delete();
                        return;
                    }

                    Console.WriteLine($"\n\nRecord with Id {recordId} was successfully deleted.\n");
                }

                connection.Close();
            }
        }

        internal static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type the Id of the record you would like to update. Type 0 to return to the main menu.\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = @Id)";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", recordId);

                    int checkQuery = Convert.ToInt32(command.ExecuteScalar());

                    if (checkQuery == 0)
                    {
                        Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n");
                        connection.Close();
                        Update();
                        return;
                    }
                }

                string date = GetDateInput();
                int quantity = GetNumberInput("\n\nPlease insert a number of glasses (no decimals allowed):\n");

                query = "UPDATE drinking_water SET date = @Date, quantity = @Quantity WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@Id", recordId);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }

    public class DrinkingWater
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}