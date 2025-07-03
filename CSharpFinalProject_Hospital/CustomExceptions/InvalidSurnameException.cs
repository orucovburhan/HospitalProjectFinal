namespace Hospital_Project.CustomExceptions;

public class InvalidSurnameException:Exception
{
    public InvalidSurnameException(string surname)
        : base($"Surname should consist of onyl letters: {surname}") { }
}