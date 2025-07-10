using System;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
using Serilog;
using Serilog;
using System;
using System.IO;
using Serilog.Events;


namespace Hospital_Project
{
    public static class Logger
    {
        static Logger()
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            // eger Logs qovlugu yoxdursa yaradir
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string logFilePath = Path.Combine(logDirectory, "log-.txt");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static void Info(string message) => Log.Information(message);
        public static void Error(string message) => Log.Error(message);
        public static void Warning(string message) => Log.Warning(message);
    }
    public static class App
    {
        public static void Start()
        {
            MainPart: ;

            string exePath = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
            string basePath = Path.Combine(projectRoot, "Logs");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(basePath, "Warnings", "warning-.txt"),
                    rollingInterval: RollingInterval.Infinite,
                    restrictedToMinimumLevel: LogEventLevel.Warning)
                .WriteTo.File(Path.Combine(basePath, "Errors", "error-.txt"),
                    rollingInterval: RollingInterval.Infinite,
                    restrictedToMinimumLevel: LogEventLevel.Error)
                .WriteTo.File(Path.Combine(basePath, "Information", "information-.txt"),
                    rollingInterval: RollingInterval.Infinite,
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();


            Hospital hospital = new Hospital("MediNova Hospital");
            hospital = Hospital.ReadFromFileHospital() ?? new Hospital("MediNova Hospital");
            void WriteToFileUser(List<User> users, User newUser)
            {
                string exePath = AppContext.BaseDirectory;
                string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
                string jsonFolder = Path.Combine(projectRoot, "JsonFiles");

                if (!Directory.Exists(jsonFolder))
                    Directory.CreateDirectory(jsonFolder);

                string usersPath = Path.Combine(jsonFolder, "users.json");

                if (File.Exists(usersPath))
                {
                    string oldJson = File.ReadAllText(usersPath);
                    if (!string.IsNullOrWhiteSpace(oldJson))
                        users = JsonSerializer.Deserialize<List<User>>(oldJson) ?? new();
                }

                users.Add(newUser);
                string newJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(usersPath, newJson);
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
            void WriteToFileDoctorApplicant(ApplicantDoctor newDoctor)
            {
                string exePath = AppContext.BaseDirectory;
                string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
                string jsonFolder = Path.Combine(projectRoot, "JsonFiles");

                if (!Directory.Exists(jsonFolder))
                    Directory.CreateDirectory(jsonFolder);

                string doctorsPath = Path.Combine(jsonFolder, "applicantDoctors.json");

                List<ApplicantDoctor> applicantDoctors = new();

                // Mövcud dataları oxu
                if (File.Exists(doctorsPath))
                {
                    string oldJson = File.ReadAllText(doctorsPath);
                    if (!string.IsNullOrWhiteSpace(oldJson))
                        applicantDoctors = JsonSerializer.Deserialize<List<ApplicantDoctor>>(oldJson) ?? new();
                }

                // Eyni email varsa exception at
                if (applicantDoctors.Any(d => d.Email == newDoctor.Email))
                    throw new Exception("A doctor with this email already exists!");

                // Əlavə et və append yaz
                applicantDoctors.Add(newDoctor);

                string newJson = JsonSerializer.Serialize(applicantDoctors, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(doctorsPath, newJson);
            }
            void WriteToFileAllDoctors(List<Doctor> doctors)
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
                string newJson = JsonSerializer.Serialize(doctors, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(doctorsPath, newJson);
            }
            void UpdateDoctorAvailability(string email, string availabilityField, bool newValue)
            {
                string exePath = AppContext.BaseDirectory;
                string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
                string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
                string doctorsPath = Path.Combine(jsonFolder, "doctors.json");

                if (!File.Exists(doctorsPath))
                    return;

                string json = File.ReadAllText(doctorsPath);
                List<Doctor> doctors = JsonSerializer.Deserialize<List<Doctor>>(json) ?? new();

                var doctor = doctors.FirstOrDefault(d => d.Email == email);
                if (doctor == null)
                    return;

                switch (availabilityField)
                {
                    case "NineToEleven":
                        doctor.NineToEleven = newValue;
                        break;
                    case "TwelveToFourteen":
                        doctor.TwelveToFourteen = newValue;
                        break;
                    case "FifteenToSeventeen":
                        doctor.FifteenToSeventeen = newValue;
                        break;
                    default:
                        Console.WriteLine("Invalid availability field.");
                        return;
                }
                string updatedJson = JsonSerializer.Serialize(doctors, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(doctorsPath, updatedJson);
            }
            void UpdateDoctorAvailabilityInHospitalFile(string email, string availabilityField, bool newValue)
{
    string exePath = AppContext.BaseDirectory;
    string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
    string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
    string hospitalPath = Path.Combine(jsonFolder, "hospital.json");

    if (!File.Exists(hospitalPath))
        return;

    string json = File.ReadAllText(hospitalPath);
    using JsonDocument doc = JsonDocument.Parse(json);
    JsonElement root = doc.RootElement;

    JsonElement departmentsElement = root.GetProperty("Departments");
    var updatedDepartments = new List<object>();

    foreach (JsonElement deptElement in departmentsElement.EnumerateArray())
    {
        string deptName = deptElement.GetProperty("Name").GetString();
        var updatedDoctors = new List<object>();

        foreach (JsonElement docElement in deptElement.GetProperty("Doctors").EnumerateArray())
        {
            string docEmail = docElement.GetProperty("Email").GetString();
            bool isTarget = docEmail == email;

            updatedDoctors.Add(new
            {
                Name = docElement.GetProperty("Name").GetString(),
                Surname = docElement.GetProperty("Surname").GetString(),
                Email = docEmail,
                Phone = docElement.GetProperty("Phone").GetString(),
                ExperienceYear = docElement.GetProperty("ExperienceYear").GetDouble(),
                Password = docElement.GetProperty("Password").GetString(),
                NineToEleven = availabilityField == "NineToEleven" && isTarget ? newValue : docElement.GetProperty("NineToEleven").GetBoolean(),
                TwelveToFourteen = availabilityField == "TwelveToFourteen" && isTarget ? newValue : docElement.GetProperty("TwelveToFourteen").GetBoolean(),
                FifteenToSeventeen = availabilityField == "FifteenToSeventeen" && isTarget ? newValue : docElement.GetProperty("FifteenToSeventeen").GetBoolean()
            });
        }

        updatedDepartments.Add(new
        {
            Name = deptName,
            Doctors = updatedDoctors
        });
    }

    var updatedHospital = new
    {
        Name = root.GetProperty("Name").GetString(),
        Departments = updatedDepartments
    };

    string updatedJson = JsonSerializer.Serialize(updatedHospital, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(hospitalPath, updatedJson);
}

            List<Doctor> ReadDoctors()
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
            List<User> ReadUsers()
            {
                try
                {
                    string exePath = AppContext.BaseDirectory;
                    string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
                    string jsonFolder = Path.Combine(projectRoot, "JsonFiles");
                    string usersPath = Path.Combine(jsonFolder, "users.json");

                    if (!File.Exists(usersPath))
                        return new List<User>();

                    string json = File.ReadAllText(usersPath);

                    if (string.IsNullOrWhiteSpace(json))
                        return new List<User>();

                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
                catch
                {
                    return new List<User>();
                }
            }
            List<ApplicantDoctor> ReadDoctorApplicants()
            {
                string exePath = AppContext.BaseDirectory;
                string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
                string jsonFolder = Path.Combine(projectRoot, "JsonFiles");

                string doctorsPath = Path.Combine(jsonFolder, "applicantDoctors.json");

                if (!File.Exists(doctorsPath))
                    return new List<ApplicantDoctor>();

                string json = File.ReadAllText(doctorsPath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<ApplicantDoctor>();

                return JsonSerializer.Deserialize<List<ApplicantDoctor>>(json) ?? new List<ApplicantDoctor>();
            }
            
            WriteToFileAllDoctors(hospital.AllDoctors);
            hospital.AllDoctors=ReadDoctors();
            static void ResetIfOneDayPassed()
            {
                string lastResetPath = "last_reset.txt";
                DateTime lastReset = DateTime.MinValue;

                if (File.Exists(lastResetPath))
                {
                    string text = File.ReadAllText(lastResetPath);
                    DateTime.TryParse(text, out lastReset);
                }

                if (DateTime.Now - lastReset >= TimeSpan.FromDays(1)) // 24 saat
                {
                    ForceResetDoctorAvailability();
                    File.WriteAllText(lastResetPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            static void ForceResetDoctorAvailability()
            {
                string exePath = AppContext.BaseDirectory;
                string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
                string jsonFolder = Path.Combine(projectRoot, "JsonFiles");

                if (!Directory.Exists(jsonFolder))
                    Directory.CreateDirectory(jsonFolder);

                
                string doctorsPath = Path.Combine(jsonFolder, "doctors.json");
                string json = File.ReadAllText(doctorsPath);
                var doctors = JsonSerializer.Deserialize<List<Doctor>>(json);

                foreach (var doctor in doctors)
                {
                    doctor.NineToEleven = true;
                    doctor.TwelveToFourteen = true;
                    doctor.FifteenToSeventeen = true;
                }

                string updatedJson = JsonSerializer.Serialize(doctors, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(doctorsPath, updatedJson);
            }    
            while (true)
            {
                RestartLogin: ;
                Console.Clear();
                string[] menuOptions =
{
    "|Admin sign in",
    "|User sign up",
    "|User sign in",
    "|Doctor sign in",
    "|Apply to become a doctor",
    "|Exit"
};

int selectedIndex = 0;

while (true)
{
    FirstPart: ;
    Console.Clear();
    Console.WriteLine("Welcome to MediNova Hospital");
    hospital = Hospital.ReadFromFileHospital() ?? new Hospital("MediNova Hospital");


    for (int i = 0; i < menuOptions.Length; i++)
    {
        if (i == selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> {menuOptions[i]}");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"  {menuOptions[i]}");
        }
    }

    ConsoleKey key = Console.ReadKey(true).Key;

    switch (key)
    {
        case ConsoleKey.UpArrow:
            selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
            break;
        case ConsoleKey.DownArrow:
            selectedIndex = (selectedIndex + 1) % menuOptions.Length;
            break;
        case ConsoleKey.Enter:
            Console.Clear();
            switch (selectedIndex)
            {
                case 0:
                {
                    ResetIfOneDayPassed();

                    Admin admin = new Admin("admin", "admin");
                    
                    Console.Write("Enter username: ");
                    string usernameAdmin = Console.ReadLine();
                    Console.Write("Enter password: ");
                    string passwordAdmin = Console.ReadLine();
                    if (usernameAdmin == admin.Name && passwordAdmin == admin.Password)
                    {
                        int selectedIndex1 = 0;
                        string[] options =
                            { "|Show All Applicants", "|View Departments", "|View Users", "|View Doctors", "|Back" };

                        while (true)
                        {
                            AdminPanael: ;
                            ConsoleKey key1;

                            do
                            {
                                Console.Clear();
                                Console.WriteLine("Welcome Admin!");
                                for (int i = 0; i < options.Length; i++)
                                {
                                    if (i == selectedIndex1)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                        Console.WriteLine("> " + options[i]);
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        Console.WriteLine("   " + options[i]);
                                    }
                                }

                                key1 = Console.ReadKey(true).Key;

                                if (key1 == ConsoleKey.UpArrow)
                                {
                                    selectedIndex1 = (selectedIndex1 == 0) ? options.Length - 1 : selectedIndex1 - 1;
                                }
                                else if (key1 == ConsoleKey.DownArrow)
                                {
                                    selectedIndex1 = (selectedIndex1 == options.Length - 1) ? 0 : selectedIndex1 + 1;
                                }

                            } while (key1 != ConsoleKey.Enter);

                            Console.Clear();
                            Console.WriteLine("You selected: " + options[selectedIndex1] + "\n");

                            switch (selectedIndex1)
                            {
                                case 0:
                                    admin.ViewApplicants(ReadDoctorApplicants(),hospital.AllDoctors,ref hospital);
                                    hospital.WriteToFileHospital();
                                    Log.Information("Admin viewed applicants");
                                    break;
                                case 1:
                                    admin.View(hospital.Departments);
                                    Log.Information("Admin viewed departments");

                                    break;
                                case 2:
                                    admin.View(ReadUsers());
                                    Log.Information("Admin viewed users");

                                    break;
                                case 3:
                                    admin.View(ReadDoctors());
                                    Log.Information("Admin viewed doctors");
                                    break;
                                case 4:
                                    Console.WriteLine("Exiting admin panel...");
                                    Log.Information("Admin exited admin panel");
                                    Thread.Sleep(1000); 
                                    goto FirstPart;
                                    break;
                            }

                            Console.WriteLine("\nPress any key to go back to menu...");
                            Console.ReadKey();
                        }
                    }

                    break;
                }
                case 1:
                    SignUp: ;
                    Console.Clear();
                    Console.Write("Enter your name: ");
                    string name = Console.ReadLine();
                    Console.Write("Enter your surname: ");
                    string surname = Console.ReadLine();
                    Console.Write("Enter your email: ");
                    string email = Console.ReadLine();
                    Console.Write("Enter your phone number: ");
                    string phone = Console.ReadLine();
                    Console.WriteLine("Set your password: (Must be at least 8 symbols,at least 1 digit,1 uppercase,1 lowercase)");
                    string password = Console.ReadLine();
                    try
                    {
                        User newUser = new User(name, surname, email, phone,password);
                        Console.WriteLine("Account successfully created.Please sign in.");
                        Log.Information($"Account successfully created - {email}"); 
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        List<User> users = new();
                        WriteToFileUser(users,newUser);
                        
                        goto FirstPart;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Log.Warning($"{name} {surname} {ex.Message}");
                        Console.WriteLine("Try again");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        continue;
                    }
                case 2: 
                    List<User> users2 = ReadUsers();

                    int SearchUser(string email)
                    {
                        for (int i = 0; i < users2.Count; i++)
                        {
                            if(users2[i].Email==email)
                                return i;
                        }
                        return -1;
                    }
                    
                    Console.Write("Enter your email: ");
                    string email2 = Console.ReadLine();
                    
                    int index = SearchUser(email2);
                    if (index == -1)
                    {
                        Console.WriteLine("User not found");
                        Log.Information($"User not found - {email2}");
                        Console.WriteLine("Press any key to go sign up.");
                        Console.ReadKey();
                        goto SignUp;
                        
                    }
                    else
                    {
                        Console.Write("Enter your password: ");
                        string password2 = Console.ReadLine();
                        if (password2 == users2[index].Password)
                        {
                            string[] departments =
                            {
                                hospital.Departments[0].Name, hospital.Departments[1].Name, hospital.Departments[2].Name
                            };
                            int selectedDepartmentIndex = 0;
                            try
                            {
                                Log.Information($"{users2[index].Name} {users2[index].Surname} have entered {departments[index]}");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.Message);
                            }
                            while (true)
                            {
                                BackToDepartmentSelection: ;

                                Console.Clear();
                                Console.WriteLine("Choose a department:\n");
                                for (int i = 0; i < departments.Length; i++)
                                {
                                    if (i == selectedDepartmentIndex)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                        Console.WriteLine($"> {departments[i]}");
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"  {departments[i]}");
                                    }
                                }

                                int backOptionIndex = departments.Length;
                                if (selectedDepartmentIndex == backOptionIndex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("> Back");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine("  Back");
                                }

                                ConsoleKey deptKey = Console.ReadKey(true).Key;
                                switch (deptKey)
                                {
                                    case ConsoleKey.UpArrow:
                                        selectedDepartmentIndex =
                                            (selectedDepartmentIndex - 1 + backOptionIndex + 1) % (backOptionIndex + 1);
                                        break;
                                    case ConsoleKey.DownArrow:
                                        selectedDepartmentIndex = (selectedDepartmentIndex + 1) % (backOptionIndex + 1);
                                        break;
                                    case ConsoleKey.Enter:
                                        if (selectedDepartmentIndex == backOptionIndex)
                                        {
                                            goto RestartLogin;
                                        }
                                        else
                                        {
                                            string deptName = hospital.Departments[selectedDepartmentIndex].Name;
                                            var doctors = hospital.GetDoctorsByDepartment(deptName);
                                            if (doctors.Count == 0)
                                            {
                                                Console.Clear();
                                                Console.WriteLine("No doctors in this department.");
                                                Console.WriteLine("Press any key to go back.");
                                                Console.ReadKey(true);
                                                break;
                                            }

                                            int selectedDoctorIndex = 0;
                                            while (true)
                                            {
                                                BackToDoctorSelection: ;
                                                Console.Clear();
                                                Console.WriteLine(
                                                    $"Department: {departments[selectedDepartmentIndex]}");
                                                Console.WriteLine("Choose a doctor:\n");
                                                for (int i = 0; i < doctors.Count; i++)
                                                {
                                                    if (i == selectedDoctorIndex)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                                        Console.WriteLine($"> |{doctors[i].Name} {doctors[i].Surname}");
                                                        Console.ResetColor();
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"  |{doctors[i].Name} {doctors[i].Surname}");
                                                    }
                                                }

                                                int doctorBackIndex = doctors.Count;
                                                if (selectedDoctorIndex == doctorBackIndex)
                                                {
                                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                                    Console.WriteLine("> Back");
                                                    Console.ResetColor();
                                                }
                                                else
                                                {
                                                    Console.WriteLine("  Back");
                                                }

                                                ConsoleKey doctorKey = Console.ReadKey(true).Key;
                                                switch (doctorKey)
                                                {
                                                    case ConsoleKey.UpArrow:
                                                        selectedDoctorIndex =
                                                            (selectedDoctorIndex - 1 + doctorBackIndex + 1) %
                                                            (doctorBackIndex + 1);
                                                        break;
                                                    case ConsoleKey.DownArrow:
                                                        selectedDoctorIndex = (selectedDoctorIndex + 1) %
                                                                              (doctorBackIndex + 1);
                                                        break;
                                                    case ConsoleKey.Enter:
                                                        if (selectedDoctorIndex == doctorBackIndex)
                                                        {
                                                            goto BackToDepartmentSelection;
                                                        }

                                                        int selectedHourIndex = 0;
                                                        hospital.AllDoctors = ReadDoctors();

                                                        while (true)
                                                        {
                                                            Console.Clear();
                                                            Console.WriteLine(
                                                                $"Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}");
                                                            Console.WriteLine("Choose a time slot:\n");

                                                            bool[] available =
                                                            {
                                                                doctors[selectedDoctorIndex].NineToEleven,
                                                                doctors[selectedDoctorIndex].TwelveToFourteen,
                                                                doctors[selectedDoctorIndex].FifteenToSeventeen,
                                                            };
                                                            string[] hours =
                                                            {
                                                                $"09:00 - 11:00 - {(available[0] ? "Free" : "Reserved")}",
                                                                $"12:00 - 14:00 - {(available[1] ? "Free" : "Reserved")}",
                                                                $"15:00 - 17:00 - {(available[2] ? "Free" : "Reserved")}",
                                                                "Back"
                                                            };
                                                            //bunu yazmayanda asagidaki you have booked sozunde free/reserved sozu de gorunurdu ona gore ayri saxladim
                                                            string[] hourRanges =
                                                            {
                                                                "09:00 - 11:00", "12:00 - 14:00", "15:00 - 17:00"
                                                            };


                                                            while (selectedHourIndex < 3 &&
                                                                   !available[selectedHourIndex])
                                                            {
                                                                selectedHourIndex = (selectedHourIndex + 1) %
                                                                    hours.Length;
                                                            }

                                                            for (int i = 0; i < hours.Length; i++)
                                                            {
                                                                if (i == selectedHourIndex)
                                                                {
                                                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                                                    Console.WriteLine($"> {hours[i]}");
                                                                    Console.ResetColor();
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine($"  {hours[i]}");
                                                                }
                                                            }

                                                            ConsoleKey hourKey = Console.ReadKey(true).Key;
                                                            switch (hourKey)
                                                            {
                                                                case ConsoleKey.UpArrow:
                                                                    do
                                                                    {
                                                                        selectedHourIndex =
                                                                            (selectedHourIndex - 1 + hours.Length) %
                                                                            hours.Length;
                                                                    } while (selectedHourIndex < 3 &&
                                                                             !available[selectedHourIndex]);

                                                                    break;

                                                                case ConsoleKey.DownArrow:
                                                                    do
                                                                    {
                                                                        selectedHourIndex =
                                                                            (selectedHourIndex + 1) % hours.Length;
                                                                    } while (selectedHourIndex < 3 &&
                                                                             !available[selectedHourIndex]);

                                                                    break;

                                                                case ConsoleKey.Enter:
                                                                    if (selectedHourIndex == 3)
                                                                    {
                                                                        goto BackToDoctorSelection;
                                                                    }
                                                                    else if (available[selectedHourIndex])
                                                                    {
                                                                        if (selectedHourIndex == 0)
                                                                        {
                                                                            UpdateDoctorAvailability(
                                                                                doctors[selectedDoctorIndex].Email,
                                                                                "NineToEleven", false);
                                                                            UpdateDoctorAvailabilityInHospitalFile(
                                                                                doctors[selectedDoctorIndex].Email,
                                                                                "NineToEleven", false);
                                                                        }
                                                                        else if (selectedHourIndex == 1)
                                                                        {
                                                                            UpdateDoctorAvailability(
                                                                                doctors[selectedDoctorIndex].Email,
                                                                                "TwelveToFourteen", false);
                                                                            UpdateDoctorAvailabilityInHospitalFile(
                                                                                doctors[selectedDoctorIndex].Email,
                                                                                "TwelveToFourteen", false);
                                                                        }
                                                                        else if (selectedHourIndex == 2)
                                                                        {
                                                                            UpdateDoctorAvailability(
                                                                                doctors[selectedDoctorIndex].Email,
                                                                                "FifteenToSeventeen", false);
                                                                            UpdateDoctorAvailabilityInHospitalFile(
                                                                                doctors[selectedDoctorIndex].Email,
                                                                                "FifteenToSeventeen", false);
                                                                        }

                                                                        Console.Clear();
                                                                        
                                                                        
                                                                        Console.WriteLine(
                                                                            $"Thank you {users2[index].Name} {users2[index].Surname}. You have booked {hourRanges[selectedHourIndex]} with Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}\nBill is preparing");
                                                                        Log.Information($"{users2[index].Name} {users2[index].Surname} booked an appointment with Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}");
                                                                        if (index != -1)
                                                                        {
                                                                            string content2 = $@"
╔════════════════════════════════════════════╗
║      🩺 UPCOMING APPOINTMENT NOTICE       ║
╚════════════════════════════════════════════╝

Notice No  : #DOC-{DateTime.Now:yyyyMMddHHmm}
Issued     : {DateTime.Now:dd/MM/yyyy HH:mm}

--- DOCTOR ---
Name       : Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}
Email      : {doctors[selectedDoctorIndex].Email}
Phone      : {doctors[selectedDoctorIndex].Phone}
Experience : {doctors[selectedDoctorIndex].ExperienceYear} yrs

--- PATIENT ---
Name       : {users2[index].Name} {users2[index].Surname}
Email      : {users2[index].Email}
Phone      : {users2[index].Phone}

--- APPOINTMENT ---
Department : {departments[selectedDepartmentIndex]}
Time       : {hourRanges[selectedHourIndex]}
Date       : {DateTime.Now:dd/MM/yyyy}

Be ready at the specified time.
";
                                                                            string content = $@"
🏥 HOSPITAL APPOINTMENT CONFIRMATION

Bill No     : #HOSP-{DateTime.Now:yyyyMMddHHmm}
Issued      : {DateTime.Now:dd/MM/yyyy HH:mm}

--- PATIENT ---
Name        : {users2[index].Name} {users2[index].Surname}
Email       : {users2[index].Email}
Phone       : {users2[index].Phone}

--- APPOINTMENT ---
Department  : {departments[selectedDepartmentIndex]}
Doctor      : Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}
Time        : {hourRanges[selectedHourIndex]}
Date        : {DateTime.Now:dd/MM/yyyy}

Please arrive 10 minutes before your scheduled time.
For rescheduling, contact us 24 hours in advance.

📞 +994 12 345 67 89
✉ MediNovaHospital@hospital.com

This is an automated message. Please do not reply.
";
                                                                            try
                                                                            {
                                                                                    
                                                                                var client =
                                                                                    new SmtpClient("smtp.gmail.com",
                                                                                        587)
                                                                                    {
                                                                                        Credentials =
                                                                                            new NetworkCredential(
                                                                                                "burhanorucov@gmail.com",
                                                                                                "ndfq jrvo fojm pndc"),
                                                                                        EnableSsl = true
                                                                                    };

                                                                                client.Send(
                                                                                    "burhanorucov@gmail.com",
                                                                                    email2, "Bill from hospital.",
                                                                                    content);
                                                                                client.Send(
                                                                                    "burhanorucov@gmail.com",doctors[selectedDoctorIndex].Email, "Bill from hospital.",
                                                                                    content2);
                                                                                Console.WriteLine("The bill has sent to your email.");
                                                                                Log.Information($"Appointment bill has sent to {email2} and {doctors[selectedDoctorIndex].Email}");

                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                Console.WriteLine(ex.Message);
                                                                                Log.Error(ex.Message);
                                                                            }

                                                                            Console.WriteLine(
                                                                                "Press any key to return to login...");
                                                                            Console.ReadKey(true);
                                                                            goto RestartLogin;
                                                                        }

                                                                    }

                                                                    break;
                                                            }

                                                                
                                                        }
                                                }
                                            }

                                        }
                                }

                            }

                        }
                        else
                        {
                            Console.WriteLine("Invalid password");
                            Console.WriteLine("Press any key to return to login...");
                            Log.Information($"Invalid password {email2}");
                            Console.ReadKey();
                        }

                        break;
                    }
                case 3:
                    List<Doctor> doctors2 = ReadDoctors();
                    int SearchDoctor(string email)
                    {
                        for (int i = 0; i < doctors2.Count; i++)
                        {
                            if (doctors2[i].Email == email)
                                return i;
                        }
                        return -1;
                    }

                    Console.Write("Enter your email: ");
                    string email3 = Console.ReadLine();
                    int index2 = SearchDoctor(email3);

                    if (index2 == -1)
                    {
                        Console.WriteLine("Doctor not found");
                        Log.Information($"Doctor not found - {email3}");
                        Console.WriteLine("Press any key to go sign up.");
                        Console.ReadKey();
                        break; // goto əvəzinə break istifadə edək
                    }

                    Console.Write("Enter your password: ");
                    string password3 = Console.ReadLine();

                    if (password3 == doctors2[index2].Password)
                    {
                        string[] choices =
                        {
                            "View your coming appointments",
                            "View your informations",
                            "Back"
                        };

                        int selectedIndex2 = 0;

                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine($"Welcome Dr. {doctors2[index2].Name} {doctors2[index2].Surname}\n");
                            Console.WriteLine("Choose an option:\n");

                            for (int i = 0; i < choices.Length; i++)
                            {
                                if (i == selectedIndex2)
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine($"> {choices[i]}");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine($"  {choices[i]}");
                                }
                            }

                            var key2 = Console.ReadKey(true).Key;

                            if (key2 == ConsoleKey.UpArrow)
                            {
                                selectedIndex2 = (selectedIndex2 - 1 + choices.Length) % choices.Length;
                            }
                            else if (key2 == ConsoleKey.DownArrow)
                            {
                                selectedIndex2 = (selectedIndex2 + 1) % choices.Length;
                            }
                            else if (key2 == ConsoleKey.Enter)
                            {
                                switch (selectedIndex2)
                                {
                                    case 0:
                                        Console.Clear();
                                        Console.WriteLine("Coming appointments:");
                                        
                                        List<Doctor> doctors3 = ReadDoctors();
                                        int index3 = SearchDoctor(email3);
                                        if(!doctors3[index3].NineToEleven)
                                            Console.WriteLine("Appointment 09:00 - 11:00");
                                        if(!doctors3[index3].TwelveToFourteen)
                                            Console.WriteLine("Appointment 12:00 - 14:00");
                                        if(!doctors3[index3].FifteenToSeventeen)
                                            Console.WriteLine("Appointment 15:00 - 17:00");
                                        Console.WriteLine("Press any key to go back");
                                        Console.ReadKey();
                                        break;
                                    case 1:
                                        Console.Clear();
                                        Console.WriteLine("Your Information:");
                                        Console.WriteLine($"Name: {doctors2[index2].Name}");
                                        Console.WriteLine($"Surname: {doctors2[index2].Surname}");
                                        Console.WriteLine($"Email: {doctors2[index2].Email}");
                                        Console.WriteLine($"Phone: {doctors2[index2].Phone}");
                                        Console.WriteLine($"Experience: {doctors2[index2].ExperienceYear} years");
                                        Console.WriteLine("Press any key to go back");
                                        Console.ReadKey();
                                        break;
                                    case 2:
                                        Log.Information($"{doctors2[index2].Name} logged out.");
                                        goto MainPart;
                                        
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect password.");
                        Log.Warning($"Incorrect password attempt by {email3}");
                        Console.WriteLine("Press any key to try again.");
                        Console.ReadKey();
                    }

                    break;

                case 4:
                    //doctor application for being a doc
                    Console.WriteLine("Welcome! Please fill the gaps.");
                    Console.Write("Enter your name: ");
                    string nameDoctorApplicant = Console.ReadLine();
                    Console.Write("Enter your surname: ");
                    string surnameDoctorApplicant = Console.ReadLine();
                    Console.Write("Enter your email: ");
                    string emailDoctorApplicant = Console.ReadLine();
                    Console.Write("Enter your phone number: ");
                    string phoneDoctorApplicant = Console.ReadLine();
                    Console.Write("Enter your experience year: ");
                    double experienceYearDoctorApplicant = double.Parse(Console.ReadLine());//bele edirikki doubleden sonra string yzanda avtomatik \n goturmesin
                    
                    Console.WriteLine("Set your password: (Must be at least 8 symbols,at least 1 digit,1 uppercase,1 lowercase)");
                    string passwordDoctorApplicant = Console.ReadLine();
                    Console.Write("Which university you had your education: ");
                    string universityDoctorApplicant = Console.ReadLine();
                    Console.Write("Choose a department: ");
                    string departmentDoctorApplicant = Console.ReadLine();
                    bool flag = false;
                    foreach (var hospitalDepartment in hospital.Departments)
                    {
                        if (hospitalDepartment.Name == departmentDoctorApplicant)
                        {
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        Console.WriteLine("You want to apply? (yes/no)");
                        string answer = Console.ReadLine();
                        try
                        {
                            if (answer == "yes")
                            {

                                ApplicantDoctor newApplicantDoctor = new ApplicantDoctor(nameDoctorApplicant,
                                    surnameDoctorApplicant, emailDoctorApplicant, phoneDoctorApplicant,
                                    experienceYearDoctorApplicant, passwordDoctorApplicant, universityDoctorApplicant);
                                newApplicantDoctor.Department=departmentDoctorApplicant;
                                WriteToFileDoctorApplicant(newApplicantDoctor);
                                Log.Information($"{nameDoctorApplicant} applied to be a doctor");
                                Console.WriteLine("Succesfully applied to doctor");
                                Thread.Sleep(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Log.Warning($"{nameDoctorApplicant} {surnameDoctorApplicant} {ex.Message}");
                            Console.WriteLine("Try again");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("There is no such department");
                    }

                    break;
                case 5:
                    Console.WriteLine("Exiting...");
                    Thread.Sleep(1000);
                    return;
            }

            Log.CloseAndFlush();
            break;
    }
}


                
            }
        }
        
    }
    
}
