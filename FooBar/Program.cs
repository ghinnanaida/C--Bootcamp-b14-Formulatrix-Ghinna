using System;
using System.Text;

class Program
{

    static void Main()
    {
        FooBarJazzLooping1();
        FooBarJazzLooping2();
        FooBarJazzUseDelegates();
    }

    delegate void Rules(int num, StringBuilder sb);

    static void FooBarJazzUseDelegates()
    {
        Console.WriteLine("\nFooBarJazz using delegates implementation..");
        int num = 35;

        void Rules1(int num, StringBuilder sb) => sb.Append(num % 3 == 0 ? "foo" : "");
        void Rules2(int num, StringBuilder sb) => sb.Append(num % 5 == 0 ? "bar" : "");
        void Rules3(int num, StringBuilder sb) => sb.Append(num % 7 == 0 ? "jazz" : "");

        Rules r = Rules1;
        r += Rules2;
        r += Rules3;

        for (int i = 1; i <= num; i++)
        {
            StringBuilder result = new StringBuilder();

            r(i, result);

            if (result.Length == 0)
                result.Append(i);

            Console.Write(result);

            if (i != num)
                Console.Write(", ");
        }
    }

    static void FooBarJazzLooping1()
    {
        Console.WriteLine("\nFooBarJazz only looping with StringBuilder Append..");

        int num = 35;

        for (int i = 1; i <= num; i++)
        {
            StringBuilder result = new StringBuilder();
            
            if (i % 3 == 0) result.Append("foo");
            if (i % 5 == 0) result.Append("bar");
            if (i % 7 == 0) result.Append("jazz");
            if (result.Length == 0) result.Append(i);
               
            Console.Write(result);

            if (i != num)
                Console.Write(", ");
        }
    }

    static void FooBarJazzLooping2()
    {
        Console.WriteLine("\nFooBarJazz only looping through if conditional..");

        int num = 35;

        for (int i = 1; i <= num; i++)
        {
            if (i % 3 != 0 && i % 5 != 0 && i % 7 != 0)
                Console.Write(i);
            if (i % 3 == 0)
                Console.Write("foo");
            if (i % 5 == 0)
                Console.Write("bar");
            if (i % 7 == 0)
                Console.Write("jazz");
            if (i != num)
                Console.Write(", ");
        }
    }

    static void FooBarJazzUseListGenerics()
    {
        throw new NotImplementedException("ntar implementnya, dah kebayang tapi masih bingung nulisnya");
    }
}


