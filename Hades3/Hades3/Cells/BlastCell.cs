using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class BlastCell : TissueCell
    {
        private static double MakeBlastCellProbability;
        private static double MakeFinalCellProbability;

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
            initializeBlastCell();
        }

        public static void InitializeDivideProbabilities(double blastProb, double finalProb)
        {
            if (blastProb + finalProb != 1.0)
                throw new Exception("blast cell divide probabilities must equal 1");

            MakeBlastCellProbability = blastProb;
            MakeFinalCellProbability = finalProb;

        }

        private void initializeBlastCell()
        {
            AddChildChoice<BlastCell>(MakeBlastCellProbability);
            AddChildChoice<FinalCell>(MakeFinalCellProbability);
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

        protected override void MoveToBetterLocationIfPossible()
        {
            if (Environment.random.NextDouble() < cellBehaviors.GetMoveProbability())
            {
                Vector3 possibleNewLocation = Environment.Instance.GetBetterLocation(cellLocation);
                if (possibleNewLocation != cellLocation)
                    cellMove(possibleNewLocation);
            }
        }

        protected override bool canEnterPipe(EndothelialCell entryPoint)
        {
            if (Environment.random.NextDouble() < cellBehaviors.GetEnterPipeProbability())
                return true;
            return false;
        }
    }
}
