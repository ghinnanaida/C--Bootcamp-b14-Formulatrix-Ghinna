using System;
using System.Numerics;

public class Employee
{
    private string _name;
    private string _position;
    private int _age;
    private readonly DateTime _hireDate;
    public readonly Guid Id;
    public static int totalEmployees = 0;
    public static int totalStaff = 0;
    public static int totalManager = 0;
    private static int _totalWorkHours = 0;
    public static int TotalWorkHours => _totalWorkHours;

    public Employee(string name, string position, int age)
    {
        this._name = name ?? throw new ArgumentNullException(nameof(name));
        this._age = age;
        this._position = position;
        this._hireDate = DateTime.Now;
        totalEmployees++;
        Id = Guid.NewGuid();
        if (position == "Staff")
            totalStaff++;
        else
            totalManager++;
        Console.WriteLine(@$"New employee hired
            Name : {name}
            Id : {Id}
            Total Employee : {totalEmployees}");
    }

    public string Name
    {
        get { return _name; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            _name = value;
        }
    }

    public int Age
    {
        get { return _age; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Age must be > 0");
            _age = value;
        }
    }

    public string Position => _position;
    public DateTime HireDate { get; }
    public int DayOfService
    {
        get
        {
            var days = (DateTime.Now.Date - _hireDate.Date).Days;
            return days == 0 ? 1 : days;
        }
    }
    public void CelebrateBirthday()
    {
        _age++;
        Console.WriteLine($"Happy Birthday {_name}! Now {_age} years old.");
    }
    public void Promotion(string position)
    {
        _position = position;
        if (position == "Manager")
        {
            totalManager++;
            totalStaff--;
        }

        Console.WriteLine($"Congratulation {_name} promoted as {_position}!");
    }
    public void Work()
    {
        int hoursWorked = 8;
        _totalWorkHours += hoursWorked;
        Console.WriteLine($"{_name} worked {hoursWorked} hours today. Total work hours : {hoursWorked * DayOfService}");
    }

    public static void GetCompanyStats()
    {
        Console.WriteLine(@$"Company Stats:
        Total Employees : {totalEmployees}
        Total Staff : {totalStaff}
        Total Manager : {totalManager}
        Total Work Hours : {_totalWorkHours}");
    }

    public override string ToString()
    {
        return $"{_name} (Age: {_age.ToString()}, Hired: {_hireDate.ToString("yyyy-MM-dd")})";
    }
}