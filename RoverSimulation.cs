using System;
using System.Linq;
using System.Text;
using System.IO;

namespace RoverSimulation
{
    class Rover
    {
        int startX;
        int startY;
        string startOrientation;
        int currX;
        int currY;
        Plateau plateau;
        string currOrientation;
        string[] orientations = { "N", "E", "S", "W" };
        bool verbose;

        public Rover(Plateau p, string start, bool v)
        {
            //store a record of the plateau this rover exists on
            plateau = p;

            //split currPosition into current/start x and y and the orientation
            string[] currPositions = start.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
            startX = currX = Int32.Parse(currPositions[0]);
            startY = currY = Int32.Parse(currPositions[1]);
            startOrientation = currOrientation = currPositions[2];

            verbose = v;
        }

        public void commandRover(string instructions)
        {

            for (int i = 0; i < instructions.Count(); i++)
            {
                if ( Char.ToUpper(instructions[i]).Equals('L') )
                {
                    turnLeft();
                }
                else if ( Char.ToUpper(instructions[i]).Equals('R') )
                {
                    turnRight();
                }
                else if ( Char.ToUpper(instructions[i]).Equals('M') )
                {
                    moveForward();
                }
                else
                {
                    //invalid insturction - notify the user and skip to next instruction
                    throw new ArgumentException( "Invalid instruction (" + instructions[i] + ") cannot complete the moves for this rover.");
                }

                if ( plateau.isOutOfBounds(currX, currY) )
                {
                    //the rover has crashed and burned, wave good bye!
                    throw new ArgumentException( "Due to bad instructions, this rover has gone over the edge..." );
                }

                if (i == instructions.Count() - 1)
                {
                    //Changed the wording of the log statement if it's the final instruction
                    if ( verbose )
                    {
                        Console.WriteLine("\nThe final instruction: " + instructions[i]);
                    }
                     
                    //print out the final position of the rover
                     Console.WriteLine("The final position is: " + getPosition());
                }
                else if ( verbose )
                {
                    //let the user know the current position based on the instructions that have been
                    //given so far.
                    Console.WriteLine("\nCurrent instruction: " + instructions[i]);
                    Console.WriteLine("The rover's current position is: " + getPosition());
                }
            }
        }

        //turn the rover right
        public void turnRight()
        {
            //the orientations array contains [ N, E, S, W ] -- get index of current Orientation
            int index = Array.IndexOf(orientations, currOrientation);

            //if current orientation is W, start at beginning of array which is N; otherwise add one to turn right
            currOrientation = index == 3 ? orientations[0] : orientations[index + 1];
        }

        //turn the rover left
        public void turnLeft()
        {
            //the orientations array contains [ N, E, S, W ] -- get index of current Orientation
            int index = Array.IndexOf(orientations, currOrientation);

            //if current orientation is N, start at end of array which is W; otherwise subtract one to turn left
            currOrientation = index == 0 ? orientations[3] : orientations[index - 1];
        }

        //move the rover forward based on it's orientation
        public void moveForward()
        {
            if (currOrientation.Equals("N"))
            {
                currY += 1;
            }
            else if (currOrientation.Equals("S"))
            {
                currY -= 1;
            }
            else if (currOrientation.Equals("E"))
            {
                currX += 1;
            }
            else if (currOrientation.Equals("W"))
            {
                currX -= 1;
            }
        }

        

        //returns a string containing the current position and orientation of the rover
        public string getPosition()
        {
            return currX + " " + currY + " " + currOrientation;
        }

    }

    class Plateau
    {
        int minX = 0;
        int minY = 0;
        int maxX;
        int maxY;

        public Plateau(string maxIndices)
        {
            //split max indices to start as maxX and maxY
            string[] maxInds = maxIndices.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
            maxX = Int32.Parse(maxInds[0]);
            maxY = Int32.Parse(maxInds[1]);
        }

        public bool isOutOfBounds( int x, int y )
        {
            // if currX or currY is negative or a higher than the max point, the rover has fallen off the plateau
            if ( ( x < this.minX ||
                   x > this.maxX ) ||
                 ( y < minY ||
                   y > maxY) )
            {
                return true;
            }

            //not out of bounds, return false
            return false;
        }
    }

    class RoverSimulation
    {
        static void Main(string[] args)
        {            
            int line_count = 1;
            int rover_count = 1;
            bool verbose;

            //make sure that an input file was passed in
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: RoverSimulation.exe c:\\path\\to\\input_file.txt");
                return;
            }
            if (!File.Exists(@args[0]))
            {
                Console.WriteLine("The specified input ( " + @args[0]+ " ) file can not be read.");
                return;
            }
            //read the input file into an array
             string[] inputFile = File.ReadAllLines(@args[0], Encoding.UTF8);

            //if -v was added to the end, turn on verbose mode
            if (args.Length == 2 && @args[1].Equals("-v"))
            {
                verbose = true;
            }
            else
            {
                verbose = false;
            }

            //don't do anything if there's no data
            if (inputFile.Length == 0)
            {
                Console.WriteLine("The input file has no valid data.");
                return;
            }

            //Create the plateau dimension
            string topRightIndices = inputFile[0];
            Plateau plateau = new Plateau(topRightIndices);

            while (line_count < inputFile.Length)
            {
                Console.WriteLine("\nMoving Rover " + rover_count + ":" +
                                  "\nStarting Coordinates: " + inputFile[line_count]);

                //create a new rover, passing the top right indices of the plateau and the starting position of the rover
                Rover rover = new Rover(plateau, inputFile[line_count], verbose);
                //command the rover to move based on the instructions read in
                try
                {
                    rover.commandRover(inputFile[line_count + 1]);
                }
                catch ( ArgumentException e )
                {
                    Console.WriteLine("ArgumentException: {0}", e.Message);
                }

                line_count += 2;
                rover_count++;
            }

            Console.WriteLine("All rovers have been moved!");
        }
    }
}
