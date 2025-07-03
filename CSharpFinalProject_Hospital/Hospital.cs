using System.Text.Json;

namespace Hospital_Project;

public class Hospital
{
    public static List<Doctor> AllDoctors = new List<Doctor>()
    {
        new Doctor("Elmira", "Orucova", "elmiraorucova17@gmail.com", "0701234561", 12, "Elmira123"),
        new Doctor("Niket", "Patel", "niket.patel@hospital.com", "0701234562", 18, "Niket1234"),
        new Doctor("Katharine", "Hurt", "katharine.hurt@hospital.com", "0701234563", 19, "Katharine123"),
        new Doctor("Eriberto", "Farinella", "eriberto.farinella@hospital.com", "0701234564", 6, "Eriberto123"),
        new Doctor("Marco", "Gerlinger", "marco.gerlinger@hospital.com", "0701234565", 24, "Marco1234"),
        new Doctor("Anne", "Mitchener", "anne.mitchener@hospital.com", "0701234566", 34, "Anne12345"),
        new Doctor("Jonathan", "Behar", "jonathan.behar2@hospital.com", "0701234567", 14, "Jonathan123"),
        new Doctor("Matthew", "Foxton", "matthew.foxton@hospital.com", "0701234568", 15, "Matthew123"),
        new Doctor("Andrew", "Moore", "andrew.moore@hospital.com", "0701234569", 17, "Andrew123"),
        new Doctor("Daniel", "Fagan", "daniel.fagan@hospital.com", "0701234570", 23, "Daniel123"),
        new Doctor("Rod", "Hughes", "rod.hughes@hospital.com", "0701234571", 29, "Rod123456"),
    };



    public List<Department> Departments = new List<Department>()
    {
        new Department("Pediatrics",new List<Doctor>(){AllDoctors[0],AllDoctors[1],AllDoctors[2],AllDoctors[3]}),
        new Department("Traumatology",new List<Doctor>(){AllDoctors[4],AllDoctors[5],AllDoctors[6]}),
        new Department("Dentistry",new List<Doctor>(){AllDoctors[7],AllDoctors[8],AllDoctors[9],AllDoctors[10]}),
    };
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

    public List<User> Users= ReadUsers();


}


