# HabitLogger
My third C# application, but the first to use ADO.Net for CRUD with SQLite.

Purpose: A console based CRUD application to track habbit.
Developed using C# and SQLite.

# Given Requirements:
✅ This is an application where you’ll log occurrences of a habit.

✅ This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)

✅ Users need to be able to input the date of the occurrence of the habit

✅ The application should store and retrieve data from a real database

✅ When the application starts, it should create a sqlite database, if one isn’t present.

✅ It should also create a table in the database, where the habit will be logged.

✅ The users should be able to insert, delete, update and view their logged habit.

✅ You should handle all possible errors so that the application never crashes.

✅ You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.

✅ Follow the DRY Principle, and avoid code repetition.

✅ Contain a README.md

# Features
### SQLite database connection and seeded data
	- The program uses a local SQLite db to store and read information. (SQLite database has to be installed in the system)
	- If no database exists, or the correct table does not exist they will be created on program start alongside seeded data.

### CRUD DB functions for tracking your habit
	- From the main menu users can Create, Read, Update or Delete entries for whichever record of habit, entered date in mm-DD-yy format.
	- Have the options to choose the measurement unit of choice (times, glasses, pages, or custom unit).
	- Can exist the CRUD operation at any time simply by pressing 0.

# Challenge accomplished:
✅ Create some unit tests for validation method.

✅ Using parameterized queries for database queries.

✅ Have the user created their own habit to track. (Only create a new record of habit, not creating a new table)

✅ Included seeded data when the table being created for the first time.

# Hard things encountered:
- First time doing CRUD with database using SQLite and ADO.Net took me quite some time, because I never work with these before (I only learned and used Entity Framework to work with, but I still hadn't fully understand).
- Also, understanding some parts of the project's task was hard for me (especially the requirments for some challenges).
- Encountered some bugs at the time setting up the database and doing some connection tests. (I remembered it was the NuGet packages outdated, so I tried and fixed that).  
- Applying some of the principles like DRY, KISS also slowed down my progess (I have to be carefull and think deeply before writing anything at all).
- The last one is overhaul my codebase to allow record of any habit, instead of just for one habit. This resulted in changing the entire database schema, adjusting the logic of CRUD, and most importantly, adding a new option to choose measurement unit (which is also a part of the challenges).

# Things I learned:
- Don't rush things, take time to understand and come up with a solution (if it's wrong, try different approach).
- The principles are very important :)), try not to underestimate them.
- There will be a solution for the thing I want to tackle, I just have to spend the time to find it.
- Use the AI to guide is not something to be ashamed at (but do please ignore it's code provided to you, it's always better to write your own code).








