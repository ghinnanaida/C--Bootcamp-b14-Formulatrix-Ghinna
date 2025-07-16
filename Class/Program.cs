// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        EmployeeMain();
    }

    public static void EmployeeMain()
    {
        var employee1 = new Employee("Ghinna Naida", "Staff", 24);
        var employee2 = new Employee("Putri A", "Staff", 25);
        var manager = new Employee("Nana", "Manager", 26);

        Console.WriteLine(@$"New Manager
            Name :{manager.Name}
            Total manager : {Employee.totalManager}");
        Console.WriteLine(@$"New Staff
            1. {employee1.Name}
            2. {employee2.Name}
            Total Staff : {Employee.totalStaff}");

        employee1.Work();
        employee2.Work();
        manager.Work();

        employee1.CelebrateBirthday();
        Console.WriteLine($"{employee1.Name} is {employee1.Age} years old.");

        employee2.Promotion("Manager");
        Employee.GetCompanyStats();

        Console.WriteLine(employee1);
        Console.WriteLine(employee2);
        Console.WriteLine(manager);

    }
}
