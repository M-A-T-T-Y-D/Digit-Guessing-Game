using System.Diagnostics;

public static class Game
{
    public static bool Result;
    public static bool Hint = false;
    public static int Turn = 1;
    public static int Length;
    public static string Name;
    public static string Difficulty;
    public static List<int> Generated;
    public static Stopwatch TotalTime = new Stopwatch();
    public static List<int> incorrectNumbers = new List<int>();
    static Random rnd = new Random();
    public static void AddToLeaderboard()
    {
        string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        string path =Path.Combine(projectRoot, "Leaderboard.txt");

        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine($"{Name},{Difficulty},{TotalTime.Elapsed.TotalSeconds},{Hint},{Turn}");
        }
        LeaderboardEntry.SortLeaderboard();
    }
    public static void RequestHint()
    {
        Console.WriteLine("\n\n---Hint---\n\n");
        int NumberHint = Generated[rnd.Next(0, Generated.Count)];
        Console.WriteLine($"The number {NumberHint} is used in the code");
        Turn++;
        Console.WriteLine($"\nYou are now on turn: {Turn}");

    }

    public static void RestartGame()
    {
        while (true)
        {
        Console.WriteLine("Would you like to go back to the menu? y/n");
        string Restart = Console.ReadLine().ToLower();
        if(Restart == "y")
        {
                MainMenu();
        }
        else if(Restart == "n")
        {
            Console.WriteLine("---Goodbye---");
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("Invalid selection please try again");
        }  
        }
        
    }
    public static void EndGame()
    {
        Console.WriteLine("\n\n---Game Over---\n\n");
        TotalTime.Stop();
        if(Result == false)
        {
            Console.WriteLine("You did not manage to guess the code\n");
            if(Turn != 10)
            {
                Turn = 10 - Turn;
                Console.WriteLine($"You still had {Turn} Turns left");
            }
            Console.WriteLine($"You took {TotalTime.Elapsed.TotalSeconds} Seconds");
            string Code = "";
            for(int i = 0; i < Generated.Count; i++)
            {
                Code += Generated[i];
            }
            Console.WriteLine($"The Correct Code was:{Code}");
            RestartGame();
            
        }
        else
        {
            Console.WriteLine("\n\n---Game Over---\n\n");
            Console.WriteLine($"Well Done you Guessed the correct code in {Turn} Trys");
            Console.WriteLine($"It took you {TotalTime.Elapsed.TotalSeconds} seconds to guess the code");

            if(Hint == false)
            {
                Console.WriteLine("You managed to do it without a hint well done");
            }
            else
            {
                Console.WriteLine("You used a Hint unfortunatly");
            }
            while (true)
            {
            Console.WriteLine("Would you like to submit your score to the leaderboard? y/n");
            string Answer =  Console.ReadLine().ToLower();
            if(Answer == "y")
            {
                AddToLeaderboard();
            }
            else if(Answer == "n")
            {
                Console.WriteLine("Your scores havent been submitted to leaderboard"); 
                RestartGame();
                
            }
            else
            {
                Console.WriteLine("Invalid Selection please try again\n\n");
            }

            }
            
        }

    }
    public static void CompareGuess(List<int> UserGuess)
    {
        int Bulls = 0;
        int Cows = 0;

        bool[] usedGenerated = new bool[Length];
        bool[] usedGuess = new bool[Length];

        for (int i = 0; i < Length; i++)
        {
            if (UserGuess[i] == Generated[i])
            {
                Bulls++;
                usedGenerated[i] = true;
                usedGuess[i] = true;
            }
        }

        for(int i = 0; i < Length; i++)
        {
            if (usedGuess[i])
            {
                continue;
            }
            for(int j = 0; j < Length; j++)
            {
                if( !usedGenerated[j] && UserGuess[i] == Generated[j])
                {
                    Cows++;
                    usedGenerated[j] = true;
                    break;
                }
            }
        }
        for(int i = 0; i < Length; i++)
        {
            bool foundInGenerated = false;

            for(int j = 0; j < Length; j++)
            {
                if(UserGuess[i] == Generated[j])
                {
                    foundInGenerated = true;
                    break;
                }
            }
            if (!foundInGenerated)
            {
                bool alreadyAdded = false;

                for(int x = 0; x < incorrectNumbers.Count; x++)
                {
                    if(incorrectNumbers[x] == UserGuess[i])
                    {
                        alreadyAdded = true;
                        break;
                    }
                }
                if (!alreadyAdded)
                {
                    incorrectNumbers.Add(UserGuess[i]);
                }
            }
        }
        Console.WriteLine($"\n\nBulls: {Bulls}\n\nCows: {Cows}");
    }
    public static List<int> UsersTurn()
    {
        while (true)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Console.WriteLine($"\n\nTurn: {Turn}");
            if(Turn != 1)
            {
                Console.WriteLine("Heres a list of numbers you have tried that were incorrect:\n");
                for(int i = 0; i < incorrectNumbers.Count; i++)
                {
                    Console.Write($" {incorrectNumbers[i]} ");
                } 
            }
            Console.WriteLine($"\nPlease enter a {Length} digit guess (45 seconds):");
            Console.WriteLine("\nEnter Hint to receive a hint or quit to exit:");
            string UserInput = Console.ReadLine().ToLower();
            if (UserInput == "quit")
            {
                Result = false;
                EndGame();
            }
            else if(UserInput == "hint")
            {
                if (Hint)
                {
                    Console.WriteLine("You have already used your hint");
                    continue;
                }
                Hint = true;
                RequestHint();
                continue;
            }
            bool HasDuplicate = false;
            bool IsNumeric = true;

            for (int i = 0; i < UserInput.Length; i++)
            {
                for(int j = i+1; j < UserInput.Length; j++)
                {
                    if(UserInput[i] == UserInput[j])
                    {
                        HasDuplicate = true;
                    }
                }
                
            }
            for(int i = 0; i < UserInput.Length; i++)
            {
                if(UserInput[i] < '0' || UserInput[i] > '9')
                {
                    IsNumeric = false;
                    break;
                }
            }
            if(Difficulty == "Normal" && HasDuplicate)
            {
                Console.WriteLine("Digits must be unique in Normal mode");
                Turn++;
                continue;
            }

            else if (UserInput.Length != Length || !IsNumeric)
                {
                    Turn++;
                    Console.WriteLine("Invalid selection please try again");
                    if(Turn == 10)
                    {
                        Console.WriteLine("You Failed To Guess In 10 Turns");
                        Result = false;
                        EndGame();
                    }
                    continue; 
                }
            else if(timer.Elapsed.TotalSeconds >= 45)
                {
                    Console.WriteLine("\n\nYou did not enter the guess in 45 seconds, your guess is disqualified please try again.\n\n");
                    Turn++;
                    return UsersTurn();
                }
                List<int> UserGuess = new List<int>();
                for(int i = 0; i < UserInput.Length; i++)
                {
                    UserGuess.Add(UserInput[i] - '0');
                }
                return UserGuess; 
        }
        
    }
    public static void GameStart()
    {
        Console.WriteLine("\n\nGame Starting\n\n");
        Generator();
        TotalTime.Start();
        bool won = false;
        while (!won)
        {
            if (Turn < 10){
            List<int> UserGuess = UsersTurn();
            CompareGuess(UserGuess);
            bool isCorrect = true;
            for(int i = 0; i < Length; i++)
                {
                    if(UserGuess[i] != Generated[i])
                    {
                        isCorrect = false;
                        break;
                    }
                }
            if (isCorrect)
                {
                    Result = true;
                    EndGame();
                }
            Turn++;
            if(Turn >= 10)
                {
                    Result = false;
                    EndGame();
                }
            }
            
            
        }
        
    }
    public static void Generator()
    {
        List<int> Digits = new List<int>();
        if(Difficulty == "Normal")
        {
            while(Digits.Count != Length)
            {
            int num = rnd.Next(0,10);
            if (!Digits.Contains(num))
            {
                Digits.Add(num);
            }
            }
            Generated = Digits;
        }
        else if(Difficulty == "Hard")
        {
            while(Digits.Count != Length)
            {
            int num = rnd.Next(0,10);
            Digits.Add(num);
            
            }
            Generated = Digits;
        }
    }
    public static void SetName()
    {
        string NameCheck = "";
        while (true)
        {
            Console.WriteLine("Please Enter a Name or Username for Leaderboard");

            NameCheck = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(NameCheck))
            {
            Console.WriteLine("You cant enter no name");
            }
            else
            {
                break;
            }
        }
        Console.WriteLine($"Are you sure you want to be known as {NameCheck}, y/n");
        string Input = Console.ReadLine().ToLower().Trim();
        if (Input == "y")
        {
            Name = NameCheck;
            Console.WriteLine("Created Name Successfully");
        }
        else if (Input == "n")
        {
            SetName();
        }
        else
        {
            Console.WriteLine("You entered an invalid option, Retrying");
            SetName();
        }
    }
    public static void MainMenu()
    {
        
        bool Menu = true;
        while(Menu == true)
        {
        Console.WriteLine("\n\nMain Menu\n Select an option:\n 1) Normal Mode\n 2) Hard Mode\n 3) Leaderboard\n 4) Quit");
        int Input = 0;
        try
        {
            Input = Convert.ToInt16(Console.ReadLine()); // Converts to Int 16 to save memory space
        }
        catch
        {
            Console.WriteLine("Invalid selection please try again");
            continue;
        }   
        if(Input == 1)
        {
            Console.WriteLine("You Selected Normal Mode");
            Difficulty = "Normal";
            GameVersion();
            Menu = false;
        }
        else if(Input == 2)
        {
            Console.WriteLine("You Selected Hard Mode");
            Difficulty = "Hard";
            GameVersion();
            Menu = false;
        }
        else if(Input == 3)
        {
            Console.WriteLine("You Selected Leaderboard");
            LeaderboardEntry.ViewLeaderboard();
            Console.WriteLine("\nWould you like to return to the menu? y/n");
            string UserInput = Console.ReadLine().ToLower();
            if(UserInput == "n")
                {
                    Console.WriteLine("--Goodbye--");
                    Environment.Exit(0);
                }
        }
        else if(Input == 4)
        {
            Menu = false;
            Environment.Exit(0);
            
        }
        else
        {
            Console.WriteLine("You made an invalid selection");
        }
        }
        
    }
    public static void GameVersion()
    {
        Console.WriteLine("What version of the game would you like to play?\n 1) 4 digit code\n 2) 5 digit code\n 3) 6 digit code");
        
        bool Menu = true;
        while(Menu == true)
        {
            int Input = 0;
            try
            {
                Input = Convert.ToInt16(Console.ReadLine()); // Converts to Int 16 to save memory space
            }
            catch
            {
                Console.WriteLine("Invalid Selection, Please try again");
                continue;
            }
            if(Input == 1)
            {
                Console.WriteLine("You Selected 4 Digit Code");
                Menu = false;
                Length = 4;

            }
            else if(Input == 2)
            {
                
                Console.WriteLine("You Selected 5 Digit Code");
                Menu = false;
                Length = 5;
            }
            else if(Input == 3)
            {
                Console.WriteLine("You Selected 6 Digit Code");
                Menu = false;
                Length = 6;
            }
            else
            {
                Console.WriteLine("You Entered an Invalid Integer");
            }
        }
        
    }
    public static void Main(string[] args)
    {
        // Welcome message + rules
        Console.WriteLine("Welcome to Cows and Bulls.");
        Console.WriteLine("The computer will generate a code which you have to guess. when you enter a code an output will show you how many bulls and cows you got. \nBulls indicate that you have got a correct digit in the correct position. \nCows indicate you got a correct digit in the wrong position.\nThe generated code cannot have the same digit in different positions IE. it must be 1234 instead of 1135. ");

        SetName();
        MainMenu();
        GameStart();
    }
}
public class LeaderboardEntry
{
    public string Name;
    public string Difficulty;
    public double TimeTaken;
    public bool HintUsed;
    public int Turn;
    static string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
    static string path =Path.Combine(projectRoot, "Leaderboard.txt");
    public static List<LeaderboardEntry> LoadLeaderboard()
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            string[] parts = line.Split(",");

