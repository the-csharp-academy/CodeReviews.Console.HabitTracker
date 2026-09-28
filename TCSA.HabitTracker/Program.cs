using TCSA.HabitTracker.DataAccessObjects;
using TCSA.HabitTracker.Seeders;
using TCSA.HabitTracker.Services;

namespace TCSA.HabitTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=habit-Tracker.db";

            var databaseInitializer = new DatabaseInitializer(connectionString);
            databaseInitializer.Initialize();

            var entryDao = new HabitEntryDao(connectionString);
            var habitDao = new HabitDao(connectionString);

            var habitEntryService = new HabitEntryService(entryDao, habitDao);
            var habitService = new HabitService(habitDao);

            var habitSeeder = new HabitSeeder(habitDao);
            habitSeeder.Seed();
            var entrySeeder = new EntrySeeder(entryDao, habitDao);
            entrySeeder.Seed();

            UserInterface userInterface= new UserInterface(habitEntryService, habitService);
            userInterface.MainMenu();
        }
    }
}
