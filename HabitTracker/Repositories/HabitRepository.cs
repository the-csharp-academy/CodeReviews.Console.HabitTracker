using HabitTracker.Database;
using HabitTracker.Models;

namespace HabitTracker.Repositories;

public class HabitRepository : RepositoryBase
{
    public void Create(Habit habit)
    {
        string insertHabitSql = """
                                INSERT INTO Habits (Name, Unit)
                                VALUES (@Name, @Unit);
                                """;
        
        (string Name, object Value)[] parameters =
        [
            ("@Name", habit.Name),
            ("@Unit", habit.Unit),
        ];

        ExecuteCommand(insertHabitSql, parameters);
    }

    public void BulkCreate(List<Habit> habits)
    {
        string insertHabitSql = """
                                INSERT INTO Habits (Name, Unit)
                                VALUES (@Name, @Unit);
                                """;

        var parameterSets = habits.Select(habit =>
            new (string Name, object Value)[]
            {
                ("@Name", habit.Name),
                ("@Unit", habit.Unit)
            }).ToArray();

        ExecuteBulkCommand(insertHabitSql, parameterSets);
    }

    public List<Habit> Read()
    {
        string selectHabitsSql = """
                                 SELECT * FROM Habits;
                                 """;

        var habits = ExecuteQuery<Habit>(selectHabitsSql, reader => new Habit
        {
            Id = Convert.ToInt32(reader.GetString(0)),
            Name = reader.GetString(1),
            Unit = reader.GetString(2)
        });

        return habits;
    }

    public Habit? Read(int id)
    {
        string selectHabitsSql = """
                                 SELECT * FROM Habits
                                 WHERE Id = @Id;
                                 """;
        
        (string Name, object Value)[] parameters =
        [
            ("@Id", id),
        ];

        var habit = ExecuteQueryReturnSingle<Habit>(selectHabitsSql, reader => new Habit
        {
            Id = Convert.ToInt32(reader.GetString(0)),
            Name = reader.GetString(1),
            Unit = reader.GetString(2)
        }, parameters);

        return habit;
    }

    public void Update(Habit habit)
    {
        string updateHabitSql = """
                                UPDATE Habits
                                SET Name = @Name, Unit = @Unit
                                WHERE Id = @Id;
                                """;

        (string Name, object Value)[] parameters =
        [
            ("@Id", habit.Id),
            ("@Name", habit.Name),
            ("@Unit", habit.Unit),
        ];

        ExecuteCommand(updateHabitSql, parameters);
    }

    public void Delete(int id)
    {
        string deleteHabitSql = """
                                DELETE FROM Habits
                                WHERE Id = @Id;
                                """;
        
        (string Name, object Value)[] parameters =
        [
            ("@Id", id),
        ];
        
        ExecuteCommand(deleteHabitSql, parameters);
    }
}