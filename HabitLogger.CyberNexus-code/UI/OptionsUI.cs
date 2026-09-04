using HabitLogger.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitLogger.UI
{
    internal class OptionsUI
    {
        private readonly ManageHabitsUI _manageHabitsUI;
        private readonly ManageLogEntriesUI _manageLogEntriesUI;
        public OptionsUI(ManageHabitsUI manageHabitsUI, ManageLogEntriesUI manageLogEntriesUI)
        {
            _manageHabitsUI = manageHabitsUI;
            _manageLogEntriesUI = manageLogEntriesUI;
        }

        public void Show()
        {
            Console.Clear();

            while (true)
            {
                var choice = AnsiConsole.Prompt(new SelectionPrompt<OptionsMenuChoices>()
                    .Title("What would you like to do?")
                    .UseConverter(x => x switch
                    {
                        OptionsMenuChoices.ManageHabits => "Manage a Habit",
                        OptionsMenuChoices.ManageLogEntries => "Manage a Log Entry",
                        OptionsMenuChoices.Back => "Back"
                    })
                    .AddChoices(Enum.GetValues<OptionsMenuChoices>()));


                switch (choice)
                {
                    case OptionsMenuChoices.ManageHabits:
                        _manageHabitsUI.Show();
                        break;
                    case OptionsMenuChoices.ManageLogEntries:
                        _manageLogEntriesUI.Show();
                        break;
                    case OptionsMenuChoices.Back:
                        return;
                }
            }
        }

    }

    enum OptionsMenuChoices
    {
        ManageHabits,
        ManageLogEntries,
        Back
    }
}
