using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class SimpleFoodMaxStorage : SimpleCellBehavior
    {
        public static int MaxValue;
        public static int MinValue;

        public SimpleFoodMaxStorage(double startingValue) : base(startingValue)
        {
        }

        public SimpleFoodMaxStorage(SimpleFoodMaxStorage old) : base(old.CurrValue)
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
