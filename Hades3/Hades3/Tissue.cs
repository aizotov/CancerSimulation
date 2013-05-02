using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hades3
{
    public class Tissue
    {
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

        public Tissue(SimulationParameters.TissueParameters tissueParams)
        {
            firstCorner = tissueParams.firstCorner;
            secondCorner = tissueParams.secondCorner;
        }

        public bool ContainsPoint(Vector3 location)
        {
            if (location.X >= firstCorner.X && location.Y >= firstCorner.Y && location.Z >= firstCorner.Z &&
                location.X <= secondCorner.X && location.Y <= secondCorner.Y && location.Z <= secondCorner.Z)
                return true;
            return false;
        }

        //private Dictionary<Vector3, LocationContents> locationContents;
        //public Dictionary<Vector3, LocationContents> LocationContents
        //{
        //    get
        //    {
        //        return locationContents;
        //    }
        //}

        //private List<TissueCell> cells;
        //public List<TissueCell> Cells
        //{
        //    get
        //    {
        //        return cells;
        //    }
        //}

        //private List<TissueCell> hungryCells;
        //public List<TissueCell> HungryCells
        //{
        //    get
        //    {
        //        return hungryCells;
        //    }
        //}

        //private List<TissueCell> newCells;
        //private List<TissueCell> dyingCells;

        //private List<Experiment> experimentsToRun = new List<Experiment>();
        //public List<Experiment> ExperimentsToRun
        //{
        //    get
        //    {
        //        return experimentsToRun;
        //    }
        //}

        //public Tissue(SimulationParameters.TissueParameters config)
        //{
        //    firstCorner = config.firstCorner;
        //    secondCorner = config.secondCorner;

        //    cells = new List<TissueCell>();
        //    hungryCells = new List<TissueCell>();
        //    newCells = new List<TissueCell>();
        //    dyingCells = new List<TissueCell>();

        //    locationContents = new Dictionary<Vector3, LocationContents>();
        //    initializeMatrix();
        //    initializeCells(config.startingCells);

        //    Console.WriteLine("created tissue from " + firstCorner + " to " + secondCorner);
        //}

        //private void initializeMatrix()
        //{

        //    for (int height = (int)firstCorner.X; height <=  (int)secondCorner.X; height++)
        //    {
        //        for (int width = (int)firstCorner.Y; width <= (int)secondCorner.Y; width++)
        //        {
        //            for (int depth = (int)firstCorner.Z; depth <= (int)secondCorner.Z; depth++)
        //            {
        //                Vector3 pos = new Vector3(height, width, depth);
        //                locationContents.Add(pos, new LocationContents(pos));
        //            }
        //        }
        //    }
        //}

        //private void runExperiment(Experiment experiment)
        //{
        //    /*
        //    List<TissueCell> cellSelection;

        //    switch (experiment.WhichCells)
        //    {
        //        case Experiment.Selection.All:
        //            cellSelection = cells;
        //            break;
        //        case Experiment.Selection.AllFinal:
        //            cellSelection = new List<TissueCell>();
        //            foreach (TissueCell c in cells)
        //            {
        //                if (c.GetType() == typeof(FinalCell))
        //                    cellSelection.Add(c);
        //            }
        //            break;
        //        case Experiment.Selection.AllBlast:
        //            cellSelection = new List<TissueCell>();
        //            foreach (TissueCell c in cells)
        //            {
        //                if (c.GetType() == typeof(BlastCell))
        //                    cellSelection.Add(c);
        //            }
        //            break;
        //        case Experiment.Selection.OneFinal:
        //            cellSelection = new List<TissueCell>();
        //            foreach (TissueCell c in cells)
        //            {
        //                if (c.GetType() == typeof(FinalCell) && c.CurrentMutationLevel == 0)
        //                {
        //                    cellSelection.Add(c);
        //                    break;
        //                }
        //            }
        //            if (cellSelection.Count != 1)
        //                Console.WriteLine("there are no final cells to select");
        //            break;
        //        case Experiment.Selection.OneBlast:
        //            cellSelection = new List<TissueCell>();
        //            foreach (TissueCell c in cells)
        //            {
        //                if (c.GetType() == typeof(BlastCell) && c.CurrentMutationLevel == 0)
        //                {
        //                    cellSelection.Add(c);
        //                    break;
        //                }
        //            }
        //            if (cellSelection.Count != 1)
        //                Console.WriteLine("there are no blast cells to select");
        //            break;
        //        default:
        //            throw new Exception("unsupported cell selection");
        //    }

        //    Console.WriteLine("cells affected by experiment " + experiment.Name + ": " + cellSelection.Count);

        //    foreach (TissueCell c in cellSelection)
        //    {
        //        if (experiment.Mutations.Contains(Experiment.GeneType.SpaceTolerance))
        //        {
        //            int num = 0;
        //            while (num < experiment.Modifier)
        //            {
        //                c.Genes[Experiment.GeneType.SpaceTolerance].Mutate();
        //                num++;
        //            }
        //        }

        //        if (experiment.Mutations.Contains(Experiment.GeneType.SelfDeath))
        //        {
        //            int num = 0;
        //            while (num < experiment.Modifier)
        //            {
        //                c.Genes[Experiment.GeneType.SelfDeath].Mutate();
        //                num++;
        //            }
        //        }
        //        //else
        //        //    throw new Exception("need to implement self repair mutations");
        //    }
        //     * */
        //}

        //public HashSet<Vector3> GetValidNeighborLocations(Vector3 initialLocation)
        //{
        //    if (SimulationCore.DEBUG)
        //    {
        //        if (initialLocation.X > secondCorner.X || initialLocation.Y > secondCorner.Y || initialLocation.Z > secondCorner.Z)
        //            throw new Exception("looking for neighbors from outside of tissue");
        //    }

        //    HashSet<Vector3> neighboringPositions = new HashSet<Vector3>();
            
        //    Vector3 neighbor;

        //    if (initialLocation.X + 1 <= secondCorner.X)
        //    {
        //        neighbor = initialLocation + Vector3.UnitX;
        //        neighboringPositions.Add(neighbor);
        //    }

        //    if (initialLocation.X - 1 >= firstCorner.X)
        //    {
        //        neighbor = initialLocation - Vector3.UnitX;
        //        neighboringPositions.Add(neighbor);
        //    }

        //    if (initialLocation.Y + 1 <= secondCorner.Y)
        //    {
        //        neighbor = initialLocation + Vector3.UnitY;
        //        neighboringPositions.Add(neighbor);
        //    }

        //    if (initialLocation.Y - 1 >= firstCorner.Y)
        //    {
        //        neighbor = initialLocation - Vector3.UnitY;
        //        neighboringPositions.Add(neighbor);
        //    }

        //    if (initialLocation.Z + 1 <= secondCorner.Z)
        //    {
        //        neighbor = initialLocation + Vector3.UnitZ;
        //        neighboringPositions.Add(neighbor);
        //    }

        //    if (initialLocation.Z - 1 >= firstCorner.Z)
        //    {
        //        neighbor = initialLocation - Vector3.UnitZ;
        //        neighboringPositions.Add(neighbor);
        //    }

        //    return neighboringPositions;
        //}

        //private HashSet<LocationContents> GetLocationContents(ICollection<Vector3> locations)
        //{
        //    HashSet<LocationContents> contents = new HashSet<LocationContents>();
        //    foreach (Vector3 location in locations)
        //        contents.Add(locationContents[location]);
        //    return contents;
        //}

        //// returns location next to specified location with less, but >= minPressure, pressure
        //// if no such location exists, returns currentLocation
        //public Vector3 GetBetterLocation(Vector3 currentLocation)
        //{
        //    HashSet<Vector3> neighbors = GetValidNeighborLocations(currentLocation);
        //    HashSet<LocationContents> neighborContents = GetLocationContents(neighbors);

        //    IList<Vector3> possibleLocations = new List<Vector3>();

        //    foreach (LocationContents possibleLocation in neighborContents)
        //    {
        //        if (possibleLocation.Pressure > SimulationCore.Instance.CellParameters.minPressure && possibleLocation.Pressure < locationContents[currentLocation].Pressure)
        //            possibleLocations.Add(possibleLocation.Position);
        //    }

        //    /*
        //    if (cells.Count >= 125)
        //    {
        //        Console.WriteLine("wtf");
        //    }*
        //     */


        //    if (possibleLocations.Count == 0)
        //        return currentLocation;
        //    else
        //        return UtilityMethods.GetRandomElementInList(possibleLocations);

        //}

    
    }
}
