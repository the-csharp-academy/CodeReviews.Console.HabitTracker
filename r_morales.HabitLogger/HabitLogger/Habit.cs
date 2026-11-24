using System;
namespace HabitLogger.Habit
{
    public class Habit
    {
        public string habitName { get; private set; }
        public int quantityPerDay { get; private set; }
        public string dateTime { get; private set; }
        public Habit(string habitName, int quantityPerDay, string dateTime)
        {
            this.habitName = habitName;
            this.quantityPerDay = quantityPerDay;
            this.dateTime = dateTime;
        }
    }
}

