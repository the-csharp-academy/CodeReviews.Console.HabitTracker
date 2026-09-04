using System;
using System.Collections.Generic;
using System.Text;

namespace HabitLogger.Entities
{
    internal class Habit
    {
        public int Id;
        public string Name { get; private set; }
        public string Measurement { get; private set; }

        public Habit(int id, string name, string measurement)
        {
            Id = id;
            Name = name;
            Measurement = measurement;
        }

        public void Updatename(string newName)
        {
            if (newName != "")
            {
                Name = newName;
            }
        }

        public void UpdateMeasurement(string newMeasurement)
        {
            if(newMeasurement != "")
            {
                Measurement = newMeasurement;
            }
        }
    }
}
