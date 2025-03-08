using System;

class Program
{
    static void Main()
    {
        // Prompt the user for their grade percentage
        Console.Write("Enter your grade percentage: ");
        string input = Console.ReadLine();

        // Convert input to an integer
        int percentage;
        if (!int.TryParse(input, out percentage))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            return;
        }

        string letter = "";
        string sign = "";

        // Determine letter grade
        if (percentage >= 90)
        {
            letter = "A";
        }
        else if (percentage >= 80)
        {
            letter = "B";
        }
        else if (percentage >= 70)
        {
            letter = "C";
        }
        else if (percentage >= 60)
        {
            letter = "D";
        }
        else
        {
            letter = "F";
        }

        // Determine grade sign (+ or -)
        int lastDigit = percentage % 10;
        if (lastDigit >= 7)
        {
            sign = "+";
        }
        else if (lastDigit < 3)
        {
            sign = "-";
        }

        // Handle special cases: No A+, No F+ or F-
        if (letter == "A" && sign == "+")
        {
            sign = ""; // A+ doesn't exist
        }
        else if (letter == "F")
        {
            sign = ""; // No F+ or F-
        }

        // Print the final grade
        Console.WriteLine($"Your letter grade is: {letter}{sign}");

        // Determine pass/fail status
        if (percentage >= 70)
        {
            Console.WriteLine("Congratulations! You passed the class.");
        }
        else
        {
            Console.WriteLine("Keep trying! You can do better next time.");
        }
    }
}
