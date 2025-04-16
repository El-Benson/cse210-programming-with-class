using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // An attempt to exceed core requirements:
        // This program goes beyond the basic requirements by not only allowing users to write and store journal entries, 
        // but also by implementing features such as saving and loading journal entries from a file, generating random writing prompts, 
        // and storing the exact timestamp of when an entry is created. The program also allows users to quit the journal management system 
        // gracefully, and provides a user-friendly menu system for managing their journal.
        
        Journal journal = new Journal();
        PromptGenerator promptGenerator = new PromptGenerator();

        bool running = true;

        while (running)
        {
            // Displaying menu options for the user to interact with the journal system.
            Console.WriteLine("\nJournal Menu:");
            Console.WriteLine("1. Write a new entry");
            Console.WriteLine("2. Display journal");
            Console.WriteLine("3. Save journal to file");
            Console.WriteLine("4. Load journal from file");
            Console.WriteLine("5. Quit");
            Console.Write("Choose an option (1-5): ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Generate a random prompt and get the user's response
                    string prompt = promptGenerator.GetRandomPrompt();
                    Console.WriteLine($"\nPrompt: {prompt}");
                    Console.Write("Your response: ");
                    string response = Console.ReadLine();
                    journal.AddEntry(new Entry(prompt, response)); // Store the entry
                    break;

                case "2":
                    Console.WriteLine("\nJournal Entries:");
                    journal.DisplayEntries(); // Display all entries
                    break;

                case "3":
                    Console.Write("Enter filename to save: ");
                    string saveFile = Console.ReadLine();
                    journal.SaveToFile(saveFile); // Save journal to file
                    break;

                case "4":
                    Console.Write("Enter filename to load: ");
                    string loadFile = Console.ReadLine();
                    journal.LoadFromFile(loadFile); // Load journal from file
                    break;

                case "5":
                    running = false; // Quit the application
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}

public class Journal
{
    private List<Entry> _entries;

    public Journal()
    {
        _entries = new List<Entry>(); // Initialize the list of entries
    }

    // Add a new entry to the journal
    public void AddEntry(Entry entry)
    {
        _entries.Add(entry);
    }

    // Display all entries with their date, prompt, and response
    public void DisplayEntries()
    {
        if (_entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            return;
        }

        foreach (Entry entry in _entries)
        {
            Console.WriteLine(entry.ToString());
        }
    }

    // Save the journal to a file
    public void SaveToFile(string fileName)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (Entry entry in _entries)
            {
                writer.WriteLine(entry.ToString());
            }
        }
        Console.WriteLine("Journal saved successfully.");
    }

    // Load the journal from a file
    public void LoadFromFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            _entries.Clear();
            foreach (string line in File.ReadLines(fileName))
            {
                string[] parts = line.Split(new[] { " | " }, StringSplitOptions.None);
                string date = parts[0];
                string prompt = parts[1];
                string response = parts[2];

                _entries.Add(new Entry(prompt, response, date));
            }
            Console.WriteLine("Journal loaded successfully.");
        }
        else
        {
            Console.WriteLine("No journal found with that name.");
        }
    }
}

public class Entry
{
    public string Date { get; }
    public string Prompt { get; }
    public string Response { get; }

    // Constructor for a new entry
    public Entry(string prompt, string response)
    {
        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        Prompt = prompt;
        Response = response;
    }

    // Constructor for loading an entry from file (with date)
    public Entry(string prompt, string response, string date)
    {
        Date = date;
        Prompt = prompt;
        Response = response;
    }

    // Override ToString to display entry as a string
    public override string ToString()
    {
        return $"{Date} | {Prompt} | {Response}";
    }
}

public class PromptGenerator
{
    private List<string> _prompts;

    public PromptGenerator()
    {
        _prompts = new List<string>
        {
            "What made you happy today?",
            "What did you learn today?",
            "What challenges did you face today?",
            "What are you grateful for?",
            "Describe a place you want to visit."
        };
    }

    // Get a random prompt
    public string GetRandomPrompt()
    {
        Random random = new Random();
        int index = random.Next(_prompts.Count);
        return _prompts[index];
    }
}
