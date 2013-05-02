using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public abstract class Cell
    {
        protected Vector3 cellLocation;
        public Vector3 CellLocation
        {
            get
            {
                return cellLocation;
            }
            set
            {
                cellLocation = value;
            }
        }

        public Cell(Vector3 location)
        {
            this.cellLocation = location;
        }

        public abstract void Tick();
    }
}
