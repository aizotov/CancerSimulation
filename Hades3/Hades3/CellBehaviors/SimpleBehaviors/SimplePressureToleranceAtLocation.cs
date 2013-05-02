﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    class SimplePressureToleranceAtLocation : SimpleCellBehavior
    {
        public static int MaxValue;
        public static int MinValue;

        public SimplePressureToleranceAtLocation(double startingValue) : base(startingValue)
        {
        }

        public SimplePressureToleranceAtLocation(SimplePressureToleranceAtLocation old) : base(old.CurrValue)
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
