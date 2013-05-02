using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class GODivide : GOCellBehavior
    {
        public GODivide(List<GOTerm> goTerms): base(goTerms)
        {
            throw new NotImplementedException("havent tested any GO functinality");
        }

        public GODivide(GODivide divideBehavior): base(divideBehavior)
        {
        }


    }
}
