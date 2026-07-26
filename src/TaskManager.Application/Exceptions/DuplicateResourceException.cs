namespace TaskManager.Application.Exceptions;

public class DuplicateResourceException(string message) : Exception(message);
