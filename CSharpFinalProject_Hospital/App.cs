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

            string exePath = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
            string basePath = Path.Combine(projectRoot, "Logs");
            Directory.CreateDirectory(Path.Combine(basePath, "Warnings"));
            Directory.CreateDirectory(Path.Combine(basePath, "Errors"));
            Directory.CreateDirectory(Path.Combine(basePath, "Information"));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(basePath, "Warnings", "warning-.txt"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .WriteTo.File(Path.Combine(basePath, "Errors", "error-.txt"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                .WriteTo.File(Path.Combine(basePath, "Information", "information-.txt"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();

            Hospital hospital = new Hospital();

            while (true)
            {
                RestartLogin: ;
                Console.Clear();
                string[] menuOptions =
{
    "Admin sign in",
    "Sign up as user",
    "User sign in",
    "Sign up as doctor",
    "Doctor sign in",
    "Exit"
};

int selectedIndex = 0;

while (true)
{
    FirstPart: ;
    Console.Clear();
    Console.WriteLine("Welcome to the Hospital\n");
    

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
                    var users = new List<User>();
                    using var fs = new FileStream("users.json", FileMode.Open);
                    users = JsonSerializer.Deserialize<List<User>>(fs);
                    foreach (var user in users)
                    {
                        Console.WriteLine(user);
                    }

                    Console.ReadKey();
                    break;
                }
                case 1:
                    SignUp: ;
                    Console.Clear();
                    Console.WriteLine("Enter your name:");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter your surname:");
                    string surname = Console.ReadLine();
                    Console.WriteLine("Enter your email:");
                    string email = Console.ReadLine();
                    Console.WriteLine("Enter your phone number:");
                    string phone = Console.ReadLine();
                    Console.WriteLine("Set your password: (Must be at least 8 symbols,at least 1 digit,1 uppercase,1 lowercase)");
                    string password = Console.ReadLine();
                    try
                    {
                        User newUser = new User(name, surname, email, phone,password);
                        Console.WriteLine("Account successfully created.Please sign in.");
                        Log.Information($"Account successfully created - {email}");
                        

                        List<User> users = new();

                        if (File.Exists("users.json"))
                        {
                            string oldJson = File.ReadAllText("users.json");
                            if (!string.IsNullOrWhiteSpace(oldJson))
                                users = JsonSerializer.Deserialize<List<User>>(oldJson) ?? new();
                        }

                        users.Add(newUser);
                        string newJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText("users.json", newJson);
                        Thread.Sleep(1500);
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
                    static List<User> ReadUsers()
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
                    
                    Console.WriteLine("Enter your email:");
                    string email2 = Console.ReadLine();
                    
                    int index = SearchUser(email2);
                    if (index == -1)
                    {
                        Console.WriteLine("User not found");
                        Log.Error($"User not found - {email2}");
                        Console.WriteLine("Press any key to go sign up.");
                        Console.ReadKey();
                        goto SignUp;
                        
                    }
                    else
                    {
                        Console.WriteLine("Enter your password:");
                        string password2 = Console.ReadLine();
                        if (password2 == users2[index].Password)
                        {
                            string[] departments =
                            {
                                hospital.Departments[0].Name, hospital.Departments[1].Name, hospital.Departments[2].Name
                            };
                            int selectedDepartmentIndex = 0;
                            Log.Information($"{users2[index].Name} {users2[index].Surname} have entered {departments[index]}");
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
                                            var doctors = hospital.Departments[selectedDepartmentIndex].Doctors;
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
                                                Console.Clear();
                                                Console.WriteLine(
                                                    $"Department: {departments[selectedDepartmentIndex]}");
                                                Console.WriteLine("Choose a doctor:\n");
                                                for (int i = 0; i < doctors.Count; i++)
                                                {
                                                    if (i == selectedDoctorIndex)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                                        Console.WriteLine($"> {doctors[i].Name} {doctors[i].Surname}");
                                                        Console.ResetColor();
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"  {doctors[i].Name} {doctors[i].Surname}");
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
                                                        else
                                                        {
                                                            int selectedHourIndex = 0;

                                                            while (true)
                                                            {
                                                                Console.Clear();
                                                                Console.WriteLine(
                                                                    $"Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}");
                                                                Console.WriteLine("Choose a time slot:\n");

                                                                bool[] available =
                                                                {
                                                                    doctors[selectedDoctorIndex].NineToEleven,
                                                                    doctors[selectedDoctorIndex].TwelveToThirteen,
                                                                    doctors[selectedDoctorIndex].FifteenToSeventeen,
                                                                };

                                                                string[] hours =
                                                                {
                                                                    $"09:00 - 11:00 - {(available[0] ? "Free" : "Reserved")}",
                                                                    $"12:00 - 14:00 - {(available[1] ? "Free" : "Reserved")}",
                                                                    $"15:00 - 17:00 - {(available[2] ? "Free" : "Reserved")}",
                                                                    "Back"
                                                                };
                                                                //bunu yazmayanda asagidaki you have booked sozunde free sozu de gorunurdu ona gore ayri saxladim
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
                                                                                doctors[selectedDoctorIndex]
                                                                                    .NineToEleven = false;
                                                                            else if (selectedHourIndex == 1)
                                                                                doctors[selectedDoctorIndex]
                                                                                    .TwelveToThirteen = false;
                                                                            else if (selectedHourIndex == 2)
                                                                                doctors[selectedDoctorIndex]
                                                                                    .FifteenToSeventeen = false;

                                                                            Console.Clear();

                                                                            Console.WriteLine(
                                                                                $"Thank you {users2[index].Name} {users2[index].Surname}. You have booked {hourRanges[selectedHourIndex]} with Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}");
                                                                            Log.Information($"{users2[index].Name} {users2[index].Surname} booked an appointment with Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}");
                                                                            if (index != -1)
                                                                            {
                                                                                string content2 = $@"
╔════════════════════════════════════════════════════════╗
║           🩺 UPCOMING PATIENT APPOINTMENT NOTICE       ║
╚════════════════════════════════════════════════════════╝

Notice No     : #DOC-{DateTime.Now:yyyyMMddHHmm}
Date Issued   : {DateTime.Now:dd/MM/yyyy HH:mm}

╔══════════════════════════════════════╗
║              DOCTOR INFO            ║
╚══════════════════════════════════════╝
Name          : Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}
Email         : {doctors[selectedDoctorIndex].Email}
Phone         : {doctors[selectedDoctorIndex].Phone}
Experience    : {doctors[selectedDoctorIndex].ExperienceYear} years

╔══════════════════════════════════════╗
║             PATIENT INFO            ║
╚══════════════════════════════════════╝
Name          : {users2[index].Name} {users2[index].Surname}
Email         : {users2[index].Email}
Phone         : {users2[index].Phone}

╔══════════════════════════════════════╗
║         APPOINTMENT DETAILS         ║
╚══════════════════════════════════════╝
Department     : {departments[selectedDepartmentIndex]}
Time Slot      : {hourRanges[selectedHourIndex]}
Date           : {DateTime.Now:dd/MM/yyyy}

════════════════════════════════════════════════════════
Please be prepared at the specified time.
════════════════════════════════════════════════════════
";


                                                                                string content = $@"
╔════════════════════════════════════════════════════════╗
║            🏥 HOSPITAL APPOINTMENT CONFIRMATION        ║
╚════════════════════════════════════════════════════════╝

Bill No       : #HOSP-{DateTime.Now:yyyyMMddHHmm}
Date Issued   : {DateTime.Now:dd/MM/yyyy HH:mm}

╔══════════════════════════════════════╗
║              PATIENT INFO           ║
╚══════════════════════════════════════╝
Name          : {users2[index].Name} {users2[index].Surname}
Email         : {users2[index].Email}
Phone         : {users2[index].Phone}

╔══════════════════════════════════════╗
║          APPOINTMENT DETAILS         ║
╚══════════════════════════════════════╝
Department     : {departments[selectedDepartmentIndex]}
Doctor         : Dr. {doctors[selectedDoctorIndex].Name} {doctors[selectedDoctorIndex].Surname}
Time Slot      : {hourRanges[selectedHourIndex]}
Date           : {DateTime.Now:dd/MM/yyyy}

╔══════════════════════════════════════╗
║               NOTICE                 ║
╚══════════════════════════════════════╝
Please arrive 10 minutes before your scheduled time.
For rescheduling, contact us 24 hours in advance.

📞 Support: +994 12 345 67 89
✉ Email  : support@hospital.com

════════════════════════════════════════════════════════
This is an automated message. Please do not reply.
════════════════════════════════════════════════════════
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
                                                                                        "burhanorucov@gmail.com",
                                                                                        doctors[selectedDoctorIndex].Email, "Bill from hospital.",
                                                                                        content2);
                                                                                    Console.WriteLine("The bill has sent to your email.");
                                                                                    Log.Information($"Appointment bill has sent to {email2} and {doctors[selectedDoctorIndex].Email}");

                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    Console.WriteLine("Error: " +
                                                                                        ex.Message);
                                                                                    Log.Error(ex.Message);
                                                                                }

                                                                                Console.WriteLine(
                                                                                    "Press any key to return to login...");
                                                                                Console.ReadKey(true);
                                                                                goto RestartLogin;
                                                                            }

                                                                            break;
                                                                        }

                                                                        break;
                                                                }

                                                                BackToDoctorSelection: ;
                                                            }

                                                            break;
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
                            Console.ReadKey();
                        }

                        break;
                    }


                case 3:
                    Console.WriteLine("Həkim Qeydiyyatı bölməsi hələ yazılmayıb.");
                    break;
                case 4:
                    Console.WriteLine("Həkim Girişi bölməsi hələ yazılmayıb.");
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
