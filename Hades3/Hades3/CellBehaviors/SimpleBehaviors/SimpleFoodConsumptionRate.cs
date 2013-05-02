﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class SimpleFoodConsumptionRate : SimpleCellBehavior
    {
        public static int MaxValue;
        public static int MinValue;

        public SimpleFoodConsumptionRate(double startingValue) : base(startingValue)
        {
        }

        public SimpleFoodConsumptionRate(SimpleFoodConsumptionRate old) : base(old.CurrValue)
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
