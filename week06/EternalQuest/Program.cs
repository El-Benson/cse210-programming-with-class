using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EternalQuest
{
    // ===========================================================
    // Eternal Quest Program
    // Exceeds Core Requirements:
    // - Includes score tracking that updates dynamically
    // - ChecklistGoal awards points on each completion + bonus on full completion
    // - Goals are serialized with Newtonsoft.Json using a custom converter for polymorphism
    // - User can create any type of goal and specify parameters
    // - All logic is implemented in a modular, extensible way
    // ===========================================================

    abstract class Goal
    {
        public string Name { get; protected set; }
        public int Points { get; protected set; }
        public bool IsComplete { get; protected set; }

        protected Goal(string name)
        {
            Name = name;
            IsComplete = false;
        }

        public abstract void RecordAchievement();
        public abstract string GetStatus();
    }

    class SimpleGoal : Goal
    {
        public SimpleGoal(string name, int points) : base(name)
        {
            Points = points;
        }

        public override void RecordAchievement()
        {
            if (!IsComplete)
            {
                IsComplete = true;
            }
        }

        public override string GetStatus()
        {
            return IsComplete ? "[X] " + Name : "[ ] " + Name;
        }
    }

    class EternalGoal : Goal
    {
        public EternalGoal(string name, int points) : base(name)
        {
            Points = points;
        }

        public override void RecordAchievement()
        {
            Points += 100;
        }

        public override string GetStatus()
        {
            return "[∞] " + Name + $" (Points Earned: {Points})";
        }
    }

    class ChecklistGoal : Goal
    {
        private int _totalCount;
        private int _completedCount;
        private int _pointsPerCheck;
        private int _bonus;

        public ChecklistGoal(string name, int totalCount, int pointsPerCheck, int bonus = 500) : base(name)
        {
            _totalCount = totalCount;
            _pointsPerCheck = pointsPerCheck;
            _bonus = bonus;
            _completedCount = 0;
        }

        public override void RecordAchievement()
        {
            if (!IsComplete)
            {
                _completedCount++;
                Points += _pointsPerCheck;
                if (_completedCount >= _totalCount)
                {
                    IsComplete = true;
                    Points += _bonus;
                }
            }
        }

        public override string GetStatus()
        {
            return (IsComplete ? "[X] " : "[ ] ") + Name + $" (Completed {_completedCount}/{_totalCount})";
        }
    }

    class GoalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(Goal).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            string type = jsonObject["$type"]?.ToString();

            Goal goal = type switch
            {
                string s when s.Contains(nameof(SimpleGoal)) => new SimpleGoal("", 0),
                string s when s.Contains(nameof(EternalGoal)) => new EternalGoal("", 0),
                string s when s.Contains(nameof(ChecklistGoal)) => new ChecklistGoal("", 0, 0),
                _ => throw new Exception("Unknown goal type")
            };

            serializer.Populate(jsonObject.CreateReader(), goal);
            return goal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, value.GetType());
        }
    }

    class Program
    {
        private static List<Goal> _goals = new();
        private static int _totalScore = 0;

        static void Main(string[] args)
        {
            LoadGoals();
            ShowMenu();
        }

        static void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Eternal Quest Goals Tracker ===");
                Console.WriteLine($"Total Score: {_totalScore}\n");
                ShowGoals();
                Console.WriteLine("\n1. Add Goal");
                Console.WriteLine("2. Record Achievement");
                Console.WriteLine("3. Save Goals");
                Console.WriteLine("4. Load Goals");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        AddGoal();
                        break;
                    case "2":
                        RecordAchievement();
                        break;
                    case "3":
                        SaveGoals();
                        break;
                    case "4":
                        LoadGoals();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        static void ShowGoals()
        {
            for (int i = 0; i < _goals.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_goals[i].GetStatus()}");
            }
        }

        static void AddGoal()
        {
            Console.WriteLine("Enter goal type (simple/eternal/checklist): ");
            var type = Console.ReadLine()?.ToLower();
            Console.Write("Enter goal name: ");
            var name = Console.ReadLine();

            switch (type)
            {
                case "simple":
                    Console.Write("Enter points for completion: ");
                    int points = int.Parse(Console.ReadLine());
                    _goals.Add(new SimpleGoal(name, points));
                    break;
                case "eternal":
                    _goals.Add(new EternalGoal(name, 0));
                    break;
                case "checklist":
                    Console.Write("Enter total number of completions required: ");
                    int total = int.Parse(Console.ReadLine());
                    Console.Write("Enter points per completion: ");
                    int per = int.Parse(Console.ReadLine());
                    _goals.Add(new ChecklistGoal(name, total, per));
                    break;
                default:
                    Console.WriteLine("Invalid goal type.");
                    break;
            }
        }

        static void RecordAchievement()
        {
            Console.WriteLine("Select a goal to record an achievement:");
            ShowGoals();
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= _goals.Count)
            {
                int previousPoints = _goals[index - 1].Points;
                _goals[index - 1].RecordAchievement();
                int earned = _goals[index - 1].Points - previousPoints;
                _totalScore += earned;
                Console.WriteLine($"You earned {earned} points!");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        static void SaveGoals()
        {
            var json = JsonConvert.SerializeObject(_goals, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                Converters = { new GoalConverter() }
            });

            File.WriteAllText("goals.json", json);
            File.WriteAllText("score.txt", _totalScore.ToString());
            Console.WriteLine("Goals and score saved successfully.");
        }

        static void LoadGoals()
        {
            if (File.Exists("goals.json"))
            {
                var json = File.ReadAllText("goals.json");
                _goals = JsonConvert.DeserializeObject<List<Goal>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Converters = { new GoalConverter() }
                }) ?? new List<Goal>();
            }

            if (File.Exists("score.txt"))
            {
                _totalScore = int.Parse(File.ReadAllText("score.txt"));
            }
        }
    }
}
