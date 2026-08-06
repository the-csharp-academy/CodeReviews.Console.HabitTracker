using System;
using System.Collections.Generic;
using System.Text;

namespace Habit.Tracker
{
    internal class HabitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Occurrence Occurrence { get; set; }
        
    }
    public class Occurrence
    {
        public DateTime Date { get; set; }
        public int Number { get; set; }
    }

}
