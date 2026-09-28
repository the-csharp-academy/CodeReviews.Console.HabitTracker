using TCSA.HabitTracker.DataAccessObjects;

namespace TCSA.HabitTracker.Seeders
{
    internal abstract class BaseSeeder<T>
    {
        protected readonly IDao<T> _dao;

        protected BaseSeeder(IDao<T> dao)
        {
            _dao = dao;
        }
        public void Seed()
        {
            if (!IsTableEmpty()) return;
            _dao.InsertMany(GenerateData());
        }

        protected abstract List<T> GenerateData();

        protected bool IsTableEmpty() => _dao.GetAll().Count == 0;

    }
}
