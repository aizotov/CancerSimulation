using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class BlastCell : TissueCell
    {
        public BlastCell(Vector3 position, SimulationParameters.SimpleBehaviorParam behaviorConfig, int startingFood) : base(position, startingFood)
        {
            cellBehaviors = new SimpleBehaviorAggregate(behaviorConfig);

            Console.WriteLine("creating first blast cell at: " + position);

            initializeBlastCell();
            checkChildProbabilities(this);
        }

        public BlastCell(Vector3 position, SimulationParameters.GOTermBehaviorParams behaviorConfig, int startingFood) : base(position, startingFood)
        {
            throw new NotImplementedException("haven't tested GO behavior");

            //insantiate go behavior aggregate
            //cellBehaviors = new GOBehaviorAggregate(behaviorConfig, this);

            Console.WriteLine("creating first blast cell at: " + position);

            initializeBlastCell();
            checkChildProbabilities(this);
        }


        public BlastCell(TissueCell parentCell) : base(parentCell)
        {
            cellBehaviors = new SimpleBehaviorAggregate((SimpleBehaviorAggregate)parentCell.CellBehaviors);
            //cellBehaviors = parentCell.CellBehaviors.GetClone();
            initializeBlastCell();
        }


        private void initializeBlastCell()
        {
            AddChildChoice<BlastCell>(0.2);
            AddChildChoice<FinalCell>(0.8);
            //AddChildChoice<BlastCell>(1);
        }

      
        protected override bool GenerationDeath()
        {
            // blast cells don't have generation death
            return false;
        }

        protected override void incrementGeneration()
        {
            return;
        }
    }
}
