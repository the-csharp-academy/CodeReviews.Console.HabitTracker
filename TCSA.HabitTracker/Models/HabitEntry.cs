namespace TCSA.HabitTracker.Models
{
    internal class HabitEntry
    {
        public int Id { get; init; }
        public int HabitId { get; init; }
        public string HabitUnit { get; init; } = string.Empty;
        public string Date { get; init; } = string.Empty;
        public double Value { get; init; }
    }
}
