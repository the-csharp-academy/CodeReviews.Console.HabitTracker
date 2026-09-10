# Habit Logger
A console-based CRUD application for tracking daily habits, built with C#.
This was my first CRUD project in C#.

## Overview
Habit Logger lets you log habits with a name, date, and quantity, then view, update, or delete those entries — all through an interactive console interface.

## Requirements
- When the application starts, it creates a SQLite database if one doesn't already exist.
- It also creates a table to store the logged habits.
- Users can insert, delete, update, and view logged entries.
- All errors are handled gracefully so the application never crashes.
- The application only closes when the user chooses to exit.
- Database interaction is done exclusively through raw SQL — no ORMs (e.g. Entity Framework) are used.

## Features
- **SQLite database**: automatically created on first run, along with the `Logs` table. If the table is empty, it's seeded with 10 sample entries.
- **Console UI**: built with [Spectre.Console](https://spectreconsole.net/) for a cleaner, more interactive experience than plain console output.
- **Habit logging**: each entry stores a habit name, date, and quantity.

## Tech Stack
- C#
- SQLite (via `Microsoft.Data.Sqlite`)
- Spectre.Console

## Challenges
- **Integrating SQLite**: I had prior database experience, so writing the SQL scripts wasn't hard — but figuring out how to wire up SQLite within a C# app took some research.
- **Default values in Spectre.Console**: getting `AnsiConsole.Ask` (or `TextPrompt`) to default to today's date was trickier than expected.
- **Applying OOP**: this is my first back-end language, and although not my first programming language overall, structuring the code with OOP principles was a real challenge.

## Lessons Learned
- **Separation of concerns in OOP**: each layer of the app should only know what it needs to. For example, controllers validate that everything needed for an insert/update/delete is correct, while the repository layer is solely responsible for talking to the database.