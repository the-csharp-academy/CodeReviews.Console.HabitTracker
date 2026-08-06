using Habit.Tracker;
using System.Configuration;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

var db = new DataAccess("Sample");

db.TestConnection();

db.CreateTable();

bool userIsActive = true;

while(userIsActive)
{
    Console.WriteLine("Welcome to Habit Tracker!");
    Console.WriteLine("Choose from the options below");
    Console.WriteLine("C - Add a record of a habit");
    Console.WriteLine("R - List All habits");
    Console.WriteLine("U - Update a habits occurrences");
    Console.WriteLine("D - Delete a habit");

    string userInput = Console.ReadLine();

    switch (userInput.ToUpper())
    {
        case "C":
            Console.WriteLine("Enter the habit name");
            string habitName = Console.ReadLine();
            Console.WriteLine("Enter how many times you have completed the habit");
            string habitOccurrence = Console.ReadLine();

            while (!habitOccurrence.All(char.IsAsciiDigit))
            {
                Console.WriteLine("Please only insert a number");
                habitOccurrence = Console.ReadLine();
            }

            int habitOccurrenceNum;
            Int32.TryParse(habitOccurrence, out habitOccurrenceNum);

            DateTime habitOccurrenceDate;

            while (true)
            {
                Console.WriteLine("Enter a date (dd/mm/yyyy): ");
                string input = Console.ReadLine();

                if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out habitOccurrenceDate))
                {
                    Console.WriteLine($"Valid date: {habitOccurrenceDate:dd MMMM yyyy}");
                    break;
                }

                Console.WriteLine("Invalid date. Please try again.");
            }

            HabitModel newHabit = new HabitModel
            {
                Name = habitName,
                Occurrence = new Occurrence
                {
                    Date = habitOccurrenceDate,
                    Number = habitOccurrenceNum
                }
            };

            db.InsertHabit(newHabit);

            break;
        case "R":
            var habits = db.ListAllHabits();

            foreach (HabitModel habit in habits)
            {
                Console.WriteLine($"{habit.Name} has been done {habit.Occurrence.Number} times");
            }
            break;
        case "U":
            // Update occurrence
            Console.WriteLine("Enter the habit name you want to update");
            string name = Console.ReadLine();
            Console.WriteLine("Enter the the number of times you have now completed the habit");
            string newOccurence = Console.ReadLine();

            while (!newOccurence.All(char.IsAsciiDigit))
            {
                Console.WriteLine("Please only insert a number");
                newOccurence = Console.ReadLine();
            }

            int newOccurenceOut;
            Int32.TryParse(newOccurence, out newOccurenceOut);

            db.UpdateHabitOccurrence(name, newOccurenceOut);

            break;
        case "D":
            // Delete habit
            Console.WriteLine("Which habit would you like to delete?");
            string habitToDelete = Console.ReadLine();
            db.DeleteHabit(habitToDelete);
            break;
        default:
            //
            break;
    }

    Console.WriteLine("press Y to continue using the habit tracker or N to exit");
    string checkUserIsActive = Console.ReadLine().ToUpper();
    if (checkUserIsActive == "Y")
    {
        userIsActive = true;
    } else
    {
        System.Environment.Exit(0);
    }
}
