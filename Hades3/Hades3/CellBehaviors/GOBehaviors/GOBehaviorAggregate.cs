using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class GOBehaviorAggregate : BehaviorAggregate
    {
        //protected List<GOTerm> goTerms;
        //protected static Dictionary<string, List<Type>> goTermBehaviorMap;
        //protected Dictionary<Type, GOCellBehavior> typeBehaviorMap;

        /*
        //from tissue cell constructor
        this.goTerms = new List<GOTerm>();
        foreach (GOTerm parentGOTerm in parentCell.goTerms)
        {
            GOTerm cloneGOTerm = new GOTerm(parentGOTerm);
            this.goTerms.Add(cloneGOTerm);
        }
        */

        /*
        public static void InstantiateGOTermBehaviorMap(List<SimulationParameters.GOTermParam> goTermParams)
        {
            goTermBehaviorMap = new Dictionary<string, List<Type>>();
            foreach (SimulationParameters.GOTermParam termParam in goTermParams)
            {
                if (goTermBehaviorMap[termParam.termName] == null)
                    goTermBehaviorMap[termParam.termName] = new List<Type>();
                foreach(Type cellBehavior in termParam.behaviorsAffected)
                    goTermBehaviorMap[termParam.termName].Add(cellBehavior);
            }
        }
        */

        /*
        // used when creating cell without parent (at start of simulation)
        public TissueCell(Vector3 location, List<SimulationParameters.GOTermParam> goTermParams) : base(location)
        {
            this.goTerms = new List<GOTerm>();

            foreach (SimulationParameters.GOTermParam termParam in goTermParams)
                goTerms.Add(new GOTerm(termParam));
            
            divideAction = new GODivideBehavior(this, goTerms);

        }
         * */

        public override double GetDeathProbability()
        {
            throw new NotImplementedException();
        }

        public override int GetPressureToleranceAtLocation()
        {
            throw new NotImplementedException();
        }

        public override int GetPressureToleranceAtNeighbors()
        {
            throw new NotImplementedException();
        }

        public override double GetDivisionProbability()
        {
            throw new NotImplementedException();
        }


        public override int GetMaxFoodStorageLevel()
        {
            throw new NotImplementedException();
        }

        public override int GetFoodConcernLevel()
        {
            throw new NotImplementedException();
        }

        public override int GetFoodConsumptionRate()
        {
            throw new NotImplementedException();
        }

        public override double GetMoveProbability()
        {
            throw new NotImplementedException();
        }

        public override double GetEnterPipeProbability()
        {
            throw new NotImplementedException();
        }

        public override double GetLeavePipeProbability()
        {
            throw new NotImplementedException();
        }

        public override double GetSurvivePipeProbability()
        {
            throw new NotImplementedException();
        }


        public override void Mutate()
        {
            /*
            GOTerm termToBeMutated = UtilityMethods.GetRandomElementInList<GOTerm>(goTerms);
            double oldWeight = termToBeMutated.Weight;
            termToBeMutated.Mutate();
            if (termToBeMutated.Weight == oldWeight)
                throw new Exception("DEBUG this shouldn't happen very often");

            List<Type> behaviorsAffected = goTermBehaviorMap[termToBeMutated.Name];
            foreach (Type cellBehavior in behaviorsAffected)
            {
                typeBehaviorMap[cellBehavior].recalculateValue(oldWeight, termToBeMutated);
            }
            */
            throw new NotImplementedException();
        }

        //public override BehaviorAggregate GetClone()
        //{
        //    throw new NotImplementedException();
        //}

        public override void ApplyMutation(Mutation m)
        {
            throw new NotImplementedException();
        }
    }
}
