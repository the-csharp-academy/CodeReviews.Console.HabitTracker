using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitLogger.Controller;

public class BaseController
{
    protected void DisplayMessage(string message, string color = "yellow")
    {


        AnsiConsole.MarkupLine($"[{color}]{message}[/]");
    }

    protected bool ConfirmDeletion(string itemName)
    {
        var confirm = AnsiConsole.Confirm($"Are you sure you want to delete [red]{itemName}[/]?");

        return confirm;
    }

}

