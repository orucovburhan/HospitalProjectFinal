using System.Text.Json;

namespace Hospital_Project;

public class Hospital
{
    public string Name { get; set; }
    public List<Doctor> AllDoctors  = ReadDoctors();

    public Hospital(string name)
    {
        Departments = new List<Department>()
        {
        new Department("Pediatrics",new List<Doctor>(){AllDoctors[0],AllDoctors[1],AllDoctors[2],AllDoctors[3]}),
        new Department("Traumatology",new List<Doctor>(){AllDoctors[4],AllDoctors[5],AllDoctors[6]}),
        new Department("Dentistry",new List<Doctor>(){AllDoctors[7],AllDoctors[8],AllDoctors[9],AllDoctors[10]}),
        };
        Name = name;
    }

    public List<Department> Departments;
    
    private static List<Doctor> ReadDoctors()
    {
        try
        {
            string exePath = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
            string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
            string doctorsPath = Path.Combine(jsonFolder, "doctors.json");

            if (!File.Exists(doctorsPath))
                return new List<Doctor>();

            string json = File.ReadAllText(doctorsPath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Doctor>();

            return JsonSerializer.Deserialize<List<Doctor>>(json) ?? new List<Doctor>();
        }
        catch
        {
            return new List<Doctor>();
        }
    }

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
    public void WriteToFileHospital()
    {
        string exePath = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
        string jsonFolder = Path.Combine(projectRoot, "JsonFiles");

        if (!Directory.Exists(jsonFolder))
            Directory.CreateDirectory(jsonFolder);

        string hospitalPath = Path.Combine(jsonFolder, "hospital.json");

        List<object> departmentsData = new List<object>();

        foreach (var dept in Departments)
        {
            List<object> doctorsData = new List<object>();

            foreach (var doc in dept.Doctors)
            {
                doctorsData.Add(new
                {
                    Name = doc.Name,
                    Surname = doc.Surname,
                    Email = doc.Email,
                    Phone = doc.Phone,
                    ExperienceYear = doc.ExperienceYear,
                    Password = doc.Password
                });
            }

            departmentsData.Add(new
            {
                Name = dept.Name,
                Doctors = doctorsData
            });
        }

        var hospitalData = new
        {
            Name = this.Name,
            Departments = departmentsData
        };

        string json = JsonSerializer.Serialize(hospitalData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(hospitalPath, json);
    }

    public Hospital ReadFromFileHospital()
    {
        string exePath = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
        string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
        string hospitalPath = Path.Combine(jsonFolder, "hospital.json");

        if (!File.Exists(hospitalPath))
            return null;  

        string json = File.ReadAllText(hospitalPath);

        if (string.IsNullOrWhiteSpace(json))
            return null;

       
        Hospital hospital = JsonSerializer.Deserialize<Hospital>(json);

        return hospital;
    }

}


