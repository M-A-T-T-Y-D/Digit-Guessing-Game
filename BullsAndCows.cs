using System.Diagnostics;
using System.Diagnostics.Contracts;

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
    public static List<int> IncorrectNumbers = new List<int>();
    static Random rnd = new Random();

    /// <summary>
    /// This code adds the users statistics to the leaderboard file
    /// Then calls the function in the LeaderboardEntry class
    /// called SortLeaderboard()
    /// </summary>
    /// <remarks>
    /// The projectRoot is collected to force the Leaderboard file to be created
    /// within the same folder as the game file is.
    /// this is to prevent other sections of the code from being able to acess it.
    /// The code then writes a new line containing the users statistics using streamwriter
    /// </remarks>
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


    /// <summary>
    /// This is the logic behind picking a number to reveal 
    /// to the user when they request a hint
    /// </summary>
    /// <remarks>
    /// The number given in the hint is always randomly selected
    /// The games Turn number is incremented
    /// </remarks>
    public static void RequestHint()
    {
        
        Console.WriteLine("\n\n---Hint---\n\n");
        int NumberHint = Generated[rnd.Next(0, Generated.Count)];
        Console.WriteLine($"The number {NumberHint} is used in the code");
        Turn++;
        Console.WriteLine($"\nYou are now on turn: {Turn}");

    }

    /// <summary>
    /// This function resets objects to their origional
    /// state then restarts the game
    /// </summary>
    /// <remarks>
    /// This function questions wether the user wants to go
    /// back to the menu, if the user selects yes, it resets
    /// objects in the class to their pre game state so they
    /// can be reset. it then calls the subsequent functions 
    /// to start a new game
    /// </remarks>
    public static void RestartGame()
    {
        while (true)
        {
        Console.WriteLine("Would you like to go back to the menu? y/n");
        string restart = Console.ReadLine().ToLower();
        if(restart == "y")
        {
            Turn = 1;
            Hint = false;
            Result = false;
            IncorrectNumbers.Clear();
            TotalTime.Reset();
            Generated.Clear();
            MainMenu();
            GameStart();
            return;
        }
        else if(restart == "n")
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
    /// <summary>
    /// This function forces the end of the game when either the turn hits
    /// 10, the user guesses correctly or the user wants to quit.
    /// </summary>
    /// <remarks>
    /// This function outputs different game over screens depending on the result
    /// it outputs the correct code by using a for loop to convert a List into
    /// a string, then outputs the time taken, wether a hint has been used
    /// the amount of turns left, then questions the user on a win if they
    /// want to submit their score to the leaderboard.
    /// the user will then be prompted if they want to return to the menu
    /// by calling RestartGame()
    /// </remarks>
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
            string code = "";
            for(int i = 0; i < Generated.Count; i++)
            {
                code += Generated[i];
            }
            Console.WriteLine($"The Correct Code was:{code}");
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
    /// <summary>
    /// Compares the users guess against the generated code and calculates
    /// the number of bulls and cows
    /// </summary>
    /// <param name="userGuess">
    /// A list of integers representing the users guessed digits
    /// </param>
    /// <remarks>
    /// Bulls are counted when a digit is correct and in the correct position.
    /// Cows are counted when a digit is correct in the wrong position.
    /// Incorrect digits are stored to assist the player in future turns.
    /// The 2 boolean lists are used so the code doesnt run for digits
    /// that have already been assigned, and are then used to identify incorrect
    /// numbers to be assigned to the class list
    /// </remarks>
    public static void CompareGuess(List<int> userGuess)
    {
        int bulls = 0;
        int cows = 0;

        bool[] usedGenerated = new bool[Length];
        bool[] usedGuess = new bool[Length];

        for (int i = 0; i < Length; i++)
        {
            if (userGuess[i] == Generated[i])
            {
                bulls++;
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
                if( !usedGenerated[j] && userGuess[i] == Generated[j])
                {
                    cows++;
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
                if(userGuess[i] == Generated[j])
                {
                    foundInGenerated = true;
                    break;
                }
            }
            if (!foundInGenerated)
            {
                bool alreadyAdded = false;

                for(int x = 0; x < IncorrectNumbers.Count; x++)
                {
                    if(IncorrectNumbers[x] == userGuess[i])
                    {
                        alreadyAdded = true;
                        break;
                    }
                }
                if (!alreadyAdded)
                {
                    IncorrectNumbers.Add(userGuess[i]);
                }
            }
        }
        Console.WriteLine($"\n\nBulls: {bulls}\n\nCows: {cows}");
    }
    /// <summary>
    /// Handles a single user turn by requesting input and validating it,
    /// enforcing time limits and processing special commands
    /// </summary>
    /// <returns>
    /// A list of integers representing a valid user guess
    /// </returns>
    /// <remarks>
    /// The user has 45 seconds to enter a guess otherwise the turn is invalidated
    /// Special commands include "hint" and "quit".
    /// Invalid inputs result in a loss of a turn.
    /// </remarks>
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
                for(int i = 0; i < IncorrectNumbers.Count; i++)
                {
                    Console.Write($" {IncorrectNumbers[i]} ");
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

    /// <summary>
    /// This function starts the game by starting the timer, resetting the
    /// win checker.
    /// </summary>
    /// <remarks>
    /// The code always checks if the user is below 10 turns.
    /// Creates a win result if the user guesses correctly.
    /// </remarks>
    public static void GameStart()
    {
        Console.WriteLine("\n\nGame Starting\n\n");
        Generator();
        TotalTime.Start();
        bool won = false;
        while (!won)
        {
            if (Turn < 10){
            List<int> userGuess = UsersTurn();
            CompareGuess(userGuess);
            bool isCorrect = true;
            for(int i = 0; i < Length; i++)
                {
                    if(userGuess[i] != Generated[i])
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
    /// <summary>
    /// Generates the secret code based on the selected code length.
    /// </summary>
    /// <remarks>
    /// In normal mode all the digits will be unique.
    /// In hard mode, duplicated digits are allowed.
    /// The generated code is stored in a class list.
    /// </remarks>
    public static void Generator()
    {
        List<int> digits = new List<int>();
        if(Difficulty == "Normal")
        {
            while(digits.Count != Length)
            {
            int num = rnd.Next(0,10);
            if (!digits.Contains(num))
            {
                digits.Add(num);
            }
            }
            Generated = digits;
        }
        else if(Difficulty == "Hard")
        {
            while(digits.Count != Length)
            {
            int num = rnd.Next(0,10);
            digits.Add(num);
            
            }
            Generated = digits;
        }
    }
    /// <summary>
    /// This function forces the user to create an allias 
    /// before the game can start
    /// </summary>
    /// <remarks>
    /// The name cannot be whitespaces or null otherwise its rejected
    /// </remarks>
    public static void SetName()
    {
        string nameCheck;
        while (true)
        {
            Console.WriteLine("Please Enter a Name or Username for Leaderboard");

            nameCheck = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nameCheck))
            {
            Console.WriteLine("You cant enter no name");
            }
            else
            {
                break;
            }
        }
        Console.WriteLine($"Are you sure you want to be known as {nameCheck}, y/n");
        string Input = Console.ReadLine().ToLower().Trim();
        if (Input == "y")
        {
            Name = nameCheck;
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
    /// <summary>
    /// The main menu for the game.
    /// </summary>
    /// <remarks>
    /// Does not allow the user to write anything but intergers.
    /// Will result in invalid if an option not on the list is selected.
    /// </remarks>
    public static void MainMenu()
    {
        
        bool menu = true;
        while(menu == true)
        {
        Console.WriteLine("\n\nMain Menu\n Select an option:\n 1) Normal Mode\n 2) Hard Mode\n 3) Leaderboard\n 4) Quit");
        int input;
        try
        {
            input = Convert.ToInt16(Console.ReadLine()); // Converts to Int 16 to save memory space
        }
        catch
        {
            Console.WriteLine("Invalid selection please try again");
            continue;
        }   
        if(input == 1)
        {
            Console.WriteLine("You Selected Normal Mode");
            Difficulty = "Normal";
            GameVersion();
            menu = false;
        }
        else if(input == 2)
        {
            Console.WriteLine("You Selected Hard Mode");
            Difficulty = "Hard";
            GameVersion();
            menu = false;
        }
        else if(input == 3)
        {
            Console.WriteLine("You Selected Leaderboard");
            LeaderboardEntry.ViewLeaderboard();
            Console.WriteLine("\nWould you like to return to the menu? y/n");
            string userInput = Console.ReadLine().ToLower();
            if(userInput == "n")
                {
                    Console.WriteLine("--Goodbye--");
                    Environment.Exit(0);
                }
        }
        else if(input == 4)
        {
            menu = false;
            Environment.Exit(0);
            
        }
        else
        {
            Console.WriteLine("You made an invalid selection");
        }
        }
        
    }
    /// <summary>
    /// Forces the player to pick a game version.
    /// </summary>
    /// <remarks>
    /// Forces any wrong inputs to be invalid.
    /// </remarks>
    public static void GameVersion()
    {
        Console.WriteLine("What version of the game would you like to play?\n 1) 4 digit code\n 2) 5 digit code\n 3) 6 digit code");
        
        bool menu = true;
        while(menu == true)
        {
            int input;
            try
            {
                input = Convert.ToInt16(Console.ReadLine()); // Converts to Int 16 to save memory space
            }
            catch
            {
                Console.WriteLine("Invalid Selection, Please try again");
                continue;
            }
            if(input == 1)
            {
                Console.WriteLine("You Selected 4 Digit Code");
                menu = false;
                Length = 4;

            }
            else if(input == 2)
            {
                
                Console.WriteLine("You Selected 5 Digit Code");
                menu = false;
                Length = 5;
            }
            else if(input == 3)
            {
                Console.WriteLine("You Selected 6 Digit Code");
                menu = false;
                Length = 6;
            }
            else
            {
                Console.WriteLine("You Entered an Invalid Integer");
            }
        }
        
    }
    /// <summary>
    /// Allows the code to start running once the file is called.
    /// </summary>
    /// <remarks>
    /// States the rules of the game with examples
    /// then calls the functions needed to start the game.
    /// </remarks>
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
    /// <summary>
    /// Loads all leaderboard entries from the leaderboard file
    /// </summary>
    /// <returns>
    /// A list of LeaderboardEntry objects containing
    /// all saved leaderboard data
    /// </returns>
    /// <remarks>
    /// If the leaderboard file doesnt exist, an empty list gets returned.
    /// Each line in the file is a new entry
    /// </remarks>
    public static List<LeaderboardEntry> LoadLeaderboard()
    {
        if (!File.Exists(path))
        {
            return new List<LeaderboardEntry>();
        }
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

    /// <summary>
    /// Saves the provided leaderboard entries to the file.
    /// </summary>
    /// <param name="entries">
    /// A list of Leaderboard entries to be written to the file
    /// </param>
    /// <remarks>
    /// Existing leaderboard data is overwritten.
    /// each entry is saved in csv format.
    /// </remarks>
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

    /// <summary>
    /// Sorts all leaderboard entries by their time taken in ascending order.
    /// </summary>
    /// <param name="entries">
    /// A list of leaderboard entries to be sorted.
    /// </param>
    /// <remarks>
    /// A bubble sort algorithm is used to arrange entries
    /// in ascending order so that fastest completion time appears first.
    /// </remarks>
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

    /// <summary>
    /// Loads, sorts and saves the leaderboard entries.
    /// </summary>
    /// <remarks>
    /// The leaderboard is sorted by time taken then saved back to the file.
    /// After sorting, the user is returned to the main menu.
    /// </remarks>
    public static void SortLeaderboard()
    {
        List<LeaderboardEntry> entries = LoadLeaderboard();

        SortByTime(entries);

        SaveLeaderboard(entries);

        Console.WriteLine("Leaderboard Sorted");
        Game.RestartGame();
    }
    /// <summary>
    /// Displays all leaderboard entries to the console.
    /// </summary>
    /// <remarks>
    /// Entries have been sored previously so are in order.
    /// All stored data for each entry is output.
    /// </remarks>
    public static void ViewLeaderboard()
    {
        List<LeaderboardEntry> entries = LoadLeaderboard();
        Console.WriteLine("   Name  Difficulty  Time  Hint  Turn");
        int num = 0;
        using (StreamReader reader = new StreamReader(path, true))
        {
            foreach(LeaderboardEntry i in entries)
            {
                num++;
                Console.WriteLine($"{num}) {i.Name}  {i.Difficulty}  {i.TimeTaken}  {i.HintUsed}  {i.Turn}");
            }
        }
    }
}

