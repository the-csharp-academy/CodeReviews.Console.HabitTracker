namespace HabitLogger.Console.Model;

public class RecordHabit
{
    public long Id { get; }
    public DateTime Date { get; }
    public int Quantity { get; }
    public long HabitId { get; }

    public RecordHabit(DateTime date, int quantity,long id)
    {
        Date = date;
        Quantity = quantity;
        Id = id;
    }
}