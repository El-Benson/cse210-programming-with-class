using System;

class Program
{
    static void Main()
    {
        // Prompt the user for their first name
        Console.Write("What is your first name? ");
        string firstName = Console.ReadLine();
        
        // Prompt the user for their last name
        Console.Write("What is your last name? ");
        string lastName = Console.ReadLine();
        
        // Display the output in the correct format
        Console.WriteLine($"Your name is {lastName}, {firstName} {lastName}.");
    }
}
