using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    

    public class EndothelialCell : Cell
    {
        private float size;
        public float Size
        {
            get
            {
                return size;
            }
        }

        private PipeOrientation orientation;
        public PipeOrientation Orientation
        {
            get
            {
                return orientation;
            }
        }

        private List<EndothelialCell> children;
        public List<EndothelialCell> Children
        {
            get
            {
                return children;
            }
        }

        private EndothelialCell parent;
        public EndothelialCell Parent
        {
            get
            {
                return parent;
            }
        }

        public List<TissueCell> travelingCells;

        public EndothelialCell(Vector3 cellPosition, EndothelialCell.PipeOrientation orientation, float size, EndothelialCell parent) : base(cellPosition)
        {
            children = new List<EndothelialCell>();
            this.orientation = orientation;
            this.size = size;
            Environment.Instance.GetSectorAt(cellPosition).PipeValue += size;
            CirculatorySystem.Instance.PipeCells.Add(this);
            Environment.Instance.GetContentsAt(cellPosition).pipes.Add(this);
            travelingCells = new List<TissueCell>();
            this.parent = parent;
        }


        public override void Tick()
        {
        }

        
        public enum PipeOrientation
        {
            posX,
            negX,
            posY,
            negY,
            posZ,
            negZ
        }

        public class PipeInfo
        {
            private Vector3 direction;
            public Vector3 Direction
            {
                get
                {
                    return direction;
                }
            }

            private PipeOrientation orientation;
            public PipeOrientation Orientation
            {
                get
                {
                    return orientation;
                }
            }

            public PipeInfo(Vector3 direction, PipeOrientation orientation)
            {
                this.direction = direction;
                this.orientation = orientation;
            }
        }

        public EndothelialCell GrowTowards(Vector3 target)
        {
            if (size <= CirculatorySystem.Instance.minSize)
                return this;

            PipeInfo pipeInfo = getGrowthDirection(this.cellLocation, target);
            Vector3 unitVector = pipeInfo.Direction;
            PipeOrientation orientation = pipeInfo.Orientation;

            Vector3 newCellPosition = this.CellLocation + unitVector;
            size = this.Size * CirculatorySystem.Instance.PipeShrinkRate;

            EndothelialCell newPipeCell = new EndothelialCell(newCellPosition, orientation, size, this);
            this.children.Add(newPipeCell);
            return newPipeCell;
        }


        public static EndothelialCell.PipeInfo getGrowthDirection(Vector3 start, Vector3 end)
        {
            EndothelialCell.PipeOrientation orientation;
            Vector3 unitVector;
            Vector3 direction = end - start;

            float absX = Math.Abs(direction.X);
            float absY = Math.Abs(direction.Y);
            float absZ = Math.Abs(direction.Z);

            if (absX > absY)
            {
                if (absX > absZ) // if X is biggest
                {
                    if (direction.X == absX)
                    {
                        unitVector = Vector3.Right;
                        orientation = PipeOrientation.posX;
                    }
                    else
                    {
                        unitVector = Vector3.Left;
                        orientation = PipeOrientation.negX;
                    }
                }

                else // if Z is biggest
                {
                    if (direction.Z == absZ)
                    {
                        unitVector = Vector3.Backward;
                        orientation = PipeOrientation.posZ;
                    }
                    else
                    {
                        unitVector = Vector3.Forward;
                        orientation = PipeOrientation.negZ;
                    }
                }
            }

            else
            {
                if (absY > absZ) // if Y is biggest
                {
                    if (direction.Y == absY)
                    {
                        unitVector = Vector3.Up;
                        orientation = PipeOrientation.posY;
                    }
                    else
                    {
                        unitVector = Vector3.Down;
                        orientation = PipeOrientation.negY;
                    }
                }
                else // if Z is biggest
                {
                    if (direction.Z == absZ)
                    {
                        unitVector = Vector3.Backward;
                        orientation = PipeOrientation.posZ;
                    }
                    else
                    {
                        unitVector = Vector3.Forward;
                        orientation = PipeOrientation.negZ;
                    }
                }
            }

            PipeInfo newPipeInfo = new PipeInfo(unitVector, orientation);
            return newPipeInfo;
        }

        /*
        public static PipeOrientation getPipeOrientation(Vector3 growthDirection)
        {
            EndothelialCell.PipeOrientation orientation;
            if (growthDirection == Vector3.Right)
                orientation = EndothelialCell.PipeOrientation.posX;
            else if (growthDirection == Vector3.Left)
                orientation = EndothelialCell.PipeOrientation.negX;
            else if (growthDirection == Vector3.Up)
                orientation = EndothelialCell.PipeOrientation.posY;
            else if (growthDirection == Vector3.Down)
                orientation = EndothelialCell.PipeOrientation.negY;
            else if (growthDirection == Vector3.Forward)
                orientation = EndothelialCell.PipeOrientation.negZ;
            else // if unitVector == Vector3.Backward
                orientation = EndothelialCell.PipeOrientation.posZ;

            return orientation;
        }*/
        

    }
}
