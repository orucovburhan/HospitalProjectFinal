using System.Text.Json;
using System.Text.RegularExpressions;
using Hospital_Project.CustomExceptions;

namespace Hospital_Project;

public abstract class Human
{
    public Human() { }
    private static List<User> ReadUsers()
    {
        try
        {
            if (!File.Exists("users.json"))
                return new List<User>();

            string json = File.ReadAllText("users.json");

            if (string.IsNullOrWhiteSpace(json))
                return new List<User>();

            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch
        {
            return new List<User>();
        }
    }

    private static List<User> users = ReadUsers();
    private static List<string> user_emails = new List<string>();
    private static List<string> user_phones = new List<string>();

    private static void AddUserInfoToList()
    {
        foreach (var user in users)
        {
            user_emails.Add(user.Email);
            user_phones.Add(user.Phone);
        }
    }
    protected Human(string? name, string? surname,string? email,string? phone)
    {
        var emailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                           + "@"
                           + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";
        var phonePattern = @"^\d{10}$";
        var namePattern = "[a-zA-Z]";
        AddUserInfoToList();
        
        if (email != null && !user_emails.Contains(email) )
        {
            if (phone != null && !user_phones.Contains(phone))
            {
                if (Regex.IsMatch(email, emailPattern))
                {
                    if (Regex.IsMatch(phone, phonePattern))
                    {
                        if (name != null && Regex.IsMatch(name, namePattern))
                        {
                            if (surname != null && Regex.IsMatch(surname, namePattern))
                            {
                                Email = email;
                                Phone = phone;
                                Name = name;
                                Surname = surname;
                                Phone = phone;
                            }
                            else
                            {
                                Console.Clear();
                                if (surname != null) throw new InvalidSurnameException(surname);
                            }
                        }
                        else
                        {
                            Console.Clear();
                            if (name != null) throw new InvalidNameException(name);
                        }

                    }
                    else
                    {
                        Console.Clear();
                        if (phone != null) throw new InvalidPhoneNumberException(phone);
                    }
                }
                else
                {
                    Console.Clear();
                    if (email != null) throw new InvalidEmailException(email);
                }
            }
            else
            {
                Console.Clear();
                if (phone != null) throw new AccountExistsException(phone); 
            }
        }
        else
        {
            Console.Clear();
            if (email != null) throw new AccountExistsException(email);
        }

    }

    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

}