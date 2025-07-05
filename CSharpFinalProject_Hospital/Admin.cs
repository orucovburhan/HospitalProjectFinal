using System.Text.Json;

namespace Hospital_Project;

public class Admin
{
    public Admin(string name, string password)
    {
        Name = name;
        Password = password;
    }
    
    public string  Name { get; set; }
    public string  Password { get; set; }
    

    public void View<T>(List<T> objs)
    {
        foreach (var obj in objs)
        {
            Console.WriteLine(obj);
            Console.WriteLine("____________________________");
        }
    }
    public void ViewApplicants(List<ApplicantDoctor> applicantsDoctors,List<Doctor> doctors,ref Hospital hospital)
    {
        if (applicantsDoctors == null || applicantsDoctors.Count == 0)
        {
            Console.WriteLine("No applicants found.");
            return;
        }
        int selectedIndex = 0;
        ConsoleKey key;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Select an applicant:\n");

            for (int i = 0; i < applicantsDoctors.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($">> {applicantsDoctors[i].Name} ({applicantsDoctors[i].Email})");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"   {applicantsDoctors[i].Name} ({applicantsDoctors[i].Email})");
                }
            }

            if (selectedIndex == applicantsDoctors.Count)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(">> Back");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("   Back");
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? applicantsDoctors.Count : selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == applicantsDoctors.Count) ? 0 : selectedIndex + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                if (selectedIndex == applicantsDoctors.Count)
                {
                    break;
                }
                else
                {
                    ShowDoctorDetails(applicantsDoctors[selectedIndex],doctors,ref hospital);
                }
            }
        }
    }
    void WriteToFileDoctor(List<Doctor> doctors, Doctor newDoctor)
    {
        string exePath = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
        string jsonFolder = Path.Combine(projectRoot, "JsonFiles");

        if (!Directory.Exists(jsonFolder))
            Directory.CreateDirectory(jsonFolder);

        string doctorsPath = Path.Combine(jsonFolder, "doctors.json");

        if (File.Exists(doctorsPath))
        {
            string oldJson = File.ReadAllText(doctorsPath);
            if (!string.IsNullOrWhiteSpace(oldJson))
                doctors = JsonSerializer.Deserialize<List<Doctor>>(oldJson) ?? new();
        }

        int index = doctors.FindIndex(d => d.Email == newDoctor.Email);

        //doctor filede tapilsa update edecek yoxsa elae edecek
        if (index != -1)
            doctors[index] = newDoctor;
        else
            doctors.Add(newDoctor);

        string newJson = JsonSerializer.Serialize(doctors, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(doctorsPath, newJson);
    }

    void RemoveApplicantDoctorByEmail(string email)
    {
        string exePath = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
        string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
        string applicantsPath = Path.Combine(jsonFolder, "applicantDoctors.json");

        List<ApplicantDoctor> applicantDoctors = new();

        if (File.Exists(applicantsPath))
        {
            string json = File.ReadAllText(applicantsPath);
            if (!string.IsNullOrWhiteSpace(json))
                applicantDoctors = JsonSerializer.Deserialize<List<ApplicantDoctor>>(json) ?? new();
        }

        int removedCount = applicantDoctors.RemoveAll(a => a.Email == email);

        if (removedCount > 0)
        {
            string updatedJson = JsonSerializer.Serialize(applicantDoctors, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(applicantsPath, updatedJson);
        }
    }
    private void ShowDoctorDetails(ApplicantDoctor applicantDoctor,List<Doctor> doctors,ref Hospital hospital)
{
    int optionIndex = 0;
    string[] options = { "Accept", "Reject", "Back" };
    ConsoleKey key;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("Applicant Details:\n");
        Console.WriteLine(applicantDoctor);
        Console.WriteLine("\nWhat do you want to do?\n");

        for (int i = 0; i < options.Length; i++)
        {
            if (i == optionIndex)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(">> " + options[i]);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("   " + options[i]);
            }
        }

        key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.UpArrow)
        {
            optionIndex = (optionIndex == 0) ? options.Length - 1 : optionIndex - 1;
        }
        else if (key == ConsoleKey.DownArrow)
        {
            optionIndex = (optionIndex == options.Length - 1) ? 0 : optionIndex + 1;
        }
        else if (key == ConsoleKey.Enter)
        {
            if (options[optionIndex] == "Accept")
            {
                Console.Clear();
                Console.WriteLine($"Applicant '{applicantDoctor.Name}' has been accepted.");
                Doctor newDoctor = new Doctor(applicantDoctor.Name, applicantDoctor.Surname, applicantDoctor.Email,applicantDoctor.Phone,applicantDoctor.ExperienceYear,applicantDoctor.Password);
                doctors.Add(newDoctor);
                int i;
                for ( i = 0; i <hospital.Departments.Count; i++)
                {
                    if (hospital.Departments[i].Name == applicantDoctor.Department)
                        break;
                }
                int k;
                bool found = false;
                for (k = 0; k < hospital.Departments.Count; k++)
                {
                    if (hospital.Departments[k].Name == applicantDoctor.Department)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    hospital.Departments[k].Doctors.Add(newDoctor);
                }
                
                WriteToFileDoctor(doctors, newDoctor);
                RemoveApplicantDoctorByEmail(applicantDoctor.Email);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(); 
                
                break;
            }
            else if (options[optionIndex] == "Reject")
            {
                Console.Clear();
                Console.WriteLine($"Applicant '{applicantDoctor.Name}' has been rejected.");
                RemoveApplicantDoctorByEmail(applicantDoctor.Email);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(); 
                break;
            }
            else if (options[optionIndex] == "Back")
            {
                break;
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
        else if (key == ConsoleKey.Escape)
        {
            break;
        }
    }
}

    
}