# HabitLogger

HabitLogger is a small command-line habit tracker built with C# and SQLite. It lets you create habits, record progress against them, and review or edit the records from a text-based menu.

## Requirements

- Windows
- .NET 10 SDK

To check that the SDK is installed, open PowerShell and run:

```powershell
dotnet --version
```

The reported version should be `10.x`.

## Setup and run

1. Open a terminal in the project folder—the folder containing `HabitLogger.csproj`. In VS Code, you can open the project folder and choose **Terminal > New Terminal**. No particular installation path is required.

2. Restore the NuGet dependencies:

   ```powershell
   dotnet restore
   ```

3. Build and start the application:

   ```powershell
   dotnet run
   ```

The project references `Microsoft.Data.Sqlite`; NuGet restores it during setup. On first run, the app creates `HabitTracker.db` in the current working directory. It adds three example habits and, when there are no entries, 100 sample records. Run the command from the project folder if you want the database file there.

## What the app does

Use the numbered menu to:

1. View available habits.
2. View all recorded entries.
3. Add a habit with a name and unit.
4. Add an entry by selecting a habit and providing a positive quantity and date.
5. Update an entry's quantity, date, or both.
6. Delete an entry after confirming with `y` or `n`.
7. Exit.

Dates are entered in `yyyy-MM-dd` format. When adding an entry, you can choose to use today's date.

## How it works

At startup, the application creates the `Habits` and `HabitEntries` tables if they do not already exist. It seeds example data only when the corresponding tables are empty. The menu then dispatches the selected operation to a method in `Program.cs`.

The `Habits` table stores a habit's name and unit. `HabitEntries` stores a habit ID, quantity, and occurrence date; its foreign key links each entry to a habit. SQLite foreign-key enforcement is enabled in the connection string.

Database operations use parameterized SQL. Input helpers validate positive integer values and dates before they are saved. Connections, commands, and readers are disposed with `using`. Database, file-access, and permission errors are reported to the user; initialization errors stop startup, while errors during a menu operation return control to the menu.

## Architectural choices

This is a small, single-project console application, so the implementation keeps the menu, input handling, and data access together in `Program.cs` instead of introducing a multi-layer architecture. This makes the flow easy to follow for a small exercise, but it also means that UI code and database logic are coupled. If the project grows, separating those responsibilities and adding automated tests would make it easier to maintain.

SQLite was chosen to keep persistence local and avoid requiring a separate database server. SQL is written directly with `Microsoft.Data.Sqlite`, which keeps the data operations explicit and avoids the extra setup of an ORM for this scope.

## Development reflection

Working on this project made the importance of handling ordinary user mistakes as well as database failures clear. Validating entries before saving and asking again after an invalid delete confirmation make the command-line flow safer. I also found that direct SQL is straightforward for a small app, though keeping menu behavior and database operations in one class would become harder to manage as features are added. For a next step, I would separate the storage logic and add focused tests around validation and data operations.
