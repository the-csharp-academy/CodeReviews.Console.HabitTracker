namespace HabitTracker.Exceptions;

public class NotFoundException(string entityName, int key)
    : Exception($"{entityName} with ID '{key}' was not found.");