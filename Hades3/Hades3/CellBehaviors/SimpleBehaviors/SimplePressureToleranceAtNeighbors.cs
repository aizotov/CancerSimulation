﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    class SimplePressureToleranceAtNeighbors : SimpleCellBehavior
    {
        public static double MaxValue;
        public static double MinValue;

        public SimplePressureToleranceAtNeighbors(double startingValue) : base(startingValue)
        {
        }

        public SimplePressureToleranceAtNeighbors(SimplePressureToleranceAtNeighbors old) : base(old.CurrValue)
        {
        }

        public override double GetMinValue()
        {
            return MinValue;
        }

        public override double GetMaxValue()
        {
            return MaxValue;
        }
    }
}
