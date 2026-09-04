# HabitLogger

A simple console-based habit tracking application built with c#, .NET and Spectre.Gonsole.

## Features

- The application creates and seeds the Habits table on startup if it does not already exist.
- The HabitLogEntry table is also created if it does not already exist.
- The User is greeted with a main menu displaying a table of the current days logged habits with options to manage either Habits or Habit log entries.

## Technologies

- C#
- .NET
- SQLite
- ADO.NET
- Spectre.Console


## Getting Started

### Prerequisites

- .NET 8 SDK

### Installation

Clone the repostitory:
```
bash git clone https://github.com/CyberNexus-code/HabitLogger.git
```

Navigate to the project
```bash
cd HabitLogger
```

Run the application:
```bash
dotnet run
```


## What I have learned

- That I still have alot to learn!
- Working with time is going to become challanging in the future, especially if you have clients in different timezones.
- Manual Dependancy injection.
- ADO.NET

## What I struggled with

- Still struggling to design and seperate responsibilities. (The HabitRepository is doing way too much).
- I Tried saving UTC time in the database and using local time in my 'UI' this proved challanging as you can easily forget to convert time at a certain point, causing problems.
