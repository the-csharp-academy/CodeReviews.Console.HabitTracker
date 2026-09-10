using HabitLogger.Controller;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;
using static Enums;

namespace HabitLogger;

internal class UserInterface
{
    private bool _menuActive = true;
    private readonly LogController _logController = new(); 

    internal void MainMenu()
    {
        //Initialize the bd
        _logController.InitializeConnection();

        while (_menuActive)
        {
            Console.Clear();

            var actionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Menu>()
                .Title("What do you want to do next?")
                .AddChoices(Enum.GetValues<Menu>()));


            switch (actionChoice)
            {
                case Menu.View:
                    ViewItems();
                    break;
                case Menu.Insert:
                    InsertItem();
                    break;
                case Menu.Update:
                    UpdateItem();
                    break;
                case Menu.Delete:
                    DeleteItem();
                    break;
                case Menu.Exit:
                    _menuActive = false;
                    break;

            }
        }
    }

    private void ViewItems()
    {

        _logController.GetAllLogs();
    }

    private void InsertItem()
    {
        _logController.InsertLog();
    }

    private void DeleteItem()
    {
        _logController.DeleteLog();
    }

    private void UpdateItem()
    {
        _logController.UpdateLog();
    }
}

