using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace testProgram
{

    //  Currently this program is accustomed to 64x256
    public class WorldGen
    {
        #region Initial args indexes
        public static readonly int index_seed                   = 0;
        public static readonly int index_mapRows                = 1;
        public static readonly int index_mapCols                = 2;

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
        public static readonly int landCode_coastalLand = 40;
        public static readonly int landCode_Land = 41;
        public static readonly int landCode_hillLand = 42;
        public static readonly int landCode_highLand = 45;
        public static readonly int landCode_mountain = 50;

        #endregion




        #region Geographic Generators

        public static void PrintIDMap(String[] args, int[,] idMap)
        {
            #region Parameters
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);
            //  Loads the requested dimensions
            int MAPROWS = Convert.ToInt32(args[index_mapRows]);
            int MAPCOLS = Convert.ToInt32(args[index_mapCols]);
            int mapsizeModifier = Convert.ToInt32((double)((MAPROWS + MAPCOLS) / 2)); // 160 on a 64x256 grid
            //  Loads the parameters for land generation
            String MAPTYPE = args[index_mapType];
            #endregion

            for (int i = 0; i < idMap.GetLength(0); i++)
            {
                for (int j = 0; j < idMap.GetLength(1); j++)
                {
                    //  Water
                    if (idMap[i,j] == landCode_deepWater)
                    {
                        Console.Write(" ");
                    }
                    else if (idMap[i, j] == landCode_offcoastWater)
                    {
                        Console.Write(" ");
                    }
                    else if (idMap[i, j] == landCode_coastalWater)
                    {
                        Console.Write(".");
                    }
                    //  Land
                    else if (idMap[i, j] == landCode_coastalLand)
                    {
                        Console.Write("~");
                    }
                    else if (idMap[i, j] == landCode_Land)
                    {
                        Console.Write("+");
                    }
                    else if (idMap[i, j] == landCode_hillLand)
                    {
                        Console.Write("H");
                    }
                    else if (idMap[i, j] == landCode_highLand)
                    {
                        Console.Write("#");
                    }
                    else if (idMap[i, j] == landCode_mountain)
                    {
                        Console.Write("M");
                    }

                    else
                    {
                        Console.Write("?");
                    }

                }
                Console.Write("|");
                Console.WriteLine();
            }

            Console.WriteLine("");
        }

        public static int[,] GenerateGeography(String[] args)
        {
            #region Parameters
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);
            //  Loads the requested dimensions
            int MAPROWS = Convert.ToInt32(args[index_mapRows]);
            int MAPCOLS = Convert.ToInt32(args[index_mapCols]);
            int mapsizeModifier = Convert.ToInt32((double)((MAPROWS + MAPCOLS) / 2)); // 160 on a 64x256 grid
            //  Loads the parameters for land generation
            String MAPTYPE = args[index_mapType];
            #endregion

            //  DEBUG Parameters
            Boolean verbose = true;
            Boolean debug = true;


            int contnumber = 1;
            double frequency = 0.15; //  This frequency works best for the size 64x128

            //  Generate the sea floor 

            //  Generate the GeoMap
            int[,] IDMap = new int[MAPROWS, MAPCOLS];

            switch (MAPTYPE)
            {
                case "CONTINENTS":
                    int continentAmountRandom = random.Next(3,4);
                    int continentSizeRank = 3;
                    IDMap = GenerateGeographyContinents(args, continentAmountRandom, continentSizeRank);

                    break;
            }





            //  Print the final
            if (verbose)
            {
                Console.WriteLine("");
                Console.WriteLine("[GENWORLD]   Generating ID Map post processing:");
                PrintIDMap(args, IDMap);
                Console.WriteLine("");
            }
            return IDMap;
        }

        


        //  Generates a map for MapType Continents
        #region Generate MapComponents

           
            //  Generates a couple of large continents, and returns a map thereof
        public static int[,] GenerateGeographyContinents(String[] args, int continentNumber, int continentSizes) {
            #region Parameters
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);
            //  Loads the requested dimensions
            int MAPROWS = Convert.ToInt32(args[index_mapRows]);
            int MAPCOLS = Convert.ToInt32(args[index_mapCols]);
            int mapsizeModifier = Convert.ToInt32((double)((MAPROWS + MAPCOLS) / 2)); // 160 on a 64x256 grid
            //  Loads the parameters for land generation
            String MAPTYPE = args[index_mapType];
            #endregion

            bool verbose = true;
            bool debug1 = true;

            int sealevelOffset;
            int mountainOffset;
            int bulgeCap;

            //  Generation Parameters
            double frequency = 3;  //  Adapt for size
            

            bulgeCap = Convert.ToInt32(((double)mapsizeModifier / 16.0));   
            //  Apply offsets that tend to create more continent-sized masses
            sealevelOffset = 32;
            mountainOffset = 56;



            //  Debugging
            //continentSizes = 4;
            int minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 10));  //
            int maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 32));
            switch (continentSizes)
            {
                case 1:
                    //  Get Continents of a very small size
                    minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 2.5));  //
                    maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 4));
                    break;
                case 2:
                    //  Get Continents of a small size
                    minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 5));  //
                    maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 16));
                    break;
                case 3:
                    //  Get Continents of a medium size
                    minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 10));  //
                    maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 32));
                    break;
                case 4:
                    //  Get Continents of a large size
                    minSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 15));  //
                    maxSizeContinent = Convert.ToInt32(((double)mapsizeModifier * 64));
                    break;
                    
                default:

                    break;
            }
            





            //  Generate a blank canvas
            int[,] geoNoise = new int[MAPROWS, MAPCOLS];
            for (int i = 0; i < geoNoise.GetLength(0); i++)
            {
                for (int j = 0; j < geoNoise.GetLength(1); j++)
                {
                    geoNoise[i, j] = 0;
                }
            }


            //  Iterates until the proper amount of continents is selected
            int continentCurrent = 0;
            int errorCount = 0;

            //  Generate NoiseMap for continents
            #region Generate a random noisemap that appears "natural" and contains multiple continents
            
            regenerateContinentsMap:
            if (errorCount >= 300)
            {
                if (debug1)
                {
                    int numLines = (continentCurrent * MAPROWS) + (continentCurrent*1) ;
                    Utility.Debug.ClearLine(numLines);
                }
                if (verbose)
                {
                    Console.WriteLine("Error Count exceeds limit, restarting...");
                }
                //  If error count restarts, assume a fuck-up and regenerate the whole mess
                geoNoise = new int[MAPROWS, MAPCOLS];
                continentCurrent = 0;
                errorCount = 0;
            }

                //  Create the original map
            while (continentCurrent < continentNumber)
            {
                //  Make the Continent Noise appear more normal
                int generatorSeed = random.Next(-999999999, 999999999);
                int[,] ContinentNoise = Utility.Perlin.GeneratePerlinInt(MAPROWS, MAPCOLS, frequency, landgen_lowerbound, landgen_upperbound, generatorSeed);
                


                //  Shave off edges
                for (int i = 0; i < ContinentNoise.GetLength(0); i++)
                {
                    for (int j = 0; j < (bulgeCap / 2); j++)
                    {
                        if (j < (bulgeCap / 2))
                        {
                            ContinentNoise[i, j] = landgen_lowerbound;
                        }

                    }

                    for (int j = ContinentNoise.GetLength(1) - (bulgeCap / 2); j < ContinentNoise.GetLength(1); j++)
                    {
                        if (j < ContinentNoise.GetLength(1))
                        {
                            ContinentNoise[i, j] = landgen_lowerbound;
                        }

                    }


                }

               
                //  Initial Modify coastlines (this is only done to break up the coastlines for better randomization
                List<List<Coords>> coastlineArray = new List<List<Coords>>();
                for (int biteration = 0; biteration < 2; biteration++)
                {
                    coastlineArray = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandBordersInRange(ContinentNoise, sealevelOffset, 999, true, true, 1);
                    foreach (List<Coords> coastlineCoords in coastlineArray)
                    {
                        foreach (Coords coord in coastlineCoords)
                        {
                            //  Generate the probability outwards or inwards
                            int coastlinebulgeProb = random.Next(0, 101);
                            //  Outwards
                            if (coastlinebulgeProb <= 75)
                            {
                                int radius = random.Next(0, bulgeCap);
                                List<Coords> circoords = Utility.Matrices.MatrixManipulation.SelectCircleRegion(ContinentNoise, coord, radius);
                                foreach (Coords circoord in circoords)
                                {
                                    if (ContinentNoise[circoord.x, circoord.y] < landgen_lowerbound)
                                    {
                                        ContinentNoise[circoord.x, circoord.y] = sealevelOffset;
                                    }

                                }
                            }
                            //  Indent
                            else
                            {
                                int radius = random.Next(0, Convert.ToInt32((double)(bulgeCap / 2)));
                                List<Coords> circoords = Utility.Matrices.MatrixManipulation.SelectCircleRegion(ContinentNoise, coord, radius);
                                foreach (Coords circoord in circoords)
                                {
                                    if (ContinentNoise[circoord.x, circoord.y] >= sealevelOffset)
                                    {
                                        ContinentNoise[circoord.x, circoord.y] = landgen_lowerbound;
                                    }

                                }
                            }
                        }
                    }
                }



                //  Shave off edges on the west and east side
                for (int i = 0; i < ContinentNoise.GetLength(0); i++)
                {
                    for (int j = 0; j < (bulgeCap / 2); j++)
                    {
                        if (j < (bulgeCap / 2))
                        {
                            ContinentNoise[i, j] = landgen_lowerbound;
                        }

                    }

                    for (int j = ContinentNoise.GetLength(1) - (bulgeCap / 2); j < ContinentNoise.GetLength(1); j++)
                    {
                        if (j < ContinentNoise.GetLength(1))
                        {
                            ContinentNoise[i, j] = landgen_lowerbound;
                        }

                    }


                }



                //  Remove landmasses that do not fall within the size range
                List<List<Coords>> validContinents = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandsInRange(ContinentNoise, sealevelOffset, 99999, false, false);
                Utility.Lists.RemoveListBelowSize(validContinents, minSizeContinent);
                Utility.Lists.RemoveListAboveSize(validContinents, 3000);
                if (validContinents.Count() < 1)
                {
                    goto regenerateContinentsMap;
                }



                // Select one continent from this map, and attempt to add it to the canvas
                List<Coords> randomContinent = new List<Coords>();
                List<int> blacklistedIndices = new List<int>();
                int randomNumContinent = random.Next(0, validContinents.Count());
                randomContinent = validContinents[randomNumContinent];

                    //  Establish a zone around all current continents where you cannot paste other continents
                List<Coords> nogoZone = new List<Coords>();
                List<List<Coords>> allLandZones = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandsInRange(geoNoise, sealevelOffset, mountainOffset, false, false);
                int sealineGap = Convert.ToInt32((double)(mapsizeModifier / 16));
                List<List<Coords>> coastlineZones = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandBordersInRange(geoNoise, sealevelOffset, mountainOffset, false, false, sealineGap);
                foreach (List<Coords> coordsList in allLandZones)
                {
                    nogoZone.AddRange(coordsList);
                }
                foreach (List<Coords> coordsList in coastlineZones)
                {
                    nogoZone.AddRange(coordsList);
                }
                    //  Verify that no coords are in the nogozone
                foreach (List<Coords> coordsList in validContinents)
                {
                    foreach (Coords coordinates in coordsList)
                    {
                        if (nogoZone.Contains(coordinates))
                        {
                            errorCount++;
                            goto regenerateContinentsMap;
                        }
                    }
                }

                    //  Verify the landmass dimensions are within a good range
                int continentColCount = Utility.Matrices.MatrixManipulation.MaxRowDistance(randomContinent);

                int rowMinimum = Convert.ToInt32((double)(mapsizeModifier / 4));
                int colMaximum = Convert.ToInt32((double)(mapsizeModifier / 4));

                if (continentColCount > rowMinimum)
                {
                    errorCount++;
                    goto regenerateContinentsMap;
                }
                





                //  Add the coords of validContinents to the canvas, increment the counter
                foreach (Coords coord in randomContinent)
                {
                    geoNoise[coord.x, coord.y] = ContinentNoise[coord.x, coord.y];
                }
                continentCurrent++;

                if (debug1) 
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    
                    for (int i = 0; i < geoNoise.GetLength(0); i++) {
                        Console.Write(continentCurrent.ToString("X"));
                        for (int j = 0; j < geoNoise.GetLength(1); j++)
                        {

                            if (geoNoise[i,j] > sealevelOffset)
                            {
                                Console.Write("-");
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                        Console.Write(continentCurrent.ToString("X"));
                        Console.WriteLine();
                    }

                    Console.ForegroundColor = ConsoleColor.White;



                }

            }

            #endregion

            //  Convert noisemap to IDMAP
            #region
                //  Create the basis of the map
            int[,] IDMAP = new int[MAPROWS, MAPCOLS];
            for (int i = 0; i < geoNoise.GetLength(0); i++)
            {
                for (int j = 0; j < geoNoise.GetLength(1); j++)
                {
                    if (geoNoise[i, j] < sealevelOffset)
                    {
                        IDMAP[i, j] = landCode_deepWater;
                    } else
                    {
                        IDMAP[i, j] = landCode_Land;
                    }
                }
            }
            //

            //  Turn Coastlines to proper value
            int coastlineOff = Convert.ToInt32((double)(mapsizeModifier / 64));
            List<List<Coords>> coastlines = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandBordersInRange(IDMAP, landCode_deepWater, landCode_deepWater, false, false, coastlineOff);
            //  Convert to coastlines
            foreach (List<Coords> coordsList in coastlines) {
                foreach (Coords coord in coordsList) {
                    IDMAP[coord.x, coord.y] = landCode_coastalLand;
                }
            }

            //  Create coastline waters
            int coastwaterOff = Convert.ToInt32((double)(mapsizeModifier / 32));
            List<List<Coords>> coastwaters = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandBordersInRange(IDMAP, landCode_Land, landCode_Land, false, false, coastwaterOff);
            //  Convert to coastlines
            foreach (List<Coords> coordsList in coastwaters)
            {
                foreach (Coords coord in coordsList)
                {
                    if (IDMAP[coord.x, coord.y] < landCode_coastalLand)
                    {
                        //IDMAP[coord.x, coord.y] = landCode_coastalWater;
                    }
                    
                }
            }

            //  Modify the edges of coastline waters
            
           
            #endregion


            //  Cleanup and post processing
            #region Perform Final Cleanup
            //  Fill in lakes
            List<List<Coords>> lakes = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandsInRange(geoNoise, landgen_lowerbound, sealevelOffset, false, false);
            
            Utility.Lists.RemoveListAboveSize(lakes, Convert.ToInt32((double)(mapsizeModifier / 16)));
            foreach (List<Coords> list in lakes)
            {
                foreach (Coords coords in list)
                {
                    //geoNoise[coords.x, coords.y] = sealevelOffset;
                }
            }

            


            #endregion



            return IDMAP;
        }


            //  Generates a large amount of scattered islands
        public static int[,] GenerateGeographyIslands(String[] args)
        {
            #region Parameters
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);
            //  Loads the requested dimensions
            int MAPROWS = Convert.ToInt32(args[index_mapRows]);
            int MAPCOLS = Convert.ToInt32(args[index_mapCols]);
            int mapsizeModifier = Convert.ToInt32((double)((MAPROWS + MAPCOLS) / 2)); // 160 on a 64x256 grid
            //  Loads the parameters for land generation
            String MAPTYPE = args[index_mapType];
            #endregion
            double frequency = 0.15;
            //  Kind of works for islands dunno why

            int[,] errorfree = new int[MAPROWS,MAPCOLS];
            return errorfree;
        }




        #endregion
        #endregion



























        //  This function generates a representation of the world using WorldTile Objects
        public static void GenerateWorld(String[] args) {
            #region Parameters
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);
            //  Loads the requested dimensions
            int MAPROWS = Convert.ToInt32(args[index_mapRows]);
            int MAPCOLS = Convert.ToInt32(args[index_mapCols]);
            int mapsizeModifier = Convert.ToInt32((double)((MAPROWS + MAPCOLS) / 2)); // 160 on a 64x256 grid
            //  Loads the parameters for land generation
            String MAPTYPE = args[index_mapType];
            #endregion

            //  Generate the Geography Map according to the parameters
            int[,] GeographyNoise = GenerateGeography(args);








           
        }

        public static void testGenerationWorld(String[] args) {
            #region Parameters
            //  Loads the initial seed for randomization
            int INITSEED = Convert.ToInt32(args[index_seed]);
            Random random = new Random(INITSEED);
            //  Loads the requested dimensions
            int MAPROWS = Convert.ToInt32(args[index_mapRows]);
            int MAPCOLS = Convert.ToInt32(args[index_mapCols]);
            int mapsizeModifier = Convert.ToInt32((double)((MAPROWS + MAPCOLS) / 2)); // 160 on a 64x256 grid
            //  Loads the parameters for land generation
            String MAPTYPE = args[index_mapType];
            #endregion


            Console.WriteLine("This is the console test for worldGenerator. ");
            //  Generate a world according to the init args
            Console.WriteLine("Parameters:");
            Console.WriteLine("SEED:                " + INITSEED);
            Console.WriteLine("                                 ");
            Console.WriteLine("ROWS:                " + MAPROWS);
            Console.WriteLine("COLS:                " + MAPCOLS);
            Console.WriteLine("MapSize Modifier:    " + mapsizeModifier);
            Console.WriteLine("                                 ");
            
            Console.WriteLine("MAPTYPE:             " + MAPTYPE);


            Console.WriteLine("                                 ");
            Console.WriteLine("-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-X-");


            GenerateWorld(args);


        }


    }
}
