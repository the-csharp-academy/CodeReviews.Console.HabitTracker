using Microsoft.Data.Sqlite;

class Program
{
  static readonly string connectionString = new SqliteConnectionStringBuilder
  {
    DataSource = "HabitTracker.db",
    ForeignKeys = true
  }.ToString();

  static void Main()
  {
    Console.Clear();
    try
    {
      InitializeDatabase();
    }
    catch (SqliteException ex)
    {
      Console.WriteLine($"Could not initialize the database: {ex.Message}");
      return;
    }
    catch (IOException ex)
    {
      Console.WriteLine($"Could not access the database: {ex.Message}");
      return;
    }
    catch (UnauthorizedAccessException ex)
    {
      Console.WriteLine($"Permission denied while accessing the database: {ex.Message}");
      return;
    }

    bool isRunning = true;

    while (isRunning)
    {
      string? choice = DisplayMenu();

      try
      {
        switch (choice)
        {
          case "1":
            ViewHabits();
            break;
          case "2":
            ViewRecords();
            break;
          case "3":
            AddHabit();
            break;

          case "4":
            AddRecord();
            break;

          case "5":
            UpdateRecord();
            break;

          case "6":
            DeleteRecord();
            break;

          case "7":
            isRunning = false;
            Console.WriteLine("Goodbye!");
            break;

          default:
            Console.WriteLine("Invalid option. Please choose 1-7.");
            break;
        }
      }
      catch (SqliteException ex)
      {
        Console.WriteLine($"The database operation failed: {ex.Message}");
      }
      catch (IOException ex)
      {
        Console.WriteLine($"A database file access error occurred: {ex.Message}");
      }
      catch (UnauthorizedAccessException ex)
      {
        Console.WriteLine($"Permission denied while accessing the database: {ex.Message}");
      }
    }
  }

  static SqliteConnection OpenConnection()
  {
    var connection = new SqliteConnection(connectionString);
    connection.Open();
    return connection;
  }

  static void InitializeDatabase()
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    command.CommandText = """
    CREATE TABLE IF NOT EXISTS Habits(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Unit TEXT NOT NULL
    );
    """;
    command.ExecuteNonQuery();


    command.CommandText = """
    CREATE TABLE IF NOT EXISTS HabitEntries (
      Id INTEGER PRIMARY KEY AUTOINCREMENT,
      HabitId INTEGER NOT NULL,
      Quantity INTEGER NOT NULL,
      OccurrenceDate TEXT NOT NULL,
      FOREIGN KEY (HabitId) REFERENCES Habits(Id)
    );
    """;

    command.ExecuteNonQuery();

    command.CommandText = "SELECT COUNT(*) FROM Habits;";

    long habitCount = Convert.ToInt64(command.ExecuteScalar());

    if (habitCount == 0)
    {
      SeedHabits();
    }
    else
    {
      Console.WriteLine("Habits already exist. Skipping seed.");
    }

    command.CommandText = "SELECT COUNT(*) FROM HabitEntries;";

    long recordCount = Convert.ToInt64(command.ExecuteScalar());

    if (recordCount == 0)
    {
      Console.WriteLine("Database is empty. Seeding data...");
      SeedDatabase();
    }
    else
    {
      Console.WriteLine("Records already exist. Skipping seed.");

    }

