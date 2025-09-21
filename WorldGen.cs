using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static testProgram.WorldGen.GeographyGenerator;

namespace testProgram
{

    //  Currently this program is accustomed to 64x256
    public class WorldGen
    {
        
        #region Initial args indexes
        public static readonly int index_seed                   = 0;
        public static readonly int index_mapSize                = 1;
        public static readonly int index_placeholder2           = 2;

        public static readonly int index_placeholder3           = 3;
        public static readonly int index_placeholder4           = 4;

        public static readonly int index_mapType                = 5;
        public static readonly int index_placeholder6           = 6;
        public static readonly int index_placeholder7           = 7;


        public static readonly int index_placeholder8           = 8;
        public static readonly int index_placeholder9           = 9;
        public static readonly int index_placeholder10          = 10;
        public static readonly int index_placeholder11          = 11;
        public static readonly int index_placeholder12          = 12;
        public static readonly int index_trueRandom             = 13;
        public static readonly int index_placeholder14   = 14;
        public static readonly int index_placeholder15   = 15;
        public static readonly int index_placeholder16   = 16;
        public static readonly int index_placeholder17   = 17;
        #endregion


        #region Values
        public static readonly int landgen_lowerbound = 0;
        public static readonly int landgen_upperbound = 63;
        public static readonly int landgen_canvasbound = 4;
        #endregion


        #region Land Codes

        //Land Codes Below Sealine
        public static int landCode_deepWater = 25;
        public static int landCode_offcoastWater = 29;
        public static int landCode_coastalWater = 30;

        //Land Codes Above Sealine
        public static readonly int landCode_outOfBounds = 15;
        public static readonly int landCode_coastalLand = 40;
        public static readonly int landCode_Land = 41;
        public static readonly int landCode_hillLand = 42;
        public static readonly int landCode_mountain = 50;

        #endregion




        
        //  <summary>
        //  This section is STEP 1: It generates the raw land, lakes, and rivers to generate further land
        // </summary>
        public class GeographyGenerator : WorldGen
        {
            //  Given an array in ID Map form, print an ASCII representation of the map
            public static void PrintIDMap(string[] args, int[,] idMap)
            {
                #region Parameters 1
                //  Loads the initial seed for randomization
                int INITSEED = Convert.ToInt32(args[index_seed]);
                Random random = new Random(INITSEED);

                //  Loads the requested dimensions
                string MAPSIZE = Convert.ToString(args[index_mapSize]);
                int mapRows;
                int mapCols;
                switch (MAPSIZE)
                {
                    case "VERY_SMALL":
                        mapRows = 128;
                        mapCols = 256;
                        break;
                    case "SMALL":
                        mapRows = 256;
                        mapCols = 512;
                        break;
                    case "MEDIUM":
                        mapRows = 512;
                        mapCols = 1024;
                        break;
                    case "LARGE":
                        mapRows = 1024;
                        mapCols = 2048;
                        break;
                    case "VERY_LARGE":
                        mapRows = 2048;
                        mapCols = 4096;
                        break;
                    default:
                        //  Defaults to Small
                        mapRows = 128;
                        mapCols = 256;
                        break;
                }
                int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                //  Loads the parameters for land generation
                string MAPTYPE = args[index_mapType];
                #endregion


                for (int i = 0; i < idMap.GetLength(0); i++)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(">|");
                    for (int j = 0; j < idMap.GetLength(1); j++)
                    {

                        //  Water
                        if (idMap[i, j] == landCode_deepWater)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.Write(" ");
                        }
                        else if (idMap[i, j] == landCode_offcoastWater)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("-");
                        }
                        else if (idMap[i, j] == landCode_coastalWater)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("+");
                        }


