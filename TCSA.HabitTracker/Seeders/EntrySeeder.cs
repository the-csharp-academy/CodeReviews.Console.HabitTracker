using TCSA.HabitTracker.DataAccessObjects;
using TCSA.HabitTracker.Models;

namespace TCSA.HabitTracker.Seeders
{
    internal class EntrySeeder : BaseSeeder<HabitEntry>
    {
        private readonly IHabitUnitReader _habitUnitReader;
        private Random _rng = new();
        public EntrySeeder(IDao<HabitEntry> EntryDao, IHabitUnitReader habitUnitReader) : base(EntryDao) 
        {
            _habitUnitReader = habitUnitReader;
        }

        protected override List<HabitEntry> GenerateData()
        {
            List<string> availableUnits = _habitUnitReader.GetAllUnits();
            if (availableUnits.Count == 0)
            {
                throw new ArgumentException("Units may not be empty");
            }
            var startDate = new DateTime(2025, 1, 1);
            int dateRange = (DateTime.Today - startDate).Days;

            List<HabitEntry> data = [];
            for (int i = 0; i < 100; i++)
            {
                string chosenUnit = availableUnits[_rng.Next(availableUnits.Count)];
                int chosenUnitId = _habitUnitReader.GetUnitId(chosenUnit);
                var newEntry = new HabitEntry()
                {
                    Value = _rng.Next(101),
                    HabitId = chosenUnitId,
                    HabitUnit = chosenUnit,
                    Date = GenerateDate("dd-MM-yyyy", startDate, dateRange)
                };
                data.Add(newEntry);
            }
            return data;
        }

        private string GenerateDate(string format, DateTime startDate, int range) => startDate.AddDays(_rng.Next(range)).ToString(format);
        
    }
}
