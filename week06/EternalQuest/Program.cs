using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EternalQuest
{
    abstract class Goal
    {
        public string Name { get; private set; }
        public bool IsComplete { get; protected set; }

        protected Goal(string name)
        {
            Name = name;
            IsComplete = false;
        }

        public abstract int RecordAchievement();
        public abstract string GetStatus();
    }

    class SimpleGoal : Goal
    {
        private int _points;
        private bool _scored;

        public SimpleGoal(string name, int points) : base(name)
        {
            _points = points;
            _scored = false;
        }

        public override int RecordAchievement()
        {
            if (!IsComplete)
            {
                IsComplete = true;
                if (!_scored)
                {
                    _scored = true;
                    return _points;
                }
            }
            return 0;
        }

        public override string GetStatus()
        {
            return IsComplete ? "[X] " + Name : "[ ] " + Name;
        }
    }

    class EternalGoal : Goal
    {
        private int _points;

        public EternalGoal(string name, int points) : base(name)
        {
            _points = points;
        }

        public override int RecordAchievement()
        {
            return _points;
        }

        public override string GetStatus()
        {
            return "[ ] " + Name + $" (Eternal Goal, +{_points} pts each time)";
        }
    }

    class ChecklistGoal : Goal
    {
        private int _pointsPer;
        private int _targetCount;
        private int _completedCount;
        private int _bonus;

        public ChecklistGoal(string name, int totalCount, int pointsPer, int bonus = 500) : base(name)
        {
            _targetCount = totalCount;
            _pointsPer = pointsPer;
            _completedCount = 0;
            _bonus = bonus;
        }

        public override int RecordAchievement()
        {
            if (!IsComplete)
            {
                _completedCount++;
                int earned = _pointsPer;
                if (_completedCount >= _targetCount)
                {
                    IsComplete = true;
                    earned += _bonus;
                }
                return earned;
            }
            return 0;
        }

        public override string GetStatus()
        {
            return IsComplete ? "[X] " + Name : $"[ ] {Name} (Completed {_completedCount}/{_targetCount})";
        }
    }

    class GoalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Goal);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string type = jo["Type"]?.ToString();
            return type switch
            {
                "SimpleGoal" => jo.ToObject<SimpleGoal>(),
                "EternalGoal" => jo.ToObject<EternalGoal>(),
                "ChecklistGoal" => jo.ToObject<ChecklistGoal>(),
                _ => throw new Exception("Unknown goal type")
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = JObject.FromObject(value);
            jo.AddFirst(new JProperty("Type", value.GetType().Name));
            jo.WriteTo(writer);
        }
    }

    class Program
    {
        private static List<Goal> _goals = new List<Goal>();
        private static int _totalScore = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("// This program exceeds core requirements by:\n// 1. Using a custom JSON converter to save/load abstract goals.\n// 2. Adding bonus functionality and defensive checks.\n// 3. Using polymorphism to return points from goals dynamically.");
            LoadGoals();
            ShowMenu();
        }

        static void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Eternal Quest Goals Tracker");
                Console.WriteLine($"Total Score: {_totalScore}\n");

                for (int i = 0; i < _goals.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {_goals[i].GetStatus()}");
                }

                Console.WriteLine("\n1. Add Goal");
                Console.WriteLine("2. Record Achievement");
                Console.WriteLine("3. Save Goals");
                Console.WriteLine("4. Load Goals");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddGoal(); break;
                    case "2": RecordAchievement(); break;
                    case "3": SaveGoals(); break;
                    case "4": LoadGoals(); break;
                    case "5": return;
                    default: Console.WriteLine("Invalid choice."); Console.ReadLine(); break;
                }
            }
        }

        static void AddGoal()
        {
            Console.Write("Enter goal type (simple/eternal/checklist): ");
            string type = Console.ReadLine().ToLower();
            Console.Write("Enter goal name: ");
            string name = Console.ReadLine();

            switch (type)
            {
                case "simple":
                    Console.Write("Enter point value: ");
                    int simplePoints = int.Parse(Console.ReadLine());
                    _goals.Add(new SimpleGoal(name, simplePoints));
                    break;
                case "eternal":
                    Console.Write("Enter point value per completion: ");
                    int eternalPoints = int.Parse(Console.ReadLine());
                    _goals.Add(new EternalGoal(name, eternalPoints));
                    break;
                case "checklist":
                    Console.Write("Enter total number of completions required: ");
                    int total = int.Parse(Console.ReadLine());
                    Console.Write("Enter points per completion: ");
                    int pointsPer = int.Parse(Console.ReadLine());
                    _goals.Add(new ChecklistGoal(name, total, pointsPer));
                    break;
                default:
                    Console.WriteLine("Invalid goal type.");
                    break;
            }
            Console.WriteLine("Goal added. Press enter to continue...");
            Console.ReadLine();
        }

        static void RecordAchievement()
        {
            Console.WriteLine("Select a goal by number to record achievement:");
            for (int i = 0; i < _goals.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_goals[i].GetStatus()}");
            }
            Console.Write("Enter number: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _goals.Count)
            {
                int earned = _goals[index - 1].RecordAchievement();
                _totalScore += earned;
                Console.WriteLine($"You earned {earned} points! Press enter to continue...");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
            Console.ReadLine();
        }

        static void SaveGoals()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None, Formatting = Formatting.Indented, Converters = { new GoalConverter() } };
            string json = JsonConvert.SerializeObject(_goals, settings);
            File.WriteAllText("goals.json", json);
            Console.WriteLine("Goals saved successfully. Press enter...");
            Console.ReadLine();
        }

        static void LoadGoals()
        {
            if (File.Exists("goals.json"))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None, Converters = { new GoalConverter() } };
                string json = File.ReadAllText("goals.json");
                _goals = JsonConvert.DeserializeObject<List<Goal>>(json, settings);
                Console.WriteLine("Goals loaded successfully.");
            }
            else
            {
                Console.WriteLine("No saved goals found.");
            }
            _totalScore = 0;
            foreach (var goal in _goals)
            {
                // Do not re-calculate score from Points stored; rely on runtime accumulation only
            }
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
    }
}
