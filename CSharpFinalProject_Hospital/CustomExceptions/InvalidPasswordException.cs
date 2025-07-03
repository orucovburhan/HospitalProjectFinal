namespace Hospital_Project.CustomExceptions;

public class InvalidPasswordException:Exception
{
    public InvalidPasswordException(string password)
        : base($"Password should contain at least one lowercase, one uppercase, one digit and should be at least 8 symboles. {password}") { }
}