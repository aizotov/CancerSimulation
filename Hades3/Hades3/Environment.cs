using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class Environment
    {
        public static Random random = new Random();

        private static Environment instance;
        public static Environment Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("uninitialized environment");
                }
                return instance;
            }
        }

        
        private Vector3 firstCorner;
        public Vector3 FirstCorner
        {
            get
            {
                return firstCorner;
            }
        }
        

        private Vector3 secondCorner;
        public Vector3 SecondCorner
        {
            get
            {
                return secondCorner;
            }
        }

        private List<Tissue> tissues;
        public List<Tissue> Tissues
        {
            get
            {
                return tissues;
            }
        }

        public int BlastCellTotal;
        public int FinalCellTotal;
        public int MutatedCellTotal;

        private List<TissueCell> cells;
        public List<TissueCell> Cells
        {
            get
            {
                return cells;
            }
        }

        private List<TissueCell> newCells;
        private List<TissueCell> dyingCells;

        private List<Signal> signals;
        public List<Signal> Signals
        {
            get
            {
                return signals;
            }
        }

        private List<Signal> newSignals;
        private List<Signal> expiredSignals;

        private Sector[, ,] sectorMap;
        private LocationContents[, ,] environmentMatrix;

        private CirculatorySystem circulatorySystem;

        private List<LocationContents> pressurePoints;
        public List<LocationContents> PressurePoints
        {
            get
            {
                return pressurePoints;
            }
        }


        public Environment(SimulationParameters simulationParams)
        {
            instance = this;

            firstCorner = simulationParams.environmentFirstCorner;
            secondCorner = simulationParams.environmentSecondCorner;

            BlastCellTotal = 0;
            FinalCellTotal = 0;
            MutatedCellTotal = 0;

            tissues = new List<Tissue>();
            foreach (SimulationParameters.TissueParameters tissueParam in simulationParams.tissueParams)
            {
                tissues.Add(new Tissue(tissueParam));
            }

            cells = new List<TissueCell>();
            newCells = new List<TissueCell>();
            dyingCells = new List<TissueCell>();

            signals = new List<Signal>();
            newSignals = new List<Signal>();
            expiredSignals = new List<Signal>();

            pressurePoints = new List<LocationContents>();

            Console.WriteLine("setting up environment matrix");
            environmentMatrix = new LocationContents[(int)secondCorner.X + 1, (int)secondCorner.Y + 1, (int)secondCorner.Z + 1];
            createMatrix(firstCorner, secondCorner);

            Console.WriteLine("setting up initial pressure around the [" + tissues.Count + "] tissue(s)");
            foreach(Tissue t in tissues)
                setupPressureAroundTissue(t, simulationParams.SectorPressureDistance, simulationParams.SectorPressureInitial, simulationParams.SectorPressureIncrement);

            Console.WriteLine("setting up sectors");
            sectorMap = new Sector[(int)secondCorner.X + 1, (int)secondCorner.Y + 1, (int)secondCorner.Z + 1];
            List<Sector> sectors = sectorSetup(simulationParams.SectorHeight, simulationParams.SectorWidth, simulationParams.SectorDepth);
            Console.WriteLine("there are [" + sectors.Count + "] total sectors");

            Console.WriteLine("creating location->sector map");
            CreateSectorMap(sectors);

            Console.WriteLine("initializing circulatory system");
            circulatorySystem = new CirculatorySystem(simulationParams.pipeParams, simulationParams.cellParameters.PipeShrink, simulationParams.PipeRootOrientation, simulationParams.PipeStartingWidth);

            Console.WriteLine("setting up initial pressue around the pipes");
            setupPressureAroundPipes(circulatorySystem, simulationParams.PipePressureDistance, simulationParams.PipePressureInitial, simulationParams.PipePressureIncrement);

            Console.WriteLine("initializing cells");
            initializeCells(simulationParams.startingCells, simulationParams.behaviorType);
        }

        public Sector GetSectorAt(Vector3 location)
        {
            return sectorMap[(int)location.X, (int)location.Y, (int)location.Z];
        }

        public LocationContents GetContentsAt(Vector3 location)
        {
            return environmentMatrix[(int)location.X, (int)location.Y, (int)location.Z];
        }

        private void setupPressureAroundTissue(Tissue tissue, int distance, int initialPressure, int pressureIncrement)
        {
            HashSet<Vector3> outerRim = new HashSet<Vector3>();
            HashSet<Vector3> innerRim = new HashSet<Vector3>();

            for (int x = (int)tissue.FirstCorner.X; x <= tissue.SecondCorner.X; x++)
            {
                for (int y = (int)tissue.FirstCorner.Y; y <= tissue.SecondCorner.Y; y++)
                {
                    for (int z = (int)tissue.FirstCorner.Z; z <= tissue.SecondCorner.Z; z++)
                    {
                        if (x == (int)tissue.FirstCorner.X)
                        {
                            outerRim.Add(new Vector3(x - 1, y, z));
                            innerRim.Add(new Vector3(x, y, z));
                        }
                        if (x == (int)tissue.SecondCorner.X)
                        {
                            outerRim.Add(new Vector3(x + 1, y, z));
                            innerRim.Add(new Vector3(x, y, z));
                        }

                        if (y == (int)tissue.FirstCorner.Y)
                        {
                            outerRim.Add(new Vector3(x, y - 1, z));
                            innerRim.Add(new Vector3(x, y, z));
                        }
                        if (y == (int)tissue.SecondCorner.Y)
                        {
                            outerRim.Add(new Vector3(x, y + 1, z));
                            innerRim.Add(new Vector3(x, y, z));
                        }

                        if (z == (int)tissue.FirstCorner.Z)
                        {
                            outerRim.Add(new Vector3(x, y, z - 1));
                            innerRim.Add(new Vector3(x, y, z));
                        }
                        if (z == (int)tissue.SecondCorner.Z)
                        {
                            outerRim.Add(new Vector3(x, y, z + 1));
                            innerRim.Add(new Vector3(x, y, z));
                        }

                    }
                }
            } 
            
            foreach (Vector3 loc in outerRim)
                pressurePoints.Add(environmentMatrix[(int)loc.X, (int)loc.Y, (int)loc.Z]);
            
            
            sendPressureWave(innerRim, outerRim, distance, initialPressure, pressureIncrement);
        }

        private void setupPressureAroundPipes(CirculatorySystem pipeSystem, int distance, int initialPressure, int pressureIncrement)
        {
            HashSet<Vector3> past = new HashSet<Vector3>();
            HashSet<Vector3> present = new HashSet<Vector3>();

            foreach(EndothelialCell pipePiece in pipeSystem.PipeCells)
                present.Add(pipePiece.CellLocation);

            sendPressureWave(past, present, distance, initialPressure, pressureIncrement);
        }

        private void sendPressureWave(HashSet<Vector3> past, HashSet<Vector3> present, int distance, int initialPressure, int pressureIncrement)
        {
            int currDistance = 0;
            int currPressure = initialPressure;

            HashSet<Vector3> newest = new HashSet<Vector3>();

            while (currDistance < distance)
            {
                // set pressure
                foreach (Vector3 loc in present)
                {
                    if (GoodPoint(loc) && pointOutsideTissues(loc))
                    {
                        environmentMatrix[(int)loc.X, (int)loc.Y, (int)loc.Z].Pressure += currPressure;
                        pressurePoints.Add(environmentMatrix[(int)loc.X, (int)loc.Y, (int)loc.Z]);
                    }
                }

                //calculate next iteration
                foreach (Vector3 loc in present)
                {
                    ICollection<Vector3> neighbors = GetAllNeighborLocations(loc);
                    newest.UnionWith(neighbors);
                }
                newest.ExceptWith(past);
                newest.ExceptWith(present);

                currDistance++;
                currPressure += pressureIncrement;

                past = new HashSet<Vector3>(present);
                present = new HashSet<Vector3>(newest);
                newest.Clear();
            }
        }

        public Tissue TissueContainingPoint(Vector3 location)
        {
            foreach (Tissue t in tissues)
            {
                if (t.ContainsPoint(location))
                    return t;
            }
            return null;
        }


        private void initializeCells(List<SimulationParameters.CellStartConfig> startingCells, SimulationParameters.BehaviorTypes cellBehaviorType)
        {
            foreach (SimulationParameters.CellStartConfig cellConfig in startingCells)
            {
                if (!GoodPoint(cellConfig.location))
                    throw new Exception("invalid cell start location " + cellConfig.location + "... it isn't part of tissue [" + firstCorner + "," + secondCorner + "]");

                if (cellBehaviorType == SimulationParameters.BehaviorTypes.SimpleBehaviors)
                {
                    if (cellConfig.cellType == typeof(BlastCell))
                        AddNewCell(new BlastCell(cellConfig.location, cellConfig.simpleBehavior, cellConfig.startingFood));
                    else if (cellConfig.cellType == typeof(FinalCell))
                        AddNewCell(new FinalCell(cellConfig.location, cellConfig.simpleBehavior, cellConfig.startingFood));
                    else
                        throw new Exception("unsupported cell type [" + cellConfig.cellType + "]");
                }

                else if (cellBehaviorType == SimulationParameters.BehaviorTypes.GOBehaviors)
                    throw new NotImplementedException("haven't tested GO behaviors yet");
                else
                    throw new Exception("unsupported cell behavior type");

            }
        }

        private void createMatrix(Vector3 firstCorner, Vector3 secondCorner)
        {
            for (int x = (int)firstCorner.X; x <= secondCorner.X; x++)
            {
                for (int y = (int)firstCorner.Y; y <= secondCorner.Y; y++)
                {
                    for (int z = (int)firstCorner.Z; z <= secondCorner.Z; z++)
                    {
                        Vector3 currLoc = new Vector3(x, y, z);
                        environmentMatrix[(int)currLoc.X, (int)currLoc.Y, (int)currLoc.Z] = new LocationContents(currLoc);

                        Tissue t = TissueContainingPoint(currLoc);
                        if (t != null)  // t is part of a tissue
                        {
                            environmentMatrix[(int)currLoc.X, (int)currLoc.Y, (int)currLoc.Z].Pressure = 0;
                        }
                        else            // t is not part of a tissue
                        {
                            environmentMatrix[(int)currLoc.X, (int)currLoc.Y, (int)currLoc.Z].Pressure = 0;
                        }


                    }// end for every z
                }// end for every y
            }// end for every x
        }


        public void AddNewCell(TissueCell newCell)
        {
            newCells.Add(newCell);
            

            if (newCell.Mutated)
                MutatedCellTotal++;

            // for the cell count
            if (newCell.GetType() == typeof(FinalCell))
                FinalCellTotal++;
            else if (newCell.GetType() == typeof(BlastCell))
                BlastCellTotal++;
            else
                throw new Exception("unsupported cell type");
        }

        public void DeleteCell(TissueCell deadCell)
        {
            dyingCells.Add(deadCell);
            
            if (deadCell.Mutated)
                MutatedCellTotal--;

            // for the cell count
            if (deadCell.GetType() == typeof(FinalCell))
                Environment.Instance.FinalCellTotal--;
            else if (deadCell.GetType() == typeof(BlastCell))
                Environment.Instance.BlastCellTotal--;
            else
                throw new Exception("unsupported cell type");
        }


        public bool GoodPoint(Vector3 point)
        {
            if (point.X >= firstCorner.X && point.Y >= firstCorner.Y && point.Z >= firstCorner.Z &&
                point.X <= secondCorner.X && point.Y <= secondCorner.Y && point.Z <= secondCorner.Z)
                return true;
            return false;
        }

        private bool pointOutsideTissues(Vector3 point)
        {
            foreach (Tissue t in tissues)
            {
                if (t.ContainsPoint(point))
                    return false;
            }
            return true;
        }


        public void RemovePressure(Vector3 location)
        {
            environmentMatrix[(int)location.X, (int)location.Y, (int)location.Z].Pressure -= SimulationCore.Instance.SimulationParams.cellParameters.PressureAtCell;
            ICollection<Vector3> neighbors = GetValidNeighborLocations(location);
            foreach (Vector3 neighbor in neighbors)
                environmentMatrix[(int)neighbor.X, (int)neighbor.Y, (int)neighbor.Z].Pressure -= SimulationCore.Instance.SimulationParams.cellParameters.PressureNearCell;
        }

        public void AddPressure(Vector3 location)
        {
            if (SimulationCore.DEBUG)
            {
                if (!GoodPoint(location))
                    throw new Exception("changing pressure outside of the tissue");
            }

            environmentMatrix[(int)location.X, (int)location.Y, (int)location.Z].Pressure += SimulationCore.Instance.SimulationParams.cellParameters.PressureAtCell;
            ICollection<Vector3> neighbors = GetValidNeighborLocations(location);
            foreach (Vector3 neighbor in neighbors)
            {
                environmentMatrix[(int)neighbor.X, (int)neighbor.Y, (int)neighbor.Z].Pressure += SimulationCore.Instance.SimulationParams.cellParameters.PressureNearCell;
            }
        }


        public List<LocationContents> GetValidNeighborContents(Vector3 initialLocation)
        {
            List<LocationContents> neighboringPositions = new List<LocationContents>();

            if (initialLocation.X + 1 < secondCorner.X)
                neighboringPositions.Add(environmentMatrix[(int)initialLocation.X + 1, (int)initialLocation.Y, (int)initialLocation.Z]);
            if (initialLocation.X - 1 >= firstCorner.X)
                neighboringPositions.Add(environmentMatrix[(int)initialLocation.X - 1, (int)initialLocation.Y, (int)initialLocation.Z]);
            if (initialLocation.Y + 1 < secondCorner.Y)
                neighboringPositions.Add(environmentMatrix[(int)initialLocation.X, (int)initialLocation.Y + 1, (int)initialLocation.Z]);
            if (initialLocation.Y - 1 >= firstCorner.Y)
                neighboringPositions.Add(environmentMatrix[(int)initialLocation.X, (int)initialLocation.Y - 1, (int)initialLocation.Z]);
            if (initialLocation.Z + 1 < secondCorner.Z)
                neighboringPositions.Add(environmentMatrix[(int)initialLocation.X, (int)initialLocation.Y, (int)initialLocation.Z + 1]);
            if (initialLocation.Z - 1 >= firstCorner.Z)
                neighboringPositions.Add(environmentMatrix[(int)initialLocation.X, (int)initialLocation.Y, (int)initialLocation.Z - 1]);

            return neighboringPositions;
        }


        public HashSet<Vector3> GetAllNeighborLocations(Vector3 initialLocation)
        {
            HashSet<Vector3> neighboringPositions = new HashSet<Vector3>();
            neighboringPositions.Add(initialLocation + Vector3.Right);
            neighboringPositions.Add(initialLocation + Vector3.Left);
            neighboringPositions.Add(initialLocation + Vector3.Up);
            neighboringPositions.Add(initialLocation + Vector3.Down);
            neighboringPositions.Add(initialLocation + Vector3.Backward);
            neighboringPositions.Add(initialLocation + Vector3.Forward);
            return neighboringPositions;
        }


        public HashSet<Vector3> GetValidNeighborLocations(Vector3 initialLocation)
        {
            HashSet<Vector3> neighboringPositions = new HashSet<Vector3>();

            if (initialLocation.X + 1 < secondCorner.X)
                neighboringPositions.Add(new Vector3(initialLocation.X + 1, initialLocation.Y, initialLocation.Z));
            if (initialLocation.X - 1 >= firstCorner.X)
                neighboringPositions.Add(new Vector3(initialLocation.X - 1, initialLocation.Y, initialLocation.Z));
            if (initialLocation.Y + 1 < secondCorner.Y)
                neighboringPositions.Add(new Vector3(initialLocation.X, initialLocation.Y + 1, initialLocation.Z));
            if (initialLocation.Y - 1 >= firstCorner.Y)
                neighboringPositions.Add(new Vector3(initialLocation.X, initialLocation.Y - 1, initialLocation.Z));
            if (initialLocation.Z + 1 < secondCorner.Z)
                neighboringPositions.Add(new Vector3(initialLocation.X, initialLocation.Y, initialLocation.Z + 1));
            if (initialLocation.Z - 1 >= firstCorner.Z)
                neighboringPositions.Add(new Vector3(initialLocation.X, initialLocation.Y, initialLocation.Z - 1));

            return neighboringPositions;
        }

        public void Tick()
        {
            foreach (TissueCell deadCell in dyingCells)
            {
                environmentMatrix[(int)deadCell.CellLocation.X, (int)deadCell.CellLocation.Y, (int)deadCell.CellLocation.Z].TissueCells.Remove(deadCell);
                cells.Remove(deadCell);
                RemovePressure(deadCell.CellLocation);
                deadCell.ContainingSector.RemoveCell(deadCell);
            }
            dyingCells.Clear();

            foreach (TissueCell newCell in newCells)
            {
                environmentMatrix[(int)newCell.CellLocation.X, (int)newCell.CellLocation.Y, (int)newCell.CellLocation.Z].TissueCells.Add(newCell);
                cells.Add(newCell);
                AddPressure(newCell.CellLocation);
                newCell.ContainingSector.AddCell(newCell);
            }
            newCells.Clear();

            foreach (Signal expiredSignal in expiredSignals)
                signals.Remove(expiredSignal);
            expiredSignals.Clear();

            foreach (Signal newSignal in newSignals)
                signals.Add(newSignal);
            newSignals.Clear();          

            foreach (TissueCell cell in cells)
                cell.Tick();

            foreach (Signal s in signals)
                s.Tick();

            circulatorySystem.Tick();
        }

        private HashSet<LocationContents> GetLocationContents(ICollection<Vector3> locations)
        {
            HashSet<LocationContents> contents = new HashSet<LocationContents>();
            foreach (Vector3 location in locations)
                contents.Add(environmentMatrix[(int)location.X, (int)location.Y, (int)location.Z]);
            return contents;
        }

        public Vector3 GetBetterLocation(Vector3 currentLocation)
        {
            List<LocationContents> neighborContents = GetValidNeighborContents(currentLocation);

            double lowestPressure = environmentMatrix[(int)currentLocation.X, (int)currentLocation.Y, (int)currentLocation.Z].Pressure;

            foreach (LocationContents possibleLocation in neighborContents)
            {
                if (possibleLocation.Pressure < lowestPressure)
                    lowestPressure = possibleLocation.Pressure;
            }

            neighborContents.RemoveAll(item => item.Pressure > lowestPressure);
            if (neighborContents.Count == 0)
                return currentLocation;

            return UtilityMethods.GetRandomElementInList<LocationContents>(neighborContents).Position;
        }

        private LocationContents selectBestLocation(List<LocationContents> possibleLocations)
        {
            LocationContents candidate = possibleLocations[0];
            for (int i = 1; i < possibleLocations.Count; i++)
            {
                if (possibleLocations[i].Pressure < candidate.Pressure)
                    candidate = possibleLocations[i];
            }
            return candidate;
        }


        public void AddSignal(Signal s)
        {
            newSignals.Add(s);
        }

        public void DeleteSignal(Signal s)
        {
            expiredSignals.Add(s);
        }

        private List<Sector> sectorSetup(int sectorHeight, int sectorWidth, int sectorDepth)
        {
            List<Sector> sectors = new List<Sector>();

            if (sectorHeight == 0 || sectorWidth == 0 || sectorDepth == 0)
                throw new Exception("invalid sector definition, non-zero dimensions required");

            for (int x = (int)firstCorner.X; x <= (int)secondCorner.X - (sectorWidth - 1); x += sectorWidth)
            {
                for (int y = (int)firstCorner.Y; y <= (int)secondCorner.Y - (sectorHeight - 1); y += sectorHeight)
                {
                    for (int z = (int)firstCorner.Z; z <= (int)secondCorner.Z - (sectorDepth - 1); z += sectorDepth)
                    {
                        Vector3 first = new Vector3(x, y, z);
                        Vector3 second = new Vector3(x+sectorWidth-1, y+sectorHeight-1, z+sectorDepth-1);
                        Sector s = new Sector(first, second);
                        sectors.Add(s);
                    }
                }
            }

            return sectors;
        }

        public void CreateSectorMap(List<Sector> sectors)
        {
            //for debug purposes
            HashSet<Vector3> usedLocations = new HashSet<Vector3>();

            int sectorCount = 0;
            foreach (Sector s in sectors)
            {
                sectorCount++;

                for (int x = (int)s.FirstCorner.X; x <= s.SecondCorner.X; x++)
                {
                    for (int y = (int)s.FirstCorner.Y; y <= s.SecondCorner.Y; y++)
                    {
                        for (int z = (int)s.FirstCorner.Z; z <= s.SecondCorner.Z; z++)
                        {
                            Vector3 newVector = new Vector3(x, y, z);
                            if (usedLocations.Contains(newVector))  // for debug purposes
                                throw new Exception("we've already seen this location before!");
                            else
                                usedLocations.Add(newVector);
                            sectorMap[(int)newVector.X, (int)newVector.Y, (int)newVector.Z] = s;
                        }
                    }
                }

            }
        }

    }
}
