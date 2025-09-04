using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace Utility
{
    //  Utility Guide
    /*
     * The Utility Guide is a program designed to automate many of the tasks I found tedious.
     * I wanted a single script that self-contained everything I needed so that I would very seldom need to import/export anything
     * Below is a list of the sections and capabilities of this script.
     * 
     *      STRUCTS
     * Below is a list of commonly used structures
     * 
     *      Coords: Represents a pair of coordinates on a 2D Matrix
     * 
     *      HANDLER METHODS
     * Below is a list of commonly used objects which can be statically called.
     * 
     *  
     *      >Files: SavES, uploads and downloads files from directories.
     *          >JSONAccess: File Manager for JSON files
     *          
     *      >Lists: Manipulates the contents of arrays and arraylists.
     *      
     *      >Matrices: manages and manipulates 2D arrays
     *          >SearchesAndPathfinding: Performs pathfinding, searches and expansions in a 2D array
     *          >Misc: Misc usage cases for matrices
     *      
     *      >PerlinGen1: Generates pseudorandom 2D matrix noise.
     *      
     *      >Images (TODO): ImageHandler is capable of image manipulation and modification
     * 
     * 
     * **/



    public struct Coords
    {
        public int x { get; set; }      //  Equivalent to ROW
        public int y { get; set; }      //  Equivalent to COL
        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString() => $"({x}, {y})";
    }
    public struct DiceRoll
    {
        //2d6+5
        public int diceNum {get; set;}
        public int diceSize {get; set;}
        public int diceModifier {get; set;}


    }



    public class Files
    {
        public class JSONAccess()
        {
            public static T ReadJsonFile<T>(string filePath)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                }

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"JSON file not found: {filePath}");
                }

                try
                {
                    // Read the JSON file content
                    string jsonContent = File.ReadAllText(filePath, Encoding.UTF8);

                    // Deserialize the JSON content to the specified type
                    T result = JsonSerializer.Deserialize<T>(jsonContent);

                    return result;
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"Failed to parse JSON file: {ex.Message}", ex);
                }
                catch (IOException ex)
                {
                    throw new IOException($"Error reading file: {ex.Message}", ex);
                }
            }
        }


        #region Basic Handlers
        public static readonly string sourceDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

        //  This function converts a string filepath to one that works as a valid filepath
        public static string GetDirectory(string path)
        {
            return sourceDirectory + path;
        }


        //  This function converts a valid directory into a short directory String
        public static string GetDirectoryString(string path)
        {
            // Check if filepath is already dynamic; if true, return path, if false, proceed
            try
            {
                if (!path.Substring(0, sourceDirectory.Length).Equals(sourceDirectory))
                {
                    Console.WriteLine("ERROR: Path does NOT begin with @C:..., returning path");
                    return path;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}: returning valid filepath ");
            }

            string result = path.Remove(0, sourceDirectory.Length);
            return result;
        }


        //  Creates a directory at the specified filepath
        public static void CreateDirectory(string path)
        {
            string filepath = GetDirectory(path);
            try
            {
                //  Test if Directory Exists
                if (Directory.Exists(filepath))
                {
                    Console.WriteLine("Directory " + path + " Already Exists");
                    return;
                }
                //  Create Directory
                DirectoryInfo di = Directory.CreateDirectory(filepath);
                Console.WriteLine("The directory " + path + " was created successfully at {0}.", Directory.GetCreationTime(filepath));
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }


        //  Read all lines of a .txt file, returns an array of Strings
        public static string[] ReadAllLines(string path)
        {
            string filepath = GetDirectory(path);
            string[] strings = File.ReadAllLines(filepath);
            return strings;
        }

        public static void WriteAllLines(string path, string[] contents)
        {
            string filepath = GetDirectory(path);
            File.WriteAllLines(filepath, contents);
        }
        //  Creates an alphabetized list .txt file from specified array at specified location
        #endregion
    }
    public class Lists
    {
        //  Removes the largest list withing a list of any objects
        public static void RemoveLargestList<T>(List<List<T>> listOfLists)
        {
            if (listOfLists == null || listOfLists.Count == 0)
                return;

            int maxIndex = 0;
            int maxCount = listOfLists[0].Count;

            for (int i = 1; i < listOfLists.Count; i++)
            {
                if (listOfLists[i].Count > maxCount)
                {
                    maxIndex = i;
                    maxCount = listOfLists[i].Count;
                }
            }

            listOfLists.RemoveAt(maxIndex);
        }

        //  Given a list of list of any objects, remove lists with an object count ABOVE minSize
        public static void RemoveListAboveSize<T>(List<List<T>> listOfLists, int minSize)
        {
            if (listOfLists == null) throw new ArgumentNullException(nameof(listOfLists));

            listOfLists.RemoveAll(subList => subList.Count > minSize);
        }

        //  Given a list of list of any objects, remove lists with an object count BELOW maxSize
        public static void RemoveListBelowSize<T>(List<List<T>> listOfLists, int maxSize)
        {
            if (listOfLists == null) throw new ArgumentNullException(nameof(listOfLists));

            listOfLists.RemoveAll(subList => subList.Count < maxSize);
        }

        //  Returns a random selection from a list
        public static List<T> GetRandomSelection<T>(List<T> source, int count, bool duplicatable, int seed)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
            if (!duplicatable && count > source.Count)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot exceed source size when duplicates are not allowed.");

            Random rng = new Random(seed);
            List<T> result = new List<T>(count);

            if (duplicatable)
            {
                // Sampling with replacement
                for (int i = 0; i < count; i++)
                {
                    int index = rng.Next(source.Count);
                    result.Add(source[index]);
                }
            }
            else
            {
                // Sampling without replacement (Fisher–Yates shuffle)
                List<T> copy = new List<T>(source);
                for (int i = 0; i < count; i++)
                {
                    int j = rng.Next(i, copy.Count);
                    (copy[i], copy[j]) = (copy[j], copy[i]);
                    result.Add(copy[i]);
                }
            }

            return result;
        }

        //  Shuffle a list into a new list
        public static IList<T> Shuffle<T>(IList<T> input, int seed)
        {
            var copy = new List<T>(input); // copy into a List<T>
            Random rand = new Random(seed);
            int n = copy.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T temp = copy[n];
                copy[n] = copy[k];
                copy[k] = temp;
            }
            return copy;
        }

        

    }
    public class Matrices
    {




        public class SearchesAndPathfinding()
        {
            public class Islands()
            {
                #region Islands Manager
                //  Given a 2D array of integers and an range of intergers, find all "islands" within that range, targets inclusive 
                public static List<List<Coords>> FindIslandsInRange(int[,] array, int targetRangeStart, int targetRangeEnd, bool hWrapping, bool vWrapping)
                {
                    int rows = array.GetLength(0);
                    int cols = array.GetLength(1);

                    bool[,] visited = new bool[rows, cols];
                    List<List<Coords>> islands = new List<List<Coords>>();

                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            if (!visited[r, c] && array[r, c] >= targetRangeStart && array[r, c] <= targetRangeEnd)
                            {
                                List<Coords> island = new List<Coords>();
                                ExploreIsland(array, r, c, targetRangeStart, targetRangeEnd, visited, island, hWrapping, vWrapping);
                                islands.Add(island);
                            }
                        }
                    }

                    return islands;
                }
                private static void ExploreIsland(int[,] array, int startRow, int startCol, int targetRangeStart, int targetRangeEnd, bool[,] visited, List<Coords> island, bool hWrapping, bool vWrapping)
                {
                    int rows = array.GetLength(0);
                    int cols = array.GetLength(1);
                    Queue<Coords> queue = new Queue<Coords>();
                    queue.Enqueue(new Coords(startRow, startCol));

                    while (queue.Count > 0)
                    {
                        Coords current = queue.Dequeue();
                        int r = current.x;
                        int c = current.y;

                        if (visited[r, c]) continue;

                        visited[r, c] = true;
                        island.Add(new Coords(r, c));

                        int[] dr = { -1, 1, 0, 0 };
                        int[] dc = { 0, 0, -1, 1 };

                        for (int i = 0; i < 4; i++)
                        {
                            int newRow = r + dr[i];
                            int newCol = c + dc[i];

                            if (hWrapping)
                            {
                                newCol = (newCol + cols) % cols;
                            }

                            if (vWrapping)
                            {
                                newRow = (newRow + rows) % rows;
                            }

                            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && !visited[newRow, newCol]
                                && array[newRow, newCol] >= targetRangeStart && array[newRow, newCol] <= targetRangeEnd)
                            {
                                queue.Enqueue(new Coords(newRow, newCol));
                            }
                        }
                    }
                }


                //  FindIslandBordersInRange: Given a 2D array of integers and a range of intergers, find the border of all islands within thickness range
                public static List<List<Coords>> FindIslandBordersInRange(int[,] grid, int minimum, int maximum, bool horizontalWrapping, bool verticalWrapping, int range)
                {
                    int rows = grid.GetLength(0);
                    int cols = grid.GetLength(1);

                    bool[,] visited = new bool[rows, cols];
                    var results = new List<List<Coords>>();

                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            if (!visited[r, c] && grid[r, c] >= minimum && grid[r, c] <= maximum)
                            {
                                // BFS to gather island
                                var island = new List<Coords>();
                                var queue = new Queue<Coords>();
                                queue.Enqueue(new Coords(r, c));
                                visited[r, c] = true;

                                while (queue.Count > 0)
                                {
                                    var current = queue.Dequeue();
                                    island.Add(current);

                                    foreach (var dir in Directions)
                                    {
                                        int nr = current.x + dir[0];
                                        int nc = current.y + dir[1];

                                        // Wrapping
                                        if (horizontalWrapping)
                                        {
                                            if (nc < 0) nc = cols - 1;
                                            if (nc >= cols) nc = 0;
                                        }
                                        if (verticalWrapping)
                                        {
                                            if (nr < 0) nr = rows - 1;
                                            if (nr >= rows) nr = 0;
                                        }

                                        if (nr >= 0 && nr < rows && nc >= 0 && nc < cols &&
                                            !visited[nr, nc] &&
                                            grid[nr, nc] >= minimum && grid[nr, nc] <= maximum)
                                        {
                                            visited[nr, nc] = true;
                                            queue.Enqueue(new Coords(nr, nc));
                                        }
                                    }
                                }

                                // Collect edge cells
                                var edgeSet = new HashSet<Coords>();
                                foreach (var cell in island)
                                {
                                    for (int dr = -range; dr <= range; dr++)
                                    {
                                        for (int dc = -range; dc <= range; dc++)
                                        {
                                            if (Math.Abs(dr) + Math.Abs(dc) > range) continue; // Manhattan check

                                            int nr = cell.x + dr;
                                            int nc = cell.y + dc;

                                            // Wrapping
                                            if (horizontalWrapping)
                                            {
                                                if (nc < 0) nc = cols - 1;
                                                if (nc >= cols) nc = 0;
                                            }
                                            if (verticalWrapping)
                                            {
                                                if (nr < 0) nr = rows - 1;
                                                if (nr >= rows) nr = 0;
                                            }

                                            // Bounds check
                                            if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;

                                            // Exclude borders when wrapping disabled
                                            if (!horizontalWrapping && (nc == 0 || nc == cols - 1)) continue;
                                            if (!verticalWrapping && (nr == 0 || nr == rows - 1)) continue;

                                            // Exclude island cells
                                            if (grid[nr, nc] >= minimum && grid[nr, nc] <= maximum) continue;

                                            edgeSet.Add(new Coords(nr, nc));
                                        }
                                    }
                                }

                                results.Add(new List<Coords>(edgeSet));
                            }
                        }
                    }

                    return results;
                }

                private static readonly int[][] Directions = new int[][]
                {
                    new int[]{ 1, 0 },
                    new int[]{ -1, 0 },
                    new int[]{ 0, 1 },
                    new int[]{ 0, -1 }
                };


                #endregion
            }

            public class VoronoiExpansions()
            {
                public static List<List<Coords>> ExpandTerritories( int[,] sourcegrid, List<Coords> seeds, int minimum, int maximum, bool horizontalWrapping, bool verticalWrapping, int seed)
                {
                    int rows = sourcegrid.GetLength(0);
                    int cols = sourcegrid.GetLength(1);

                    int[,] grid = sourcegrid;
                    Utility.Matrices.MatrixModification.Rotate2DArray(grid, 1);
                    Utility.Matrices.MatrixModification.Flip2DArray(grid, true, true);


                    // Ownership grid: -1 = unclaimed, otherwise index of team
                    int[,] owner = new int[rows, cols];
                    for (int r = 0; r < rows; r++)
                        for (int c = 0; c < cols; c++)
                            owner[r, c] = -1;

                    // Result teams
                    List<List<Coords>> teams = new List<List<Coords>>();
                    Queue<(Coords pos, int team)> frontier = new Queue<(Coords, int)>();

                    // Random for deterministic tie-breaking
                    Random rng = new Random(seed);

                    // Initialize seeds
                    for (int i = 0; i < seeds.Count; i++)
                    {
                        Coords s = seeds[i];
                        if (s.x < 0 || s.x >= rows || s.y < 0 || s.y >= cols)
                        {
                            teams.Add(new List<Coords>()); // invalid
                            continue;
                        }

                        int value = grid[s.x, s.y];
                        if (value < minimum || value > maximum)
                        {
                            teams.Add(new List<Coords>()); // invalid
                            continue;
                        }

                        // Valid seed
                        teams.Add(new List<Coords>() { s });
                        owner[s.x, s.y] = i;
                        frontier.Enqueue((s, i));
                    }

                    // Directions (orthogonal only)
                    int[,] dirs = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

                    // BFS level-synchronous
                    while (frontier.Count > 0)
                    {
                        // Collect all expansion candidates for this wave
                        Dictionary<(int, int), List<int>> candidates = new Dictionary<(int, int), List<int>>();
                        int levelCount = frontier.Count;

                        for (int k = 0; k < levelCount; k++)
                        {
                            var (pos, team) = frontier.Dequeue();

                            for (int d = 0; d < 4; d++)
                            {
                                int nx = pos.x + dirs[d, 0];
                                int ny = pos.y + dirs[d, 1];

                                // Wrapping
                                if (horizontalWrapping)
                                {
                                    if (ny < 0) ny = cols - 1;
                                    else if (ny >= cols) ny = 0;
                                }
                                if (verticalWrapping)
                                {
                                    if (nx < 0) nx = rows - 1;
                                    else if (nx >= rows) nx = 0;
                                }

                                // Skip out of bounds if no wrapping
                                if (nx < 0 || nx >= rows || ny < 0 || ny >= cols)
                                    continue;

                                // Already claimed?
                                if (owner[nx, ny] != -1)
                                    continue;

                                // Valid value?
                                int value = grid[nx, ny];
                                if (value < minimum || value > maximum)
                                    continue;

                                var key = (nx, ny);
                                if (!candidates.ContainsKey(key))
                                    candidates[key] = new List<int>();
                                candidates[key].Add(team);
                            }
                        }

                        // Resolve conflicts
                        foreach (var kvp in candidates)
                        {
                            (int cx, int cy) = kvp.Key;
                            List<int> claimers = kvp.Value;

                            int winner;
                            if (claimers.Count == 1)
                            {
                                winner = claimers[0];
                            }
                            else
                            {
                                // Tie-breaker with seeded RNG
                                winner = claimers[rng.Next(claimers.Count)];
                            }

                            owner[cx, cy] = winner;
                            Coords newCoord = new Coords(cx, cy);
                            teams[winner].Add(newCoord);
                            frontier.Enqueue((newCoord, winner));
                        }
                    }


                    return teams;
                }

                public static void VoronoiTestPrint(int[,] grid, List<List<Coords>> territories, List<Coords> sources)
                {
                    int rows = grid.GetLength(0);
                    int cols = grid.GetLength(1);
                    char[,] output = new char[rows, cols];


                    // Mark unclaimed cells with '.'
                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < grid.GetLength(1); j++)
                        {
                            output[i, j] = ' ';
                        }
                    }

                    //  Mark sources with '*'
                    foreach (Coords coords in sources)
                    {
                        output[coords.x, coords.y] = '*';
                    }

                    // Assign a letter to each team
                    int counter = 0;
                    foreach (List<Coords> coordslist in territories)
                    {
                        char symbol = (char)('A' + counter); // A, B, C, ...
                        foreach (Coords value in  coordslist)
                        {
                            output[value.x, value.y] = symbol;
                        }
                        counter++;
                    }

                    // Print the map
                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < grid.GetLength(1); j++)
                        {
                            Console.Write(output[i, j]);
                        }
                        Console.WriteLine();
                    }
                }

            }

            public class Pathfinding()
            {
                //  Given a coords section, find the path
                public static List<Coords> FindPath(List<Coords> coordsList, Coords source, Coords target)
                {
                    List<Coords> path = new List<Coords>();
                    int dx = Math.Sign(target.x - source.x);
                    int dy = Math.Sign(target.y - source.y);

                    int x = source.x;
                    int y = source.y;

                    while (x != target.x || y != target.y)
                    {
                        if (!coordsList.Contains(new Coords(x, y)))
                        {
                            coordsList.Add(new Coords(x, y));
                        }
                        path.Add(new Coords(x, y));

                        if (x != target.x) x += dx;
                        if (y != target.y) y += dy;
                    }

                    if (!coordsList.Contains(target))
                    {
                        coordsList.Add(target);
                    }
                    path.Add(target);

                    return coordsList;
                }
            }


        }

        public class MatrixModification()
        {

            // Rotate 2D array clockwise n times (each rotation = 90 degrees)
            public static T[,] Rotate2DArray<T>(T[,] input, int rotations)
            {
                int rowCount = input.GetLength(0);
                int colCount = input.GetLength(1);

                // Normalize rotations (4 rotations = original array)
                rotations = ((rotations % 4) + 4) % 4;

                T[,] result = input;

                for (int r = 0; r < rotations; r++)
                {
                    T[,] rotated = new T[colCount, rowCount];

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < colCount; j++)
                        {
                            rotated[j, rowCount - 1 - i] = result[i, j];
                        }
                    }

                    result = rotated;
                    rowCount = result.GetLength(0);
                    colCount = result.GetLength(1);
                }

                return result;
            }

            // Flip 2D array vertically and/or horizontally
            public static T[,] Flip2DArray<T>(T[,] input, bool verticalFlip, bool horizontalFlip)
            {
                int rowCount = input.GetLength(0);
                int colCount = input.GetLength(1);

                T[,] result = new T[rowCount, colCount];

                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        int newI = verticalFlip ? rowCount - 1 - i : i;
                        int newJ = horizontalFlip ? colCount - 1 - j : j;

                        result[newI, newJ] = input[i, j];
                    }
                }

                return result;
            }


            //  Given a 2D array of doubles, normalize the array to a new scale
            public static int[,] NormalizeToInteger(double[,] inputArray, int min, int max)
            {
                int row = inputArray.GetLength(0);
                int col = inputArray.GetLength(1);


                int newmax = max;
                int[,] intArray = new int[row, col];

                double minValue = double.MaxValue;
                double maxValue = double.MinValue;

                // Find min and max values in the double array
                for (int x = 0; x < row; x++)
                {
                    for (int y = 0; y < col; y++)
                    {
                        if (inputArray[x, y] < minValue) minValue = inputArray[x, y];
                        if (inputArray[x, y] > maxValue) maxValue = inputArray[x, y];
                    }
                }

                // Normalize and scale values to the new range
                for (int x = 0; x < row; x++)
                {
                    for (int y = 0; y < col; y++)
                    {
                        intArray[x, y] = (int)(min + (inputArray[x, y] - minValue) / (maxValue - minValue) * (newmax - min));
                    }
                }

                return intArray;
            }


            //  Given a List of List of Coords, convert it into an array of values. Each list is represented by an int; unclaimed cells are represented by a zero
            public static int[,] ConvertCoordstoArray(List<List<Coords>> superlistCoords, int rows, int cols)
            {
                int[,] array = new int[rows, cols];
                //  Initialize array

                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        array[i, j] = 0;
                    }
                }

                int ID = 1;
                foreach (List<Coords> coordslist in superlistCoords)
                {
                    foreach (Coords coord in coordslist)
                    {
                        array[coord.x, coord.y] = ID;
                    }
                    ID++;

                }



                return array;
            }
        }


        public class MatrixManipulation()
        {

            #region Select Regions within Matrices
            //  Get a list of Coords within a circular region around a center, in radius radius 
            public static List<Coords> SelectCircleRegion<T>(T[,] array, Coords center, int radius)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);
                List<Coords> coordsList = new List<Coords>();

                for (int r = Math.Max(0, center.x - radius); r <= Math.Min(rows - 1, center.x + radius); r++)
                {
                    for (int c = Math.Max(0, center.y - radius); c <= Math.Min(cols - 1, center.y + radius); c++)
                    {
                        int deltaX = r - center.x;
                        int deltaY = c - center.y;
                        if (deltaX * deltaX + deltaY * deltaY <= radius * radius)
                        {
                            coordsList.Add(new Coords(r, c));
                        }
                    }
                }

                return coordsList;
            }


            //  Get a circular region within an oval
            public static List<Coords> SelectOvalRegion<T>(T[,] array, int radiusHorizontal, int radiusVertical)
            {
                List<Coords> result = new List<Coords>();

                if (array == null || radiusVertical <= 0 || radiusHorizontal <= 0)
                {
                    return result;
                }


                int rows = array.GetLength(0);
                int cols = array.GetLength(1);

                // Calculate center coordinates
                double centerX = (cols - 1) / 2.0;
                double centerY = (rows - 1) / 2.0;

                // Calculate squared radii for comparison (avoiding square roots for performance)
                double radiusVSquared = radiusVertical * radiusVertical;
                double radiusHSquared = radiusHorizontal * radiusHorizontal;

                // Determine the bounding box to iterate through (for efficiency)
                int minRow = Math.Max(0, (int)(centerY - radiusVertical));
                int maxRow = Math.Min(rows - 1, (int)(centerY + radiusVertical));
                int minCol = Math.Max(0, (int)(centerX - radiusHorizontal));
                int maxCol = Math.Min(cols - 1, (int)(centerX + radiusHorizontal));

                // Iterate through the bounding box
                for (int row = minRow; row <= maxRow; row++)
                {
                    for (int col = minCol; col <= maxCol; col++)
                    {
                        // Calculate normalized coordinates relative to center
                        double dx = col - centerX;
                        double dy = row - centerY;

                        // Check if point is inside the oval using ellipse equation
                        // (dx/radiusHorizontal)^2 + (dy/radiusVertical)^2 <= 1
                        if ((dx * dx) / radiusHSquared + (dy * dy) / radiusVSquared <= 1.0)
                        {
                            result.Add(new Coords(col, row));
                        }
                    }
                }

                return result;
            }


            #endregion




            //  Get the maximum column distance within a set of points
            public static int MaxColDistance(List<Coords> points)
            {
                if (points == null || points.Count < 2) return 0;

                int minY = int.MaxValue;
                int maxY = int.MinValue;

                foreach (var p in points)
                {
                    if (p.y < minY) minY = p.y;
                    if (p.y > maxY) maxY = p.y;
                }

                return Math.Abs(maxY - minY);
            }


            //  Get the maximum row distance betweens two points
            public static int MaxRowDistance(List<Coords> points)
            {
                if (points == null || points.Count < 2) return 0;

                int minX = int.MaxValue;
                int maxX = int.MinValue;

                foreach (var p in points)
                {
                    if (p.x < minX) minX = p.x;
                    if (p.x > maxX) maxX = p.x;
                }

                return Math.Abs(maxX - minX);
            }


            


            





        }

        
    }
    

    public static class Perlin
    {
        //Perlin.Generate
        public static double[,] GeneratePerlinNoise(int rows, int cols, double frequency, int seed)
        {
            double[,] noise = new double[rows, cols];
            PerlinNoise perlin = new PerlinNoise(seed);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    // Scale coordinates so larger arrays sample the same underlying noise field
                    double sampleX = (x / (double)cols) * frequency;
                    double sampleY = (y / (double)rows) * frequency;

                    noise[y, x] = perlin.Noise(sampleX, sampleY);
                }
            }

            return noise;
        }
        public static int[,] GeneratePerlinInt(int rows, int cols, double frequency, int minRange, int maxRange, int seed)
        {
            double[,] perlinArrayDouble = GeneratePerlinNoise(rows, cols, frequency, seed);
            int[,] perlinArray = Utility.Matrices.MatrixModification.NormalizeToInteger(perlinArrayDouble, minRange, maxRange);
            return perlinArray;
        }

        private class PerlinNoise
        {
            private readonly int[] permutation;

            public PerlinNoise(int seed)
            {
                permutation = new int[512];
                var random = new Random(seed);

                int[] p = new int[256];
                for (int i = 0; i < 256; i++) p[i] = i;

                // Shuffle
                for (int i = 255; i > 0; i--)
                {
                    int swapIndex = random.Next(i + 1);
                    int temp = p[i];
                    p[i] = p[swapIndex];
                    p[swapIndex] = temp;
                }

                // Duplicate to avoid overflow
                for (int i = 0; i < 512; i++) permutation[i] = p[i % 256];
            }

            public double Noise(double x, double y)
            {
                int xi = (int)Math.Floor(x) & 255;
                int yi = (int)Math.Floor(y) & 255;

                double xf = x - Math.Floor(x);
                double yf = y - Math.Floor(y);

                double u = Fade(xf);
                double v = Fade(yf);

                int aa = permutation[permutation[xi] + yi];
                int ab = permutation[permutation[xi] + yi + 1];
                int ba = permutation[permutation[xi + 1] + yi];
                int bb = permutation[permutation[xi + 1] + yi + 1];

                double x1, x2;
                x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
                x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

                return Lerp(x1, x2, v);
            }

            private static double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);

            private static double Lerp(double a, double b, double t) => a + t * (b - a);

            private static double Grad(int hash, double x, double y)
            {
                int h = hash & 7; // 8 directions
                double u = (h < 4) ? x : y;
                double v = (h < 4) ? y : x;
                return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
            }
        }

    }

    public static class Debug
    {

        public static void ClearLine(int numLines)
        {
            if (numLines <= 0) return;

            int currentLine = Console.CursorTop;
            int linesToClear = Math.Min(numLines, currentLine + 1);

            for (int i = 0; i < linesToClear; i++)
            {
                Console.SetCursorPosition(0, currentLine - i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, currentLine - linesToClear + 1);
        }
    }

    public class ImageHandler
    {
        #region Image File Manipulation

        //  Get Bitmap at specified string
        public static Bitmap getBitmap(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "The path cannot be null.");
            }
            Bitmap bitmap = new Bitmap(Utility.Files.GetDirectory(path));
            return bitmap;
        }


        // Given a string of filepaths, load the Bitmaps at each filepath and return the array
        public static Bitmap[] getBitmapArray(String[] paths)
        {
            Bitmap[] bitmaps = new Bitmap[paths.Length];
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths), "The Paths array cannot be null.");
            }

            foreach (string path in paths)
            {
                if (path is null)
                {
                    throw new ArgumentNullException(nameof(paths), "The Paths array cannot be null.");
                }
            }
            String[] temp_paths = paths;
            for (int i = 0; i < temp_paths.Length; i++)
            {
                bitmaps[i] = new Bitmap(Utility.Files.GetDirectory(temp_paths[i]));
            }

            return bitmaps;
        }

        //  Save the bitmap as a .png file at the specificed path
        public static void saveImage(Bitmap image, String path)
        {
            if (path == null)
            {
                Console.WriteLine("Error: provided path is null");
                return;
            }

            Console.WriteLine("Saving Image: ");

            String filepath = Utility.Files.GetDirectory(path);
            Console.WriteLine(filepath);

            try { image.Save(filepath, System.Drawing.Imaging.ImageFormat.Png); }
            catch (Exception e)
            {
                Console.WriteLine("Error: filepath " + filepath + "is not valid");
            }

        }


        //  Given an array of Bitmaps, combine them in order and return the combined bitmap
        public static Bitmap CombineBitmaps(Bitmap[] images)
        {
            if (images == null || images.Length == 0)
            {
                throw new ArgumentException("The array of images cannot be null or empty.");
            }

            // Determine the dimensions of the output bitmap based on the first image
            int width = images[0].Width;
            int height = images[0].Height;

            // Ensure all images are the same size
            foreach (var image in images)
            {
                if (image.Width != width || image.Height != height)
                {
                    throw new ArgumentException("All images must have the same dimensions.");
                }
            }

            // Create a new bitmap to hold the combined image
            Bitmap combinedImage = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(combinedImage))
            {
                // Set the background of the combined image to transparent
                g.Clear(Color.Transparent);

                // Draw each image in order
                foreach (var image in images)
                {
                    g.DrawImage(image, new Rectangle(0, 0, width, height));
                }
            }

            return combinedImage;
        }

        #endregion

        #region Image Modification
        //  Get rotated idiot
        public static Bitmap RotateBitmap(Bitmap source, int amount)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Normalize rotations (every 4 is a full rotation)
            int normalizedRotations = (amount % 4 + 4) % 4;

            if (normalizedRotations == 0)
                return (Bitmap)source.Clone(); // No rotation needed

            Bitmap rotated = (Bitmap)source.Clone();
            switch (normalizedRotations)
            {
                case 1:
                    rotated.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    rotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    rotated.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return rotated;
        }





        #endregion

    }
}
