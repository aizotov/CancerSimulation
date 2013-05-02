using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class GOTerm
    {
        public static double minWeight;
        public static double maxWeight;

        private string description;
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private double pValue;
        public double PValue
        {
            get
            {
                return pValue;
            }
        }

        private double weight;
        public double Weight
        {
            get
            {
                return weight;
            }
        }

        public GOTerm(SimulationParameters.GOTermParam termParams)
        {
            name = termParams.termName;
            description = termParams.description;
            pValue = termParams.pValue;
            weight = termParams.startingValue;
        }

        // copy constructor
        public GOTerm(GOTerm original)
        {
            pValue = original.pValue;
            weight = original.weight;
            description = original.description;
        }


        public void Mutate()
        {
            double r = Environment.random.NextDouble();
            if (r > 0.5)
                mutateUp();
            else
                mutateDown();
        }

        private void mutateUp()
        {
            weight += SimulationCore.Instance.SimulationParams.cellParameters.MutateBy;
            if (weight > maxWeight)
                weight = maxWeight;
        }

        private void mutateDown()
        {
            weight -= SimulationCore.Instance.SimulationParams.cellParameters.MutateBy;
            if (weight < minWeight)
                weight = minWeight;
        }

    }
}
