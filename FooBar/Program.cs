using System;
using System.Text;

class Program
{

    static void Main()
    {
        int number = 40;
        FooBarJazzLooping1(number);
        FooBarJazzLooping2(number);
        FooBarJazzUseDelegates(number);
        FooBarJazzUseDictionary(number);
    }

    static void FooBarJazzUseDictionary(int number)
    {
        Console.WriteLine("\n\nFooBarJazz use dictionary looping for rules..");
            
        Dictionary<int, string> ruleMap = new Dictionary<int, string>
        {
            {3,"foo"},
            {4,"baz"},
            {5,"bar"},
            {7,"jazz"},
            {9,"huzz"},
        };
        
        void Rules(int num, int denominator, string word, StringBuilder sb) => sb.Append(num % denominator == 0 ? word : "");

        for (int num = 1; num <= number; num++)
        {
            StringBuilder result = new StringBuilder();

            foreach (var rule in ruleMap)
            {
                int denominator = rule.Key;
                string word = rule.Value;

                Rules(num, denominator, word, result);
            }

            if (result.Length == 0)
                result.Append(num);

            Console.Write(result);

            if (num != number)
                Console.Write(", ");
        }

    }
    delegate void Rules(int num, StringBuilder sb);

    static void FooBarJazzUseDelegates(int number)
    {
        Console.WriteLine("\n\nFooBarJazz using delegates implementation..");


        void Rules1(int number, StringBuilder sb) => sb.Append(number % 3 == 0 ? "foo" : "");
        void Rules2(int number, StringBuilder sb) => sb.Append(number % 4 == 0 ? "baz" : "");
        void Rules3(int number, StringBuilder sb) => sb.Append(number % 5 == 0 ? "bar" : "");
        void Rules4(int number, StringBuilder sb) => sb.Append(number % 7 == 0 ? "jazz" : "");
        void Rules5(int number, StringBuilder sb) => sb.Append(number % 9 == 0 ? "huzz" : "");

        Action<int, StringBuilder> ruleChain = Rules1;
        ruleChain += Rules2;
        ruleChain += Rules3;
        ruleChain += Rules4;
        ruleChain += Rules5;

        for (int num = 1; num <= number; num++)
        {
            StringBuilder result = new StringBuilder();

            ruleChain(num, result);

            if (result.Length == 0)
                result.Append(num);

            Console.Write(result);

            if (num != number)
                Console.Write(", ");
        }
    }

    static void FooBarJazzLooping1(int number)
    {
        Console.WriteLine("\n\nFooBarJazz only looping with StringBuilder Append..");

        for (int num = 1; num <= number; num++)
        {
            StringBuilder result = new StringBuilder();
            
            if (num % 3 == 0) result.Append("foo");
            if (num % 4 == 0) result.Append("baz");
            if (num % 5 == 0) result.Append("bar");
            if (num % 7 == 0) result.Append("jazz");
            if (num % 9 == 0) result.Append("huzz");
            if (result.Length == 0) result.Append(num);
               
            Console.Write(result);

            if (num != number)
                Console.Write(", ");
        }
    }

    static void FooBarJazzLooping2(int number)
    {
        Console.WriteLine("\n\nFooBarJazz only looping through if conditional..");

        for (int num = 1; num <= number; num++)
        {
            if (num % 3 != 0 && num % 4 != 0 && num % 5 != 0 && num % 7 != 0 && num % 9 != 0)
                Console.Write(num);
            if (num % 3 == 0)
                Console.Write("foo");
            if (num % 4 == 0)
                Console.Write("baz");
            if (num % 5 == 0)
                Console.Write("bar");
            if (num % 7 == 0)
                Console.Write("jazz");
            if (num % 9 == 0)
                Console.Write("huzz");
            if (num != number)
                Console.Write(", ");
        }
    }  
}


