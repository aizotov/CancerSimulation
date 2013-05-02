using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public abstract class SimpleCellBehavior : CellBehavior
    {
        public SimpleCellBehavior(double startingValue)
        {
            CurrValue = startingValue;
        }

        public void MutateRandom()
        {
            double r = Environment.random.NextDouble();
            if (r < 0.5)
                MutateUp();
            else
                MutateDown();
        }

        public void MutateTo(double newValue)
        {
            if (SimulationCore.DEBUG)
            {
                if (newValue > GetMaxValue())
                    throw new Exception("invalid mutation... [" + newValue + "] exceeds max value of [" + GetMaxValue() + "]");
                if (newValue < GetMinValue())
                    throw new Exception("invalid mutation... [" + newValue + "] is less than min value of [" + GetMinValue() + "]");
            }
            CurrValue = newValue;
        }

        private void MutateUp()
        {
            CurrValue += SimulationCore.Instance.SimulationParams.cellParameters.MutateBy;
            if (CurrValue > GetMaxValue())
                CurrValue = GetMaxValue();
        }

        private void MutateDown()
        {
            CurrValue -= SimulationCore.Instance.SimulationParams.cellParameters.MutateBy;
            if (CurrValue < GetMinValue())
                CurrValue = GetMinValue();
        }

        public abstract double GetMaxValue();
        public abstract double GetMinValue();

    }
}
