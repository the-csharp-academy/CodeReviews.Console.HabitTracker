using System;
using System.Collections.Generic;
using System.Text;

namespace Habit_Logger
{
    internal class Habit
    {
        public int Id { get; init; }
        public string HabitName { get; set; }
        public DateOnly Date { get; set; }
        public int Quantity { get; set; }

        public Habit(string habitName, DateOnly date, int quantity)
        {
            HabitName = habitName;
            Date = date;
            Quantity = quantity;
        }

        public Habit(int id, string habitName, DateOnly date, int quantity)
            : this(habitName, date, quantity)
        {
            Id = id;
        }
    }
}
