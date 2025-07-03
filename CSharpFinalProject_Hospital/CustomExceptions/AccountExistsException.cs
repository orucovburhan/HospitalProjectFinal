namespace Hospital_Project.CustomExceptions;

public class AccountExistsException: Exception
{
    public AccountExistsException(string data)
        : base($"An account exists with: {data}") { }
}