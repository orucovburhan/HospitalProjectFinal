using System.Text.RegularExpressions;
using Hospital_Project.CustomExceptions;

namespace Hospital_Project;
public class ApplicantDoctor : Human
{
    public ApplicantDoctor(string? name, string? surname, string? email, string? phone, double experienceYear, string? password,string? uniName)
        : base(name, surname, email, phone)
    {
        var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

        if (password != null && Regex.IsMatch(password, passwordPattern))
        {
            Password = password;
            ExperienceYear = experienceYear;
            UniName = uniName;
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
Experience: {ExperienceYear} year
University name: {UniName}
";

    public string? Password { get; set; }
    public double ExperienceYear { get; set; }
    public string UniName { get; set; }
    
    public string Status { get; set; } = "Waiting";
    public string Department { get; set; }




}