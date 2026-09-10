using System;
using System.Collections.Generic;
using System.Text;

namespace HabitLogger.Models;
public class Log
{
    public int Id { get; set; }
    public string Habit {  get; set; }
    public string Date { get; set; }
    public int Quantity { get; set; }

}

