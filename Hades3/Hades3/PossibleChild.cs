using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class PossibleChild
    {
        private double selectionProbability;
        public double SelectionProbability
        {
            get
            {
                return selectionProbability;
            }
        }

        private Type cellType;
        public Type CellType
        {
            get
            {
                return cellType;
            }
        }

        public PossibleChild(double selectionProbability, Type cellType)
        {
            this.selectionProbability = selectionProbability;
            this.cellType = cellType;
        }
    }
}
