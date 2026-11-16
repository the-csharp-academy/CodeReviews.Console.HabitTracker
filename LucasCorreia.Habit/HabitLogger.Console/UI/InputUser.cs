using System.Globalization;

namespace HabitLogger.Console.UI;

public class InputUser
{
    public int InputInt(string number)
    {
        if (Int32.TryParse(number, out int isNumber)) return isNumber;
        
        throw new FormatException($"Cannot convert {number} to int");
    }

    public long InputLong(string number)
    {
        if (Int64.TryParse(number, out long isNumber)) return isNumber;

        throw new FormatException($"Cannot convert {number} to long");
    }

    public DateTime InputDate(string date)
    {
        if (DateTime.TryParseExact(
                date, 
                "yyyy-MM-dd", 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out var exactDate))
        {
            return exactDate;
        }

        throw new ArgumentException($"Date invalid: '{date}'.");
    }
}