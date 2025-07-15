// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
Console.WriteLine("Ini Ghinna");

void FooBar(int num)
{
    for (int i = 1; i <= num; i++)
    {
        // if (i % 3 == 0 && i % 5 == 0)
        if (i % 15 == 0)
            Console.Write("foobar");
        else if (i % 3 == 0)
            Console.Write("foo");
        else if (i % 5 == 0)
            Console.Write("bar");
        else
            Console.Write(i);
        if (i != num)
            Console.Write(", ");
    }
}

// FooBar(35);


// Panda p1 = new Panda("Pan Dee");
// Console.WriteLine(p1.Name);          // Pan Dee
// Console.WriteLine(Panda.Population); // 1
// Panda p2 = new Panda("Pan Dah");
// Console.WriteLine(p2.Name);          // Pan Dah
// Console.WriteLine(Panda.Population); // 2
// Panda p3 = new Panda("Pan Da");
// Console.WriteLine(p3.Name);          // Pan Da
// Console.WriteLine(Panda.Population); // 3
// Panda p4 = new Panda("Pan Daa");
// Panda p5 = new Panda("Pan Daaa");
// Console.WriteLine(p4.Name);          // Pan Daa
// Console.WriteLine(p5.Name);          // Pan Daaa
// Console.WriteLine(Panda.Population); // 5
// Panda p6 = new Panda("Pan Daaaa");
// Panda p7 = p6;
// Console.WriteLine(p6.Name);          // Pan Daaaa
// Console.WriteLine(p7.Name);          // Pan Daaaa 

// p6.Name = "Pan Daaaaa";
// Console.WriteLine(p6.Name);          // Pan Daaaaa
// Console.WriteLine(p7.Name);          // Pan Daaaaa kalau class - Pan Daaaa kalau struct

// public class Panda // ini reference type
// // public struct Panda // ini value type
// {
//     public string Name;             // Instance field
//     public static int Population;   // Static field

//     public Panda(string n)          // Constructor
//     {
//         Name = n;                   // Assign the instance field
//         Population = Population + 1; // Increment the static Population field
//     }
// }


// int a = int.MinValue;
// Console.WriteLine(a);
// a--;
// Console.WriteLine(a);

// int x = int.MaxValue;
// // int y = unchecked(x + 1); // No error, even if project is checked == int y = x+1
// int y = checked(x + 1); // generate overflow error
// Console.WriteLine(x);
// Console.WriteLine(y);
// unchecked { int z = x + 1; };
// Console.WriteLine(z);

// string raw = """"The """ sequence denotes raw string literals.""""; // Contains """ within the string

// Console.WriteLine(raw);

// int[] a = new int[1000];
// Console.WriteLine(a[123]);

// int[] b = [1, 2, 3, 4, 5, 6, 8, 7];
// Console.WriteLine(b[1]);

// Point[] a = new Point[1000];
// for (int i = 0; i < a.Length; i++) // Iterate from 0 to 999
//     a[i] = new Point();


// Console.WriteLine(a[123].X);

// public class Point { public int X, Y; }

Lingkaran l = new Lingkaran();
l.jariJari = 10;
Console.WriteLine(l.jariJari);
Console.WriteLine(l.HitungLuas());
public abstract class Bentuk
{
    public abstract double HitungLuas();
}

public class Lingkaran : Bentuk
{
    public double jariJari;
    public override double HitungLuas()
    {
        return Math.PI * jariJari * jariJari;
    }
}