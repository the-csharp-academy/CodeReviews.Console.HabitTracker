using Bogus;
using HabitTracker.Models;

namespace HabitTracker.Database.Fakers;

public sealed class HabitLogFaker : Faker<HabitLog>
{
    public HabitLogFaker(List<Habit> habits)
    {
        RuleFor(h => h.HabitId, f => f.PickRandom(habits).Id);
        RuleFor(h => h.Occurrences, f => f.Random.Int(1, 10));
        RuleFor(h => h.OccurrenceDate, f => f.Date.RecentDateOnly(10));
    }
}