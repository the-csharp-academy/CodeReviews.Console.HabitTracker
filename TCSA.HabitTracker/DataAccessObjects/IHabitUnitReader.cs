namespace TCSA.HabitTracker.DataAccessObjects
{
    internal interface IHabitUnitReader
    {
        List<string> GetAllUnits();
        int GetUnitId(string habitUnit);

    }
}
