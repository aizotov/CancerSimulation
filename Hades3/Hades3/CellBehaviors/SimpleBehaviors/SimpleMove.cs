using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class SimpleMove : SimpleCellBehavior
    {
        public static double MaxValue;
        public static double MinValue;

        public SimpleMove(double startingValue) : base(startingValue)
        {
        }

        public SimpleMove(SimpleMove old) : base(old.CurrValue)
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
