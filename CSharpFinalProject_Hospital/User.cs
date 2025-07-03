using System.ComponentModel.Design;
using System.Text.Json;
using System.Text.RegularExpressions;
using Hospital_Project.CustomExceptions;

namespace Hospital_Project;

public class User : Human
{
    public User()
    {
    }
    public User(string? name, string? surname, string? email, string? phone, string? password) : base(name, surname, email, phone)
    {
        var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
       
            if (password != null && Regex.IsMatch(password, passwordPattern))
            {
                Password = password;
            }
            else
            {
                Console.Clear();
                    if (password != null)
                        throw new InvalidPasswordException(password);
            }
       
    }
    public string? Password { get; set; }

    public override string ToString()
        => $@"
Name: {Name}
Surname: {Surname}
Email: {Email}
Phone: {Phone}
Password: {Password}
";
}
