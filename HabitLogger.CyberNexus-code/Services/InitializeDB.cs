using HabitLogger.Interfaces;
using Microsoft.Data.Sqlite;

namespace HabitLogger.Services
{
    internal class InitializeDB : IInitializeDB
    {
        public void InitializeDatabase()
        {
            
            Console.WriteLine("starting Program.cs");

            string path = @"./db";

            if (!Directory.Exists(path))
            {
                Console.WriteLine("db directory does not exist. Creating it...");
                try
                {
                    Directory.CreateDirectory("./db");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The process failed: {0}", ex.ToString());
                }
            }
        }

       
    }
}
