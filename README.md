# 🏋️ Habit Logger

  A console-based CRUD application built in **C# (.NET 10)** using **pure ADO.NET** and **SQLite**. Designed with a focus on **Object-Oriented Programming (OOP)** principles, clean architecture, and defensive programming.

  ## 📌 Features
  - **Full CRUD Functionality:**
    - **Create:** Log new habits with name, date, and quantity.
    - **Read:** Display all logged habits in a formatted console table.
    - **Update:** Modify existing habit entries by ID.
    - **Delete:** Remove habits by ID with verification.
  - **Robust Input Validation:**
    - Strict date parsing using `DateOnly.TryParseExact` (`dd-MM-yyyy`).
    - Convenient `"today"` shortcut for quick logging.
    - Safe numeric parsing (`int.TryParse`) preventing application crashes.
  - **Visual Polish:**
    - Custom border alignment formatting for clean console rendering.
    - Color-coded user feedback (success/error alerts).

  ## 🔒 Security & Best Practices

  • SQL Injection Prevention: 100% parameterized SQL queries (@HabitName, @Date, @Quantity) using SqliteCommand.
  Parameters.
  • Resource Management: Deterministic disposal of unmanaged database connections, commands, and data readers using
  using var statements (IDisposable).
  • Clean Code (DRY): Reusable validation helper methods (GetValidString, GetValidDate, GetValidNumber) to eliminate
  code duplication.

  ## 🛠️ Technologies Used

  • Language: C# 13 / .NET 10
  • Database: SQLite
  • Data Access: ADO.NET (Microsoft.Data.Sqlite)

  ## 🚀 Getting Started

  ### Prerequisites

  • .NET 10 SDK https://dotnet.microsoft.com/download installed.

  ### Running the Application

  1. Clone the repository:
    git clone https://github.com/tetrisy/Habit-Logger

  2. Navigate to the project directory:
    cd Habit-Logger/Habit-Logger

  3. Run the application:
    dotnet run


  (The SQLite database file habits.db and the required table will be created automatically on startup).

  ## 💡 Key Takeaways

  Building this project with low-level ADO.NET provided a deep understanding of database connection lifecycles,
  connection pooling, SQL execution plans, and data streaming.