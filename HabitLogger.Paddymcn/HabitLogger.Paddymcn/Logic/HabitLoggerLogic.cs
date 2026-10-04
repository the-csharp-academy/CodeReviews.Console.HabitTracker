using System.Globalization;

namespace HabitLogger.Paddymcn.Logic;
public class HabitLoggerLogic
{
    public Biscuit PromptForBiscuitDetails()
    {
        Console.WriteLine("Enter Brand Name:");
        string brandName = CheckEntryNotBlank(Console.ReadLine());
        
        Console.WriteLine("Enter Description:");
        string description = CheckEntryNotBlank(Console.ReadLine());
        
        Console.WriteLine("Enter Calories Per Item:");
        int caloriesPerItem = CheckValidNumber(Console.ReadLine());

        Console.WriteLine("Enter Number Eaten:");
        int numberEaten = CheckValidNumber(Console.ReadLine());
        
        DateTime dateEaten = CheckDateEntry();
        
        Biscuit biscuit = new()
        {
            BrandName = brandName,
            Description = description,
            CaloriesPerItem = caloriesPerItem,
            NumberEaten = numberEaten,
            TotalCalories = checked(caloriesPerItem * numberEaten),
            DateEaten = dateEaten
        };
        return biscuit;
    }

    public int CheckEntry()
    {
        Console.WriteLine("Enter id:");
        string idTextIn = CheckEntryNotBlank(Console.ReadLine());  
        int id = CheckValidNumber(idTextIn);
        return id;
    }
    public DateTime CheckDateEntry()
    {
        while (true)
        {
            Console.WriteLine("Enter date (dd-MM-yy), or 't' for today:");
            string input = CheckEntryNotBlank(Console.ReadLine());

            if (input.Trim().ToLower() == "t")
            {
                return DateTime.Today;
            }

            if (!DateTime.TryParseExact(input, "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateEaten))
            {
                Console.WriteLine("Not a valid format. Use dd-MM-yy.");
                continue;
            }

            if (dateEaten > DateTime.Today)
            {
                Console.WriteLine("The date can't be in the future.");
                continue;
            }

            return dateEaten;
        }
    }

    int CheckValidNumber(string numberIn)
    {
        int userNumber;
         while ((!int.TryParse(numberIn, out userNumber)) || (userNumber <= 0))
         { 
             Console.WriteLine("Not a valid entry. Enter again:");
             numberIn = CheckEntryNotBlank(Console.ReadLine());
         }
        return userNumber;
    }

    string CheckEntryNotBlank(string entry)
    {
        while (string.IsNullOrWhiteSpace(entry))
        {
            Console.WriteLine("Not a valid entry. Enter again:");
            entry = Console.ReadLine();
        };
        return entry;

    }
}