namespace Hospital_Project.CustomExceptions;

public class InvalidNameException:Exception
{
    public InvalidNameException(string name)
        : base($"Name should consist of only letters: {name}") { }
}