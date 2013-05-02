using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public abstract class Signal
    {
        protected double maxStrength;
        public double MaxStrength
        {
            get
            {
                return maxStrength;
            }
        }

        protected HashSet<Vector3> affectedLocations;
        public HashSet<Vector3> AffectedLocations
        {
            get
            {
                return affectedLocations;
            }
        }

        protected TissueCell parentCell;
        public TissueCell ParentCell
        {
            get
            {
                return parentCell;
            }
        }

        public Signal(TissueCell parentCell)
        {
            this.parentCell = parentCell;
            affectedLocations = new HashSet<Vector3>();
        }

        public abstract void Tick();

    }
}
