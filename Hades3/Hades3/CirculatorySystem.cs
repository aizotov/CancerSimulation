using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class CirculatorySystem
    {
        public float minSize = 0.4f;

        private bool setupShrink = true;


        private static CirculatorySystem instance;
        public static CirculatorySystem Instance
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

        private float pipeShrinkRate;
        public float PipeShrinkRate
        {
            get
            {
                return pipeShrinkRate;
            }
        }

        private List<EndothelialCell> pipeCells;
        public List<EndothelialCell> PipeCells
        {
            get
            {
                return pipeCells;
            }
        }

        private HashSet<TissueCell> hungryCells;
        private List<GrowingPipe> growingPipes;

        public CirculatorySystem(SimulationParameters.PipeParameters pipeLocations, float pipeShrinkRate, EndothelialCell.PipeOrientation rootOrientation, float startingWidth)
        {
            instance = this;

            this.pipeShrinkRate = pipeShrinkRate;

            pipeCells = new List<EndothelialCell>();
            hungryCells = new HashSet<TissueCell>();
            growingPipes = new List<GrowingPipe>();

            EndothelialCell root = new EndothelialCell(pipeLocations.location, rootOrientation, startingWidth, null);
            setupPipes(root, pipeLocations.children);
        }

        private void setupPipes(EndothelialCell currPos, List<SimulationParameters.PipeParameters> children)
        {
            if (children == null || children.Count == 0)
                return;

            foreach (SimulationParameters.PipeParameters child in children)
            {
                EndothelialCell newestPiece = createPipe(currPos, child.location);
                setupPipes(newestPiece, child.children);
            }
        }


        // returns the EndothelialCell at target location
        private EndothelialCell createPipe(EndothelialCell from, Vector3 target)
        {
            EndothelialCell currCell = from;
            float size = from.Size;
            Vector3 currLoc = currCell.CellLocation;
            EndothelialCell.PipeInfo pipeInfo;
            
            while (currLoc != target)
            {
                size *= pipeShrinkRate;
                pipeInfo = EndothelialCell.getGrowthDirection(currLoc, target);
                currLoc = currLoc + pipeInfo.Direction;
                EndothelialCell newPipePiece = new EndothelialCell(currLoc, pipeInfo.Orientation, size, currCell);
                currCell.Children.Add(newPipePiece);
                currCell = newPipePiece;
            }

            return currCell;
        }


        public void Tick()
        {
		    foreach(TissueCell hc in hungryCells){
                EndothelialCell closestPipeSection;
                if (hc.ClosestPipe == null)
                    closestPipeSection = findClosestPipeSectionTo(hc);
                else
                    closestPipeSection = pipeSearch(hc, hc.ClosestPipe);
                EndothelialCell newGrowth = closestPipeSection.GrowTowards(hc.CellLocation);
                hc.ClosestPipe = newGrowth;
		    }
            hungryCells.Clear();
        }


        #region handlePipeGrowth

        public void AddHungryCell(TissueCell c)
        {
            hungryCells.Add(c);
        }

        public void RemoveHungryCell(TissueCell c)
        {
            hungryCells.Remove(c);
        }

        private EndothelialCell findClosestPipeSectionTo(TissueCell hungryCell)
        {
		    Vector3 hungryCellLocation = hungryCell.CellLocation;

		    EndothelialCell closestCell = pipeCells[0];
		    double closestDistance = UtilityMethods.CityBlockDistance(closestCell.CellLocation, hungryCellLocation);
		
		    double dist;
		    for(int i=1; i<pipeCells.Count; i++){		// can use higher step count for efficieny, for ex step through every 3 or 4 cells.
                dist = UtilityMethods.CityBlockDistance(pipeCells[i].CellLocation, hungryCellLocation);
			    if(dist < closestDistance){
				    closestDistance = dist;
				    closestCell = pipeCells[i];
			    }
		    }
		    return closestCell;
	    }

        private EndothelialCell pipeSearch(TissueCell target, EndothelialCell startingLocation)
        {
            Vector3 targetLocation = target.CellLocation;
            int currDistance = UtilityMethods.CityBlockDistance(startingLocation.CellLocation, targetLocation);

            EndothelialCell parent = pipeSearchParent(target, startingLocation, currDistance);
            int pDistance = UtilityMethods.CityBlockDistance(parent.CellLocation, targetLocation);

            EndothelialCell child = pipeSearchChildren(target, startingLocation, currDistance);
            int cDistance = UtilityMethods.CityBlockDistance(child.CellLocation, targetLocation);

            if (pDistance < cDistance)
                return parent;
            return child;
        }

        private EndothelialCell pipeSearchParent(TissueCell target, EndothelialCell currPos, int currDistance)
        {
            if (currPos.Parent == null)
                return currPos;

            int tmpDistance = UtilityMethods.CityBlockDistance(currPos.Parent.CellLocation, target.CellLocation);
            if (tmpDistance < currDistance)
                return pipeSearchParent(target, currPos.Parent, tmpDistance);
            else
                return currPos;
        }

        private EndothelialCell pipeSearchChildren(TissueCell target, EndothelialCell currPos, int currDistance)
        {
            if (currPos.Children.Count == 0)
                return currPos;

            int tmpDistance;
            EndothelialCell bestChild = null;

            foreach (EndothelialCell child in currPos.Children)
            {
                tmpDistance = UtilityMethods.CityBlockDistance(child.CellLocation, target.CellLocation);
                if (tmpDistance < currDistance)
                {
                    currDistance = tmpDistance;
                    bestChild = child;
                }
            }

            if (bestChild == null)
                return currPos;
            else
                return pipeSearchChildren(target, bestChild, currDistance);
        }

        #endregion
    }
}
