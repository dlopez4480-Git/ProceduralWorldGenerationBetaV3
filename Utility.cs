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
     *          >Exploration: Performs pathfinding, searches and expansions in a 2D array
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
            public static void WriteJsonFile(object obj, string filePathShort)
            {
                //  Convert filepath to a usable directory
                string filePath = Utility.Files.GetDirectory(filePathShort);

                if (obj == null) throw new ArgumentNullException(nameof(obj));
                if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

                // Ensure directory exists
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Serialize with indentation for readability
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                // Serialize and write to file
                string json = JsonSerializer.Serialize(obj, options);
                File.WriteAllText(filePath, json);
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

        //  Collapses a list of lists into a new list
        public static List<T> CollapseLists<T>(List<List<T>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            // Pre-allocate to reduce reallocations if you want:
            int totalCount = 0;
            foreach (var inner in source)
            {
                if (inner != null) totalCount += inner.Count;
            }

            var result = new List<T>(totalCount);
            foreach (var inner in source)
            {
                if (inner == null) continue; // skip null inner lists
                result.AddRange(inner);
            }

            return result;
        }


    }

    public class Matrices
    {
        public class Exploration()
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


                
                private static List<List<Coords>> FindIslandBordersInRange_BORKED(int[,] grid, int minimum, int maximum, bool horizontalWrapping, bool verticalWrapping, int range)
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

                //  FindIslandBordersInRange: Given a 2D array of integers and a range of intergers, find the border of all islands within thickness range
                public static List<List<Coords>> FindIslandBordersInRange(int[,] grid,int minimum,int maximum,bool horizontalWrapping,bool verticalWrapping, int range)
                {
                    int rows = grid.GetLength(0);
                    int cols = grid.GetLength(1);

                    // Step 1: Find islands (reusing previous logic)
                    var islands = FindIslandsInRange(grid, minimum, maximum, horizontalWrapping, verticalWrapping);

                    // Map all island cells for exclusion
                    var islandCells = new HashSet<(int, int)>();
                    foreach (var island in islands)
                    {
                        foreach (var cell in island)
                        {
                            islandCells.Add((cell.x, cell.y));
                        }
                    }

                    var bordersList = new List<List<Coords>>();

                    // Step 2: For each island, find border cells
                    foreach (var island in islands)
                    {
                        var borders = new HashSet<(int, int)>();

                        foreach (var cell in island)
                        {
                            // Explore all cells within "range" Manhattan distance
                            for (int dx = -range; dx <= range; dx++)
                            {
                                for (int dy = -range; dy <= range; dy++)
                                {
                                    if (Math.Abs(dx) + Math.Abs(dy) > range) continue; // Manhattan constraint

                                    int newRow = cell.x + dx;
                                    int newCol = cell.y + dy;

                                    // Handle wrapping
                                    if (horizontalWrapping)
                                    {
                                        if (newCol < 0) newCol = (newCol % cols + cols) % cols;
                                        if (newCol >= cols) newCol = newCol % cols;
                                    }

                                    if (verticalWrapping)
                                    {
                                        if (newRow < 0) newRow = (newRow % rows + rows) % rows;
                                        if (newRow >= rows) newRow = newRow % rows;
                                    }

                                    // Bounds check (only if wrapping disabled)
                                    if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols)
                                        continue;

                                    // Exclude cells that belong to *any* island
                                    if (islandCells.Contains((newRow, newCol))) continue;

                                    borders.Add((newRow, newCol));
                                }
                            }
                        }

                        // Convert set to list of Coords
                        var borderList = new List<Coords>();
                        foreach (var (r, c) in borders)
                        {
                            borderList.Add(new Coords(r, c));
                        }
                        bordersList.Add(borderList);
                    }

                    return bordersList;
                }

                #endregion
            }

            public class VoronoiExpansions()
            {
                public static List<List<Coords>> ExpandTerritories( int[,] sourcegrid, List<Coords> seeds, int minimum, int maximum, bool horizontalWrapping, bool verticalWrapping, int seed)
                {
                    int rows = sourcegrid.GetLength(0);
                    int cols = sourcegrid.GetLength(1);

                    int[,] grid = sourcegrid;
                    Utility.Matrices.MatrixModification.Rotate2DMatrix(grid, 1);
                    Utility.Matrices.MatrixModification.Flip2DMatrix(grid, true, true);


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

            public class Pathfinding
            {
                public static List<Coords> FindPath(Coords start, Coords end, int[,] grid)
                {
                    int rows = grid.GetLength(0);
                    int cols = grid.GetLength(1);

                    bool[,] visited = new bool[rows, cols];
                    Dictionary<Coords, Coords?> parent = new Dictionary<Coords, Coords?>();
                    Queue<Coords> queue = new Queue<Coords>();

                    visited[start.x, start.y] = true;
                    queue.Enqueue(start);
                    parent[start] = null; // start has no parent

                    while (queue.Count > 0)
                    {
                        Coords current = queue.Dequeue();
                        if (current.x == end.x && current.y == end.y)
                        {
                            // Reconstruct path
                            return ReconstructPath(parent, end);
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            int nx = current.x + dx[i];
                            int ny = current.y + dy[i];

                            if (nx >= 0 && nx < rows && ny >= 0 && ny < cols)
                            {
                                if (!visited[nx, ny] && grid[nx, ny] <= grid[current.x, current.y])
                                {
                                    visited[nx, ny] = true;
                                    Coords next = new Coords(nx, ny);
                                    parent[next] = current;
                                    queue.Enqueue(next);
                                }
                            }
                        }
                    }

                    // No valid path
                    return new List<Coords>();
                }
                private static readonly int[] dx = { -1, 1, 0, 0 };
                private static readonly int[] dy = { 0, 0, -1, 1 };
                private static List<Coords> ReconstructPath(Dictionary<Coords, Coords?> parent, Coords end)
                {
                    List<Coords> path = new List<Coords>();
                    Coords? current = end;
                    while (current != null)
                    {
                        path.Add(current.Value);
                        current = parent[current.Value];
                    }
                    path.Reverse();
                    return path;
                }


                public static List<Coords> DownhillRandomWalk(int[,] grid,Coords start,int length,int targetVal, int seed)
                {
                    int width = grid.GetLength(0);
                    int height = grid.GetLength(1);
                    var path = new List<Coords>();
                    var rng = new Random(seed);

                    // Offsets for orthogonal movement
                    (int dx, int dy)[] orthogonal = { (1, 0), (-1, 0), (0, 1), (0, -1) };

                    // Track forbidden cells (visited + their neighbors)
                    bool[,] forbidden = new bool[width, height];

                    // Helper: mark cell and all 8 neighbors forbidden
                    void MarkForbidden(int cx, int cy)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = cx + dx;
                                int ny = cy + dy;
                                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                                    forbidden[nx, ny] = true;
                            }
                    }

                    // Initialize
                    path.Add(start);
                    MarkForbidden(start.x, start.y);

                    Coords current = start;

                    // Immediate success check
                    if (grid[current.x, current.y] == targetVal)
                        return path;

                    for (int step = 1; step < length; step++)
                    {
                        int currentVal = grid[current.x, current.y];

                        // Collect valid neighbors
                        var candidates = new List<Coords>();
                        foreach (var (dx, dy) in orthogonal)
                        {
                            int nx = current.x + dx;
                            int ny = current.y + dy;

                            if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                                continue;

                            // Must be downhill (value >= current value)
                            if (grid[nx, ny] < currentVal) continue;

                            // Only move to cell not forbidden,
                            // but allow moving back to immediate previous cell? No, per spec only the cell we are *currently on* is exempt,
                            // so we can't revisit previous anyway.
                            if (forbidden[nx, ny]) continue;

                            candidates.Add(new Coords(nx, ny));
                        }

                        if (candidates.Count == 0)
                        {
                            // No valid moves
                            return new List<Coords>();
                        }

                        // Choose random neighbor
                        var next = candidates[rng.Next(candidates.Count)];
                        current = next;
                        path.Add(current);
                        MarkForbidden(current.x, current.y);

                        // Check target
                        if (grid[current.x, current.y] == targetVal)
                            return path;
                    }

                    // Failed to reach target within allowed length
                    return new List<Coords>();
                }


            }
            public class Misc
            {
                //  Finds valid starting points given an array
                public static List<Coords> FindValidIslandPlacements(int[,] inputArray, List<Coords> sectionList, int minimum, int maximum, int distance, bool horizontalWrapping, bool verticalWrapping)
                {
                    int rows = inputArray.GetLength(0);
                    int cols = inputArray.GetLength(1);
                    var validStarts = new List<Coords>();

                    if (sectionList == null || sectionList.Count == 0)
                    {
                        return validStarts;
                    }

                    // Step 1: Precompute forbidden cells into bool grid
                    bool[,] forbidden = new bool[rows, cols];
                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            if (inputArray[r, c] >= minimum && inputArray[r, c] <= maximum)
                            {
                                for (int dr = -distance; dr <= distance; dr++)
                                {
                                    for (int dc = -distance; dc <= distance; dc++)
                                    {
                                        if (Math.Abs(dr) + Math.Abs(dc) > distance) continue;

                                        int newR = r + dr;
                                        int newC = c + dc;

                                        // Handle wrapping
                                        if (horizontalWrapping)
                                            newC = (newC % cols + cols) % cols;
                                        if (verticalWrapping)
                                            newR = (newR % rows + rows) % rows;

                                        // Skip if out of bounds when wrapping disabled
                                        if (newR < 0 || newR >= rows || newC < 0 || newC >= cols)
                                            continue;

                                        forbidden[newR, newC] = true;
                                    }
                                }
                            }
                        }
                    }

                    // Step 2: Find "start" of sectionList (closest to (0,0))
                    Coords originalStart = sectionList
                        .OrderBy(c => Math.Abs(c.x) + Math.Abs(c.y))
                        .ThenBy(c => c.x)
                        .ThenBy(c => c.y)
                        .First();

                    // Step 3: Precompute relative offsets
                    var offsets = sectionList
                        .Select(c => new Coords(c.x - originalStart.x, c.y - originalStart.y))
                        .ToArray();

                    // Step 4: Determine safe row and column ranges (skip impossible starts when no wrapping)
                    int minOffsetRow = offsets.Min(o => o.x);
                    int maxOffsetRow = offsets.Max(o => o.x);
                    int minOffsetCol = offsets.Min(o => o.y);
                    int maxOffsetCol = offsets.Max(o => o.y);

                    int startRowMin = verticalWrapping ? 0 : -minOffsetRow;
                    int startRowMax = verticalWrapping ? rows - 1 : rows - 1 - maxOffsetRow;
                    int startColMin = horizontalWrapping ? 0 : -minOffsetCol;
                    int startColMax = horizontalWrapping ? cols - 1 : cols - 1 - maxOffsetCol;

                    // Step 5: Test each candidate start within reduced ranges
                    for (int r = startRowMin; r <= startRowMax; r++)
                    {
                        for (int c = startColMin; c <= startColMax; c++)
                        {
                            // Optimization: skip immediately if start is forbidden
                            if (forbidden[r, c])
                            {
                                continue;
                            }

                            bool valid = true;
                            foreach (var offset in offsets)
                            {
                                int newR = r + offset.x;
                                int newC = c + offset.y;

                                // Handle wrapping
                                if (horizontalWrapping)
                                    newC = (newC % cols + cols) % cols;
                                if (verticalWrapping)
                                    newR = (newR % rows + rows) % rows;

                                // Bounds check if wrapping disabled
                                if (newR < 0 || newR >= rows || newC < 0 || newC >= cols)
                                {
                                    valid = false;
                                    break;
                                }

                                if (forbidden[newR, newC])
                                {
                                    valid = false;
                                    break;
                                }
                            }

                            if (valid)
                            {
                                validStarts.Add(new Coords(r, c));
                            }
                        }
                    }

                    return validStarts;
                }

               
            }
            
            


        }

        public class MatrixModification()
        {
            //  Modify and Transform Matrices 
            #region Matrix Transformation

                // Rotate 2D matrix clockwise n times (each rotation = 90 degrees)
            public static T[,] Rotate2DMatrix<T>(T[,] input, int rotations)
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


                // Flip 2D matrix vertically and/or horizontally
            public static T[,] Flip2DMatrix<T>(T[,] input, bool verticalFlip, bool horizontalFlip)
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


                //  Given a list of Coords, translate it to a new position
            public static List<Coords> TranslatePosition(int[,] inputArray,List<Coords> sectionList, Coords newStartingCoord,bool horizontalWrapping,bool verticalWrapping)
            {
                int rows = inputArray.GetLength(0);
                int cols = inputArray.GetLength(1);

                if (sectionList == null || sectionList.Count == 0)
                    return new List<Coords>();

                // Step 1: Find the "start" coord (closest to [0,0])
                Coords originalStart = sectionList
                    .OrderBy(c => Math.Abs(c.x) + Math.Abs(c.y)) // Manhattan distance
                    .ThenBy(c => c.x) // Row priority
                    .ThenBy(c => c.y) // Column priority
                    .First();

                // Step 2: Compute offsets for each cell relative to the "start"
                var offsets = sectionList
                    .Select(c => new Coords(c.x - originalStart.x, c.y - originalStart.y))
                    .ToList();

                var newCoords = new List<Coords>();

                // Step 3: Apply offsets to newStartingCoord
                foreach (var offset in offsets)
                {
                    int newRow = newStartingCoord.x + offset.x;
                    int newCol = newStartingCoord.y + offset.y;

                    // Handle wrapping
                    if (horizontalWrapping)
                    {
                        newCol = (newCol % cols + cols) % cols;
                    }
                    if (verticalWrapping)
                    {
                        newRow = (newRow % rows + rows) % rows;
                    }

                    // Bounds check if wrapping disabled
                    if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols)
                        continue;

                    newCoords.Add(new Coords(newRow, newCol));
                }

                return newCoords;
            }
            public static List<Coords> TranslateIsland(Array grid, List<Coords> island,  Coords startingLocation, bool horizontalWrapping, bool verticalWrapping)
            {
                if (island == null || island.Count == 0)
                    return new List<Coords>();

                int rows = grid.GetLength(0);
                int cols = grid.GetLength(1);

                //  Find the current "starting location" of the island
                //    (smallest x; tie-breaker smallest y)
                Coords currentStart = island
                    .OrderBy(c => c.x)
                    .ThenBy(c => c.y)
                    .First();

                // 2. Calculate translation offset
                int offsetX = startingLocation.x - currentStart.x;
                int offsetY = startingLocation.y - currentStart.y;

                var newCoords = new List<Coords>();

                foreach (var cell in island)
                {
                    int newX = cell.x + offsetX;
                    int newY = cell.y + offsetY;

                    // Handle vertical wrapping or exclusion
                    if (newX < 0 || newX >= rows)
                    {
                        if (verticalWrapping)
                        {
                            // wrap around using modulo
                            newX = ((newX % rows) + rows) % rows;
                        }
                        else
                        {
                            continue; // exclude cell
                        }
                    }

                    // Handle horizontal wrapping or exclusion
                    if (newY < 0 || newY >= cols)
                    {
                        if (horizontalWrapping)
                        {
                            newY = ((newY % cols) + cols) % cols;
                        }
                        else
                        {
                            continue; // exclude cell
                        }
                    }

                    newCoords.Add(new Coords(newX, newY));
                }

                return newCoords;
            }



            #endregion

            //  Select portions of matrices and perform calculations with them
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
            public static List<Coords> SelectOvalRegion<T>(T[,] array, Coords center, int height, int width)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);


                List<Coords> ovalCoords = new List<Coords>();

                // Radii (floating point for precision)
                double rx = height / 2.0;
                double ry = width / 2.0;

                // Bounding box to search within
                int minX = Math.Max(0, (int)Math.Floor(center.x - rx));
                int maxX = Math.Min(rows - 1, (int)Math.Ceiling(center.x + rx));
                int minY = Math.Max(0, (int)Math.Floor(center.y - ry));
                int maxY = Math.Min(cols - 1, (int)Math.Ceiling(center.y + ry));

                for (int i = minX; i <= maxX; i++)
                {
                    for (int j = minY; j <= maxY; j++)
                    {
                        double dx = (i - center.x) / rx;
                        double dy = (j - center.y) / ry;

                        if (dx * dx + dy * dy <= 1.0)
                        {
                            ovalCoords.Add(new Coords(i, j));
                        }
                    }
                }

                return ovalCoords;
            }

                //  Given a list of Coords, get the complemetary set
            public static List<Coords> SelectCoordsComplement<T>(T[,] array, List<Coords> coordsList)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);

                // Use a HashSet for fast lookup of "inside" coords
                HashSet<(int, int)> insideSet = new HashSet<(int, int)>();
                foreach (var c in coordsList)
                {
                    insideSet.Add((c.x, c.y));
                }

                List<Coords> outsideCoords = new List<Coords>();

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (!insideSet.Contains((i, j)))
                        {
                            outsideCoords.Add(new Coords(i, j));
                        }
                    }
                }

                return outsideCoords;
            }




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

            #endregion

            #region Apply Influence within Arrays
            public static int[,] InfluenceArray(double[,] influence, int[,] baseMap, double radiationStrength)
            {
                int rows = baseMap.GetLength(0);
                int cols = baseMap.GetLength(1);

                // Output array starts as a copy of baseMap
                int[,] result = new int[rows, cols];
                Array.Copy(baseMap, result, baseMap.Length);

                // Determine a practical radius of effect based on strength
                // e.g. radius = ceil(strength * 3) for significant falloff
                int radius = Math.Max(1, (int)Math.Ceiling(radiationStrength * 3));

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double sourceVal = influence[r, c];
                        if (Math.Abs(sourceVal) < double.Epsilon)
                            continue; // skip zero sources

                        // Affect neighbors within radius
                        for (int nr = Math.Max(0, r - radius); nr <= Math.Min(rows - 1, r + radius); nr++)
                        {
                            for (int nc = Math.Max(0, c - radius); nc <= Math.Min(cols - 1, c + radius); nc++)
                            {
                                double dist = Math.Sqrt((nr - r) * (nr - r) + (nc - c) * (nc - c));
                                if (dist > radius) continue;

                                // Influence decays exponentially with distance
                                double decay = Math.Exp(-dist / radiationStrength);

                                // Change is proportional to sourceVal * decay
                                double delta = sourceVal * decay;

                                result[nr, nc] = (int)Math.Round(result[nr, nc] + delta);
                            }
                        }
                    }
                }

                return result;
            }

            #endregion





        }

    }
    
    public static class Noise
    {
        public static class Perlin
        {
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
        
        public static class Gradients
        {
            public static int[,] CreateGradientMap( int mapRows, int mapCols, int minimumVal, int maximumVal, bool middleCenter, int seed, double distortStrength)
            {
                int[,] map = new int[mapRows, mapCols];

                // Create Linear Gradient
                for (int r = 0; r < mapRows; r++)
                {
                    double t;
                    if (middleCenter)
                    {
                        double distFromMid = Math.Abs((mapRows - 1) / 2.0 - r);
                        double maxDist = (mapRows - 1) / 2.0;
                        t = distFromMid / maxDist;           // 0 center -> 1 edge
                    }
                    else
                    {
                        double distFromEdge = Math.Min(r, mapRows - 1 - r);
                        double maxDist = (mapRows - 1) / 2.0;
                        t = 1.0 - (distFromEdge / maxDist);  // 1 edge -> 0 center
                    }

                    int rowVal = (int)Math.Round(
                        maximumVal + (minimumVal - maximumVal) * t);

                    for (int c = 0; c < mapCols; c++)
                        map[r, c] = rowVal;
                }

                //  Apply distortion (TODO: Create this so that it can use the previous noise smoother)
                double[,] noise = GenerateSmoothNoise(mapRows, mapCols, seed, scale: 0.1);


                for (int r = 0; r < mapRows; r++)
                {
                    for (int c = 0; c < mapCols; c++)
                    {
                        // noise[r,c] is in [-1,1]; scale by distortStrength
                        double offset = noise[r, c] * distortStrength;
                        int newVal = (int)Math.Round(map[r, c] + offset);

                        // Clamp to range
                        if (newVal < minimumVal) newVal = minimumVal;
                        if (newVal > maximumVal) newVal = maximumVal;

                        map[r, c] = newVal;
                    }
                }

                return map;
            }
            private static double[,] GenerateSmoothNoise(int rows, int cols, int seed, double scale)
            {
                // Smaller scale => larger features (less wavy)
                Random rand = new Random(seed);
                int gridRows = (int)Math.Ceiling(rows * scale) + 2;
                int gridCols = (int)Math.Ceiling(cols * scale) + 2;

                // Create coarse random grid of values in [-1,1]
                double[,] coarse = new double[gridRows, gridCols];
                for (int i = 0; i < gridRows; i++)
                    for (int j = 0; j < gridCols; j++)
                        coarse[i, j] = rand.NextDouble() * 2.0 - 1.0;

                // Interpolate to full size
                double[,] noise = new double[rows, cols];
                for (int y = 0; y < rows; y++)
                {
                    double gy = y * scale;
                    int g0y = (int)Math.Floor(gy);
                    double ty = gy - g0y;

                    for (int x = 0; x < cols; x++)
                    {
                        double gx = x * scale;
                        int g0x = (int)Math.Floor(gx);
                        double tx = gx - g0x;

                        // Corners of the cell
                        double v00 = coarse[g0y, g0x];
                        double v10 = coarse[g0y, g0x + 1];
                        double v01 = coarse[g0y + 1, g0x];
                        double v11 = coarse[g0y + 1, g0x + 1];

                        // Bilinear interpolation
                        double v0 = Lerp(v00, v10, tx);
                        double v1 = Lerp(v01, v11, tx);
                        noise[y, x] = Lerp(v0, v1, ty);
                    }
                }
                return noise;
            }
            private static double Lerp(double a, double b, double t) => a + (b - a) * t;
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
