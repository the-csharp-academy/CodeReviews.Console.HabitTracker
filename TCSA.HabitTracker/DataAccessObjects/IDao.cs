namespace TCSA.HabitTracker.DataAccessObjects
{
    internal interface IDao<T>
    {
        List<T> GetAll();
        void Insert(T item);
        void InsertMany(List<T> items);
    }
}
