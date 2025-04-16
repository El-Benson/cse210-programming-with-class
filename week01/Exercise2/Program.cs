using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        // Attempt to exceed core requirements:
        // - The Journal class manages journal entries and includes functionality for writing, displaying, saving, and loading entries.
        // - Writing prompts are randomly selected from a predefined set of prompts.
        // - Entries include the date they were written.
        // - The program supports saving and loading entries from a file.
        // - Proper abstraction of the Journal and Entry classes.
        
        Journal myJournal = new Journal();
        
        // Display options menu
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Journal Program ===");
            Console.WriteLine("1. Write a Journal Entry");
            Console.WriteLine("2. Display All Entries");
            Console.WriteLine("3. Save Journal");
            Console.WriteLine("4. Load Journal");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    myJournal.WriteEntry();
                    break;
                case "2":
                    myJournal.DisplayEntries();
                    break;
                case "3":
                    myJournal.SaveToFile();
                    break;
                case "4":
                    myJournal.LoadFromFile();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}

// Class for Journal that manages entries
public class Journal
{
    private List<Entry> _entries = new List<Entry>();

    // Writes a new entry with a prompt and current date
    public void WriteEntry()
    {
        string prompt = GetRandomPrompt();
        Console.WriteLine($"Prompt: {prompt}");
        Console.Write("Write your journal entry: ");
        string content = Console.ReadLine();

        Entry newEntry = new Entry(content, prompt);
        _entries.Add(newEntry);
        Console.WriteLine("Your entry has been saved!\n");
    }

    // Displays all journal entries with their dates and prompts
    public void DisplayEntries()
    {
        if (_entries.Count == 0)
        {
            Console.WriteLine("No entries available.\n");
            return;
        }

        foreach (var entry in _entries)
        {
            entry.Display();
        }

        Console.WriteLine();
    }

    // Saves all entries to a file
    public void SaveToFile()
    {
        using (StreamWriter sw = new StreamWriter("journal.txt"))
        {
            foreach (var entry in _entries)
            {
                sw.WriteLine(entry.ToString());
            }
        }
        Console.WriteLine("Journal saved successfully.\n");
    }

    // Loads all entries from a file
    public void LoadFromFile()
    {
        if (File.Exists("journal.txt"))
        {
            _entries.Clear();
            string[] lines = File.ReadAllLines("journal.txt");

            foreach (string line in lines)
            {
                string[] parts = line.Split(new[] { " | " }, StringSplitOptions.None);
                string date = parts[0];
                string prompt = parts[1];
                string content = parts[2];

                Entry entry = new Entry(content, prompt, date);
                _entries.Add(entry);
            }

            Console.WriteLine("Journal loaded successfully.\n");
        }
        else
        {
            Console.WriteLine("No saved journal found.\n");
        }
    }

    // Returns a random writing prompt
    private string GetRandomPrompt()
    {
        List<string> prompts = new List<string>
        {
            "What made you happy today?",
            "What did you learn today?",
            "What challenges did you face today?",
            "What are you grateful for?",
            "Describe a place you want to visit."
        };

        Random random = new Random();
        int index = random.Next(prompts.Count);
        return prompts[index];
    }
}

// Class for each journal entry, stores the content, date, and prompt
public class Entry
{
    public string Date { get; }
    public string Prompt { get; }
    public string Content { get; }

    public Entry(string content, string prompt)
    {
        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        Prompt = prompt;
        Content = content;
    }

    public Entry(string content, string prompt, string date)
    {
        Date = date;
        Prompt = prompt;
        Content = content;
    }

    // Displays the entry with the date, prompt, and content
    public void Display()
    {
        Console.WriteLine($"Date: {Date}\nPrompt: {Prompt}\nContent: {Content}\n");
    }

    public override string ToString()
    {
        return $"{Date} | {Prompt} | {Content}";
    }
}
