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

        public class PipeLocations
        {
            public Vector3 location;
            public List<PipeLocations> children;
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

            public int FinalCellGenerationLimit;

            public double BlastCellDivideIntoBlastProbability;
            public double BlastCellDivideIntoFinalProbability;

            public int PressureAtCell;
            public int PressureNearCell;

            public double MutateBy;
        }

        public class PipeParameters
        {
            public double MinimumCellToPipeRatio;
            public EndothelialCell.PipeOrientation PipeRootOrientation;
            public float PipeStartingWidth;
            public float PipeShrink;
            public double MinPipeSizeForTravel;
            public int MaxGrowthPerTurn;
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
            public int FoodConsumptionRate;
            public int FoodMaxStorage;
            public int FoodConcernLevel;
            public double MoveProbability;

            public double EnterPipeProbability;
            public double LeavePipeProbability;
            public double SurvivePipeProbability;

            public double CallPipeProbability;
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

        
        public PipeLocations PipeCorners;
        public PipeParameters PipeParams;

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
