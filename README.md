This is a simple application built with C# and SQLite, made to help users track daily habits. It allows for logging daily quantities for a predefined habit, update previous entries, delete records, and view your progress.
Based on: https://www.thecsharpacademy.com/project/12/habit-logger

**Features**
 - Create Table: Automatically creates a habit_tracker table in a SQLite
   database upon first run.
 - Insert Record: Add new entries for your habit, including the date and
   quantity.
 - View All Records: Display all logged habit entries.
 - Update Record: Modify a specfic habit entries using the entry ID.
 - Delete Record: Remove a specific habit entries using the entry ID.
 - Basic Input Validation: Ensures date and quantity inputs are in the
   correct format.

## How to Use

**Requirements**
.NET SDK

If needed, you can install the Microsoft.Data.Sqlite NuGet package if it's not already referenced. You can add it by doing:
    dotnet add package Microsoft.Data.Sqlite

Running the Application:
 - Open your terminal or command prompt and navigate to the proper directory
   containing the program.cs file.

Run the application using:

  dotnet run
  
Then follow the on-screen instructions to interact with the habit tracker. The database file will be created in the directory where you run the application.


##  Resources Used

 - C#: The primary programming language.
 - SQLite: A lightweight, file-based database for data storage.
 - Microsoft.Data.Sqlite: The ADO.NET provider for SQLite.
 - The C# Academy Tutorial Video

##  Reflection

This was my first project with SQL and this kind of setup, so it was interesting to learn. I did try my best to not go "holding hands" with the tutorial but did look back to ensure I was doing it correctly. I  want to practice more with databases and also refactoring my code to make it more clean and optimized so I may do this again later. I also learned about the CRUD acronym.
