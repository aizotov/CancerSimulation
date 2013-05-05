using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    class FinalCell : TissueCell
    {        
        protected int generation;

        public FinalCell(Vector3 position, SimulationParameters.SimpleBehaviorParam behaviorConfig, int startingFood) : base(position, startingFood)
        {
            cellBehaviors = new SimpleBehaviorAggregate(behaviorConfig);

            this.generation = 1;
            initializeFinalCell();
            checkChildProbabilities(this);
        }

        public FinalCell(Vector3 position, SimulationParameters.GOTermBehaviorParams behaviorConfig, int startingFood) : base(position, startingFood)
        {
            throw new NotImplementedException("havent tested GO behaviors");
        }

        public FinalCell(TissueCell parentCell) : base(parentCell)
        {
            initializeFinalCell();
            //if (parentCell.GetType() == typeof(BlastCell))
            //{
            //    this.generation = 1;
            //    if (SimulationCore.DEBUG)
            //    {
            //        if (parentCell.CellBehaviors.GetType() == typeof(GOBehaviorAggregate))
            //            throw new NotImplementedException("have not implemented GO behaviors yet");
            //    }

            //    //cellBehaviors = new SimpleBehaviorAggregate(SimulationCore.Instance.SimulationParams.FinalCellConfig);
            //}

            cellBehaviors = new SimpleBehaviorAggregate((SimpleBehaviorAggregate)parentCell.CellBehaviors);
            if (parentCell.GetType() == typeof(FinalCell))
                this.generation = ((FinalCell)parentCell).generation + 1;




            //cellBehaviors = new SimpleBehaviorAggregate((SimpleBehaviorAggregate)parentCell.CellBehaviors);
            /*
            else if (parentCell.GetType() == typeof(FinalCell))
            {
                this.generation = ((FinalCell)parentCell).generation + 1;
                cellBehaviors = new SimpleBehaviorAggregate((SimpleBehaviorAggregate)parentCell.CellBehaviors);
            }
             * */
        }

        private void initializeFinalCell()
        {
            AddChildChoice<FinalCell>(1.0);
        }

        protected override bool GenerationDeath()
        {
            if (generation > SimulationCore.Instance.SimulationParams.cellParameters.FinalCellGenerationLimit)
            {
                die();
                return true;
            }
            return false;
        }

        protected override void incrementGeneration()
        {
            generation++;
        }

        protected override void MoveToBetterLocationIfPossible()
        {
        }

        protected override bool canEnterPipe(EndothelialCell entryPoint)
        {
            return false;
        }

    }
}