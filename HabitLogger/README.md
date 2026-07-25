# Habit Logger
### C# Console Application | SQLite | ADO.NET

A console-based CRUD application to track daily habits, built as part of the C# Academy learning path. This was my first project involving a real database connection.

---

## Requirements

| Feature | Status |
|---------|--------|
| Log occurrences of a habit | ✅ |
| Habit tracked by quantity, not time | ✅ |
| Users can input the date of the occurrence | ✅ |
| Store and retrieve data from a real database | ✅ |
| Create a SQLite database on startup if not present | ✅ |
| Create a table in the database where habits are logged | ✅ |
| Insert, delete, update and view logged habits | ✅ |
| Handle all possible errors so the application never crashes | ✅ |
| Only interact with the database using ADO.NET (no Entity Framework or Dapper) | ✅ |
| Follow the DRY Principle and avoid code repetition | ✅ |
| Parameterized queries for security | ✅ |
| Let users create their own habits to track | ✅ |
| Seed data into the database automatically on first run | ✅ |
| Unit tests for validation methods | ❌ |

---

## How It Works

On startup the application creates the database and table if they don't exist yet, then reads the table structure using `PRAGMA table_info` to reload any custom habits added in previous sessions.

If the table is empty, `FillTable` automatically seeds 50 randomly generated entries.

From the main menu the user can:

1. View all entries
2. Add a new entry
3. Delete an entry
4. Update an entry
5. Add a new habit to track

---

## Challenges

**Dynamic SQL** was the biggest challenge. Since users can add custom habits that become new database columns, every INSERT and UPDATE query needs to be built at runtime. Column names can't use SQL parameters, so they require Regex validation to prevent injection instead.

**SQLite and ADO.NET** were both new to me. I started by following the project guide, then repeated the project three additional times on my own to get comfortable with all the required commands.

**Date input** was tricky until I understood `DateTime.TryParseExact` properly. I decided to enforce a single clear input format rather than accepting multiple formats, which simplified the validation logic.

---

## Areas to Improve

**DRY Principle** — at my current level I feel the code is reasonably optimized, but I'm aware there is likely room to reduce repetition further without over-engineering into too many small methods.

**Unit tests** — I did not implement unit tests for this project. It's something I want to add in future projects, particularly for input validation methods.

---

## What I Learned

- Connecting to and interacting with SQLite using ADO.NET
- Parameterized queries and why they matter for security
- Building SQL queries dynamically at runtime
- Using `PRAGMA table_info` to read and restore table structure across sessions
- Separating input collection from data logic for cleaner, reusable code
- Applying the DRY principle in a real project

