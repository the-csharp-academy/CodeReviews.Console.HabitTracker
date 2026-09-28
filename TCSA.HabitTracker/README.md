# Console Habit Logger
This project is part of the curricullum of [The C# Academy](https://thecsharpacademy.com/).
It consists of a CRUD based application in which the user is able to store habit entries. Developed using C#, SQLite and Visual Studio

## External Dependencies
* [ADO.NET SQLite Provider](https://learn.microsoft.com/pt-br/dotnet/standard/data/sqlite/?tabs=net-cli) for Database manipulation.
* [Spectre Console](https://spectreconsole.net/) to provide a clean User Interface.

---

# Given Requirements
- [x] This is an application where you’ll log occurrences of a habit.
- [x] This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)
- [x] Users need to be able to input the date of the occurrence of the habit
- [x] The application should store and retrieve data from a real database
- [x] When the application starts, it should create a sqlite database, if one isn’t present.
- [x] It should also create a table in the database, where the habit will be logged.
- [x] The users should be able to insert, delete, update and view their logged habit.
- [x] You should handle all possible errors so that the application never crashes.
- [x] You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.
- [x] Follow the [DRY Principle](https://thecsharpacademy.com/article/30006/dry-principle-csharp), and avoid code repetition.
- [x] Your project needs to contain a Read Me file where you'll explain how your app works and tell a little bit about your thought progress.

# Given Challenges
- [ ] If you already have a bit of experience with programming, we highly recommend you get into the habit of **writing unit tests** for a few methods in your project. Any method that outputs data and doesn't talk to a database (those are tested in **integration tests**) can be unit tested. A good example is any method that deals with validation. [Here's a quick tutorial.](https://thecsharpacademy.com/article/30009/unit-testing-for-beginners)

- [x] If you haven't, [try using parameterized queries](https://reintech.io/blog/mastering-parameterized-queries-ado-net) to make your application more secure.

- [x] Let the users create their own habits to track. That will require that you let them choose the unit of measurement of each habit. Hot tip: **You should not create a table for each habit**.

- [x] Seed Data into the database automatically when the database gets created for the first time, generating a few habits and inserting a hundred records with randomly generated values. This is specially helpful during development so you don't have to reinsert data every time you create the database.

---

# Features
* **SQLite database**
	- All data is stored inside a SQLite database file
	- The database file is generated when the app is run for the first time
	- If the database tables are empty, the app populates it with default data. More detail in Seeding ==implementation below==.
* **Custom Habit Units**
	- The user can **create custom Habits** with custom Units. Allowing each Habit Entry (log) to contain specific units for the habits the user performed at a given date
	- **The units are arbitrary**. Instead of a set of units the app gives the user to choose from, the user can make his own units. E.g. Steps walked, Hours Slept, Books Read, Musics Listened (minutes), Musics Listened (hours), etc.
	- For more information regarding the database Habit to Habit Entry (habit logs) structure ==see below==.
* **Basic CRUD operatoins**
	- The user can perform basic Create, Remove, Update and Delete operations for each table in the database
* **Command Line UI**
	- Colorful text to emphasise different contexts
	- Table presentation
	- Arrow keys selection prompt of options
	- Default selection and confirmation prompts

---

# Database Structure
The SQLite database is created with the following commands:
```
CREATE TABLE IF NOT EXISTS Habit (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	HabitUnit TEXT NOT NULL UNIQUE,
	HabitDescription TEXT
);
```
Habit table stores information related to a Habit that the user will create and use to create Entries. It has a one to many relationship with the HabitEntry table
* **HabitUnit**: Is the actual unit that will be displayed when the user choose to see all Entries
* **HabitDescription**: It is an optional habit description in which the user may inform a detailed description of what an habit is


```
CREATE TABLE IF NOT EXISTS HabitEntry (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	HabitId INTEGER NOT NULL,
	Date TEXT NOT NULL,
	Value REAL NOT NULL,
	FOREIGN KEY (HabitId) REFERENCES Habit(Id) 
	ON DELETE CASCADE
);
```
Habit Entry table stores the actual occurance of an habit. It depends on the Habit table to define which habit the user performed that will be logged in the HabitEntry
* **HabitId**: Foreign key to join Habit and HabitEntry together to display readable information regarding a habit entry
* **Value**: Number counting the amount of units the user performed in the given Log.
* **Date**: The date of the given log, stored in a specific format: dd-MM-yyyy
* **ON DELETE CASCADE**: If a Habit is deleted, every occurance of this habit stored in HabitEntry will also be deleted.

---

# Challenges faced during development
- Separating User Interface, data access and services
- Remembering certain SQL commands and dealing with wrong Table members at creation.
- Avoiding repetition of code while performing SQLite connections and operationa. This was and stil is a big
challenge for me since I didn't really grasp how to not do it repeatedly in every single method form the data access objects. 
I preffered to leave as it is since it made connections contexts (garbage colllector wise) easy to spot and understand.
- Abstractions. This was my second project where I implement inheritance and different interfaces for the same object.
- Again, abstractions, but for generic types. Still trying to wrap my head around them.
- Creating the seeding objects that derive from a single base class where the main method does the work.
- Solving for the time it took to seed the database. 


# Lessons Learned
- Refresher into how to perform relational database creation and using the JOIN command to perform a complete query.
- Setting up Interfaces with specific methods in mind, making it safe to use certain object members for specific functionalities.
- Separating the data acces layer from the user interaction layer. Even though this was not a requirement, I did it anyways. I thought it would be a good idea to begin the project like this
to avoid future headaches and spaghetti code. This way every database operation remains in atomic methods from Data Access Objects
- Designing Data Access Objects. Complementing the previous point, I was looking for ways to make database access more modular and containerized and came across DAOs.
My design is pretty simple just to get things done, but I'm quite happy with it.
- The use of database Transactions to make a sequence of commands in a single connections with the possibility of rollback. This solves the long duration time for seeding the database since I needed 100 random Insertions.
- Making good commit messages to provide better context for future me and other people that stumble across my code.
- INumber interface feature to deal with numbers generally in generic types.
 
# Areas to Improve
- File organization. Maybe break down the project solution into the main project and a class library.
- User interface and business rules seggregation. It is not perfect yet, there are many instances where the UI mixes up with processing the logic behind the scenes.
- Break down DAOs. I chose not to do this to not consume a large amount of time rearranging code to provide the same functinality, but for future projects, instead of having every single CRUD operation in one object, I could make one DAO have inserter, updater, selecter objects or something like that
- Making unit tests. I chose not to follow throught with this challenge mainly because I got confused with how to implement it with spectre console user interface for input, output and validation values. And honestly, I'm also eager to jump to the next project. 
# Resources used
- [ADO.NET SQLite provider docs](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli)
- [Spectre Console docs](https://spectreconsole.net/)
- [W3Schools for SQL commands](https://www.w3schools.com/sql/sql_syntax.asp)
- [Stack Overflow to deal with specific problems like parsing DateTime](https://stackoverflow.com/questions)
- [How to make good commit messages article](https://cbea.ms/git-commit/)