using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Habit_Logger
{
    internal class Menu
    {
        private readonly DatabaseManager dbManager;
        public Menu(DatabaseManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public void CallMenu()
        {
            int choice;

            while (true)
            {
                DisplayMenu();
                choice = GetValidNumber("Choose an option (1-5): ");
                if (choice == 0)
                {
                    return;
                }
                Console.Clear();
                ExecuteMenuOption(choice);
                Console.Clear();
            }
        }

        public void DisplayMenu()
        {
            Console.WriteLine("================== Habit Logger ==================");
            Console.WriteLine($"|{"|", 49}");
            Console.WriteLine($"{"|",-8}{"1. Add new habbit",-41}|");
            Console.WriteLine($"{"|",-8}{"2. View habits",-41}|");
            Console.WriteLine($"{"|",-8}{"3. Update habit",-41}|");
            Console.WriteLine($"{"|",-8}{"4. Delete habit",-41}|");
            Console.WriteLine($"{"|",-8}{"0. Exit",-41}|");
            Console.WriteLine($"|{"|",49}");
            Console.WriteLine("==================================================");
        }

        private void ExecuteMenuOption(int choice)
        {
            switch (choice)
            {
                case 1:
                    dbManager.InsertHabit(CreateHabit());
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The habit was saved sucesfully!");
                    Console.ResetColor();
                    Console.WriteLine("\nPress any key to go back to menu...");
                    Console.ReadLine();
                    break;
                case 2:
                    ShowHabitsMenu();
                    break;
                case 3:
                    UpdateHabit();
                    break;
                case 4:
                    DeleteHabit();
                    break;
                case 0:
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please choose between 0-4.");
                    Console.ResetColor();
                    break;
            }
        }

        private Habit CreateHabit()
        {
            string? habitName = GetValidString("Enter habit name: ");

            DateOnly date = GetValidDate("Enter date (dd-mm-yyyy). Enter today for today: ");

            int habitQuantity = GetValidNumber("Enter quantity: ");

            return new Habit(habitName, date, habitQuantity);
        }

        private void DisplayHabits()
        {
            List<Habit> habits = dbManager.GetHabits();

            if(habits.Count() == 0)
            {
                Console.WriteLine("There are no habits logged.");
                return;
            }

            Console.WriteLine("================== Habits List ===================");
            Console.WriteLine($"| {"ID", -3}{"|", -9} {"Habit", -13}{"|", -5}{"Date", -8}|  {"Qty.", -5}|");
            Console.WriteLine("--------------------------------------------------");
            foreach (Habit habit in habits)
            {
                Console.WriteLine($"| {habit.Id,-3}{"|",-1} {habit.HabitName,-17}{"|",5}{habit.Date,-12}|  {habit.Quantity,-5}|");
            }
            Console.WriteLine("==================================================");
        }

        private void ShowHabitsMenu()
        {
            DisplayHabits();
            Console.WriteLine("\nPress any key to go back to menu...");
            Console.ReadLine();
        }

        private void UpdateHabit()
        {
            DisplayHabits();

            int updateId = GetValidNumber("\nEnter ID of the habit you want to update: ");

            string? newHabitName = GetValidString("Enter new habit name: ");

            DateOnly newDate = GetValidDate("Enter new date (dd-mm-yyyy). Enter today for today: ");

            int newHabitQuantity = GetValidNumber("Enter new quantity: ");

            if (dbManager.UpdateHabit(updateId, newHabitName, newDate, newHabitQuantity))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Habit updated succesfully.");
                Console.ResetColor();
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Habit with that ID was not found.");
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private void DeleteHabit()
        {
            DisplayHabits();
            int deleteId = GetValidNumber("\nEnter ID of the habit you want to delete: ");

            bool deleted = dbManager.EraseHabit(deleteId) > 0;

            if (deleted)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("The habit was deleted succesfully.");
                Console.ResetColor();
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Habit with that ID was not found.");
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private string GetValidString(string prompt)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(text))
            {
                Console.Write("Enter a valid habit name: ");
                text = Console.ReadLine();
            }

            return text;
        }

        private DateOnly GetValidDate(string prompt)
        {
            Console.Write(prompt);
            string? dateInput = Console.ReadLine();
            DateOnly checkedDate;
            if (dateInput == "today")
            {
                checkedDate = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                while (!DateOnly.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkedDate))
                {
                    Console.Write("Invalid input. Pleae enter a valid date: ");
                    dateInput = Console.ReadLine();
                }
            }

            return checkedDate;
        }

        private int GetValidNumber(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            int number;
            while (!int.TryParse(input, out number))
            {
                Console.Write("Invalid input. Pleae enter a valid number: ");
                input = Console.ReadLine();
            }

            return number;
        }
    }
}