    Console.WriteLine("Database initialized successfully!");
  }

  static string? DisplayMenu()
  {
    Console.WriteLine("\n===== HABIT TRACKER =====\n");
    Console.WriteLine("1. View Habit");
    Console.WriteLine("2. View all entries");
    Console.WriteLine("3. Add New Habit");
    Console.WriteLine("4. Add a entry");
    Console.WriteLine("5. Update an entry");
    Console.WriteLine("6. Delete an entry");
    Console.WriteLine("7. Exit\n");

    Console.Write("Choose an option: ");
    return Console.ReadLine();
  }



  static bool TryReadPositiveInt(string prompt, out int value)
  {
    Console.Write(prompt);
    return int.TryParse(Console.ReadLine(), out value) && value > 0;
  }

  static string? ReadDateInput(string prompt, bool allowTodayOption = false)
  {
    if (allowTodayOption)
    {
      Console.WriteLine("Do you want to use today's date? (y/n): ");
      string? choice = Console.ReadLine();
      if (string.Equals(choice, "y", StringComparison.OrdinalIgnoreCase))
      {
        return DateTime.Today.ToString("yyyy-MM-dd");
      }
    }

    Console.Write(prompt);
    return Console.ReadLine();
  }

  static bool IsValidDate(string? date)
  {
    return DateTime.TryParseExact(
      date,
      "yyyy-MM-dd",
      null,
      System.Globalization.DateTimeStyles.None,
      out _);
  }

  static bool RecordExists(int recordId)
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    command.CommandText = """
    SELECT COUNT(*)
    FROM HabitEntries
    WHERE Id = $id;
    """;

    command.Parameters.AddWithValue("$id", recordId);
    return Convert.ToInt64(command.ExecuteScalar()) > 0;
  }

  static (int Quantity, string OccurrenceDate) GetRecord(int recordId)
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    command.CommandText = """
    SELECT Quantity, OccurrenceDate
    FROM HabitEntries
    WHERE Id = $id;
    """;

    command.Parameters.AddWithValue("$id", recordId);

    using var reader = command.ExecuteReader();
    if (!reader.Read())
    {
      throw new InvalidOperationException("Record not found.");
    }

    return (reader.GetInt32(0), reader.GetString(1));
  }

  static void AddHabit()
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();


    string? userHabit = "";
    while (string.IsNullOrWhiteSpace(userHabit))
    {
      Console.Write("Enter habit name: ");
      userHabit = Console.ReadLine();
    }

    string? userUnit = "";
    while (string.IsNullOrWhiteSpace(userUnit))
    {
      Console.Write("Enter unit: ");
      userUnit = Console.ReadLine();
    }

    command.CommandText = """
    INSERT INTO Habits (Name, Unit)
    VALUES ($userHabit, $userUnit)
    """;

    command.Parameters.AddWithValue("$userHabit", userHabit);
    command.Parameters.AddWithValue("$userUnit", userUnit);

    command.ExecuteNonQuery();
    Console.WriteLine("Habit added successfully");

  }
  static void ViewRecords()
  {
    Console.WriteLine($"\n{"Id",-10} {"Habit",-20} {"Quantity",-12} {"Unit",-12} {"Date",-12}");

    Console.WriteLine("-----------------------------------------------------------");

    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    command.CommandText = """
    SELECT 
      HabitEntries.Id,
      Habits.Name,
      HabitEntries.Quantity,
      Habits.Unit,
      HabitEntries.OccurrenceDate 
    FROM HabitEntries
    INNER JOIN Habits
    ON HabitEntries.HabitId = Habits.Id;
    """;

    using var reader = command.ExecuteReader();
    bool hasRecords = false;

    while (reader.Read())
    {
      hasRecords = true;

      int id = reader.GetInt32(0);
      string name = reader.GetString(1);
      int quantity = reader.GetInt32(2);
      string unit = reader.GetString(3);
      string date = reader.GetString(4);

      Console.WriteLine($"{id,-10} {name,-20} {quantity,-12} {unit,-12} {date,-12}");
    }

    if (!hasRecords)
    {
      Console.WriteLine("No entries found.");
    }

    Console.WriteLine("-----------------------------------------");
  }


  static void AddRecord()
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    ViewHabits();

    int userHabitChoice;

    while (true)
    {
      if (!TryReadPositiveInt("Choose Habit: ", out userHabitChoice))
      {
        Console.WriteLine("Please enter a valid ID.");
        continue;
      }
      command.Parameters.Clear();
      command.CommandText = """
        SELECT COUNT(*)
        FROM Habits
        WHERE Id = $id;
        """;

      command.Parameters.AddWithValue("$id", userHabitChoice);

      if (Convert.ToInt64(command.ExecuteScalar()) <= 0)
      {
        Console.WriteLine("No habit with that ID found.");
        continue;
      }
      break;
    }

    int quantity;

    while (true)
    {
      if (!TryReadPositiveInt("Enter quantity: ", out quantity))
      {
        Console.WriteLine("Please enter a valid quantity greater than zero.");
        continue;
      }

      break;
    }

    string? date;

    while (true)
    {

      date = ReadDateInput("Enter date (yyyy-mm-dd): ", allowTodayOption: true);
      if (!IsValidDate(date))
      {
        Console.WriteLine("Please enter a valid date in yyyy-MM-dd format.");
        continue;
      }
      break;
    }

    command.Parameters.Clear();
    command.CommandText = """
    INSERT INTO HabitEntries (HabitId, Quantity, OccurrenceDate)
    VALUES ($habitId, $quantity, $date);
    """;

    command.Parameters.AddWithValue("$quantity", quantity);
    command.Parameters.AddWithValue("$date", date);
    command.Parameters.AddWithValue("$habitId", userHabitChoice);

    int rowsAffected = command.ExecuteNonQuery();
    Console.WriteLine($"{rowsAffected} record(s) added successfully");
  }

  static void ViewHabits()
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    Console.WriteLine("AVAILABLE HABITS\n");

    command.CommandText = """
    SELECT *
    FROM Habits;
    """;

    using var reader = command.ExecuteReader();
    bool hasRecords = false;



    while (reader.Read())
    {
      hasRecords = true;
      int id = reader.GetInt32(0);
      string name = reader.GetString(1);
      string unit = reader.GetString(2);

      Console.WriteLine($"{id,-5} {name,-20} {unit}");
    }
    if (!hasRecords)
    {
      Console.WriteLine("No entries found.");
      return;
    }
  }

  static void UpdateRecord()
  {
    if (!TryReadPositiveInt("Which record ID do you want to update?", out int recordId))
    {
      Console.WriteLine("Please enter a valid ID.");
      return;
    }

    if (!RecordExists(recordId))
    {
      Console.WriteLine("Record not found.");
      return;
    }

    var (Quantity, OccurrenceDate) = GetRecord(recordId);
    int newQuantity = Quantity;
    string? newDate = OccurrenceDate;

    Console.WriteLine($"\nCurrent quantity: {newQuantity}");
    Console.WriteLine($"Current date: {newDate}");

    while (true)
    {
      Console.WriteLine("\nWhat would you like to update?");
      Console.WriteLine("1. Quantity");
      Console.WriteLine("2. Date");
      Console.WriteLine("3. Both");

      string? choice = Console.ReadLine();

      if (choice != "1" && choice != "2" && choice != "3")
      {
        Console.WriteLine("Invalid selection.");
        continue;
      }

      bool isValid = choice switch
      {
        "1" => UpdateQuantity(out newQuantity),
        "2" => UpdateDate(out newDate),
        "3" => UpdateQuantity(out newQuantity) && UpdateDate(out newDate),
        _ => false
      };

      if (!isValid)
      {
        continue;
      }
      break;
    }

    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    command.CommandText = """
    UPDATE HabitEntries
    SET Quantity = $quantity,
        OccurrenceDate = $date
    WHERE Id = $id;
    """;

    command.Parameters.AddWithValue("$quantity", newQuantity);
    command.Parameters.AddWithValue("$date", newDate);
    command.Parameters.AddWithValue("$id", recordId);

    int rowsAffected = command.ExecuteNonQuery();
    if (rowsAffected == 0)
    {
      Console.WriteLine("Record not found.");
    }
    else
    {
      Console.WriteLine("Record updated successfully!");
    }
  }

  static bool UpdateQuantity(out int newQuantity)
  {
    while (true)
    {

      if (!TryReadPositiveInt("What is the new quantity?", out newQuantity))
      {
        Console.WriteLine("Please enter a valid positive quantity.");
        continue;

      }
      return true;
    }
  }

  static bool UpdateDate(out string? newDate)
  {
    while (true)
    {

      Console.Write("What is the new occurrence date (yyyy-MM-dd)? ");
      newDate = Console.ReadLine();

      if (!IsValidDate(newDate))
      {
        Console.WriteLine("Please enter a valid date in yyyy-MM-dd format.");
        continue;
      }

      return true;
    }
  }

  static void DeleteRecord()
  {
    int recordId;

    while (true)
    {
      if (!TryReadPositiveInt("Enter the record you wish to delete:", out recordId))
      {
        Console.WriteLine("Please enter a valid ID.");
        continue;
      }

      if (!RecordExists(recordId))
      {
        Console.WriteLine("Record not found.");
        continue;
      }
      break;
    }

    while (true)
    {
      Console.Write($"Are you sure you want to delete record {recordId}? (y/n): ");
      string? confirmation = Console.ReadLine();

      if (string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase))
      {
        break;
      }

      if (string.Equals(confirmation, "n", StringComparison.OrdinalIgnoreCase))
      {
        Console.WriteLine("Deletion cancelled.");
        return;
      }

      if (confirmation is null)
      {
        Console.WriteLine("Input ended. Deletion cancelled.");
        return;
      }

      Console.WriteLine("Invalid response. Please enter y or n.");
    }

    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    command.CommandText = """
    DELETE FROM HabitEntries
    WHERE Id = $id;
    """;

    command.Parameters.AddWithValue("$id", recordId);

    if (command.ExecuteNonQuery() > 0)
    {
      Console.WriteLine("Record successfully deleted.");
    }

  }
  static void SeedDatabase()
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();

    List<int> habitIds = [];

    command.CommandText = "SELECT Id FROM Habits;";

    using (var reader = command.ExecuteReader())
    {
      while (reader.Read())
      {
        habitIds.Add(reader.GetInt32(0));
      }
    }

    command.CommandText = """
    INSERT INTO HabitEntries (HabitId, Quantity, OccurrenceDate)
    VALUES ($habitId ,$quantity, $date);
    """;

    var habitIdParameter = command.Parameters.Add("$habitId", SqliteType.Integer);
    var quantityParameter = command.Parameters.Add("$quantity", SqliteType.Integer);
    var dateParameter = command.Parameters.Add("$date", SqliteType.Text);

    Random random = new();

    if (habitIds.Count == 0)
    {
      Console.WriteLine("No habits found.");
      return;
    }

    for (int i = 0; i < 100; i++)
    {
      int quantity = random.Next(1, 51);

      int daysAgo = random.Next(0, 31);
      string date = DateTime.Today.AddDays(-daysAgo).ToString("yyyy-MM-dd");

      int randomId = habitIds[random.Next(habitIds.Count)];

      quantityParameter.Value = quantity;
      dateParameter.Value = date;
      habitIdParameter.Value = randomId;

      command.ExecuteNonQuery();
    }
    Console.WriteLine("100 sample records inserted successfully!");
  }

  static void SeedHabits()
  {
    using var connection = OpenConnection();
    using var command = connection.CreateCommand();
    string[] habitNames = ["Drinking Water", "Reading", "Exercise"];
    string[] habitUnits = ["Litres", "Pages", "Repetitions"];

    var nameParameter = command.Parameters.Add("$name", SqliteType.Text);
    var unitParameter = command.Parameters.Add("$unit", SqliteType.Text);

    command.CommandText = """
    INSERT INTO Habits(Name, Unit)
    VALUES($name, $unit);
    """;

    for (int i = 0; i < habitNames.Length; i++)
    {
      nameParameter.Value = habitNames[i];
      unitParameter.Value = habitUnits[i];

      command.ExecuteNonQuery();
    }


  }

}
