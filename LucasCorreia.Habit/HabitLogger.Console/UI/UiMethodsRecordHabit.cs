using System;
using HabitLogger.Console.Queries;

namespace HabitLogger.Console.UI;

public class UiMethodsRecordHabit
{
    private readonly InputUser _inputUser = new InputUser();
    private readonly RecordHabitQueries _queries = new RecordHabitQueries();

    public void InsertRecord()
    {
        try
        {
            System.Console.WriteLine("Insert a new Record in Habit");

            System.Console.WriteLine("Write a id the Habit:");
            long idHabit = _inputUser.InputLong(System.Console.ReadLine()!);
            System.Console.WriteLine("Write the Date:");
            DateTime date = _inputUser.InputDate(System.Console.ReadLine()!);
            System.Console.WriteLine("Write the Quantity:");
            int quantity = _inputUser.InputInt(System.Console.ReadLine()!);

            _queries.Create(idHabit,date,quantity);

            System.Console.WriteLine("Record created");
        }
        catch (FormatException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        } 
        catch (ArgumentException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
    }

    public void UpdateRecord()
    {
        try
        {
            System.Console.WriteLine("Update a Record\n");
            System.Console.WriteLine("Write the id ");
            long id = _inputUser.InputLong(System.Console.ReadLine()!);
            System.Console.WriteLine("Write a new Date:");
            DateTime date = _inputUser.InputDate(System.Console.ReadLine()!);
            System.Console.WriteLine("Write a new Quantity:");
            int quantity = _inputUser.InputInt(System.Console.ReadLine()!);

            
            if (_queries.Update(id, date, quantity))
            {
                System.Console.WriteLine("Record Updated");
            }
            else
            {
                System.Console.WriteLine("Not Updated");
            };
        } 
        catch (FormatException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
        catch (ArgumentException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
    }

    public void DeleteRecord()
    {
        try
        {
            System.Console.WriteLine("Delete a Record\n");
            System.Console.WriteLine("Write a id to Delete");
            long id = _inputUser.InputLong(System.Console.ReadLine()!);

            if (_queries.Delete(id))
            {
                System.Console.WriteLine("Deleted record");
            }
            else
            {
                System.Console.WriteLine("Not Deleted");
            }
        }
        catch (FormatException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
        catch (ArgumentException e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
    }

    public void ListRecordByIdHabit()
    {
        System.Console.WriteLine("List of Record by Habit\n");
        System.Console.WriteLine("Write a id to get the List");
        long id = _inputUser.InputLong(System.Console.ReadLine()!);

        var list = _queries.GetAllById(id);

        foreach (var record in list)
        {
            System.Console.WriteLine($"Id: {record.Id} - Date: {record.Date} - Quantity: {record.Quantity}");
        }
    }
}
