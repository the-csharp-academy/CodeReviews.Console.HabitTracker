using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitLogger.Entities
{
    internal class HabitLogEntry
    {
        public int Id { get; private set; }
        public Habit Habit { get; private set; }
        public int Quantity { get; private set; }
        public DateTime TimeEntry { get; private set; } = DateTime.Now;

        public HabitLogEntry(int id, Habit habit, int quantity, DateTime timeEntry)
        {
            Id = id;
            Habit = habit;
            Quantity = quantity;
            TimeEntry = timeEntry;
        }

        public void UpdateHabit(Habit habit)
        {
            Habit = habit;
        }
        public void UpdateQuantity(string newQuantity)
        {
            Quantity = string.IsNullOrWhiteSpace(newQuantity) ? Quantity : int.Parse(newQuantity);
        }
        public void UpdateTimeStamp(DateTime timestamp)
        {          
            TimeEntry = timestamp == DateTime.MinValue ? TimeEntry : timestamp;
        }
    }
}
