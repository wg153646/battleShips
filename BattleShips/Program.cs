using System.Runtime.CompilerServices;

namespace BattleShips
{
    internal class Program
    {
        

        
        static void Main(string[] args)
        {
            char[,] ships = new char[8, 8];
            fillBlank(ships);
            //StartMenu();    //Start menu enters user to run 
            DisplayGrid(RandomizeBoard(ships));
        }

        
        static void DisplayGrid(char[,] shipGrid)
        {
            for (int i = 0; i < shipGrid.GetLength(0); i++)
            {
                for (int q = 0; i < shipGrid.GetLength(1); q++)
                {
                    Console.Write($"[{shipGrid[i, q]}]");
                    
                }
            }
        }
        static void AddShips(char[,] ships)
        {
            char[,] newShips = new char[8, 8];
            for (int i = 0; i < newShips.GetLength(0); i++)
            {
                for (int q = 0; i < newShips.GetLength(1); q++)
                {
                    Console.Write($"[{newShips[i, q]}]");
                }
            }
        } 


        static void StartMenu()
        {
            bool validInput = false;
            string rawUserInput = String.Empty;
            int userChoice;

            while (validInput == false)
            {
                Console.WriteLine("Welcome to battleships!\n\n");
                Console.WriteLine("Select from the following options: ");
                Console.WriteLine("1) Play game\n2) Resume game\n3) Read Instructions\n4)Quit game");

                Console.Write("Enter selection: ");
                rawUserInput = Console.ReadLine();
                Console.WriteLine("\n\n");
                if (rawUserInput.Length==1 && "1234".Contains(rawUserInput)){ //Checks if input is valid
                    validInput = true;
                    
                    userChoice = Convert.ToInt32(rawUserInput);
                    switch (userChoice){
                        case 1:
                            //PLAY GAME HERE

                            ;
                            break;
                        case 2:
                            //RESUME GAME HERE
                            ;
                            break;
                        case 3:
                            //READ RULES
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
                else { Console.Clear(); Console.WriteLine("Invalid input, please enter a value again.");}
            }
                      


        }


        static void OutputRules()   //Outputs rules.txt to console
        {
            using (StreamReader sr = new StreamReader("rules.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
                
            }
        }

        static char[,] RandomizeBoard(char[,] board)
        {
            //Start off by placing 2x destroyers (1 cell each)
            int destroyerCount=0, submarineCount=0, carrierCount=0; //Amount of these boats currently on the board
            int destroyerQty=2, submarineQty=2, carrierQty=1;   //Amount of boats there should be on the board

            int xMin=0, xMax=8, yMin=0, yMax=8;

            //First fill carriers, then submarines, then destroyers
            while (carrierCount != carrierQty)
            {
                // 1) Get random orientation for carrier
                // 2)Switch case to get position vector scope
                // 3)Then place boat on grid

                //1)
                char orientation = RandomOrientation();
                int[] basePositionVect = { };    //Position vector of the end of the boat.
                int[,] cellsToFill = new int[2,1]; //Represnts 3x2 matrix of coordinates
                int borderSpace = 2;//BORDER WIDTH OF LOCI THING
                switch (orientation)
                {
                    case 'n':
                        //Ship points north, so decrease yMax by 2.
                        basePositionVect[0] = RandomVect(xMin, xMax, yMin, yMax - borderSpace)[0];
                        basePositionVect[1] = RandomVect(xMin, xMax, yMin, yMax - borderSpace)[1];

                        //Fill in the 2 cells ABOVE the base too.
                        cellsToFill[0,0] = basePositionVect[0];
                        cellsToFill[0,1] = basePositionVect[1];

                        cellsToFill[1, 0] = basePositionVect[0];
                        cellsToFill[1, 1] = basePositionVect[1] + 1;    //Y plus 1

                        cellsToFill[1, 0] = basePositionVect[0];
                        cellsToFill[1, 1] = basePositionVect[1] + 2;    //Y plus 2

                        break;
                    case 'e':
                        //Ship points east so decrease xMax by 2.
                        basePositionVect[0] = RandomVect(xMin, xMax - borderSpace, yMin, yMax)[0];
                        basePositionVect[1] = RandomVect(xMin, xMax - borderSpace, yMin, yMax)[1];

                        //Fill in the 2 cells RIGHT to the base too.
                        cellsToFill[0, 0] = basePositionVect[0];
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[0, 0] = basePositionVect[0] + 1;    //X Plus 1
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[0, 0] = basePositionVect[0] + 2;    //X Plus 2
                        cellsToFill[0, 1] = basePositionVect[1];
                        break;
                    case 's':
                        //Ship points south so increase yMin by 2.
                        basePositionVect[0] = RandomVect(xMin, xMax, yMin+ borderSpace, yMax)[0];
                        basePositionVect[1] = RandomVect(xMin, xMax, yMin + borderSpace, yMax)[1];
                        //Fill in the 2 cells BELOW the base too.
                        cellsToFill[0, 0] = basePositionVect[0];
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[1, 0] = basePositionVect[0];
                        cellsToFill[1, 1] = basePositionVect[1] - 1;    //Y Minus 1

                        cellsToFill[1, 0] = basePositionVect[0];
                        cellsToFill[1, 1] = basePositionVect[1] - 2;    //Y Minus 2
                        break;
                    case 'w':
                        //Ship points west so increase xMin by 2.
                        basePositionVect[0] = RandomVect(yMin+ borderSpace, xMax, yMin, yMax)[0];
                        basePositionVect[1] = RandomVect(yMin + borderSpace, xMax, yMin, yMax)[1];

                        //Fill in the 2 cells LEFT to the base too.
                        cellsToFill[0, 0] = basePositionVect[0];
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[0, 0] = basePositionVect[0] - 1;    //X Minus 1
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[0, 0] = basePositionVect[0] - 2;    //X Minus 2
                        cellsToFill[0, 1] = basePositionVect[1];

                        break;
                    default:
                        Console.WriteLine("Unexpected error occured.");
                        break;
                    
                }
                carrierCount++; //Increment carrier count
                //Now that we have the cells to fill lets fill them.
                //for c in cell mark cell as a ship.                           

            }

            while (submarineCount != submarineQty)
            {
                // 1) Get random orientation for carrier
                // 2)Switch case to get position vector scope
                // 3)Then place boat on grid

                //1)
                char orientation = RandomOrientation();
                int[] basePositionVect = { };    //Position vector of the end of the boat.
                int[,] cellsToFill = new int[2, 1]; //Represnts 3x2 matrix of coordinates
                int borderSpace = 1;// BORDER WIDTH OF LOCI THING
                switch (orientation)
                {
                    case 'n':
                        //Ship points north, so decrease yMax by 2.
                        basePositionVect[0] = RandomVect(xMin, xMax, yMin, yMax - borderSpace)[0];
                        basePositionVect[1] = RandomVect(xMin, xMax, yMin, yMax - borderSpace)[1];

                        //Fill in the 2 cells ABOVE the base too.
                        cellsToFill[0, 0] = basePositionVect[0];
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[1, 0] = basePositionVect[0];
                        cellsToFill[1, 1] = basePositionVect[1] + 1;    //Y plus 1


                        break;
                    case 'e':
                        //Ship points east so decrease xMax by 2.
                        basePositionVect[0] = RandomVect(xMin, xMax - borderSpace, yMin, yMax)[0];
                        basePositionVect[1] = RandomVect(xMin, xMax - borderSpace, yMin, yMax)[1];

                        //Fill in the 2 cells RIGHT to the base too.
                        cellsToFill[0, 0] = basePositionVect[0];
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[0, 0] = basePositionVect[0] + 1;    //X Plus 1
                        cellsToFill[0, 1] = basePositionVect[1];

                        break;
                    case 's':
                        //Ship points south so increase yMin by 2.
                        basePositionVect[0] = RandomVect(xMin, xMax, yMin + borderSpace, yMax)[0];
                        basePositionVect[1] = RandomVect(xMin, xMax, yMin + borderSpace, yMax)[1];
                        //Fill in the 2 cells BELOW the base too.
                        cellsToFill[0, 0] = basePositionVect[0];
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[1, 0] = basePositionVect[0];
                        cellsToFill[1, 1] = basePositionVect[1] - 1;    //Y Minus 1

                        break;
                    case 'w':
                        //Ship points west so increase xMin by 2.
                        basePositionVect[0] = RandomVect(yMin + borderSpace, xMax, yMin, yMax)[0];
                        basePositionVect[1] = RandomVect(yMin + borderSpace, xMax, yMin, yMax)[1];

                        //Fill in the 2 cells LEFT to the base too.
                        cellsToFill[0, 0] = basePositionVect[0];
                        cellsToFill[0, 1] = basePositionVect[1];

                        cellsToFill[0, 0] = basePositionVect[0] - 1;    //X Minus 1
                        cellsToFill[0, 1] = basePositionVect[1];


                        break;
                    default:
                        Console.WriteLine("Unexpected error occured.");
                        break;
                }
                submarineCount++; //Increment carrier count
                //Now that we have the cells to fill lets fill them.

                //for c in cell mark cell as a ship.

            }

            while (destroyerCount != destroyerQty)
            {
                // 1) Get random orientation for carrier
                // 2)Switch case to get position vector scope
                // 3)Then place boat on grid

                //1)
                char orientation = RandomOrientation();
                int[] basePositionVect = { };    //Position vector of the end of the boat.
                int[,] cellsToFill = new int[2, 1]; //Represnts 3x2 matrix of coordinates


                basePositionVect[0] = RandomVect(xMin, xMax, yMin, yMax)[0];
                basePositionVect[1] = RandomVect(xMin, xMax, yMin, yMax)[1];

                //Fill in the 2 cells ABOVE the base too.
                cellsToFill[0, 0] = basePositionVect[0];
                cellsToFill[0, 1] = basePositionVect[1];

                

                //Now that we have the cells to fill lets fill them.
                for (int i = 0; i < cellsToFill.GetLength(0); i++) {
                    board[i,0] ='e';
                }
                //for c in cell mark cell as a ship.
                destroyerCount++; //Increment carrier count
            }


            return board;

        }


        static char[,] fillBlank(char[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = 'O'; // O Represents empty space.
                }
            }
            return grid;
        }

        static int[] RandomVect(int xMin, int xMax, int yMin, int yMax)
        {
            Random rnd = new Random();
            int x = rnd.Next(xMin, xMax);
            int y = rnd.Next(yMin, yMax);
            return new int[] { x,y };   //Return randomly generated vector
        }
        static char RandomOrientation()
        {
            Random rnd = new Random();
            char[] orientations = { 'n', 'e', 's', 'w' };   //North, East, South, West
            return orientations[rnd.Next(orientations.Length)]; //Return a random choice of the orientation options
        }


        
    }
}