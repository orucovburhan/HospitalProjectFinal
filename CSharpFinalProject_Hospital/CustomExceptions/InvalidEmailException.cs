namespace Hospital_Project.CustomExceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException(string email)
        : base($"Invalid email address: {email}") { }
}
