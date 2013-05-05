using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public abstract class BehaviorAggregate
    {
        public abstract void Mutate();
       
        public abstract double GetDivisionProbability();
        public abstract double GetDeathProbability();

        public abstract int GetPressureToleranceAtLocation();
        public abstract int GetPressureToleranceAtNeighbors();

        public abstract int GetMaxFoodStorageLevel();
        public abstract int GetFoodConcernLevel();
        public abstract int GetFoodConsumptionRate();

        public abstract double GetMoveProbability();

        public abstract double GetEnterPipeProbability();
        public abstract double GetLeavePipeProbability();
        public abstract double GetSurvivePipeProbability();

        public abstract double GetCallPipeProbability();        

        public abstract void ApplyMutations(List<Mutation> mutationList);
    }
}
