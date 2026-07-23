using static STUDY.HabitLogger.Enums;
using static STUDY.HabitLogger.HabitController;

namespace STUDY.HabitLogger
{
    internal class UserInteface
    {
        public void Menu()
        {
            CreatingDbTable();
            bool notEnd = true;
            while(notEnd)
            {
                Console.Clear();

                Console.WriteLine("Choose operation (0-4):");
                Console.WriteLine("1.View all habits.");
                Console.WriteLine("2.Add new habit.");
                Console.WriteLine("3.Update habit.");
                Console.WriteLine("4.Delete habit.");
                Console.WriteLine("0.Exit.");
                int operation;
                while (!int.TryParse(Console.ReadLine(), out operation) || operation>4 || operation<0)
                {
                    Console.WriteLine("Wrong data format. The string must contain only digits in [0-4] range. Try again.");
                }

                switch((MainMenu)operation)
                {
                    case MainMenu.View:
                        GetDbData();
                        break;
                    case MainMenu.Add:
                        AddNewDbData();
                        break;
                    case MainMenu.Update:
                        UpdateDbData();
                        break;
                    case MainMenu.Delete:
                        DeleteDbData();
                        break;
                    case MainMenu.Exit:
                        notEnd = false;
                        break;
                    
                }
            }
        }
    }
}
