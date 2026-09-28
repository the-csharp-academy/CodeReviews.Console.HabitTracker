namespace HabitTracker.Model;

public record Habit(int Id, string Name, int Quantity, DateTime OccurrenceAt);