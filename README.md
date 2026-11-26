# HabitTracker
This is a console based project for CRUD operations for managing Habits and Habit Logs. User can Create, Read, Update and Delete each entity.

# Requirements
- [x] Users need to be able to input the date of the occurrence of the habit
- [x] The application should store and retrieve data from a real database
- [x] When the application starts, it should create a sqlite database, if one isn’t present.
- [x] It should also create a table in the database, where the habit will be logged.
- [x] The users should be able to insert, delete, update and view their logged habit.
- [x] You should handle all possible errors so that the application never crashes.
- [x] You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.
- [x] Follow the DRY Principle, and avoid code repetition.
- [x] Include a README.
- [ ] Wrute unit tests.
- [x] Use parameterized queries.
- [x] Seed Data into the database automatically.

# How to run
1. Clone the project
2. Navigate to directory
3. `dotnet restore`
4. `dotnet run`

# Features
User can choose the entity to operate.

<img width="239" height="103" alt="image" src="https://github.com/user-attachments/assets/ce8227d2-2342-4484-87ef-903c3db1533a" />

## Habit
User can choose operation.

<img width="182" height="143" alt="image" src="https://github.com/user-attachments/assets/a44195cf-ac9e-4787-aa7a-4ec8a7ae500f" />

### Create
<img width="359" height="53" alt="image" src="https://github.com/user-attachments/assets/0351e177-ef9e-42c4-88cd-f3cb53920834" />

### Read
<img width="367" height="351" alt="image" src="https://github.com/user-attachments/assets/a77ec699-31cf-4ce7-8f8e-dc28e330ac30" />

### Update
<img width="364" height="75" alt="image" src="https://github.com/user-attachments/assets/0f8f2c1c-4d01-4f84-8744-e787b0e4d63f" />

### Delete
<img width="278" height="30" alt="image" src="https://github.com/user-attachments/assets/9599600c-fb37-4f10-8798-056b7cdd1d62" />

### Exit
<img width="211" height="88" alt="image" src="https://github.com/user-attachments/assets/9f349c09-de2c-44d8-a9a0-e511c54350bd" />

Same operations are applicable to HabitLog entity.

# Architecture
Application is divided into sections. There is Database layer with methods for creating database, tables and seeding data with Bogus library. Next one is Models layer with base Habit and HabitLog models. There is also a Reposiitory layer with base class containing generic CRUD operations methods and child classes implementing CRID for Habit and HabitLog entities. Last one is UI layer with for user interactions. Used Spectre.Console libary there.

# Reflection
The task was not hard to impolement but was a great excercise to close some knowledge gaps. Learned new thechnologies such as ADO.NET, working with Sqlite in .NET applications and KISS principle. Starting implementation with writing code in one file and refactor it later helped to gain better understanding WHY and WHEN kiss or dry principles are applicable and not just code like this because it's 'best practice' or 'convention'.






