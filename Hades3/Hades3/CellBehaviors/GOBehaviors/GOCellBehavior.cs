using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public abstract class GOCellBehavior : CellBehavior
    {
        private List<GOTerm> goTerms;
        public List<GOTerm> GoTerms
        {
            set
            {
                goTerms = value;
            }
        }

        public GOCellBehavior(List<GOTerm> goTerms)
        {
            this.goTerms = goTerms;
            calculateValue();
        }

        // copy constructor
        public GOCellBehavior(GOCellBehavior original)
        {
            CurrValue = original.CurrValue;
            goTerms = new List<GOTerm>();
            foreach (GOTerm oldGOTerm in goTerms)
            {
                GOTerm newGOTerm = new GOTerm(oldGOTerm);
                goTerms.Add(newGOTerm);
            }
        }

        public void recalculateValue(double oldWeight, GOTerm mutatedTerm)
        {
            CurrValue -= mutatedTerm.PValue * oldWeight;
            CurrValue += mutatedTerm.PValue * mutatedTerm.Weight;
        }

        protected void calculateValue()
        {
            CurrValue = 0;
            foreach (GOTerm term in goTerms)
                CurrValue += term.Weight * term.PValue;
        }


    }
}