            LeaderboardEntry entry = new LeaderboardEntry
            {
                Name = parts[0],
                Difficulty = parts[1],
                TimeTaken = double.Parse(parts[2]),
                HintUsed = bool.Parse(parts[3]),
                Turn = int.Parse(parts[4])
            };

            entries.Add(entry);
        }

        return entries;
    }
    public static void SaveLeaderboard(List<LeaderboardEntry> entries)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach(LeaderboardEntry i in entries)
            {
                writer.WriteLine($"{i.Name},{i.Difficulty},{i.TimeTaken},{i.HintUsed},{i.Turn}");
            }
        }
    }
    public static void SortByTime(List<LeaderboardEntry> entries)
    {
        for(int i = 0; i < entries.Count - 1; i++)
        {
            for(int j = 0; j < entries.Count - i - 1; j++)
            {
                if(entries[j].TimeTaken > entries[j + 1].TimeTaken)
                {
                    LeaderboardEntry temp = entries[j];
                    entries[j] = entries[j + 1];
                    entries[j + 1] = temp;
                }
            }
        }

    }
    public static void SortLeaderboard()
    {
        List<LeaderboardEntry> entries = LoadLeaderboard();

        SortByTime(entries);

        SaveLeaderboard(entries);

        Console.WriteLine("Leaderboard Sorted");
        Game.RestartGame();
    }

    public static void ViewLeaderboard()
    {
        List<LeaderboardEntry> entries = LoadLeaderboard();
        Console.WriteLine("   Name  Difficulty  Time  Hint  Turn");
        int Num = 0;
        using (StreamReader reader = new StreamReader(path, true))
        {
            foreach(LeaderboardEntry i in entries)
            {
                Num++;
                Console.WriteLine($"{Num}) {i.Name}  {i.Difficulty}  {i.TimeTaken}  {i.HintUsed}  {i.Turn}");
            }
        }
    }
}

