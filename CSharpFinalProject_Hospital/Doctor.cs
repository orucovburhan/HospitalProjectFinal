using System.Text.RegularExpressions;
using Hospital_Project.CustomExceptions;

namespace Hospital_Project;
public class Doctor:Human
{
    public Doctor(string? name, string? surname, string? email, string? phone,double experienceYear, string? password) : base(name, surname, email, phone)
    {
        var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
       
        if (password != null && Regex.IsMatch(password, passwordPattern))
        {
            Password = password;
            ExperienceYear = experienceYear;
        }
        else
        {
            Console.Clear();
            if (password != null)
                throw new InvalidPasswordException(password);
        }
       
    }

    public override string ToString()
        => $@"
Name: {Name}
Surname: {Surname}
Email: {Email}
Phone: {Phone}
Password: {Password}
";
    public string? Password { get; set; }

    public double ExperienceYear { get; set; }
    public bool NineToEleven { get; set; } = true;
    public bool TwelveToThirteen { get; set; } = true;
    public bool FifteenToSeventeen { get; set; } = true;


}