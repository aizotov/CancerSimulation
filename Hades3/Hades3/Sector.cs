using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class Sector
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

        private int cellCount = 0;
        public int CellCount
        {
            get
            {
                return cellCount;
            }
        }

        private double pipeValue = 0;
        public double PipeValue
        {
            get
            {
                return pipeValue;
            }
            set
            {
                pipeValue = value;
                cellPipeRatio = pipeValue / cellCount;
            }
        }

        // pipeValue / cellCount
        private double cellPipeRatio;
        public double CellPipeRatio
        {
            get
            {
                return cellPipeRatio;
            }
        }

        public Sector(Vector3 firstCorner, Vector3 secondCorner)
        {
            this.firstCorner = firstCorner;
            this.secondCorner = secondCorner;
        }

        public void AddCell(TissueCell cell)
        {
            cellCount++;
            cellPipeRatio = pipeValue / cellCount;
        }

        public void RemoveCell(TissueCell cell)
        {
            cellCount--;

            cellPipeRatio = pipeValue / cellCount;
        }

        public bool ContainsPoint(Vector3 point)
        {
            if (point.X >= firstCorner.X && point.Y >= firstCorner.Y && point.Z >= firstCorner.Z && point.X <= secondCorner.X && point.Y <= secondCorner.Y && point.Z <= secondCorner.Z)
                return true;
            return false;
        }

    }
}
