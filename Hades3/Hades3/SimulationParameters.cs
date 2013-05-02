using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using System.IO;

namespace Hades3
{
    public class SimulationParameters
    {
        public enum BehaviorTypes
        {
            SimpleBehaviors,
            GOBehaviors
        }

        public BehaviorTypes behaviorType;

        public class PipeParameters
        {
            public Vector3 location;
            public List<PipeParameters> children;
        }

        public class TissueParameters
        {
            public Vector3 firstCorner;
            public Vector3 secondCorner;
        }

        public class CellParameters
        {
            public int MaxPressureToleranceAtLocation;
            public int MinPressureToleranceAtLocation;

            public int MaxPressureToleranceAtNeighbors;
            public int MinPressureToleranceAtNeighbors;

            public int MaxFoodConsumptionRate;
            public int MinFoodConsumptionRate;
            public int MaxFoodStorage;
            public int MinFoodStorage;
            public int MaxFoodConcernLevel;
            public int MinFoodConcernLevel;

            public double BlastCellMutationProbability;// = 0;//0.05;
            public double BlastCellRepairProbability;// = 0;//0.10;
            
            public double FinalCellMutationProbability;// = 0;//0.05;
            public double FinalCellRepairProbability;// = 0;//0.10;
            public int FinalCellGenerationLimit;// = 50;
            
            public int PressureAtCell;
            public int PressureNearCell;
            public int MinPressure;

            public Color BlastCellColor;
            public Color FinalCellColor;
            public Color EndothelialCellColor;

            public float PipeShrink;
            public double MutateBy;
        }

        public interface BehaviorParam {}

        public class GOTermParam
        {
            public string termName;
            public string description;
            public double pValue;
            public double startingValue;
            public List<Type> behaviorsAffected;
        }

        public class GOTermBehaviorParams : BehaviorParam
        {
            List<GOTermParam> goTermParams;
        }

        public class SimpleBehaviorParam : BehaviorParam
        {
            public int startingPressureToleranceAtLocation;
            public int startingPressureToleranceAtNeighbors;
            public double startingDivideRate;
            public double startingDeathRate;
            public double FoodConsumptionRate;
            public double FoodMaxStorage;
            public double FoodConcernLevel;
            public double MoveProbability;

            public double EnterPipeProbability;
            public double LeavePipeProbability;
            public double SurvivePipeProbability;
        }

        public class CellStartConfig
        {
            public Type cellType;
            public Vector3 location;
            public SimpleBehaviorParam simpleBehavior;
            public GOTermBehaviorParams goBehavior;
            public int startingFood;
        }

        public String description;
        public List<TissueParameters> tissueParams;
        public Vector3 CameraEye;
        public Vector3 CameraLookAt;


        public CellParameters cellParameters;

        public Vector3 environmentFirstCorner;
        public Vector3 environmentSecondCorner;

        public List<CellStartConfig> startingCells;
        public SimpleBehaviorParam FinalCellConfig;

        public List<GOTermParam> goTermParams;

        public double MinPipeSizeForTravel;
        public PipeParameters pipeParams;
        public double MinimumCellToPipeRatio;
        public EndothelialCell.PipeOrientation PipeRootOrientation;
        public float PipeStartingWidth;

