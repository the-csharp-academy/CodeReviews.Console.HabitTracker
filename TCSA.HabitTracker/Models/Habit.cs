namespace TCSA.HabitTracker.Models
{
    internal class Habit
    {
        public int Id { get; init; }
        public string Unit { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }
}
