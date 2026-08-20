# Habit Tracker

## How to run

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — check your version with:
  ```bash
  dotnet --version
  ```

### From the command line

Run these from the repository root:

```bash
dotnet run --project Habit.Tracker/Habit.Tracker/Habit.Tracker.csproj
```

### From Visual Studio

1. Open `Habit.Tracker/Habit.Tracker.slnx`.
2. Make sure `Habit.Tracker` is set as the startup project.
3. Press **F5** to build and run.

> On first launch the app tests the database connection and creates the `Habits` table if
> it doesn't already exist. A `sample.db` SQLite file is generated automatically in the
> working directory, so no manual database setup is required.

---

## How to use

When the app starts, it shows a menu:

```
Welcome to Habit Tracker!
Choose from the options below
C - Add a record of a habit
R - List All habits
U - Update a habits occurrences
D - Delete a habit
```

Type the letter for the action you want (case-insensitive) and press **Enter**:

| Option | Action  | What happens                                                                                     |
|:------:|---------|--------------------------------------------------------------------------------------------------|
| `C`    | Create  | Prompts for a habit name, the number of times completed, and a date (`dd/mm/yyyy`), then saves it. |
| `R`    | Read    | Lists every stored habit as `"<name> has been done <n> times"`.                                  |
| `U`    | Update  | Asks for a habit name, checks it exists, then updates its number of occurrences.                 |
| `D`    | Delete  | Asks for a habit name, checks it exists, then removes it.                                         |

After each action you're asked to press **Y** to continue or **N** to exit.

---

## How it works

The application is a single console project made up of a few focused files:

| File             | Responsibility                                                                              |
|------------------|---------------------------------------------------------------------------------------------|
| `Program.cs`     | Entry point. Runs the menu loop, reads user input, validates it, and calls the data layer.  |
| `DataAccess.cs`  | Encapsulates all SQLite work — connecting, creating the table, and running CRUD queries.     |
| `Habit.cs`       | The data models (`HabitModel` and `Occurrence`) that represent a habit in memory.           |
| `App.config`     | Holds the database connection string.                                                       |

**Flow of a request:**

1. `Program.cs` displays the menu and reads the user's choice.
2. Based on the choice, it gathers and validates any additional input (name, number, date).
3. It builds a `HabitModel` (where relevant) and calls the matching method on `DataAccess`.
4. `DataAccess` opens a SQLite connection, runs a parameterized query, and returns the result.
5. `Program.cs` prints the outcome and loops back to the menu.

### Database

The data is stored in a SQLite table created on startup:

| Column   | Type    | Notes                          |
|----------|---------|--------------------------------|
| `Id`     | INTEGER | Primary key, auto-increment.   |
| `Name`   | TEXT    | The habit's name.              |
| `Date`   | TEXT    | The date the habit was logged. |
| `Number` | INT     | How many times it was done.    |

The connection string (`Data Source=sample.db`) is defined in `App.config` under the name
`Sample` and read at runtime via `ConfigurationManager`.

---

## Architecture & design choices

- **Console + menu loop.** The app runs in a `while` loop that redraws the menu and processes
  one action per iteration, which keeps the interaction model simple and predictable.
- **Dedicated data-access class.** All database logic lives in `DataAccess`, separate from the
  UI code in `Program.cs`. This keeps SQL in one place and makes the query logic easier to find
  and change.
- **Model classes.** `HabitModel` and `Occurrence` give the data a clear shape when it moves
  between the database and the console, instead of passing loose values around.
- **`Microsoft.Data.Sqlite` with parameterized queries.** A lightweight, file-based database
  that needs no separate server, and parameterized commands guard against SQL injection.
- **Configuration via `App.config`.** The connection string is kept out of the code so it can be
  changed without recompiling.
- **.NET 10 with nullable reference types and implicit usings enabled** for modern, concise C#.

---

## Technologies

- C# / .NET 10
- [Microsoft.Data.Sqlite](https://learn.microsoft.com/dotnet/standard/data/sqlite/)
- System.Configuration.ConfigurationManager
- SQLite

---

## Reflection

My main learning from this project was:

* How the using statement works to manage resources and handle memory cleanup automatically.
* How to use parameterized queries to prevent SQL injection attacks.