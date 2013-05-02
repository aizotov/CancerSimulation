using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class PipeTravelInfo
    {
        public PipeTravelDirection TravelDirection;

        public EndothelialCell ContainingPipe;

        public PipeTravelInfo(EndothelialCell entryPoint)
        {
            ContainingPipe = entryPoint;
            TravelDirection = randomlyChooseDirection();
        }

        private PipeTravelDirection randomlyChooseDirection()
        {
            if (Environment.random.NextDouble() < 0.5)
                return PipeTravelDirection.toParent;
            return PipeTravelDirection.toChildren;
        }
    }

    public enum PipeTravelDirection
    {
        toParent,
        toChildren
    }

}