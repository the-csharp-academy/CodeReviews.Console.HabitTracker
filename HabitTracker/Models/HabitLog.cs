namespace HabitTracker.Models;

public class HabitLog
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public int Occurrences { get; set; }
    public DateOnly OccurrenceDate { get; set; }
}