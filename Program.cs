using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using Utility;

namespace testProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Code Begun");
            testWorldgen();



            //testVoronoi();

            //matrixSizeTest();

        }


        

        static void matrixSizeTest()
        {
            Random rand = new Random();
            int seed = rand.Next(-99999999, 99999999);
            double freq1 = 3;
            double freq2 = 3;
            double freq3 = 3;

            int minimum = 16;

            //  64x192
            int[,] testArray1 = Utility.Perlin.GeneratePerlinInt(128, 512, freq1, 0, 31, seed);
                        
            
            
            int[,] testArray2 = Utility.Perlin.GeneratePerlinInt(256, 1024, freq1, 0, 31, seed);

            int[,] testArray3 = Utility.Perlin.GeneratePerlinInt(512, 2048, freq1, 0, 31, seed);



            //  64x192
            Console.WriteLine("64x192");
            for (int i = 0;i < testArray1.GetLength(0);i++)
            {
                for (int j = 0; j < testArray1.GetLength(1);j++)
                {
                    if (testArray1[i,j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");

            //  128x384
            Console.WriteLine("128x384");
            for (int i = 0; i < testArray2.GetLength(0); i++)
            {
                for (int j = 0; j < testArray2.GetLength(1); j++)
                {
                    if (testArray2[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");


            //  256x384
            Console.WriteLine("256x384");
            for (int i = 0; i < testArray3.GetLength(0); i++)
            {
                for (int j = 0; j < testArray3.GetLength(1); j++)
                {
                    if (testArray3[i, j] > minimum)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
                Console.WriteLine("");
            }
            Console.WriteLine("");


        }
        static void testVoronoi()
        {
            //Land Codes Below Sealine
            int landCode_deepWater = 25;
            int landCode_offcoastWater = 29;
            int landCode_coastalWater = 30;

            //Land Codes Above Sealine
            int landCode_coastalLand = 40;
            int landCode_Land = 41;
            int landCode_hillLand = 42;
            int landCode_highLand = 45;
            int landCode_mountain = 50;


            Random rand = new Random();
            
            //58791614

            int randseedint = rand.Next(0,9999999);
            String randseed = Convert.ToString(randseedint);
            Boolean testable = true;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            String[] args = {
                randseed,       //  00 Seed: used for randomization
                "64",           //  01 Rows
                "256",          //  02 Cols
                "NULL",         //  03 Placeholder
                "NULL",         //  04 Placeholder
                "CONTINENTS",   //  05 MapType
                "NULL",         //  06 Placeholder
                "NULL",         //  07 Placeholder
                "NULL",         //  08 Placeholder
                "NULL",         //  09 Placeholder
                "NULL",         //  10 Placeholder
                "NULL",         //  11 Placeholder
                "NULL",         //  12 Placeholder
                "NULL",         //  13 Placeholder
                "NULL",         //  14 Placeholder
                "NULL",         //  15 Placeholder
                "NULL",         //  16 Placeholder
                "NULL",         //  17 Placeholder
                "NULL",         //  18 Placeholder

            
            };

            //  Print the status
            #region Verbose
            Console.WriteLine("This is the console test for the Voronoi Partitioner. ");
            //  Generate a world according to the init args
            Console.WriteLine("Parameters:");
            Console.WriteLine("SEED:                " + args[0]);
            Console.WriteLine("                                 ");
            Console.WriteLine("ROWS:                " + args[1]);
            Console.WriteLine("COLS:                " + args[2]);
            Console.WriteLine("                                 ");
            Console.WriteLine("MAPTYPE:             " + args[5]);

            Console.WriteLine("                                 ");
            Console.WriteLine("-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-V-");



            #endregion

            //  Generate map
            #region Generate Map
            int[,] testWorld = WorldGen.GenerateGeographyContinents(args, 2, 4);
            WorldGen.PrintIDMap(args, testWorld);
            Console.WriteLine("");
            #endregion

            //  Generate source seeds
            List<List<Coords>> islandsCoords = Utility.Matrices.SearchesAndPathfinding.Islands.FindIslandsInRange(testWorld, landCode_coastalLand, 999, false, false);
            List<Coords> sources = new List<Coords>();
            foreach (List<Coords> coordslists in islandsCoords)
            {
                List<Coords> addable = Utility.Lists.GetRandomSelection<Coords>(coordslists, 3, false, randseedint);
                sources.AddRange(addable);

            }

            
           
            //  Perform the Voronoi
            List<List<Coords>> voronoiLists = Utility.Matrices.SearchesAndPathfinding.VoronoiExpansions.ExpandTerritories(testWorld, sources, landCode_coastalLand, 999, false, false, randseedint);
            //  Print the Voronoi
            Utility.Matrices.SearchesAndPathfinding.VoronoiExpansions.VoronoiTestPrint(testWorld, voronoiLists, sources);
        }

        static void testVoronoi2()
        {
            Random rand = new Random();
            int randseedint = rand.Next(0, 9999999);

            int arrayROWS = 64;
            int arrayCOLS = 256;
            int sizeModifer = Convert.ToInt32((double)((64+256)/2));    //160

            int minsources = 10;
            int maxsources = 20;


            #region Create and print the original Voronoi sources and lists
            int[,] randomArray = new int[arrayROWS, arrayCOLS];



            int numberInput = 1;
            //Sets to the array
            for (int i = 0; i < randomArray.GetLength(0); i++)
            {
                for (int j = 0; j < randomArray.GetLength(1); j++)
                {
                    randomArray[i, j] = numberInput;
                }

            }




            List<Coords> sources = new List<Coords>();
            int numseeds = rand.Next(minsources, maxsources);
            for (int counter = 0; counter < numseeds; counter++)
            {
            repeatLoop:
                int xCoord = rand.Next(0, arrayROWS);
                int yCoord = rand.Next(0, arrayCOLS);
                Coords example = new Coords(xCoord, yCoord);

                //  Check there are no values in radius
                List<Coords> radiusregion = Utility.Matrices.MatrixManipulation.SelectCircleRegion(randomArray, example, Convert.ToInt32((double)(sizeModifer / 16)));


                foreach (Coords coord in radiusregion)
                {
                    if (sources.Contains(coord))
                    {
                        goto repeatLoop;
                    }
                }
                sources.Add(example);
            }

            List<List<Coords>> voronoiLists = Utility.Matrices.SearchesAndPathfinding.VoronoiExpansions.ExpandTerritories(randomArray, sources, numberInput, 999, false, false, randseedint);
            //  Print the Voronoi
            Utility.Matrices.SearchesAndPathfinding.VoronoiExpansions.VoronoiTestPrint(randomArray, voronoiLists, sources);


            #endregion



            Console.WriteLine();
            Console.WriteLine("Converting to int array:");
            Console.WriteLine();
            int[,] newarray = new int[arrayROWS, arrayCOLS];

            //  Copy newarray 
            for (int i = 0; i < newarray.GetLength(0); i++)
            {
                for (int j = 0; j < newarray.GetLength(1); j++)
                {
                    newarray[i, j] = 0;
                }
                
            }

            int value = 1;
            foreach (List<Coords> coordslist in voronoiLists) {
                foreach (Coords coord in coordslist)
                {
                    newarray[coord.x, coord.y] = value;
                }
                value++;
            }


            
            for (int i = 0; i < newarray.GetLength(0); i++)
            {
                for (int j = 0; j < newarray.GetLength(1); j++)
                {
                    
                    Console.Write(" " + newarray[i,j].ToString("X2"));
                }
                Console.WriteLine();
            }

            //  Generate edges



        }

        static void testWorldgen()
        {
            /** Seeds used
                * 41525353
                * 34567890
                * 69696969
                * 99999999
                * 56866774 (Good looking continents)
                * 
                * Mostly land: 95651631
                * 
                 
                */
            Random rand = new Random();
            String randseed = Convert.ToString(rand.Next(-999999,9999999));

            //randseed = "5265958";
            Boolean testable = true;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            String size1 = "64";
            String size2 = "256";

            size1 = "128";
            size2 = "512";


            String[] args = { 
                randseed,       //  00 Seed: used for randomization
                size1,           //  01 Rows
                size2,          //  02 Cols
                "NULL",         //  03 Placeholder
                "NULL",         //  04 Placeholder
                "CONTINENTS",   //  05 MapType
                "NULL",         //  06 Placeholder
                "NULL",         //  07 Placeholder
                "NULL",         //  08 Placeholder
                "NULL",         //  09 Placeholder
                "NULL",         //  10 Placeholder
                "NULL",         //  11 Placeholder
                "NULL",         //  12 Placeholder
                "NULL",         //  13 Placeholder
                "NULL",         //  14 Placeholder
                "NULL",         //  15 Placeholder
                "NULL",         //  16 Placeholder
                "NULL",         //  17 Placeholder
                "NULL",         //  18 Placeholder

            
            };
            WorldGen.testGenerationWorld(args);
            WorldGen.testGenerationWorld(args);



            //  restest
            int testnumber = 8;
            if (testable) {
                for (int i = 0; i < testnumber; i++) {
                    randseed = Convert.ToString(rand.Next(0, 99999999));


                    args[0] = randseed;
                    Console.WriteLine();
                    Console.WriteLine();
                    WorldGen.testGenerationWorld(args);
                    Console.WriteLine();
                    Console.WriteLine();

                }
            }




            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }


    }
}
