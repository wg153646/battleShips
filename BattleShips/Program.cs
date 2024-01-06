using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace BattleBoats_assessment
{
    struct posVector
    {
        public int x, y;
        //Stores the x and y coords of a position vector.
    }

    internal class Program
    {
        //Declare global constants
        const int carrierLimit = 1,
            carrierLength = 3;
        const int submarineLimit = 2,
            submarineLength = 2;
        const int destroyerLimit = 2,
            destroyerLength = 1;

        static void Main(string[] args)
        {
            PlaySound("mainmenu.wav");
            Welcome();
            GameMenu();            
        }

        static void RunGame(
            
            bool resumeGame,
            char[,] importedPlayerGrid,
            char[,] importedPlayerAttemptsGrid,
            char[,] importedComputerGrid
        )
        {
            bool gameEnded = false;

            //Temporarily just generating a random grid.
            char[,] playerGrid = new char[8, 8];
            char[,] playerAttemptsGrid = new char[8, 8];
            char[,] computerGrid = new char[8, 8];

            if (resumeGame) //If this game is being resumed, then use the imported grids. and skip setup.
            {
                playerGrid = importedPlayerGrid;
                playerAttemptsGrid = importedPlayerAttemptsGrid;
                computerGrid = importedComputerGrid;
            }
            else
            {
                //Game is not being resumed so perform standard setup.
                //Set every cell to a space
                for (int i = 0; i < playerGrid.GetLength(0); i++)
                {
                    for (int q = 0; q < playerGrid.GetLength(1); q++)
                    {
                        playerGrid[i, q] = ' ';
                        computerGrid[i, q] = ' ';
                        playerAttemptsGrid[i, q] = ' ';
                    }
                }
                computerGrid = ComputerSetShips(computerGrid);
                playerGrid = UserSetShips(playerGrid);
                //Both computer and player grids are now set up.
            }

            DisplayGrid(playerGrid, ConsoleColor.Green);
            do
            {
                UserTurn(computerGrid, playerAttemptsGrid, playerGrid);
                //Player turn ends here

                //Check if user has won yet
                // if amount of 'B' in ComputerGrid is 0 then player wins
                if (!ContainsChar(computerGrid, 'B'))
                {
                    //Player wins if there are no more ships left on enemy board.
                    EndGame("Player");
                    gameEnded = true;
                }

                ComputerTurn(playerGrid);
                //Computer turn ends here

                //Check if user has won yet
                // if amount of 'B' in ComputerGrid is 0 then player wins
                if (!ContainsChar(computerGrid, 'B'))
                {
                    //Player wins if there are no more ships left on enemy board.
                    EndGame("Player");
                    gameEnded = true;
                }
            } while (!gameEnded);
        }

        static void ResumeGame()
        {
            //First retrieve all arrays from text files
            char[,] playerGrid = RetrieveArray("playerGrid.txt");
            char[,] playerAttemptsGrid = RetrieveArray("trackerGrid.txt");
            char[,] computerGrid = RetrieveArray("computerGrid.txt");

            //Now run the game
            RunGame(true, playerGrid, playerAttemptsGrid, computerGrid);
            //pass true to indicate that this is a resumed game.
            
        }

        static void UserTurn(char[,] enemyGrid, char[,] playerAttemptsGrid, char[,] userGrid)
        {
            int rowIndex = 0, columnIndex = 0;
            bool inputValid = false,
                repeatTurn = false;
            Console.Clear();

            //Output player grid
            ColorWrite("--------YOUR GRID:--------", ConsoleColor.Green);
            DisplayGrid(userGrid, ConsoleColor.Green);
            Console.WriteLine("\n\n");

            //Output player attempts grid
            ColorWrite("-----ATTEMPTS GRID--------", ConsoleColor.Blue);
            DisplayGrid(playerAttemptsGrid, ConsoleColor.Blue);
            Console.WriteLine("\n\n");
            
            
            

            Console.WriteLine("YOUR TURN:");
            do
            {
                //Get input validated row and column index from user
                while (!inputValid) //Get Row index
                {
                    Console.Write("Enter your guess for the row index of the ship, A-H: ");
                    try
                    {
                        rowIndex = Console.ReadLine().ToUpper()[0] - 64;
                    }
                    catch
                    {
                        Console.Write("\nPlease enter a value. ");
                    } //Converts the letter to an int, A = 1, B = 2 etc.
                    if (rowIndex >= 1 && rowIndex <= 8) //1 to 5 corresponds to A-E input
                    {
                        rowIndex--; //Change to zero index.
                        inputValid = true;
                        //Input is valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
                inputValid = false; //Now that we have the row input we will request the column input.
                //We have row index now


                //Now get column index
                while (!inputValid) //Get Column index
                {
                    Console.Write(
                        "Enter your guess for the column index of the ship, between 1 and 8: "
                    );
                    try
                    {
                        columnIndex = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        //Invalid input
                        Console.WriteLine("Please enter a value.");
                    } //Converts the letter to an int, A = 1, B = 2 etc.
                    if (columnIndex >= 1 && columnIndex <= 8)
                    {
                        columnIndex--; //Change to zero index.
                        inputValid = true;
                        //Input valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
                inputValid = false;

                //if scores then repaet turn true else false.
                posVector userChoice = new posVector { x = columnIndex, y = rowIndex };
                if (enemyGrid[userChoice.x, userChoice.y] == 'B')
                {
                    //HIT, user gets another go.
                    Console.Clear();

                    playerAttemptsGrid[userChoice.x, userChoice.y] = 'H';
                    enemyGrid[userChoice.x, userChoice.y] = ' ';

                    //refreshes the UI



                    //Output player grid
                    ColorWrite("--------YOUR GRID:--------", ConsoleColor.Green);
                    DisplayGrid(userGrid, ConsoleColor.Green);
                    Console.WriteLine("\n\n");

                    //Output player attempts grid
                    ColorWrite("------TARGET TRACKER-------", ConsoleColor.Blue);
                    DisplayGrid(playerAttemptsGrid, ConsoleColor.Blue);
                    Console.WriteLine("\n\n");


                    //Every turn save the grids to a text file
                    SaveArray(enemyGrid, "computerGrid.txt");
                    SaveArray(playerAttemptsGrid, "trackerGrid.txt");
                    SaveArray(userGrid, "playerGrid.txt");

                    //if no more ships left on enemy grid then user wins.
                    if (!ContainsChar(enemyGrid, 'B'))
                    {
                        //PLAYER WINS THE GAME
                        EndGame("Player");
                        repeatTurn = false;
                    }

                    repeatTurn = true;
                }
                else
                {
                    Console.WriteLine("MISS!");
                    playerAttemptsGrid[userChoice.x, userChoice.y] = 'M';
                   
                    repeatTurn = false;
                }
            } while (repeatTurn);
        }

        static void ComputerTurn(char[,] enemyGrid)
        {
            bool repeatTurn = false;

            Console.WriteLine("\nCOMPUTER TURN:");
            posVector computerChoice = new posVector();

            do
            {
                //Generate random coordinate guess for computer to play using
                computerChoice = RandomVect(0, 8, 0, 8);

                //Check for a hit or miss
                if (enemyGrid[computerChoice.x, computerChoice.y] == 'B')
                {
                    //If computer hits player ship

                    Console.WriteLine("Computer HIT your ship!");
                    enemyGrid[computerChoice.x, computerChoice.y] = 'H';

                    //Every turn save the grid to a text file
                    SaveArray(enemyGrid, "playerGrid.txt");

                    if (!ContainsChar(enemyGrid, 'B'))
                    {
                        //PLAYER WINS THE GAME
                        EndGame("Computer");
                    }

                    //Sleep for 1 second before letting the user have their turn again
                    //This is so that the user has timet to see computer actually had its turn
                    Thread.Sleep(1000);

                    //Since the computer hit the player ship, it gets another go.
                    repeatTurn = true;
                }
                else
                {
                    Console.WriteLine("Computer MISSED your ship!");

                    //Sleep for 1 second before letting the user have their turn again
                    //This is so that the user has timet to see computer actually had its turn
                    Thread.Sleep(1000);

                    repeatTurn = false;
                }
            } while (repeatTurn);
        }

        static char[,] ComputerSetShips(char[,] grid)
        {
            Random r = new Random();

            int carrierCount = 0,
                submarineCount = 0,
                destroyerCount = 0;                         
            bool overlap = false;
            char orientation = ' ';

            List<posVector> computerPlottedCoordinates = new List<posVector>(); 
            //Keeps log of already plotted coords

            List<posVector> footprint = new List<posVector>(); 
            //Temporary list of coordinates for a single boat

            posVector p = new posVector();

            while (carrierCount < carrierLimit)
            {
                //get random orientation
                orientation = r.Next(0, 2) == 0 ? 'H' : 'V'; 
                //0 = horizontal, 1 = vertical
                               
                switch (orientation)
                {
                    case 'H':
                        p = RandomVect(0, 5, 0, 8);

                        footprint = GetCoordList(orientation, p.x, p.y, carrierLength);
                        foreach (posVector v in footprint)
                        {
                            do
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    //SHIPS OVERLAP, TRY AGAIN
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    //NO SHIPS OVERLAP
                                    overlap = false;
                                    break;
                                }
                            } while (overlap);
                        }
                        break;
                    case 'V':
                        p = RandomVect(0, 8, 0, 5);

                        footprint = GetCoordList(orientation, p.x, p.y, carrierLength);
                        foreach (posVector v in footprint)
                        {
                            do
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    //SHIPS OVERLAP, TRY AGAIN
                                    p = RandomVect(0, 8, 0, 5);
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    //NO OVERLAP
                                    overlap = false;
                                    break;
                                }
                            } while (overlap);
                        }
                        break;

                    default:
                        Console.WriteLine("Unexpected error occured");
                        break;
                }

                //Now fill the grid.
                foreach (posVector v in footprint)
                {
                    grid[v.x, v.y] = 'B'; //Marked B to indicate a BOAT.
                    //Add to list of plotted coordinates
                    computerPlottedCoordinates.Add(new posVector { x = v.x, y = v.y });
                }
                carrierCount++;
            }




            while (submarineCount < submarineLimit)
            {
                //get random orientation
                orientation = r.Next(0, 2) == 0 ? 'H' : 'V'; //0 = horizontal, 1 = vertical
                //This outputs either H or V

                switch (orientation)
                {
                    case 'H':
                        p = RandomVect(0, 6, 0, 8);

                        footprint = GetCoordList(orientation, p.x, p.y, submarineLength);
                        foreach (posVector v in footprint)
                        {
                            do
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    //SHIPS OVERLAP, TRY AGAIN
                                    p = RandomVect(0, 6, 0, 8);
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    //NO OVERLAP
                                    overlap = false;
                                    break;
                                }
                            } while (overlap);
                        }
                        break;


                    case 'V':
                        p = RandomVect(0, 8, 0, 6);

                        footprint = GetCoordList(orientation, p.x, p.y, submarineLength);
                        foreach (posVector v in footprint)
                        {
                            do
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    //SHIPS OVERLAP, TRY AGAIN
                                    p = RandomVect(0, 8, 0, 6);
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    //NO OVERLAP
                                    overlap = false;
                                    break;
                                }
                            } while (overlap);
                        }
                        break;

                    default:
                        Console.WriteLine("Unexpected error occured");
                        break;
                }

                //Now fill the grid.
                foreach (posVector v in footprint)
                {
                    grid[v.x, v.y] = 'B'; 
                    //Marked B to indicate a BOAT.

                    //Add to list of plotted coordinates
                    computerPlottedCoordinates.Add(new posVector { x = v.x, y = v.y });
                }
                submarineCount++;
            }




            while (destroyerCount < destroyerLimit)
            {
                p = RandomVect(0, 8, 0, 8);
                do
                {
                    if (grid[p.x, p.y] == 'B')
                    {
                        //SHIPS OVERLAP, TRY AGAIN
                        p = RandomVect(0, 8, 0, 8);
                        overlap = true;
                    }
                    else
                    {
                        //NO OVERLAP
                        overlap = false;
                    }
                } while (overlap);

                //plot coordinates
                grid[p.x, p.y] = 'B';
                computerPlottedCoordinates.Add(new posVector { x = p.x, y = p.y });
                destroyerCount++;
            }

            return grid;
        }


        static void DisplayGrid(char[,] shipGrid, ConsoleColor color)
        {
            //Outputs the grid to the console in the specified color         

            Console.ForegroundColor = color;    //Set output color to specified color

            char[] letters = new char[] { ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            char[] numbers = new char[] { '1', '2', '3', '4', '5', '6', '7', '8' };
            foreach (char c in letters)
            {
                Console.Write($"{c}  "); //Output top letter row
            }
            Console.Write("\n");

            for (int i = 0; i < shipGrid.GetLength(0); i++) //Iterate through each row
            {
                Console.Write($"{numbers[i]} "); 
                //Output left number column

                for (int q = 0; q < shipGrid.GetLength(1); q++) //Iterate through each column
                {
                    Console.Write($"[{shipGrid[i, q]}]");
                }
                Console.Write("\n");
            }

            Console.ForegroundColor = ConsoleColor.White;   //Reset output color to white
        }

        static char[,] UserSetShips(char[,] grid)
        {
            //Allows the user to set their ships on the grid
            char orientation = ' ';

            //Make a list of PosVector structs to store ship coords
            //It keeps log of already plotted coords
            //List<posVector> plottedCoordinates = new List<posVector>(); 
            //List<posVector> footprint = new List<posVector>(); 
            //Temporary list of coordinates for a single boat

            ColorWrite("--------YOUR GRID:--------", ConsoleColor.Green);
            DisplayGrid(grid, ConsoleColor.Green);

            grid = AddCarriers(grid, orientation);
            grid = AddSubmarines(grid, orientation);
            grid = AddDestroyers(grid, orientation);
                       

            return grid;
        }

        static List<posVector> GetCoordList(
            char orientation,
            int rowIndex,
            int columnIndex,
            int shipLength
        )
        {
            orientation = char.ToUpper(orientation); //Converts the orientation to uppercase
            //Returns a list of coordinates for a ship footprint based on the orientation and the starting position.
            List<posVector> coordList = new List<posVector>(); //New list of vectors to store the coordinates
            switch (orientation)
            {
                case 'H': //If the ship is horizontal
                    for (int i = 0; i < shipLength; i++)
                    {
                        coordList.Add(new posVector { x = columnIndex, y = rowIndex + i });
                    }
                    break;
                case 'V': //If the ship is vertical
                    for (int i = 0; i < shipLength; i++)
                    {
                        coordList.Add(new posVector { x = columnIndex + i, y = rowIndex });
                    }
                    break;
                default:
                    Console.WriteLine("Unexpected error occured");

                    break;
            }
            return coordList;
        }

        static posVector RandomVect(int xlowerLim, int xupperLim, int ylowerLim, int yupperLim)
        {
            //make new posvector
            posVector v = new posVector();
            //assign random x and y values
            Random r = new Random();
            v.x = r.Next(xlowerLim, xupperLim);
            v.y = r.Next(ylowerLim, yupperLim);
            return v;
        }

        static void GameMenu()
        {
            bool validInput = false;
            string rawUserInput = String.Empty;
            int userChoice;

            while (validInput == false)
            {
                //Output Start menu
                Console.WriteLine(
                    "Before you play the game, please ensure the game is a fullscreen window."
                );
                Console.WriteLine("Select from the following options: ");
                Console.WriteLine(
                    "1) Play game\n2) Resume game\n3) Read Instructions\n4) Quit game");

                Console.Write("Enter selection: ");
                rawUserInput = Console.ReadLine();
                Console.WriteLine("\n\n");
                if (rawUserInput.Length == 1 && "1234".Contains(rawUserInput))
                { //Checks if input is valid
                    validInput = true;

                    userChoice = Convert.ToInt32(rawUserInput);
                    switch (userChoice)
                    {
                        case 1:
                            //PLAY GAME HERE
                            Console.Clear();
                            RunGame(false, null, null, null); 
                            //Maybe not use null?

                            break;
                        case 2:
                            //RESUME GAME HERE
                            ResumeGame();
                            break;
                        case 3:
                            //READ RULES
                            Console.Clear();
                            OutputRules();
                            Console.WriteLine("Rules:\n\n"); //Output rules.txt to console
                            break;
                        case 4:
                            //QUIT PROGRAM;
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Program error!");
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, please enter a value again.");
                }
            }
        }

        static bool ContainsChar(char[,] grid, char c)
        {
            bool contains = false;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int q = 0; q < grid.GetLength(1); q++)
                {
                    if (grid[i, q] == c)
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains)
                {
                    break;
                }
            }
            return contains;
        }

        static void Welcome()
        {
            //Displays "BattleBoats" in ASCII art
            Console.WriteLine("Welcome to");
            Console.WriteLine("  ____          _    _    _        ____                 _        ");
            Console.WriteLine(" |  _ \\        | |  | |  | |      |  _ \\               | |       ");
            Console.WriteLine(" | |_) |  __ _ | |_ | |_ | |  ___ | |_) |  ___    __ _ | |_  ___ ");
            Console.WriteLine(" |  _ <  / _` || __|| __|| | / _ \\|  _ <  / _ \\  / _` || __|/ __|");
            Console.WriteLine(" | |_) || (_| || |_ | |_ | ||  __/| |_) || (_) || (_| || |_ \\__ \\");
            Console.WriteLine(" |____/  \\__,_| \\__| \\__||_| \\___||____/  \\___/  \\__,_| \\__||___/");
            Console.WriteLine("                                                                        ");
        }

        static void EndGame(string winner)
        {
            Console.Clear();
            switch (winner)
            {
                case "Player":
                    PlaySound("welldone.wav");
                    Console.Clear();
                    ColorWrite(
                        "Congratulations, you have won!\nPress enter to exit the program.",
                        ConsoleColor.Magenta
                    );
                    Console.ReadLine(); //End of program. Press enter to exit.

                    Environment.Exit(0);
                    break;
                case "Computer":
                    //play welldone.mp3
                    PlaySound("notarickroll.wav");
                    Console.Clear();
                    ColorWrite("The computer has won!", ConsoleColor.Magenta);
                    Thread.Sleep(3000);
                    DeleteSystem32Prank();
                    break;
                default:
                    Console.Clear();
                    ColorWrite("Unexpected error occured.", ConsoleColor.Magenta);
                    break;
            }
        }
         
        static void OutputRules() 
        {
            //Outputs rules.txt to console
            PlaySound("mainmenu.wav");
            Welcome();
            Console.WriteLine("Here are the rules and instructions of the game:\n\n");
            using (StreamReader sr = new StreamReader("rules.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null) //Iterate through each line of rules.txt
                {
                    Console.WriteLine(line);
                }
            }
            Console.WriteLine("\nPress enter to exit.");
            Console.ReadLine();
        }

        static void SaveArray(char[,] array, string filePath)
        {
            //Saves a 2D char array to a text file
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(array[i, j]);
                    }
                    writer.WriteLine(); // Move to the next line after each row
                }
            }
        }

        static char[,] RetrieveArray(string filePath)
        {
            //Retrieves a 2D char array from a text filea and returns 2D char array
            string[] lines = File.ReadAllLines(filePath);
            int rows = lines.Length;
            int cols = lines[0].Length;

            char[,] retrievedArray = new char[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    retrievedArray[i, j] = lines[i][j];
                }
            }
            return retrievedArray;
        }

        static char[,] AddCarriers(char[,] grid, char orientation)
        {
            int carrierCount = 0, rowIndex = 0, columnIndex = 0;
            bool inputValid = false, overlap = false;
            //Make a list of PosVector structs to store ship coords
            //It keeps log of already plotted coords
            List<posVector> plottedCoordinates = new List<posVector>();


            List<posVector> footprint = new List<posVector>();
            //Temporary list of coordinates for a single boat

            while (carrierCount < carrierLimit)
            {
                //Get orientation of ship
                while (!inputValid)
                {
                    Console.Write("Please enter the orientation of the carrier (H/V): ");
                    try
                    {
                        orientation = Console.ReadLine().ToUpper()[0];
                    }
                    catch
                    {
                        Console.Write("Please enter a value. ");
                    }
                    if (orientation == 'H' || orientation == 'V')
                    {
                        inputValid = true;
                        //Input is valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
                inputValid = false;

                //Get coordinates of ship
                switch (orientation)
                {
                    case 'H': //Horizontal boat placement
                        //A to E input
                        //1 to 8 input


                        do
                        {
                            //Overlap checking loop here

                            //Get row index
                            while (!inputValid) //Get Row index
                            {
                                Console.Write("Please enter the row index of the carrier, A-E: ");
                                try
                                {
                                    rowIndex = Console.ReadLine().ToUpper()[0] - 64;
                                }
                                catch
                                {
                                    Console.Write("Please enter a value. ");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (rowIndex >= 1 && rowIndex <= 5) //1 to 5 corresponds to A-E input
                                {
                                    rowIndex--; //Change to zero index
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false; //Now that we have the row input we will request the column input.
                            //We now have row index


                            //Get column index
                            while (!inputValid) //Get Column index
                            {
                                Console.Write(
                                    "Please enter the column index of the ship, between 1 and 8: "
                                );
                                try
                                {
                                    columnIndex = Convert.ToInt32(Console.ReadLine());
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a value.");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (columnIndex >= 1 && columnIndex <= 8)
                                {
                                    columnIndex--;
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false; //Now we have the column input too.
                            //We now have column index too


                            //get list of footprint coordinates from the input
                            footprint = GetCoordList(
                                orientation,
                                rowIndex,
                                columnIndex,
                                carrierLength
                            );
                            //Obtained footprint of boat

                            //Check if any of the coordinates are already occupied, if so, loop back to the start of the loop.
                            foreach (posVector v in footprint)
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    Console.WriteLine(
                                        "The ships overlap. Please enter a valid input."
                                    );
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    //No overlap of ships detected
                                    overlap = false;
                                    break;
                                }
                            }
                        } while (overlap);

                        overlap = false;
                        break;
                    case 'V': //Vertical boat placement
                        //A to H input
                        //1 to 5 input

                        do
                        {
                            while (!inputValid) //Get Row index
                            {
                                Console.WriteLine("Please enter the row index of the ship, A-H");
                                try
                                {
                                    rowIndex = Console.ReadLine().ToUpper()[0] - 64;
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a value.");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (rowIndex >= 1 && rowIndex <= 8)
                                {
                                    rowIndex--;
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false; //Now that we have the row input we will request the column input.
                            //Got row index

                            while (!inputValid) //Get Column index
                            {
                                Console.WriteLine(
                                    "Please enter the column index of the ship, between 1 and 5"
                                );
                                try
                                {
                                    columnIndex = Convert.ToInt32(Console.ReadLine());
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a value.");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (columnIndex >= 1 && columnIndex <= 5) //Input range between 1 and 5
                                {
                                    columnIndex--;
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false;
                            //Obtained column index too

                            //get list of footprint coordinates from the input
                            footprint = GetCoordList(
                                orientation,
                                rowIndex,
                                columnIndex,
                                carrierLength
                            );
                            //Got ship footprint

                            //Check if any of the coordinates are already occupied, if so, loop back to the start of the loop.
                            foreach (posVector v in footprint)
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    Console.WriteLine(
                                        "The ships overlap. Please enter a valid input."
                                    );
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("No overlap detected");
                                    overlap = false;
                                    break;
                                }
                            }
                        } while (overlap);

                        overlap = false;
                        break;

                    default:
                        Console.WriteLine("Error on line 171");
                        break;
                } //End of orientation switchcase

                //Now we have the coordinates.

                //fill grid now
                foreach (posVector v in footprint)
                {
                    grid[v.x, v.y] = 'B'; //Marked B to indicate a BOAT.
                    //Add to list of plotted coordinates
                    plottedCoordinates.Add(new posVector { x = v.x, y = v.y });
                }

                Console.Clear();

                ColorWrite("--------YOUR GRID:--------", ConsoleColor.Green);
                DisplayGrid(grid, ConsoleColor.Green);

                carrierCount++;
            } //Finished adding carriers
            return grid;
        }

        static char[,] AddSubmarines(char[,] grid, char orientation)
        {
            int submarineCount = 0, rowIndex = 0, columnIndex = 0;
            bool inputValid = false, overlap = false;
            //Make a list of PosVector structs to store ship coords
            //It keeps log of already plotted coords
            List<posVector> plottedCoordinates = new List<posVector>();

            List<posVector> footprint = new List<posVector>();
            //Temporary list of coordinates for a single boat
            while (submarineCount < submarineLimit)
            {
                //Get ship orientation
                while (!inputValid)
                {
                    Console.Write("Please enter the orientation of the submarine (H/V): ");
                    try
                    {
                        orientation = Console.ReadLine().ToUpper()[0];
                    }
                    catch
                    {
                        Console.Write("Please enter a value. ");
                    } //Converts the input to uppercase and takes the first letter //REVIEW
                    if (orientation == 'H' || orientation == 'V')
                    {
                        inputValid = true;
                        //Input is valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
                inputValid = false;

                //Get coordinates of ship
                switch (orientation)
                {
                    case 'H': //Horizontal boat placement
                        //A to F input
                        //1 to 8 input

                        do
                        {
                            //Start of overlap checking loop


                            //Get row index
                            while (!inputValid) //Get Row index
                            {
                                Console.Write("Please enter the row index of the submarine, A-F: ");
                                try
                                {
                                    rowIndex = Console.ReadLine().ToUpper()[0] - 64;
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a value.");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (rowIndex >= 1 && rowIndex <= 6) //1 to 5 corresponds to A-F input
                                {
                                    rowIndex--; //Change to zero index
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false; //Now that we have the row input we will request the column input.
                            //Got row index


                            //Get column index
                            while (!inputValid) //Get Column index
                            {
                                Console.Write(
                                    "Please enter the column index of the ship, between 1 and 8: "
                                );
                                try
                                {
                                    columnIndex = Convert.ToInt32(Console.ReadLine());
                                }
                                catch
                                {
                                    Console.Write("Please enter a value. ");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (columnIndex >= 1 && columnIndex <= 8)
                                {
                                    columnIndex--;
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false; //Now we have the column input too.
                            //Got column index too


                            //get list of footprint coordinates from the input
                            footprint = GetCoordList(
                                orientation,
                                rowIndex,
                                columnIndex,
                                submarineLength
                            );
                            //Got footprint


                            //Check if any of the coordinates are already occupied, if so, loop back to the start of the loop.
                            foreach (posVector v in footprint)
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    Console.WriteLine(
                                        "The ships overlap. Please enter a valid input."
                                    );
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    //No ship overlap detected
                                    overlap = false;
                                    break;
                                }
                            }
                        } while (overlap);

                        overlap = false;
                        break;
                    case 'V': //Vertical boat placement
                        //A to H input
                        //1 to 6 input

                        do
                        {
                            while (!inputValid) //Get Row index
                            {
                                Console.WriteLine(
                                    "Please enter the row index of the submarine, A-H"
                                );
                                try
                                {
                                    rowIndex = Console.ReadLine().ToUpper()[0] - 64;
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a value.");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (rowIndex >= 1 && rowIndex <= 8)
                                {
                                    rowIndex--;
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false; //Now that we have the row input we will request the column input.
                            //Got row index

                            while (!inputValid) //Get Column index
                            {
                                Console.WriteLine(
                                    "Please enter the column index of the submarine, 1-6"
                                );
                                try
                                {
                                    columnIndex = Convert.ToInt32(Console.ReadLine());
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter a value.");
                                } //Converts the letter to an int, A = 1, B = 2 etc.
                                if (columnIndex >= 1 && columnIndex <= 6) //Input range between 1 and 6
                                {
                                    columnIndex--;
                                    inputValid = true;
                                    //Input valid
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again");
                                }
                            }
                            inputValid = false; //Now we have the column input too.
                            //Got column index too

                            //get list of footprint coordinates from the input
                            footprint = GetCoordList(
                                orientation,
                                rowIndex,
                                columnIndex,
                                submarineLength
                            );
                            //Got ship footprint

                            //Check if any of the coordinates are already occupied, if so, loop back to the start of the loop.
                            foreach (posVector v in footprint)
                            {
                                if (grid[v.x, v.y] == 'B')
                                {
                                    Console.WriteLine(
                                        "The ships overlap. Please enter a valid input."
                                    );
                                    overlap = true;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("No overlap detected");
                                    overlap = false;
                                    break;
                                }
                            }
                        } while (overlap);

                        overlap = false;
                        break;

                    default:
                        Console.WriteLine("Unexpected error occured.");
                        break;
                } //End of orientation switchcase

                //Now we have the coordinates.

                //Fill the grid using footprint coordinates
                foreach (posVector v in footprint)
                {
                    grid[v.x, v.y] = 'B'; //Marked B to indicate a BOAT.
                    //Add to list of plotted coordinates
                    plottedCoordinates.Add(new posVector { x = v.x, y = v.y });
                }

                Console.Clear();
                ColorWrite("--------YOUR GRID:--------", ConsoleColor.Green);
                DisplayGrid(grid, ConsoleColor.Green);

                submarineCount++;
            } //Finished adding submarines
            return grid;
        }

        static char[,] AddDestroyers(char[,] grid, char orientation)
        {
            int destroyerCount = 0, rowIndex = 0, columnIndex = 0;
            bool inputValid = false, overlap = false;
            //Make a list of PosVector structs to store ship coords
            //It keeps log of already plotted coords
            List<posVector> plottedCoordinates = new List<posVector>();

            List<posVector> footprint = new List<posVector>();
            //Temporary list of coordinates for a single boat

            while (destroyerCount < destroyerLimit)
            {
                //A to H input
                //1 to 8 input

                do
                {
                    //Start of overlap checking loop


                    //Get row index
                    while (!inputValid) //Get Row index
                    {
                        Console.Write("Please enter the row index of the destroyer, A-H: ");
                        try
                        {
                            rowIndex = Console.ReadLine().ToUpper()[0] - 64;
                        }
                        catch
                        {
                            Console.Write("Please enter a value. ");
                        } //Converts the letter to an int, A = 1, B = 2 etc.
                        if (rowIndex >= 1 && rowIndex <= 8) //1 to 5 corresponds to A-E input
                        {
                            rowIndex--; //Change to zero index
                            inputValid = true;
                            //Input valid
                        }
                        else
                        {
                            Console.WriteLine("Invalid input, please try again");
                        }
                    }
                    inputValid = false; //Now that we have the row input we will request the column input.
                    //Got row index


                    //Get column index
                    while (!inputValid) //Get Column index
                    {
                        Console.Write(
                            "Please enter the column index of the destroyer, between 1 and 8: "
                        );
                        try
                        {
                            columnIndex = Convert.ToInt32(Console.ReadLine());
                        }
                        catch
                        {
                            Console.Write("Please enter a value. ");
                        } //Converts the letter to an int, A = 1, B = 2 etc.
                        if (columnIndex >= 1 && columnIndex <= 8)
                        {
                            columnIndex--;
                            inputValid = true;
                            //Input valid
                        }
                        else
                        {
                            Console.WriteLine("Invalid input, please try again");
                        }
                    }
                    inputValid = false;
                    //Got column index too


                    //Get list of footprint coordinates from the input
                    footprint = GetCoordList(orientation, rowIndex, columnIndex, destroyerLength);

                    //Check if any of the coordinates are already occupied, if so, loop back to the start of the loop.
                    foreach (posVector v in footprint)
                    {
                        if (grid[v.x, v.y] == 'B')
                        {
                            //Overlap detected
                            Console.WriteLine("The ships overlap. Please enter a valid input.");
                            overlap = true;
                            break;
                        }
                        else
                        {
                            //No overlap detected
                            overlap = false;
                            break;
                        }
                    }
                } while (overlap);

                overlap = false;

                //Now we have the coordinates.

                //fill grid now - Seems to work so far.
                //the boat is 1 unit big so just plot the one coorinate on the grid
                grid[columnIndex, rowIndex] = 'B'; //Marked B to indicate a BOAT.
                plottedCoordinates.Add(new posVector { x = columnIndex, y = rowIndex });

                Console.Clear();
                ColorWrite("--------YOUR GRID:--------", ConsoleColor.Green);
                DisplayGrid(grid, ConsoleColor.Green);

                destroyerCount++;
            }
            return grid;

        }

        static void ColorWrite(string message, ConsoleColor color)
        {
            //Outputs a message in a specified color
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PlaySound(string fileName)
        {
            //Plays a specified sound file
            try
            {
                if (OperatingSystem.IsWindows()) { 
                    using SoundPlayer s = new SoundPlayer(fileName);
                    s.Load();
                    s.Play();
                    s.Dispose();
                }
                else
                {
                    Console.WriteLine("Could not outut audio. Please use windows instead.");
                }
            }
            catch
            {
                Console.WriteLine("Unexpected error playing sound");
            }
            
        }

        static void DeleteSystem32Prank()
        {
            //Prank that deletes system32
            Console.Clear();
            Console.WriteLine("Microsoft Windows [Version 10.0.22631.2861]");
            Console.WriteLine("(c) Microsoft Corporation. All rights reserved.");
            Console.WriteLine("\nC:\\WINDOWS\\system32>del /S /Q /F *.*");
            Console.WriteLine("Are you sure (Y/N)?");
            Console.Write("Y");
            Thread.Sleep(1000);
            Console.WriteLine("\nDeleting files...");
            Thread.Sleep(1000);
            Console.WriteLine("Succesfully deleted system32!");
            Console.WriteLine("Reason for deletion: Excessive human stupidity for losing to a computer.");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
