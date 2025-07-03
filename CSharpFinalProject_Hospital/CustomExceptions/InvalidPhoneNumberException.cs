namespace Hospital_Project.CustomExceptions;

public class InvalidPhoneNumberException:Exception
{
        public InvalidPhoneNumberException(string phone)
            : base($"Invalid phone number: {phone}") { }
}