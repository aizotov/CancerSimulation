using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Hades3
{
    public abstract class TissueCell : Cell
    {
        protected BehaviorAggregate cellBehaviors;
        public BehaviorAggregate CellBehaviors
        {
            get
            {
                return cellBehaviors;
            }
        }

        public bool Mutated = false;

        protected Sector containingSector;
        public Sector ContainingSector
        {
            get
            {
                return containingSector;
            }
        }

        protected EndothelialCell closestPipe;
        public EndothelialCell ClosestPipe
        {
            get
            {
                return closestPipe;
            }
            set
            {
                closestPipe = value;
            }
        }

        protected int hungerLevel;

        private PipeTravelInfo pipeTravel = null;
        public PipeTravelInfo PipeTravel
        {
            get
            {
                return pipeTravel;
            }
        }
        
        // used when creating cell from another cell
        public TissueCell(TissueCell parentCell) : base(parentCell.cellLocation)
        {
            Mutated = parentCell.Mutated;
            containingSector = Environment.Instance.GetSectorAt(parentCell.cellLocation);
            hungerLevel = parentCell.hungerLevel;
        }

        public TissueCell(Vector3 location, int startingFood) : base(location)
        {
            containingSector = Environment.Instance.GetSectorAt(location);
            hungerLevel = startingFood;
        }

        #region cellChoices

        protected readonly List<PossibleChild> cellChoices = new List<PossibleChild>();
        public object Create(int supportedTypeIndex)
        {
            object untyped = Activator.CreateInstance(cellChoices[supportedTypeIndex].CellType);
            return untyped;
        }

        public T Create<T>(Type t, TissueCell parentCell)
        {
            Object[] args = new Object[1];
            args[0] = parentCell;

            T typed = default(T);
            object untyped = Activator.CreateInstance(t, parentCell);
            if (!(untyped is T))
            {
                throw new Exception("attempted to create invalid cell type");
            }
            typed = (T)(untyped);
            return typed;
        }

        public void AddChildChoice<T>(double probability) where T : TissueCell
        {
            cellChoices.Add(new PossibleChild(probability, typeof(T)));
        }

        // throws an exception if child probabilities don't add up to 1
        // should be called at the end of every cell constructor after children and probabilities are defined
        protected void checkChildProbabilities(TissueCell c)
        {
            double count = 0;
            foreach (PossibleChild possibleChild in cellChoices)
            {
                count += possibleChild.SelectionProbability;
            }
            if (count != 1)
                throw new Exception("invalid child cell selection in cell [" + c.GetType() + "]");
        }
        #endregion

        public TissueCell Divide(TissueCell parentCell)
        {
            double first = 0;
            double r = Environment.random.NextDouble();
            foreach (PossibleChild possibleChild in cellChoices)
            {
                if (r >= first && r <= possibleChild.SelectionProbability + first)
                    return Create<TissueCell>(possibleChild.CellType, parentCell);
                first = possibleChild.SelectionProbability;
            }
            return null;
        }

        protected abstract void incrementGeneration();

        public TissueCell Divide(Type type, TissueCell parentCell)
        {
            return Create<TissueCell>(type, parentCell);
        }

        protected void die()
        {
            Environment.Instance.DeleteCell(this);
        }

        protected void MoveToBetterLocationIfPossible()
        {
            if (Environment.random.NextDouble() < cellBehaviors.GetMoveProbability())
            {
                Vector3 possibleNewLocation = Environment.Instance.GetBetterLocation(cellLocation);
                if (possibleNewLocation != cellLocation)
                    cellMove(possibleNewLocation);
            }
        }

        protected bool EnoughSpaceToDivide()
        {
            if (Environment.Instance.GetContentsAt(cellLocation).Pressure > cellBehaviors.GetPressureToleranceAtLocation()) 
                return false;

            List<LocationContents> neighbors = Environment.Instance.GetValidNeighborContents(cellLocation);
            foreach (LocationContents neighbor in neighbors)
            {
                if (neighbor.Pressure > cellBehaviors.GetPressureToleranceAtNeighbors())
                //if (neighbor.Pressure > SimulationCore.Instance.SimulationParams.cellParameters.pressureAtCell + 1)
                //if (neighbor.Pressure > cellBehaviors.GetPressureTolerance())
                    return false;
            }

            return true;
        }

        protected abstract bool GenerationDeath();

        protected bool dieIfNecessary()
        {
            if (Environment.random.NextDouble() < cellBehaviors.GetDeathProbability())
            {
                die();
                return true;
            }
            return false;
        }

        protected void divideIfPossible()
        {
            double r = Environment.random.NextDouble();
            if (r < cellBehaviors.GetDivisionProbability())
            {
                hungerLevel /= 2;
                incrementGeneration();
                TissueCell newCell = Divide(this);

                Environment.Instance.AddNewCell(newCell);

                Environment.Instance.AddSignal(new PleaseMoveSignal(this, PleaseMoveSignal.PleaseMoveSignalMaxRange, 1));
            }
        }

        protected bool EnoughFoodInSector()
        {
            //Console.WriteLine("food ratio is: " + containingSector.PipeValue + " / (" + containingSector.FinalCellRealCount + containingSector.BlastCellRealCount + ")  =  " + containingSector.CellPipeRatio);
            if(containingSector.CellPipeRatio > SimulationCore.Instance.SimulationParams.MinimumCellToPipeRatio)
                return true;
            return false;
        }

        public override void Tick()
        {
            if (pipeTravel != null)     // if traveling in pipe...
                travelAlongPipe();

            else
            {
                if (GenerationDeath())
                    return;

                if (dieIfNecessary())
                    return;

                if (EnoughFoodInSector())
                {
                    CirculatorySystem.Instance.RemoveHungryCell(this);
                    if (hungerLevel < cellBehaviors.GetMaxFoodStorageLevel())
                        hungerLevel++;
                    if (EnoughSpaceToDivide())
                        divideIfPossible();
                }
                else if (hungerLevel == 0)
                {
                    die();
                    return;
                }
                else if (hungerLevel < cellBehaviors.GetFoodConcernLevel())
                {
                    hungerLevel -= cellBehaviors.GetFoodConsumptionRate();
                    CirculatorySystem.Instance.AddHungryCell(this);
                }

                if(pipeTravel == null)
                    getPushed();
                if (pipeTravel == null)
                    MoveToBetterLocationIfPossible();
            }
        }

        public void AcceptMutations(List<Mutation> mutations)
        {
            Console.WriteLine("cell has acccepted [" + mutations.Count + "] mutations");
            cellBehaviors.ApplyMutations(mutations);
            if (Mutated == false)
            {
                Mutated = true;
                Environment.Instance.MutatedCellTotal++;
            }
        }

        private bool getPushed()
        {
            List<PleaseMoveSignal> theSignals = Environment.Instance.GetContentsAt(cellLocation).GetPushSignals();
            theSignals.RemoveAll(item => item.ParentCell == this);  // cell should only be pushed by signals originating from other cells... instead this will be in next method 
            if (theSignals.Count != 0)
            {
                ridePushWave(theSignals);
                return true;
            }
            return false;
        }

        protected void ridePushWave(List<PleaseMoveSignal> theSignals)
        {
            /*
            if (pushSignalCooldown == 0)
            {
                Environment.Instance.AddSignal(new PleaseMoveSignal(this, PleaseMoveSignal.PleaseMoveSignalMaxRange, 1));
                pushSignalCooldown = 3; //TODO not sure if this should be inside or outside of the if statement
            }
             * */

            HashSet<Vector3> neighbors = Environment.Instance.GetValidNeighborLocations(cellLocation);

            foreach (PleaseMoveSignal signal in theSignals)
            {
                if(signal.ParentCell != this)
                    neighbors.ExceptWith(signal.AffectedLocations);
            }

            if (neighbors.Count == 0)
                return;

            List<Vector3> neighborList = new List<Vector3>(neighbors);

            neighborList.RemoveAll(item => Environment.Instance.GetContentsAt(item).Pressure > this.cellBehaviors.GetPressureToleranceAtLocation()); // areas with pressure>pressureTolerance are invalid move locations
            if (neighborList.Count == 0)
                return;

            Vector3 moveTo = UtilityMethods.GetRandomElementInList(neighborList);
            cellMove(moveTo);
        }

        public void ChangedSector(Sector oldSector, Sector newSector)
        {
            oldSector.RemoveCell(this);
            newSector.AddCell(this);
            containingSector = newSector;
        }

        protected void cellMove(Vector3 newLocation)
        {
            List<EndothelialCell> possibleEntryPoints = Environment.Instance.GetContentsAt(newLocation).pipes;
            possibleEntryPoints.RemoveAll(pipe => pipe.Size < SimulationCore.Instance.SimulationParams.MinPipeSizeForTravel);
            if (possibleEntryPoints.Count > 0)
            {
                EndothelialCell entryPoint = UtilityMethods.GetRandomElementInList<EndothelialCell>(possibleEntryPoints);
                if (canEnterPipe(entryPoint))
                    enterPipe(entryPoint);
                return;                 // if cell is not able to move into pipe it should not move...
            }

            Sector oldSector = Environment.Instance.GetSectorAt(cellLocation);
            Sector newSector = Environment.Instance.GetSectorAt(newLocation);

            if (oldSector != newSector)
                ChangedSector(oldSector, newSector);

            if (SimulationCore.DEBUG)
            {
                if (!Environment.Instance.GoodPoint(this.CellLocation))
                    throw new Exception("invalid move from " + this.CellLocation);
                if (!Environment.Instance.GoodPoint(newLocation))
                    throw new Exception("invalid move to " + newLocation);
            }

            Vector3 oldLocation = this.CellLocation;
            Environment.Instance.RemovePressure(oldLocation);
            Environment.Instance.GetContentsAt(oldLocation).TissueCells.Remove(this);
            Environment.Instance.GetContentsAt(newLocation).TissueCells.Add(this);
            Environment.Instance.AddPressure(newLocation);
            this.CellLocation = newLocation;
        }


        #region pipeTravel

        private void travelAlongPipe()
        {
            if (pipeTravel.ContainingPipe.Size < SimulationCore.Instance.SimulationParams.MinPipeSizeForTravel || Environment.random.NextDouble() < cellBehaviors.GetLeavePipeProbability())
            {
                leavePipe();
                return;
            }

            if (Environment.random.NextDouble() > cellBehaviors.GetSurvivePipeProbability())
            {
                pipeTravel.ContainingPipe.travelingCells.Remove(this);
                die();
                return;
            }


            if (pipeTravel.TravelDirection == PipeTravelDirection.toParent)
            {
                // no parent to travel to
                if (pipeTravel.ContainingPipe.Parent == null)
                {
                    //leavePipe();
                    //return;

                    // change direction
                    pipeTravel.TravelDirection = PipeTravelDirection.toChildren;
                    pipeTravelToChildren();
                }

                pipeTravelToParent();
            }

            else if (pipeTravel.TravelDirection == PipeTravelDirection.toChildren)
            {
                // no children to travel to
                if (pipeTravel.ContainingPipe.Children.Count == 0)
                {
                    leavePipe();
                    return;

                    // change direction
                    //pipeTravel.TravelDirection = PipeTravelDirection.toParent;
                    //pipeTravelToParent();
                }

                pipeTravelToChildren();
            }
            else
                throw new Exception("unsupported travel direction");
        }

        private void pipeTravelToParent()
        {
            pipeTravel.ContainingPipe.travelingCells.Remove(this);
            pipeTravel.ContainingPipe = pipeTravel.ContainingPipe.Parent;
            pipeTravel.ContainingPipe.travelingCells.Add(this);
        }

        private void pipeTravelToChildren()
        {
            pipeTravel.ContainingPipe.travelingCells.Remove(this);
            pipeTravel.ContainingPipe = UtilityMethods.GetRandomElementInList<EndothelialCell>(pipeTravel.ContainingPipe.Children);
            pipeTravel.ContainingPipe.travelingCells.Add(this);
        }

        private bool canEnterPipe(EndothelialCell entryPoint)
        {
            if (Environment.random.NextDouble() < cellBehaviors.GetEnterPipeProbability())
                return true;
            return false;
        }

        private void enterPipe(EndothelialCell entryPoint)
        {
            pipeTravel = new PipeTravelInfo(entryPoint);

            containingSector.RemoveCell(this);
            Environment.Instance.GetContentsAt(cellLocation).TissueCells.Remove(this);
            Environment.Instance.RemovePressure(cellLocation);
            entryPoint.travelingCells.Add(this);
        }

        private void leavePipe()
        {
            pipeTravel.ContainingPipe.travelingCells.Remove(this);

            List<LocationContents> nearbyLocatons = Environment.Instance.GetValidNeighborContents(pipeTravel.ContainingPipe.CellLocation);
            nearbyLocatons.RemoveAll(loc => loc.pipes.Count > 0);  // remove all neighboring locations with pipes
            Vector3 exitPoint;
            if (nearbyLocatons.Count == 0)
                exitPoint = pipeTravel.ContainingPipe.CellLocation;
            else
            {
                LocationContents exitPointC = UtilityMethods.GetRandomElementInList<LocationContents>(nearbyLocatons);
                exitPoint = exitPointC.Position;
            }

            cellLocation = exitPoint;

            pipeTravel = null;

            containingSector = Environment.Instance.GetSectorAt(cellLocation);
            containingSector.AddCell(this);
            Environment.Instance.GetContentsAt(cellLocation).TissueCells.Add(this);
            Environment.Instance.AddPressure(cellLocation);
        }

        #endregion

    }
}
