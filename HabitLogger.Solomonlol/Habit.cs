namespace STUDY.HabitLogger
{
    internal class Habit
    {
        private int Id { get; set; }
        private string HabitName { get; set; }
        private int Quantity { get; set; }
        private DateOnly Date { get; set; }
        public Habit(int id, string habitName, int quantity, DateOnly date)
        {
            Id = id;
            HabitName = habitName;
            Quantity = quantity;
            Date = date;
        }

        public void Print()
        {
            Console.WriteLine($"{Id}\t{HabitName}\t\t{Quantity}\t\t{Date}");
        }
    }
}