                        //  Land
                        else if (idMap[i, j] == landCode_coastalLand)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.Write("+");
                        }
                        else if (idMap[i, j] == landCode_Land)
                        {

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.Write("+");

                        }
                        else if (idMap[i, j] == landCode_hillLand)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("H");
                        }
                        else if (idMap[i, j] == landCode_mountain)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("M");
                            
                        }
                        else if (idMap[i, j] == landCode_outOfBounds)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            Console.Write(" ");
                            
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("?");
                            
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;

                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("|<");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("");
            }

            //  Generate the correct outputs based on the relavent arguements
            public static int[,] GenerateGeographyIDMap(string[] args)
            {
                #region Parameters 1
                //  Loads the initial seed for randomization
                int INITSEED = Convert.ToInt32(args[index_seed]);
                Random random = new Random(INITSEED);

                //  Loads the requested dimensions
                string MAPSIZE = Convert.ToString(args[index_mapSize]);
                int mapRows;
                int mapCols;
                switch (MAPSIZE)
                {
                    case "VERY_SMALL":
                        mapRows = 128;
                        mapCols = 256;
                        break;
                    case "SMALL":
                        mapRows = 256;
                        mapCols = 512;
                        break;
                    case "MEDIUM":
                        mapRows = 512;
                        mapCols = 1024;
                        break;
                    case "LARGE":
                        mapRows = 1024;
                        mapCols = 2048;
                        break;
                    case "VERY_LARGE":
                        mapRows = 2048;
                        mapCols = 4096;
                        break;
                    default:
                        //  Defaults to Small
                        mapRows = 128;
                        mapCols = 256;
                        break;
                }
                int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                //  Loads the parameters for land generation
                string MAPTYPE = args[index_mapType];
                #endregion


                //  DEBUG Parameters
                bool verbose = true;
                bool printMap = false;
                bool debug = true;


                int contnumber = 1;
                double frequency = 0.15; //  This frequency works best for the size 64x128

                //  Generate the sea floor 

                //  Generate the GeoMap
                int[,] IDMap = new int[mapRows, mapCols];

                switch (MAPTYPE)
                {
                    case "CONTINENTS":
                        int continentAmountRandom = random.Next(3, 5);
                        continentAmountRandom = 3;
                        int continentSizeRank = 3;
                        IDMap = GeoGraphyGenComponents.GenerateGeographyContinents(args, continentAmountRandom, continentSizeRank);
                        break;
                    default:
                        break;

                }








                //  Print the final
                if (printMap)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[GENWORLD]   Generating ID Map post processing:");
                    PrintIDMap(args, IDMap);
                    Console.WriteLine("");
                }
                if (verbose)
                {
                    Console.WriteLine("GENWORLD:    Generated Landmasses");
                }
                return IDMap;
            }

            //  Contains the components for generating a geography
            
            //  Generates individual components for landmasses
            public class GeoGraphyGenComponents : GeographyGenerator
            {
                #region Universal Map Components
                //  Given an int array in ID form, make the coastlines look more natural
                public static int[,] naturalizeCoastlines(string[] args, int[,] array, int chanceCrater, int cycles, bool cleanup)
                {
                    #region Parameters
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the requested dimensions
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows;
                    int mapCols;
                    switch (MAPSIZE)
                    {
                        case "VERY_SMALL":
                            mapRows = 128;
                            mapCols = 512;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 2048;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 4096;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 8192;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion


                    //  Initial Modify coastlines (this is only done to break up the coastlines for better randomization
                    List<List<Coords>> coastlineArray = new List<List<Coords>>();
                    
                    //int chanceCrater = 75;


                    for (int cyclesCount = 0; cyclesCount < cycles; cyclesCount++)
                    {
                        coastlineArray = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(array, landCode_coastalLand, 999, false, false, 1);
                        
                        foreach (List<Coords> coastlineCoords in coastlineArray)
                        {
                            foreach (Coords coord in coastlineCoords)
                            {
                                //  Generate the probability outwards or inwards
                                int coastlinebulgeProb = random.Next(0, 101);
                                
                                //  Raise Land
                                if (coastlinebulgeProb <= chanceCrater)
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)mapsizeModifier/64));

                                    List<Coords> circoords = Utility.Matrices.MatrixModification.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {

                                        if (array[circoord.x, circoord.y] < landCode_coastalLand)
                                        {
                                            array[circoord.x, circoord.y] = landCode_Land;
                                        }

                                    }
                                }
                                //  Crater
                                else
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)(mapsizeModifier / 64)));
                                    List<Coords> circoords = Utility.Matrices.MatrixModification.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {
                                        if (array[circoord.x, circoord.y] >= landCode_coastalLand)
                                        {
                                            array[circoord.x, circoord.y] = landCode_deepWater;
                                        }

                                    }
                                }
                            }
                        }
                    }










                    if (cleanup)
                    {
                        List<List<Coords>> validLands = Utility.Matrices.Exploration.Islands.FindIslandsInRange(array, landCode_coastalLand, 9999999, false, false);
                        Utility.Lists.RemoveListAboveSize(validLands, mapsizeModifier / 32);
                        
                        int[,] newarray = new int[mapRows,mapCols];
                        for (int i = 0; i < newarray.GetLength(0); i++) {
                            for (int j = 0; j < newarray.GetLength(1); j++)
                            {
                                newarray[i, j] = landCode_deepWater;
                            }
                        }

                        foreach (List<Coords> coordslist in validLands)
                        {
                            foreach (Coords coords in coordslist)
                            {
                                newarray[coords.x, coords.y] = landCode_Land;
                            }
                        }
                    }

                    return array;
                }

                //  Given an int array in ID form, make the coastlines look more natural
                public static int[,] naturalizeMountains(string[] args, int[,] array, int chanceCrater, int cycles, bool cleanup)
                {
                    #region Parameters
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the requested dimensions
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows;
                    int mapCols;
                    switch (MAPSIZE)
                    {
                        case "VERY_SMALL":
                            mapRows = 128;
                            mapCols = 512;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 2048;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 4096;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 8192;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion


                    //  Initial Modify coastlines (this is only done to break up the coastlines for better randomization
                    List<List<Coords>> mountainArray = new List<List<Coords>>();

                    //int chanceCrater = 75;


                    for (int cyclesCount = 0; cyclesCount < cycles; cyclesCount++)
                    {
                        mountainArray = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(array, landCode_mountain, 999, false, false, 1);

                        foreach (List<Coords> mountCoords in mountainArray)
                        {
                            foreach (Coords coord in mountCoords)
                            {
                                //  Generate the probability outwards or inwards
                                int coastlinebulgeProb = random.Next(0, 101);

                                //  Raise Land
                                if (coastlinebulgeProb <= chanceCrater)
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)mapsizeModifier / 64));

                                    List<Coords> circoords = Utility.Matrices.MatrixModification.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {

                                        if (array[circoord.x, circoord.y] >= landCode_coastalLand && array[circoord.x, circoord.y] < landCode_mountain)
                                        {
                                            array[circoord.x, circoord.y] = landCode_mountain;
                                        }

                                    }
                                }
                                //  Crater
                                else
                                {
                                    int radius = random.Next(0, Convert.ToInt32((double)(mapsizeModifier / 64)));
                                    List<Coords> circoords = Utility.Matrices.MatrixModification.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {
                                        if (array[circoord.x, circoord.y] == landCode_mountain)
                                        {
                                            array[circoord.x, circoord.y] = landCode_Land;
                                        }

                                    }
                                }
                            }
                        }
                    }










                    if (cleanup)
                    {
                        List<List<Coords>> validLands = Utility.Matrices.Exploration.Islands.FindIslandsInRange(array, landCode_mountain, 9999999, false, false);
                        Utility.Lists.RemoveListAboveSize(validLands, mapsizeModifier / 32);

                        int[,] newarray = new int[mapRows, mapCols];
                        for (int i = 0; i < newarray.GetLength(0); i++)
                        {
                            for (int j = 0; j < newarray.GetLength(1); j++)
                            {
                                newarray[i, j] = landCode_Land;
                            }
                        }

                        foreach (List<Coords> coordslist in validLands)
                        {
                            foreach (Coords coords in coordslist)
                            {
                                newarray[coords.x, coords.y] = landCode_Land;
                            }
                        }
                    }

                    return array;
                }

                //  "Naturalize" the borders of  TODO
                public static int[,] naturalizeBorders(string[] args, int[,] array, int chanceCrater, int cycles, int tileID_lowerRange, int tileID_upperRange, int tileID_offset, int radius)
                {
                    #region Parameters
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the requested dimensions
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows;
                    int mapCols;
                    switch (MAPSIZE)
                    {
                        case "VERY_SMALL":
                            mapRows = 128;
                            mapCols = 512;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 2048;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 4096;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 8192;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion


                    //  Initial Modify coastlines (this is only done to break up the coastlines for better randomization
                    List<List<Coords>> borderingArray = new List<List<Coords>>();

                    //int chanceCrater = 75;


                    for (int cyclesCount = 0; cyclesCount < cycles; cyclesCount++)
                    {
                        borderingArray = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(array, tileID_lowerRange, tileID_upperRange, false, false, 1);

                        foreach (List<Coords> coordsList in borderingArray)
                        {
                            foreach (Coords coord in coordsList)
                            {
                                //  Generate the probability outwards or inwards
                                int bulgeProbability = random.Next(0, 101);

                                //  Raise Land
                                if (bulgeProbability <= chanceCrater)
                                {
                                    

                                    List<Coords> circoords = Utility.Matrices.MatrixModification.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {

                                        if (array[circoord.x, circoord.y] >= tileID_lowerRange && array[circoord.x, circoord.y] <= tileID_upperRange)
                                        {
                                            array[circoord.x, circoord.y] = tileID_lowerRange;
                                        }

                                    }
                                }
                                //  Crater
                                else
                                {
                                    
                                    List<Coords> circoords = Utility.Matrices.MatrixModification.SelectCircleRegion(array, coord, radius);
                                    foreach (Coords circoord in circoords)
                                    {
                                        if (array[circoord.x, circoord.y] < tileID_lowerRange || array[circoord.x, circoord.y] > tileID_upperRange)
                                        {
                                            array[circoord.x, circoord.y] = tileID_offset;
                                        }

                                    }
                                }
                            }
                        }
                    }










                    

                    return array;
                }


                #endregion


                //  Generates a couple of large continents, and returns a map thereof
                public static int[,] GenerateGeographyContinents(string[] args, int continentNumber, int continentSizes)
                {
                    #region Parameters 1
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the requested dimensions
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows;
                    int mapCols;
                    switch (MAPSIZE)
                    {
                        case "VERY_SMALL":
                            mapRows = 128;
                            mapCols = 256;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 512;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 1024;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 2048;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 4096;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 128;
                            mapCols = 256;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion

                    #region DebugParameters
                    bool verbose = true;
                    bool DEBUG_allContinents        = false;    //  Prints a map of all continents on the map
                    bool DEBUG_printSelectedCont    = true;     //  Prints the map of the selected continent
                    bool DEBUG_mapgenErrorValid     = true;     //  Prints the current map if/when the continent map is wiped
                    bool DEBUG_mapRegeneration      = true;     // Alerts when the map regenerates
                    bool DEBUG_errorcountTester     = false ;     //  Prints the current error count when a regen is triggered


                    #endregion

                    #region Generators
                    //  Generation Parameters
                    double frequency = 3;  //  Adapt for size

                    //  Generate Initial Array
                    int landOffset = 38;
                    int[,] canvasNoise = new int[mapRows, mapCols];

                    //  Assign continentSize range
                    int minSizeContinent = 0;
                    int maxSizeContinent = 0;
                    int maximumTileCount = mapRows * mapCols;   
                    switch (continentSizes)
                    {
                        case 1:
                            //  Get Continents of a very small size
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 2.5));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 16));
                            break;
                        case 2:
                            //  Get Continents of a small size (1 Minute Generation)
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            break;
                        case 3:
                            //  Get Continents of a medium size ( +2 Minute Generation)
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 32));
                            break;
                        case 4:
                            //  Get Continents of a large size
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 32));
                            break;

                        default:
                            //  Get Continents of a small size, by default
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            break;
                    }


                    //  Dimensions Values
                    int bordermodifierEdgesRow = mapRows / 32;
                    int bordermodifierEdgesCol = mapCols / 32;

                    
                    #endregion


                    int continentCurrentNumber = 0;
                    int errorIncrementor = 0;
                    int errorLimit = 500;

                    regenerateContinents:
                    if (errorIncrementor > errorLimit)
                    {
                        if (DEBUG_mapgenErrorValid)
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error in generation; " + continentCurrentNumber + " generated: regenerating");
                            Console.WriteLine();
                            for (int i = 0; i < canvasNoise.GetLength(0); i++)
                            {
                                Console.Write(">|");
                                for (int j = 0; j < canvasNoise.GetLength(1); j++)
                                {
                                    if (canvasNoise[i, j] >= landCode_Land)
                                    {
                                        Console.Write("X");
                                    }
                                    else
                                    {
                                        Console.Write(" ");
                                    }
                                }
                                Console.Write("|<");
                                Console.WriteLine();
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                        }

                        
                        continentCurrentNumber = 0;
                        errorIncrementor = 0;
                        canvasNoise = new int[mapRows, mapCols];
                        goto regenerateContinents;
                    }

                    //  Generate Continents
                    while (continentCurrentNumber < continentNumber)
                    {
                        //  Generate a map
                        int genSeed = random.Next(-99999999, 999999999);
                        int[,] continentsMap = Utility.Noise.Perlin.GeneratePerlinInt(mapRows, mapCols, frequency, landgen_lowerbound, landgen_upperbound, genSeed);
                        //  Convert to ID form
                        for (int i = 0; i < continentsMap.GetLength(0); i++) {
                            for (int j = 0; j < continentsMap.GetLength(1); j++)
                            {
                                if (continentsMap[i,j] >= landOffset)
                                {
                                    continentsMap[i, j] = landCode_Land;
                                } else
                                {
                                    continentsMap[i, j] = landCode_deepWater;
                                }
                            }
                        }


                        #region Naturalize 
                        //  Shave off the edges
                        //  Make sure the edges are oceanic
                        for (int i = 0; i < canvasNoise.GetLength(0); i++)
                        {
                            for (int j = 0; j <= bordermodifierEdgesCol; j++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == bordermodifierEdgesCol)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }


                                canvasNoise[i, j] = landCode_deepWater;
                            }
                            for (int j = canvasNoise.GetLength(1) - bordermodifierEdgesCol; j < canvasNoise.GetLength(1); j++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == canvasNoise.GetLength(1) - bordermodifierEdgesCol)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }

                                canvasNoise[i, j] = landCode_deepWater;
                            }

                        }
                        for (int j = 0; j < canvasNoise.GetLength(1); j++)
                        {
                            for (int i = 0; i < bordermodifierEdgesRow; i++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == bordermodifierEdgesRow)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }
                                canvasNoise[i, j] = landCode_deepWater;
                            }
                            for (int i = canvasNoise.GetLength(0) - bordermodifierEdgesRow; i < canvasNoise.GetLength(0); i++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == canvasNoise.GetLength(0) - bordermodifierEdgesRow)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }
                                canvasNoise[i, j] = landCode_deepWater;
                            }
                        }

                        //  Naturalize coastlines
                        int randomChanceCrater = random.Next(15, 25);
                        continentsMap = WorldGen.GeographyGenerator.GeoGraphyGenComponents.naturalizeCoastlines(args, continentsMap, randomChanceCrater, 2, false);

                        //  Shave off the edges, again
                        //  Make sure the edges are oceanic
                        for (int i = 0; i < canvasNoise.GetLength(0); i++)
                        {
                            for (int j = 0; j <= bordermodifierEdgesCol; j++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == bordermodifierEdgesCol)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }


                                canvasNoise[i, j] = landCode_deepWater;
                            }
                            for (int j = canvasNoise.GetLength(1) - bordermodifierEdgesCol; j < canvasNoise.GetLength(1); j++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == canvasNoise.GetLength(1) - bordermodifierEdgesCol)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }

                                canvasNoise[i, j] = landCode_deepWater;
                            }

                        }
                        for (int j = 0; j < canvasNoise.GetLength(1); j++)
                        {
                            for (int i = 0; i < bordermodifierEdgesRow; i++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == bordermodifierEdgesRow)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }
                                canvasNoise[i, j] = landCode_deepWater;
                            }
                            for (int i = canvasNoise.GetLength(0) - bordermodifierEdgesRow; i < canvasNoise.GetLength(0); i++)
                            {
                                int probabilityCoastCrater = random.Next(0, 101);
                                if (probabilityCoastCrater >= 40 && j == canvasNoise.GetLength(0) - bordermodifierEdgesRow)
                                {
                                    Coords exampleCoord = new Coords(i, j);
                                    int probabilityRadis = random.Next(mapsizeModifier / 64, mapsizeModifier / 16);
                                    List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, exampleCoord, probabilityRadis);
                                    foreach (Coords cirCoord in circCoords)
                                    {
                                        canvasNoise[cirCoord.x, cirCoord.y] = landCode_deepWater;
                                    }
                                }
                                canvasNoise[i, j] = landCode_deepWater;
                            }
                        }




                        #endregion

                        #region Select out invalid map sizes
                        //  TODO: Use ComplementList to iterate less
                        //  Create a list of all continents below the size limit, then delete them
                        List<List<Coords>> TooSmallConts = Utility.Matrices.Exploration.Islands.FindIslandsInRange(continentsMap, landCode_coastalLand, 999999, false, false);
                        Utility.Lists.RemoveListAboveSize(TooSmallConts, minSizeContinent);
                        foreach (List<Coords> listCoords in TooSmallConts)
                        {
                            foreach (Coords coord in listCoords)
                            {
                                continentsMap[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }

                        //  Create a list Of all continents above the size limit, then delete them
                        List<List<Coords>> TooLargeConts = Utility.Matrices.Exploration.Islands.FindIslandsInRange(continentsMap, landCode_coastalLand, 999999, false, false);
                        Utility.Lists.RemoveListBelowSize(TooLargeConts, maxSizeContinent);
                        foreach (List<Coords> listCoords in TooLargeConts)
                        {
                            foreach (Coords coord in listCoords)
                            {
                                continentsMap[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }
                        #endregion

                        #region Select out map with invalid dimensions
                        //  TODO: This code works for a row-to-col ratio of 1:4. Please modify this so that it works across ratios
                        //  Remove continents wider than they are tall
                        List<List<Coords>> wideContinents = Utility.Matrices.Exploration.Islands.FindIslandsInRange(continentsMap, landCode_coastalLand, 999999, false, false);
                        List<List<Coords>> badConts = new List<List<Coords>>();
                        int rowMinimum = Convert.ToInt32((double)(mapsizeModifier/2));
                        int colMaximum = Convert.ToInt32((double)(mapsizeModifier/2));

                        foreach (List<Coords> contLists in wideContinents)
                        {

                            int continentColCount = Utility.Matrices.MatrixModification.MaxColDistance(contLists);
                            int continentRowCount = Utility.Matrices.MatrixModification.MaxRowDistance(contLists);

                            //  Check that these two do not vary signficiantly
                            if (continentColCount > Convert.ToInt32((double)(continentRowCount * (1.10))) || continentColCount < Convert.ToInt32((double)(continentRowCount * (0.90))))
                            {
                                badConts.Add(contLists);
                            }
                            if (continentRowCount > Convert.ToInt32((double)(continentColCount * (1.10))) || continentRowCount < Convert.ToInt32((double)(continentColCount * (0.90))))
                            {
                                //badConts.Add(contLists);
                            }


                            if (continentColCount > colMaximum)
                            {
                                //badConts.Add(contLists);
                            }
                            if (continentRowCount > Convert.ToInt32((double)(mapRows*(0.60))))
                            {
                                //badConts.Add(contLists);
                            }


                        }
                        
                        foreach (List<Coords> badContsList in badConts)
                        {
                            foreach (Coords coord in badContsList)
                            {
                                continentsMap[coord.x, coord.y] = landCode_outOfBounds;
                            }
                        }

                        #endregion

                        #region Check for any valid continents left
                        //  Check that ANY continents exist
                        List<List<Coords>> validContinents = Utility.Matrices.Exploration.Islands.FindIslandsInRange(continentsMap, landCode_coastalLand, 999999, false, false);
                        if (validContinents.Count() < 1)
                        {
                            if (DEBUG_errorcountTester)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("No Valid Continents:");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            
                            goto regenerateContinents;
                        }



                        #endregion

                        //  Debug Print
                        if (DEBUG_allContinents)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine();
                            Console.WriteLine("Continents Info:");
                            Console.WriteLine("");
                            foreach (List<Coords> continents in validContinents)
                            {
                                int rowCount = Utility.Matrices.MatrixModification.MaxRowDistance(continents);
                                int colCount = Utility.Matrices.MatrixModification.MaxColDistance(continents);
                                Console.WriteLine(">Continent: " + continents.Count());
                                Console.WriteLine(">Continent Size: x" + Convert.ToInt32((double)(continents.Count() / mapsizeModifier)));
                                Console.WriteLine(">Dimensions: " + rowCount + " x " + colCount);
                                Console.WriteLine("");
                            }
                            Console.WriteLine();

                            for (int i = 0; i < continentsMap.GetLength(0); i++)
                            {
                                Console.Write(">|");
                                for (int j = 0; j < continentsMap.GetLength(1); j++)
                                {
                                    if (continentsMap[i, j] >= landCode_Land)
                                    {
                                        Console.Write("-");
                                    }
                                    else if (continentsMap[i, j] == landCode_deepWater)
                                    {
                                        Console.Write(" ");
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write("X");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                    }
                                }
                                Console.Write("|<");
                                Console.WriteLine();
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }




                        #region Assign the continent value to a random position
                        List<Coords> randomContinent = new List<Coords>();
                        int randomNumContinent = random.Next(0, validContinents.Count());
                        randomContinent = validContinents[randomNumContinent];

                        //  Create a passable continent map
                        int[,] passableContinentMap = canvasNoise;
                        //  Fill in the edges
                        for (int i = 0; i < passableContinentMap.GetLength(0); i++)
                        {
                            for (int j = 0; j < bordermodifierEdgesCol; j++)
                            {
                                passableContinentMap[i, j] = landCode_Land;
                            }
                            for (int j = passableContinentMap.GetLength(1) - bordermodifierEdgesCol; j < passableContinentMap.GetLength(1); j++)
                            {
                                passableContinentMap[i, j] = landCode_Land;
                            }
                        }
                        for (int j = 0; j < passableContinentMap.GetLength(1); j++)
                        {
                            for (int i = 0; i < bordermodifierEdgesRow; i++)
                            {
                                passableContinentMap[i, j] = landCode_Land;
                            }
                            for (int i = passableContinentMap.GetLength(0) - bordermodifierEdgesRow; i < passableContinentMap.GetLength(0); i++)
                            {
                                passableContinentMap[i, j] = landCode_Land;
                            }
                        }



                        List<Coords> potentialStarts = Utility.Matrices.Exploration.Misc.FindValidIslandPlacements(canvasNoise, randomContinent, landCode_coastalLand, 9999, Convert.ToInt32((double)mapsizeModifier / 64), false, false);
                        //  Check there are valid starts
                        if (potentialStarts.Count() < 1)
                        {
                            if (DEBUG_mapRegeneration)
                            {
                                Console.ForegroundColor= ConsoleColor.Red;
                                Console.WriteLine("No Valid Locations: regenerating the whole map");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            errorIncrementor = errorLimit*2;
                            goto regenerateContinents;
                        }
                        int randomIndexStart = random.Next(0, potentialStarts.Count());
                        List<Coords> newContinentLocation = Utility.Matrices.MatrixModification.TranslatePosition(canvasNoise, randomContinent, potentialStarts[randomIndexStart], false, false);
                        
                        foreach (Coords coord in newContinentLocation)
                        {
                            if (canvasNoise[coord.x, coord.y] <= landCode_coastalWater)
                            {
                                /**
                                if (DEBUG_mapRegeneration)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Land Overwrite tripped: regenerating the whole map");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                errorIncrementor = errorLimit;
                                goto regenerateContinents;

                                **/
                                canvasNoise[coord.x, coord.y] = landCode_Land;
                            }
                            else
                            {
                                canvasNoise[coord.x, coord.y] = landCode_Land;
                            }
                                
                        }
                        #endregion

                        //  Print the current canvasMap
                        if (DEBUG_printSelectedCont)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine();
                            
                            for (int i = 0; i < canvasNoise.GetLength(0); i++)
                            {
                                Console.Write(">|");
                                for (int j = 0; j < canvasNoise.GetLength(1); j++)
                                {
                                    if (canvasNoise[i, j] >= landCode_Land)
                                    {
                                        Console.Write("-");
                                    }
                                    else if (canvasNoise[i, j] == landCode_deepWater)
                                    {
                                        Console.Write(" ");
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write("X");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                    }
                                }
                                Console.Write("|<");
                                Console.WriteLine();
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        continentCurrentNumber++;

                    }
                    
                    
                    //  Perform more detailed continent modification
                    bool modifyingNewConts = true;
                    while (modifyingNewConts)
                    {
                        //  Base convert into a valid map
                        for (int i = 0; i < canvasNoise.GetLength(0); i++)
                        {
                            for (int j = 0; j < canvasNoise.GetLength(1); j++)
                            {
                                if (canvasNoise[i, j] < landCode_coastalLand)
                                {
                                    canvasNoise[i, j] = landCode_deepWater;
                                }
                                else
                                {
                                    canvasNoise[i, j] = landCode_Land;
                                }
                            }
                        }

                        //  TODO: For these, make sure to manage maxsize of lakes, hill and mtn clusters
                        #region Generate Lakes
                       

                        #endregion


                        #region Generate Hills
                        //  Generate a list of good looking mountains and create a list of them
                        int[,] HillsArrayNoise = Utility.Noise.Perlin.GeneratePerlinInt(mapRows, mapCols, 30.00, 0, 31, random.Next(-99999999, 999999999));
                        int hillMin = 18;


                        //  Sort out mountains that are too small, and get a random assortment
                        List<List<Coords>> selectHills = Utility.Matrices.Exploration.Islands.FindIslandsInRange(HillsArrayNoise, hillMin, 99999, false, false);
                        Utility.Lists.RemoveListBelowSize(selectHills, mapsizeModifier / 32);


                        //  Locate valid mountains and add them to the list
                        List<List<Coords>> validHills = new List<List<Coords>>();

                        foreach (List<Coords> hilList in selectHills)
                        {
                            //  Make sure the lakes are fully enclosed
                            foreach (Coords hilCoord in hilList)
                            {
                                bool closeToOcean = false;
                                List<Coords> circlCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, hilCoord, mapsizeModifier / 64); //  TODO Check
                                foreach (Coords circoord in circlCoords)
                                {
                                    if (canvasNoise[circoord.x, circoord.y] <= landCode_coastalLand)
                                    {
                                        closeToOcean = true;
                                    }
                                }

                                if (!closeToOcean)
                                {
                                    validHills.Add(hilList);
                                }
                            }
                        }

                        //  Randomly remove some of these mountains
                        validHills = Utility.Lists.GetRandomSelection(validHills, Convert.ToInt32((double)(validHills.Count()) * 0.25), false, random.Next(-99999999, 999999999));

                        //  Create the mountains in canvas noise
                        foreach (List<Coords> hilList in validHills)
                        {
                            foreach (Coords hilCoord in hilList)
                            {
                                canvasNoise[hilCoord.x, hilCoord.y] = landCode_hillLand;
                            }
                        }

                        #endregion

                        #region Generate Mountains
                        //  Generate a list of good looking mountains and create a list of them
                        int[,] MtnsArrayNoise = Utility.Noise.Perlin.GeneratePerlinInt(mapRows, mapCols, 65.00, 0, 31, random.Next(-99999999, 999999999));
                        int mtnMin = 18;
                        

                        //  Sort out mountains that are too small, and get a random assortment
                        List<List<Coords>> selectMtns = Utility.Matrices.Exploration.Islands.FindIslandsInRange(MtnsArrayNoise, mtnMin, 99999, false, false);
                        Utility.Lists.RemoveListBelowSize(selectMtns, mapsizeModifier / 64);



                        //  Locate valid mountains and add them to the list
                        List<List<Coords>> validMtns = new List<List<Coords>>();
                        foreach (List<Coords> mtnList in selectMtns)
                        {
                            //  Make sure the lakes are fully enclosed
                            foreach (Coords mtnCoord in mtnList)
                            {
                                bool closeToOcean = false;
                                List<Coords> circlCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(canvasNoise, mtnCoord, mapsizeModifier / 32); //  TODO Check
                                foreach (Coords circoord in circlCoords)
                                {
                                    if (canvasNoise[circoord.x, circoord.y] <= landCode_coastalLand)
                                    {
                                        closeToOcean = true;
                                    }
                                }

                                if (!closeToOcean)
                                {
                                    validMtns.Add(mtnList);
                                }
                            }
                        }

                        //  Randomly remove some of these mountains
                        validMtns = Utility.Lists.GetRandomSelection(validMtns, Convert.ToInt32((double)(validMtns.Count()) * 0.25), false, random.Next(-99999999, 999999999));

                        //  Create the mountains in canvas noise
                        foreach (List<Coords> mtnList in validMtns)
                        {
                            foreach (Coords mtnCoord in mtnList)
                            {
                                canvasNoise[mtnCoord.x, mtnCoord.y] = landCode_mountain;
                            }
                        }

                        canvasNoise = naturalizeMountains(args, canvasNoise, 10, 1, true);



                        //  Generate neighoring hills
                        //  Get the surronding land and turn it into hills
                        List<List<Coords>> neighboringHills = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(canvasNoise, landCode_mountain, landCode_mountain, false, false, mapsizeModifier / 64);
                        List<List<Coords>> immediateneighboringHills = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(canvasNoise, landCode_mountain, landCode_mountain, false, false, mapsizeModifier / 128);

                        foreach (List<Coords> hilList in neighboringHills)
                        {
                            foreach (Coords hilCoord in hilList)
                            {
                                int probabilityHills = random.Next(0, 101);
                                if (probabilityHills < 10 && canvasNoise[hilCoord.x, hilCoord.y] == landCode_Land)
                                {
                                    canvasNoise[hilCoord.x, hilCoord.y] = landCode_hillLand;
                                }
                                
                            }
                        }
                        
                        foreach (List<Coords> hilList in immediateneighboringHills)
                        {
                            foreach (Coords hilCoord in hilList)
                            {
                                canvasNoise[hilCoord.x, hilCoord.y] = landCode_hillLand;
                            }
                        }

                        #endregion

                        //  Break loop
                        modifyingNewConts = false;
                    }

 
                    bool finalCleanup = true;
                    while (finalCleanup)
                    {
                        //  Pick up big continents
                        List<List<Coords>> finalMapList = Utility.Matrices.Exploration.Islands.FindIslandsInRange(canvasNoise, landCode_coastalLand, 9999, false, false);
                        Utility.Lists.RemoveListBelowSize(finalMapList, 32);
                        int[,] finalNoise = new int[mapRows, mapCols];
                        for (int i = 0; i < finalNoise.GetLength(0); i++)
                        {
                            for (int j = 0; j < finalNoise.GetLength(1); j++)
                            {
                                finalNoise[i, j] = landCode_deepWater;
                            }
                        }
                        foreach (List<Coords> listcoords in finalMapList)
                        {
                            foreach (Coords coord in listcoords)
                            {
                                finalNoise[coord.x, coord.y] = canvasNoise[coord.x, coord.y];
                            }
                        }


                        

                        

                        //  Convert the coastlines, coastwaters, etc
                        List<List<Coords>> coastlineLand = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(canvasNoise, landCode_deepWater, landCode_coastalWater, false, false, mapsizeModifier / 128);
                        List<List<Coords>> LandExcess = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(canvasNoise, landCode_deepWater, landCode_coastalWater, false, false, mapsizeModifier / 32);
                        List<List<Coords>> coastlineOffWater = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(canvasNoise, landCode_coastalLand, 99999999, false, false, mapsizeModifier / 32);
                        List<List<Coords>> coastlineWater = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(canvasNoise, landCode_coastalLand, 99999999, false, false, mapsizeModifier / 64);
                        foreach (List<Coords> coastCoords in coastlineOffWater)
                        {
                            foreach (Coords coastCoord in coastCoords)
                            {
                                canvasNoise[coastCoord.x, coastCoord.y] = landCode_offcoastWater;
                            }
                        }
                        foreach (List<Coords> coastCoords in coastlineWater)
                        {
                            foreach (Coords coastCoord in coastCoords)
                            {
                                canvasNoise[coastCoord.x, coastCoord.y] = landCode_coastalWater;
                            }
                        }
                        foreach (List<Coords> coastCoords in LandExcess)
                        {
                            foreach (Coords coastCoord in coastCoords)
                            {
                                canvasNoise[coastCoord.x, coastCoord.y] = landCode_Land;
                            }
                        }
                        foreach (List<Coords> coastCoords in coastlineLand)
                        {
                            foreach (Coords coastCoord in coastCoords)
                            {
                                canvasNoise[coastCoord.x, coastCoord.y] = landCode_coastalLand;
                            }
                        }





                        finalCleanup = false;
                    }

                    



                    return canvasNoise;
                }
                

                public static int[,] GenerateGeographyIslands(string[] args, int islandSizes)
                {
                    #region Parameters 1
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the requested dimensions
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows;
                    int mapCols;
                    switch (MAPSIZE)
                    {
                        case "VERY_SMALL":
                            mapRows = 128;
                            mapCols = 256;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 512;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 1024;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 2048;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 4096;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 128;
                            mapCols = 256;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion

                    #region Generators
                    //  Generation Parameters
                    double frequency = 3;  //  Adapt for size

                    //  Generate Initial Array
                    int landOffset = 38;
                    int[,] canvasNoise = new int[mapRows, mapCols];

                    //  Assign continentSize range
                    int minSizeContinent = 0;
                    int maxSizeContinent = 0;
                    int maximumTileCount = mapRows * mapCols;
                    switch (islandSizes)
                    {
                        case 1:
                            //  Get Continents of a very small size
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 2.5));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 16));
                            break;
                        case 2:
                            //  Get Continents of a small size (1 Minute Generation)
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            break;
                        case 3:
                            //  Get Continents of a medium size ( +2 Minute Generation)
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 24));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 32));
                            break;
                        case 4:
                            //  Get Continents of a large size
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 32));
                            break;

                        default:
                            //  Get Continents of a small size, by default
                            minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 10));
                            maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 20));
                            break;
                    }


                    //  Dimensions Values
                    int bordermodifierEdgesRow = mapRows / 32;
                    int bordermodifierEdgesCol = mapCols / 32;


                    #endregion


                    

                    return canvasNoise;
                }



            }
            public class RiverGenComponents : GeographyGenerator
            {
               public static void GenerateRivers(string[] args, int[,] landMap)
               {
                    #region Parameters
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the requested dimensions
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows;
                    int mapCols;
                    switch (MAPSIZE)
                    {
                        case "VERY_SMALL":
                            mapRows = 128;
                            mapCols = 512;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 2048;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 4096;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 8192;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 256;
                            mapCols = 1024;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion

                    

                    //  Select out regions which are too small for rivers
                    int mapSizeMin = mapsizeModifier / 15;

                    List<List<Coords>> newMapList = Utility.Matrices.Exploration.Islands.FindIslandsInRange(landMap, landCode_coastalLand, 99999, false, false);
                    Utility.Lists.RemoveListBelowSize(newMapList, mapSizeMin);
                    int[,] IDMap = new int[mapRows, mapCols];

                    foreach (List<Coords> listOfCoords in newMapList)
                    {
                        foreach (Coords coords in listOfCoords) {
                            IDMap[coords.x, coords.y] = landMap[coords.x, coords.y];
                        }
                    }


                    //  Create a zone where rivers are supercommon
                    double frequencyZone = 30;
                    int genSeed = random.Next(-99996, 77777);
                    
                   







                    List<List<Coords>> mountainEdgesSuper = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(IDMap, landCode_mountain, 9999, false, false, 1);
                    List<Coords> mountainEdges= Utility.Lists.CollapseLists(mountainEdgesSuper);
                    
                    List<List<Coords>> waterEdgesSuper = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(IDMap, landCode_deepWater, landCode_coastalLand, false, false, 1);
                    List<Coords> waterEdges = Utility.Lists.CollapseLists(waterEdgesSuper);
                    //  TODO: River Generator that finds the location
                    List<Coords> riverCoords = new List<Coords>();

                    bool incomplete = true;
                    while (incomplete)
                    {
                        int randomMtnCoord = random.Next(0, mountainEdges.Count());
                        int randomWtrCoord = random.Next(0, waterEdges.Count());
                        Coords start = mountainEdges[randomMtnCoord];




                        //  Perform river calculations
                        int riverSeed = random.Next(-99996, 77777);
                        int riverLength = Convert.ToInt32((double)mapsizeModifier / 16);
                        riverCoords = Utility.Matrices.Exploration.Pathfinding.DownhillRandomWalk(IDMap, start, riverLength, landCode_coastalWater, riverSeed);
                        if (riverCoords.Count() > 2)
                        {
                            incomplete = false;
                        }

                    }
                    

                    for (int i = 0; i < IDMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < IDMap.GetLength(1); j++)
                        {
                            Coords exampleCoords = new Coords(i, j);

                            if (IDMap[i,j] <= landCode_coastalWater)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkBlue;
                            } 
                            else if (IDMap[i, j] >= landCode_coastalLand && IDMap[i, j] <= landCode_hillLand)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                            } 
                            else if (IDMap[i, j] == landCode_mountain)
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                            }
                            
                            if (riverCoords.Contains(exampleCoords))
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write("#");
                            }
                            else
                            {
                                Console.Write(" ");
                            }


                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine("");
                    }
               }
            }







            

        }

        public class ClimateBiomeGenerator { 
            public class TemperatureGenComponents
            {
                public static void printTemperatureMap(string[] args, int[,] temperatureMap, int[,] worldMapID, bool displayLandOnly)
                {
                    for (int i = 0; i < temperatureMap.GetLength(0); i++)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(">|");
                        for (int j = 0; j < temperatureMap.GetLength(1); j++)
                        {

                            //  Generate color scheme
                            //  Frozen
                            if (temperatureMap[i, j] < 20)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkBlue;
                            }
                            //  Cold
                            else if (temperatureMap[i, j] >= 20 && temperatureMap[i, j] < 40)
                            {
                                Console.BackgroundColor = ConsoleColor.Blue;
                            }
                            //  Temperate
                            else if (temperatureMap[i, j] >= 40 && temperatureMap[i, j] < 60)
                            {
                                Console.BackgroundColor = ConsoleColor.Cyan;
                            }
                            //  Warm
                            else if (temperatureMap[i, j] >= 60 && temperatureMap[i, j] < 80)
                            {
                                Console.BackgroundColor = ConsoleColor.Yellow;
                            }
                            //  Hot
                            else if (temperatureMap[i, j] >= 80 && temperatureMap[i, j] < 90)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                            }
                            //  Scorching
                            else if (temperatureMap[i, j] >= 90)
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                            }

                            //  Print world overlay
                            if (worldMapID[i, j] < landCode_coastalLand)
                            {
                                if (displayLandOnly)
                                {
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.Write(" ");
                                } 
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.Write(" ");
                                }
                                    
                            }
                            else if (worldMapID[i, j] >= landCode_coastalLand && worldMapID[i, j] < landCode_mountain)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("+");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write("^");
                            }

                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("|<");
                        Console.WriteLine("");

                    }
                }

                public static int[,] generateTemperatureMap(string[] args, int[,] worldMapID)
                {
                    #region Parameters 1
                    //  Loads the initial seed for randomization
                    int INITSEED = Convert.ToInt32(args[index_seed]);
                    Random random = new Random(INITSEED);

                    //  Loads the requested dimensions
                    string MAPSIZE = Convert.ToString(args[index_mapSize]);
                    int mapRows;
                    int mapCols;
                    switch (MAPSIZE)
                    {
                        case "VERY_SMALL":
                            mapRows = 128;
                            mapCols = 256;
                            break;
                        case "SMALL":
                            mapRows = 256;
                            mapCols = 512;
                            break;
                        case "MEDIUM":
                            mapRows = 512;
                            mapCols = 1024;
                            break;
                        case "LARGE":
                            mapRows = 1024;
                            mapCols = 2048;
                            break;
                        case "VERY_LARGE":
                            mapRows = 2048;
                            mapCols = 4096;
                            break;
                        default:
                            //  Defaults to Small
                            mapRows = 128;
                            mapCols = 256;
                            break;
                    }
                    int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

                    //  Loads the parameters for land generation
                    string MAPTYPE = args[index_mapType];
                    #endregion

                    //  Generate a semiRandom temperature gradient between 0 and 100
                    int randomSeed = random.Next(-9999, 9999);
                    int minValTemperature = -10;
                    int maxValTemperature = 89;
                    double waveval = 5.5;

                    int[,] temperatureMap = Utility.Noise.Gradients.CreateGradientMap(worldMapID.GetLength(0), worldMapID.GetLength(1), minValTemperature, maxValTemperature, false, random.Next(-999789, 779999), waveval);


                    //  Create influencable array complement, and influence the array
                    double[,] landDistortion = new double[worldMapID.GetLength(0), worldMapID.GetLength(1)];
                    for (int i = 0; i < landDistortion.GetLength(0); i++)
                    {
                        for (int j = 0; j < landDistortion.GetLength(1); j++)
                        {
                            landDistortion[i, j] = 1.0;

                            //  Checks if it is on or near a mountain tile; mountains tend to be colder
                            if (worldMapID[i, j] == landCode_mountain)
                            {
                                List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(worldMapID, new Coords(i, j), mapsizeModifier / 128);
                                foreach (Coords coord in circCoords)
                                {
                                    landDistortion[coord.x, coord.y] -= 0.75;
                                }
                            }

                            //  Checks if it is on or near a hill tile; hill tend to be colder
                            else if (worldMapID[i, j] == landCode_hillLand)
                            {
                                List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(worldMapID, new Coords(i, j), mapsizeModifier / 128);
                                foreach (Coords coord in circCoords)
                                {
                                    landDistortion[coord.x, coord.y] -= 0.5;
                                }
                            }


                            //  Checks if it is near a water tile: water tiles tend to moderate heat
                            else if (worldMapID[i, j] == landCode_deepWater || worldMapID[i, j] == landCode_offcoastWater || worldMapID[i, j] == landCode_coastalWater)
                            {
                                List<Coords> circCoords = Utility.Matrices.MatrixModification.SelectCircleRegion(worldMapID, new Coords(i, j), mapsizeModifier / 64);
                                foreach (Coords coord in circCoords)
                                {
                                    //  If hotter, cool it down
                                    if (temperatureMap[coord.x, coord.y] > 60)
                                    {
                                        landDistortion[coord.x, coord.y] -= 0.25;
                                    }
                                    //  If colder, warm it up
                                    else if (temperatureMap[coord.x, coord.y] < 50)
                                    {
                                        landDistortion[coord.x, coord.y] += 0.25;
                                    }
                                    //landDistortion[coord.x, coord.y] += 0.5;
                                }
                            }


                        }

                    }
                    temperatureMap = Utility.Matrices.MatrixModification.InfluenceArray(landDistortion, temperatureMap, mapsizeModifier / 128);


                    //




                    //  Reinforce temperature
                    landDistortion = new double[worldMapID.GetLength(0), worldMapID.GetLength(1)];
                    List<List<Coords>> listOfHots = Utility.Matrices.Exploration.Islands.FindIslandBordersInRange(temperatureMap, 90, 999999, false, false, mapsizeModifier/64);
                    foreach (List<Coords> centercoordList in listOfHots)
                    {
                        foreach (Coords centercoord in centercoordList)
                        {
                            List<Coords> CIRcoordsList = Utility.Matrices.MatrixModification.SelectCircleRegion(temperatureMap, centercoord, mapsizeModifier / 128);
                            foreach (Coords circentercoord in CIRcoordsList)
                            {
                                landDistortion[circentercoord.x, circentercoord.y] += 0.25;
                            }
                            landDistortion[centercoord.x, centercoord.y] += 3;
                        }
                         
                    }
                    temperatureMap = Utility.Matrices.MatrixModification.InfluenceArray(landDistortion, temperatureMap, mapsizeModifier / 128);


                    return temperatureMap;
                }
            }
            
            public class HydrationGenComponents
            {
                public static int[,] generateHydrationMap(string[] args, int[,] worldMapID, int[,] temperatureMap)
                {
                    double[,] hydrationMapInfluence = new double[worldMapID.GetLength(0), worldMapID.GetLength(1)];
                    //  Get all coastlines, set them to 3

                    
                    
                    for (int i = 0; i < worldMapID.GetLength(0); i++)
                    {
                        for (int j = 0; j < worldMapID.GetLength(1); j++)
                        {
                            
                        }
                    }
                    return temperatureMap;
                }
            }



        }
        
        //  This function generates a representation of the world using WorldTile Objects
        public static void GenerateWorld(string[] args) {
            #region Parameters 1
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);

            //  Loads the requested dimensions
            string MAPSIZE = Convert.ToString(args[index_mapSize]);
            int mapRows;
            int mapCols;
            switch (MAPSIZE)
            {
                case "VERY_SMALL":
                    mapRows = 128;
                    mapCols = 256;
                    break;
                case "SMALL":
                    mapRows = 256;
                    mapCols = 512;
                    break;
                case "MEDIUM":
                    mapRows = 512;
                    mapCols = 1024;
                    break;
                case "LARGE":
                    mapRows = 1024;
                    mapCols = 2048;
                    break;
                case "VERY_LARGE":
                    mapRows = 2048;
                    mapCols = 4096;
                    break;
                default:
                    //  Defaults to Small
                    mapRows = 128;
                    mapCols = 256;
                    break;
            }
            int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

            //  Loads the parameters for land generation
            string MAPTYPE = args[index_mapType];
            #endregion

            bool printMapsAsGenerated = true;

            //  Generate the Geography Map 
            int[,] GeographyMap = GeographyGenerator.GenerateGeographyIDMap(args);
            if (printMapsAsGenerated)
            {
                Console.WriteLine("GENWORLD:    Geography Map");
                GeographyGenerator.PrintIDMap(args, GeographyMap);
            }
            
            //  Generate the Rivers Map
            //RiverGenComponents.GenerateRivers(args, GeographyMap);
            if (printMapsAsGenerated)
            {

            }

            //  Generate the Temperature Map
            int[,] TemperatureMap = WorldGen.ClimateBiomeGenerator.TemperatureGenComponents.generateTemperatureMap(args, GeographyMap);
            if (printMapsAsGenerated)
            {
                Console.WriteLine("GENWORLD:    Temperature Map");
                WorldGen.ClimateBiomeGenerator.TemperatureGenComponents.printTemperatureMap(args, TemperatureMap, GeographyMap, true);
            }
                

        }

        public static void testGenerationWorld(string[] args) {
            #region Parameters 1
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);

            //  Loads the requested dimensions
            string MAPSIZE = Convert.ToString(args[index_mapSize]);
            int mapRows;
            int mapCols;
            switch (MAPSIZE)
            {
                case "VERY_SMALL":
                    mapRows = 128;
                    mapCols = 256;
                    break;
                case "SMALL":
                    mapRows = 256;
                    mapCols = 512;
                    break;
                case "MEDIUM":
                    mapRows = 512;
                    mapCols = 1024;
                    break;
                case "LARGE":
                    mapRows = 1024;
                    mapCols = 2048;
                    break;
                case "VERY_LARGE":
                    mapRows = 2048;
                    mapCols = 4096;
                    break;
                default:
                    //  Defaults to Small
                    mapRows = 128;
                    mapCols = 256;
                    break;
            }
            int mapsizeModifier = Convert.ToInt32((double)((mapRows + mapCols) / 2)); // 640 on a 256x1024 grid

            //  Loads the parameters for land generation
            string MAPTYPE = args[index_mapType];
            #endregion


            #region Display Info
            Console.WriteLine("");
            Console.WriteLine();
            //  Generate a world according to the init args
            Console.WriteLine("         Parameters          ");
            Console.WriteLine("SEED:                " + INITSEED);
            Console.WriteLine("");
            Console.WriteLine("MAPSIZE:             " + MAPSIZE);
            Console.WriteLine("Rows:                " + mapRows);
            Console.WriteLine("Cols:                " + mapCols);
            Console.WriteLine("MapSize Modifier:    " + mapsizeModifier);
            Console.WriteLine("                                 ");

            Console.WriteLine("MAPTYPE:             " + MAPTYPE);
            Console.WriteLine("");

            Console.WriteLine("                                 ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-");
            Console.ForegroundColor = ConsoleColor.White;
            #endregion

            GenerateWorld(args);

        }



    }
}
