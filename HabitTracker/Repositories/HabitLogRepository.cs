using HabitTracker.Database;
using HabitTracker.Models;

namespace HabitTracker.Repositories;

public class HabitLogRepository : RepositoryBase
{
    public void Create(HabitLog habitLog)
    {
        string insertHabitLogSql = """
                                   INSERT INTO HabitLogs (HabitId, Occurrences, OccurrenceDate)
                                   VALUES (@habitId, @occurrences, @date);
                                   """;

        (string Name, object Value)[] parameters =
        [
            ("@habitId", habitLog.HabitId),
            ("@occurrences", habitLog.Occurrences),
            ("@date", habitLog.OccurrenceDate),
        ];

        ExecuteCommand(insertHabitLogSql, parameters);
    }
    
    public void BulkCreate(List<HabitLog> habitLogs)
    {
        string insertHabitSql = """
                                INSERT INTO HabitLogs (HabitId, Occurrences, OccurrenceDate)
                                VALUES (@habitId, @occurrences, @date);
                                """;

        var parameterSets = habitLogs.Select(habitLog =>
            new (string Name, object Value)[]
            {
                ("@habitId", habitLog.HabitId),
                ("@occurrences", habitLog.Occurrences),
                ("@date", habitLog.OccurrenceDate)
            }).ToArray();

        ExecuteBulkCommand(insertHabitSql, parameterSets);
    }

    public List<HabitLog> Read()
    {
        string selectHabitLogsSql = """
                                    SELECT * FROM HabitLogs;
                                    """;

        var habitLogs = ExecuteQuery<HabitLog>(selectHabitLogsSql, reader => new HabitLog
        {
            Id = Convert.ToInt32(reader.GetString(0)),
            HabitId = Convert.ToInt32(reader.GetString(1)),
            Occurrences = Convert.ToInt32(reader.GetString(2)),
            OccurrenceDate = DateOnly.FromDateTime(Convert.ToDateTime(reader.GetString(3))),
        });

        return habitLogs;
    }
    
    public HabitLog? Read(int id)
    {
        string selectHabitLogsSql = """
                                 SELECT * FROM HabitLogs
                                 WHERE Id = @Id;
                                 """;
        
        (string Name, object Value)[] parameters =
        [
            ("@Id", id)
        ];
        
        var habitLog = ExecuteQueryReturnSingle<HabitLog>(selectHabitLogsSql, reader => new HabitLog
        {
            Id = Convert.ToInt32(reader.GetString(0)),
            HabitId = Convert.ToInt32(reader.GetString(1)),
            Occurrences = Convert.ToInt32(reader.GetString(2)),
            OccurrenceDate = DateOnly.FromDateTime(Convert.ToDateTime(reader.GetString(3))),
        }, parameters);

        return habitLog;
    }

    public void Update(HabitLog habitLog)
    {
        string updateHabitLogSql = """
                                   UPDATE HabitLogs
                                   SET HabitId = @habitId, Occurrences = @occurrences, OccurrenceDate = @date
                                   WHERE Id = @id;
                                   """;
        (string Name, object Value)[] parameters =
        [
            ("@id", habitLog.Id),
            ("@habitId", habitLog.HabitId),
            ("@occurrences", habitLog.Occurrences),
            ("@date", habitLog.OccurrenceDate),
        ];

        ExecuteCommand(updateHabitLogSql, parameters);
    }

    public void Delete(int id)
    {
        string deleteHabitLogSql = """
                                DELETE FROM HabitLogs
                                WHERE Id = @Id;
                                """;
        
        (string Name, object Value)[] parameters =
        [
            ("@Id", id)
        ];

        ExecuteCommand(deleteHabitLogSql, parameters);
    }
}