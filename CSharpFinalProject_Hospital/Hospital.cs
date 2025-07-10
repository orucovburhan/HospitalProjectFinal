using System.Text.Json;

namespace Hospital_Project;

public class Hospital
{
    public string Name { get; set; }

    public List<Doctor> AllDoctors = new List<Doctor>()
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


    public Hospital(string name)
    {
        string exePath = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
        string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
        string hospitalPath = Path.Combine(jsonFolder, "hospital.json");
        Departments = new List<Department>()
        {
            new Department("Pediatrics",
                new List<Doctor>() { AllDoctors[0], AllDoctors[1], AllDoctors[2], AllDoctors[3] }),
            new Department("Traumatology", new List<Doctor>() { AllDoctors[4], AllDoctors[5], AllDoctors[6] }),
            new Department("Dentistry",
                new List<Doctor>() { AllDoctors[7], AllDoctors[8], AllDoctors[9], AllDoctors[10] }),
        };
        Name = name;
        if (File.Exists(hospitalPath))
            Departments = ReadDepartmentsFromFile();
        else
        {
            WriteToFileHospital();
        }
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
                    Password = doc.Password,
                    NineToEleven = doc.NineToEleven,
                    TwelveToFourteen = doc.TwelveToFourteen,
                    FifteenToSeventeen = doc.FifteenToSeventeen
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
    public List<Department> ReadDepartmentsFromFile()
    {
        string exePath = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
        string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
        string hospitalPath = Path.Combine(jsonFolder, "hospital.json");

        if (!File.Exists(hospitalPath))
            return new List<Department>();

        string json = File.ReadAllText(hospitalPath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Department>();

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("Departments", out JsonElement departmentsElement))
        {
            List<Department> departments = new List<Department>();

            foreach (JsonElement deptElement in departmentsElement.EnumerateArray())
            {
                string name = deptElement.GetProperty("Name").GetString();
                List<Doctor> doctors = new List<Doctor>();

                foreach (JsonElement docElement in deptElement.GetProperty("Doctors").EnumerateArray())
                {
                    string docName = docElement.GetProperty("Name").GetString();
                    string docSurname = docElement.GetProperty("Surname").GetString();
                    string docEmail = docElement.GetProperty("Email").GetString();
                    string docPhone = docElement.GetProperty("Phone").GetString();
                    double docExp = docElement.GetProperty("ExperienceYear").GetDouble();
                    string docPassword = docElement.GetProperty("Password").GetString();

                    doctors.Add(new Doctor(docName, docSurname, docEmail, docPhone, docExp, docPassword));
                }

                departments.Add(new Department(name, doctors));
            }

            return departments;
        }

        return new List<Department>();
    }

    public static Hospital ReadFromFileHospital()
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
    public List<Doctor> GetDoctorsByDepartment(string departmentName)
{
    if (string.IsNullOrWhiteSpace(departmentName))
        return new List<Doctor>();

    string exePath = AppContext.BaseDirectory;
    string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
    string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
    string hospitalPath = Path.Combine(jsonFolder, "hospital.json");

    if (!File.Exists(hospitalPath))
        return new List<Doctor>();

    string json = File.ReadAllText(hospitalPath);
    using JsonDocument doc = JsonDocument.Parse(json);
    JsonElement root = doc.RootElement;

    if (!root.TryGetProperty("Departments", out JsonElement departmentsElement))
        return new List<Doctor>();

    foreach (JsonElement deptElement in departmentsElement.EnumerateArray())
    {
        string deptName = deptElement.GetProperty("Name").GetString();
        if (!deptName.Equals(departmentName, StringComparison.OrdinalIgnoreCase))
            continue;

        List<Doctor> doctors = new();

        foreach (JsonElement docElement in deptElement.GetProperty("Doctors").EnumerateArray())
        {
            string name = docElement.GetProperty("Name").GetString();
            string surname = docElement.GetProperty("Surname").GetString();
            string email = docElement.GetProperty("Email").GetString();
            string phone = docElement.GetProperty("Phone").GetString();
            double experience = docElement.GetProperty("ExperienceYear").GetDouble();
            string password = docElement.GetProperty("Password").GetString();

            bool nineToEleven = docElement.GetProperty("NineToEleven").GetBoolean();
            bool twelveToFourteen = docElement.GetProperty("TwelveToFourteen").GetBoolean();
            bool fifteenToSeventeen = docElement.GetProperty("FifteenToSeventeen").GetBoolean();

            Doctor doctor = new(name, surname, email, phone, experience, password)
            {
                NineToEleven = nineToEleven,
                TwelveToFourteen = twelveToFourteen,
                FifteenToSeventeen = fifteenToSeventeen
            };

            doctors.Add(doctor);
        }

        return doctors;
    }

    return new List<Doctor>();
}



}


