namespace Hospital_Project;

public class Department
{
    public string Name { get; set; }
    public List<Doctor> Doctors = new List<Doctor>(){};
    
    public Department(string name, List<Doctor> doctors)
    {
        Name = name;
        Doctors = doctors;
    }
}