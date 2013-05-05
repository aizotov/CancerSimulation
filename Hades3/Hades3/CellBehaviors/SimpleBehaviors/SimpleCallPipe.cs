using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class SimpleCallPipe : SimpleCellBehavior
    {
        public static double MaxValue;
        public static double MinValue;

        public SimpleCallPipe(double startingValue) : base(startingValue)
        {
        }

        public SimpleCallPipe(SimpleCallPipe old) : base(old.CurrValue)
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
