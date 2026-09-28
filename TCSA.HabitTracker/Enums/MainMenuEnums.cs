using System.Text.RegularExpressions;

namespace TCSA.HabitTracker.Enums
{
    internal static class MainMenuEnums
    {
        public enum EntryMenuOptions
        {
            ViewAllEntries,
            InsertEntry,
            DeleteEntry,
            UpdateEntry,
            Exit
        }

        public enum HabitMenuOptions
        {
            ViewHabits,
            InsertHabit,
            DeleteHabit,
            UpdateHabit,
            Exit,
        }

        public enum MainMenuOptions
        {
            EntriesMenu,
            HabitsMenu,
            Exit
        }

        public static string MenuOptionFormatter(HabitMenuOptions option)
        {
            return Regex.Replace(option.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
        public static string MenuOptionFormatter(EntryMenuOptions option)
        {
            return Regex.Replace(option.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
        public static string MenuOptionFormatter(MainMenuOptions option)
        {
            return Regex.Replace(option.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
    }
}
