using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class GrowingPipe
    {
        /*

        private TissueCell target;
        private EndothelialCell currentGrowth;

        private int distanceToTarget;
        public int DistanceToTarget
        {
            get
            {
                return distanceToTarget;
            }
        }

        public void Tick()
        {
            distanceToTarget = UtilityMethods.CityBlockDistance(currentGrowth.CellLocation, target.CellLocation);

            EndothelialCell.PipeOrientation orientation;
            Vector3 unitVector;

            Vector3 direction = target.CellLocation - currentGrowth.CellLocation;
            if (direction.Equals(Vector3.Zero))     // and if we haven't reached it
                target = null;
            else
            {
                float absX = Math.Abs(direction.X);
                float absY = Math.Abs(direction.Y);
                float absZ = Math.Abs(direction.Z);

                if (absX > absY)
                {
                    if (absX > absZ) // if X is biggest
                    {
                        orientation = EndothelialCell.PipeOrientation.X;
                        if (direction.X == absX)
                        {
                            unitVector = Vector3.Right;
                        }
                        else
                            unitVector = Vector3.Left;
                    }

                    else // if Z is biggest
                    {
                        orientation = EndothelialCell.PipeOrientation.Z;
                        if (direction.Z == absZ)
                            unitVector = Vector3.Backward;
                        else
                            unitVector = Vector3.Forward;
                    }
                }

                else
                {
                    if (absY > absZ) // if Y is biggest
                    {
                        orientation = EndothelialCell.PipeOrientation.Y;
                        if (direction.Y == absY)
                            unitVector = Vector3.Up;
                        else
                            unitVector = Vector3.Down;
                    }
                    else // if Z is biggest
                    {
                        orientation = EndothelialCell.PipeOrientation.Z;
                        if (direction.Z == absZ)
                            unitVector = Vector3.Backward;
                        else
                            unitVector = Vector3.Forward;
                    }
                }

                Vector3 newCellPosition = currentGrowth.CellLocation + unitVector;
                double float = currentGrowth.Size * 0.9;

                EndothelialCell newPipeCell = new EndothelialCell(newCellPosition, orientation, size);
                Environment.Instance.SectorMap[newCellPosition].PipeValue += size; 
            }
        }

        private Vector3 transformMoveVector(Vector3 originalVector)
        {
            if (originalVector.X == 1)
                return Vector3.Right;
            if (originalVector.X == -1)
                return Vector3.Left;
            if (originalVector.Y == 1)
                return Vector3.Up;
            if (originalVector.Y == -1)
                return Vector3.Down;
            if (originalVector.Z == 1)
                return Vector3.Backward;
            if (originalVector.Z == -1)
                return Vector3.Forward;
            else
                throw new Exception("invalid move vector " + originalVector);

        }
        */
    }
}
