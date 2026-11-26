using HabitTracker;
using HabitTracker.Database;

DatabaseInitialization dbInit = new DatabaseInitialization();
dbInit.CreateDatabase();

UserInterface userInterface = new UserInterface();
userInterface.MainMenu();
