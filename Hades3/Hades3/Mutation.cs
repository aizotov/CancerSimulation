using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public enum BehaviorKind
    {
        DivisionProbability,
        DeathProbability,
        MoveProbability,
        EnterPipeProbability,
        ExitPipeProbability,
        SurvivePipeProbability,
        CallPipeProbability,
        PressureToleranceAtLocation,
        PressureToleranceAtNeighbors,
        FoodConsumptionRate,
        FoodMaxStorage,
        FoodConcernLevel
    }


    public class Mutation
    {
        public BehaviorKind behavior;
        public double value;

        public Mutation(BehaviorKind behavior, double value)
        {
            this.behavior = behavior;
            this.value = value;
        }

        public Mutation(BehaviorKind behavior)
        {
        }

        public string ToString()
        {
            string s = "";
            s += behavior + " : " + value;
            return s;
        }
    }

}
