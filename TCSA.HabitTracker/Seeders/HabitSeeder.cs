using TCSA.HabitTracker.DataAccessObjects;
using TCSA.HabitTracker.Models;

namespace TCSA.HabitTracker.Seeders
{
    internal class HabitSeeder : BaseSeeder<Habit>
    {

        public HabitSeeder(HabitDao habitDao) : base(habitDao) { }

        protected override List<Habit> GenerateData()
        {
            return new List<Habit>
            {
                new Habit() { Unit = "Study Hours", Description = "Time spent studying." },
                new Habit() { Unit = "Daily Run (Km)", Description = "Length of my daily runs." },
                new Habit() { Unit = "Walk Steps", Description = "Amount of steps I took" },
                new Habit() { Unit = "Offscreen Time (Minutes)", Description = "Time away from screens" }
            };
        }
    }
}