        public SimulationParameters()
        {
            bool newFile = false;
            if (newFile)
            {
                String filename = "params5.json";
                description = "single tissue, blast cell start";

                environmentFirstCorner = new Vector3(0, 0, 0);
                environmentSecondCorner = new Vector3(40, 40, 40);

                tissueParams = new List<TissueParameters>();

                TissueParameters tissue1 = new TissueParameters();
                tissueParams.Add(tissue1);

                // location and dimensions
                tissue1.firstCorner = new Vector3(5, 5, 5);
                tissue1.secondCorner = new Vector3(14, 14, 14);

                // starting cell
                startingCells = new List<CellStartConfig>();
                CellStartConfig firstCell = new CellStartConfig();
                firstCell.cellType = typeof(BlastCell);
                firstCell.location = new Vector3(7,7,7);
                SimpleBehaviorParam firstCellBehavior = new SimpleBehaviorParam();
                firstCellBehavior.startingDivideRate = 0.9;
                firstCellBehavior.startingPressureToleranceAtLocation = 200;
                firstCellBehavior.startingDeathRate = 0.2;
                firstCell.simpleBehavior = firstCellBehavior;
                firstCell.goBehavior = null;
                startingCells.Add(firstCell);

                FinalCellConfig = new SimpleBehaviorParam();
                FinalCellConfig.startingDeathRate = 0.03;
                FinalCellConfig.startingDivideRate = 0.3;
                FinalCellConfig.startingPressureToleranceAtLocation = 200;

                //tissue1.BloodVesselCellLocations = new List<Vector3>();

                CellParameters cp = new CellParameters();
                cellParameters = cp;
                //cp.BlastCellMaxDivideProbability = 0.9;
                //cp.BlastCellMaxDeathProbability = 0.01;
                cp.BlastCellMutationProbability = 0;//0.05;
                cp.BlastCellRepairProbability = 0;//0.10;
                cp.BlastCellColor = Color.Red;
                //cp.FinalCellMaxDivideProbability = 0.30;
                //cp.FinalCellMaxDeathProbability = 0.03;
                cp.FinalCellMutationProbability = 0;//0.05;
                cp.FinalCellRepairProbability = 0;//0.10;
                cp.FinalCellGenerationLimit = 50;
                cp.FinalCellColor = Color.Green;
                cp.PressureAtCell = 100;
                cp.PressureNearCell = 25;
                cp.MinPressure = 25;
                cp.EndothelialCellColor = Color.Purple;



                /*
                GOTermParam go1 = new GOTermParam();
                go1.termName = "go1";
                go1.description = "first test GO";
                go1.pValue = 0.0000001;
                go1.startingValue = 0.1;
                go1.behaviorsAffected = new List<Type>();
                go1.behaviorsAffected.Add(typeof(GODivide));

                GOTermParam go2 = new GOTermParam();
                go1.termName = "go2";
                go1.description = "second test GO";
                go1.pValue = 0.0000001;
                go1.startingValue = 0.2;
                go1.behaviorsAffected = new List<Type>();
                go1.behaviorsAffected.Add(typeof(GODivide));

                GOTermParam go3 = new GOTermParam();
                go1.termName = "go3";
                go1.description = "third test GO";
                go1.pValue = 0.0000001;
                go1.startingValue = 0.3;
                go1.behaviorsAffected = new List<Type>();
                go1.behaviorsAffected.Add(typeof(GODivide));

                goTermParams = new List<GOTermParam>();
                goTermParams.Add(go1);
                goTermParams.Add(go2);
                goTermParams.Add(go3);
                */

                behaviorType = BehaviorTypes.SimpleBehaviors;


                /*
                for (int x = 5; x < 40; x += 10)
                {
                    for (int z = 5; z < 40; z += 10)
                    {
                        for (int y = 0; y < 40; y++)
                        {
                            tissue1.BloodVesselCellLocations.Add(new Vector3(x, y, z));
                        }
                    }
                }
                */
                //BloodVesselCellLocations.Add(new Vector3(0, 0, 0));

                string output = JsonConvert.SerializeObject(this);
                string mydocpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010\Projects\Hades3\Hades3\Hades3\JSONParams\";
                StringBuilder sb = new StringBuilder();
                sb.Append(output);
                using (StreamWriter outfile = new StreamWriter(mydocpath + @"\" + filename))
                {
                    outfile.Write(sb.ToString());
                }
            }


        }

        public int SectorWidth;
        public int SectorHeight;
        public int SectorDepth;

        public int SectorPressureDistance;
        public int SectorPressureInitial;
        public int SectorPressureIncrement;

        public int PipePressureDistance;
        public int PipePressureInitial;
        public int PipePressureIncrement;
        
    }


}
