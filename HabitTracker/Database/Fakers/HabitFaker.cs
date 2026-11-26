using Bogus;
using HabitTracker.Models;

namespace HabitTracker.Database.Fakers;

public sealed class HabitFaker : Faker<Habit>
{
    public HabitFaker()
    {
        RuleFor(h => h.Name, f => f.Lorem.Sentence(2));
        RuleFor(h => h.Unit, f => f.Lorem.Word());
    }
}