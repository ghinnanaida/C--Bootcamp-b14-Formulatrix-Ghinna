// See https://aka.ms/new-console-template for more information
int num2 = 30;

for (int i = 1; i <= num2; i++)
{
    // if (i % 3 == 0 && i % 5 == 0)
    if (i % 15 == 0 )
        Console.Write("foobar");
    else if (i % 3 == 0)
        Console.Write("foo");
    else if (i % 5 == 0)
        Console.Write("bar");
    else
        Console.Write(i);
    if (i != num2)
        Console.Write(", ");
}

Console.WriteLine();

int num = 35;

for (int i = 1; i <= num; i++)
{
    if (i % 3 != 0 && i % 5 != 0)
        Console.Write(i);
    if (i % 3 == 0)
        Console.Write("foo");
    if (i % 5 == 0)
        Console.Write("bar");
    if (i != num)
        Console.Write(", ");
}


