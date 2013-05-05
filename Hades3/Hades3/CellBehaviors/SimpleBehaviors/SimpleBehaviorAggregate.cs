using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class SimpleBehaviorAggregate : BehaviorAggregate
    {
        private SimpleDivide divideBehavior;
        private SimpleDeathBehavior deathBehavior;
        private SimplePressureToleranceAtLocation pressureToleranceAtLocation;
        private SimplePressureToleranceAtNeighbors pressureToleranceAtNeighbors;
        private SimpleFoodConsumptionRate foodConsumptionBehavior;
        private SimpleFoodMaxStorage foodMaxStorage;
        private SimpleFoodConcern foodConcern;
        private SimpleMove moveAbility;
        private SimpleCallPipe callPipeProbability;

        private SimplePipeEnter pipeEnter;
        private SimplePipeLeave pipeLeave;
        private SimplePipeSurvival pipeSurvival;

        public SimpleBehaviorAggregate(SimulationParameters.SimpleBehaviorParam startingValues)
        {
            if (startingValues.startingDivideRate == 0)
                Console.WriteLine("possible invalid divide rate param");
            divideBehavior = new SimpleDivide(startingValues.startingDivideRate);
            SimpleDivide.MaxValue = 1.0;
            SimpleDivide.MinValue = 0.0;

            if (startingValues.startingDeathRate == 0)
                Console.WriteLine("possible invalid death rate param");
            deathBehavior = new SimpleDeathBehavior(startingValues.startingDeathRate);
            SimpleDeathBehavior.MaxValue = 1.0;
            SimpleDeathBehavior.MinValue = 0.0;


            if (startingValues.startingPressureToleranceAtLocation == 0)
                Console.WriteLine("possible invalid pressure tol @ location param");
            pressureToleranceAtLocation = new SimplePressureToleranceAtLocation(startingValues.startingPressureToleranceAtLocation);
            SimplePressureToleranceAtLocation.MaxValue = SimulationCore.Instance.SimulationParams.cellParameters.MaxPressureToleranceAtLocation;
            SimplePressureToleranceAtLocation.MinValue = SimulationCore.Instance.SimulationParams.cellParameters.MinPressureToleranceAtLocation;

            if (startingValues.startingPressureToleranceAtNeighbors == 0)
                Console.WriteLine("possible invalid perssure tol @ neighbors param");
            pressureToleranceAtNeighbors = new SimplePressureToleranceAtNeighbors(startingValues.startingPressureToleranceAtNeighbors);
            SimplePressureToleranceAtNeighbors.MaxValue = SimulationCore.Instance.SimulationParams.cellParameters.MaxPressureToleranceAtNeighbors;
            SimplePressureToleranceAtNeighbors.MinValue = SimulationCore.Instance.SimulationParams.cellParameters.MinPressureToleranceAtNeighbors;


            if (startingValues.FoodConsumptionRate == 0)
                Console.WriteLine("possible inavlid food consumption rate param");
            foodConsumptionBehavior = new SimpleFoodConsumptionRate(startingValues.FoodConsumptionRate);
            SimpleFoodConsumptionRate.MaxValue = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodConsumptionRate;
            SimpleFoodConsumptionRate.MinValue = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodConsumptionRate;

            if (startingValues.FoodMaxStorage == 0)
                Console.WriteLine("possible invalid food max storage rate param");
            foodMaxStorage = new SimpleFoodMaxStorage(startingValues.FoodMaxStorage);
            SimpleFoodMaxStorage.MaxValue = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodStorage;
            SimpleFoodMaxStorage.MinValue = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodStorage;

            if (startingValues.FoodConcernLevel == 0)
                Console.WriteLine("possible invalid food concern lvl param");
            foodConcern = new SimpleFoodConcern(startingValues.FoodConcernLevel);
            SimpleFoodConcern.MaxValue = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodConcernLevel;
            SimpleFoodConcern.MinValue = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodConcernLevel;


            //if (startingValues.MoveProbability == 0)
            //    Console.WriteLine("possible invalid move probablity param");
            moveAbility = new SimpleMove(startingValues.MoveProbability);
            SimpleMove.MaxValue = 1.0;
            SimpleMove.MinValue = 0.0;


            pipeEnter = new SimplePipeEnter(startingValues.EnterPipeProbability);
            SimplePipeEnter.MaxValue = 1.0;
            SimplePipeEnter.MinValue = 0.0;

            pipeLeave = new SimplePipeLeave(startingValues.LeavePipeProbability);
            SimplePipeLeave.MaxValue = 1.0;
            SimplePipeLeave.MinValue = 0.0;

            pipeSurvival = new SimplePipeSurvival(startingValues.SurvivePipeProbability);
            SimplePipeSurvival.MaxValue = 1.0;
            SimplePipeSurvival.MinValue = 0.0;

            callPipeProbability = new SimpleCallPipe(startingValues.CallPipeProbability);
            SimpleCallPipe.MaxValue = 1.0;
            SimpleCallPipe.MinValue = 0.0;
        }

        // copy constructor
        public SimpleBehaviorAggregate(SimpleBehaviorAggregate old)
        {
            this.divideBehavior = new SimpleDivide(old.divideBehavior);
            this.deathBehavior = new SimpleDeathBehavior(old.deathBehavior);

            this.pressureToleranceAtLocation = new SimplePressureToleranceAtLocation(old.pressureToleranceAtLocation);
            this.pressureToleranceAtNeighbors = new SimplePressureToleranceAtNeighbors(old.pressureToleranceAtNeighbors);

            this.foodConsumptionBehavior = new SimpleFoodConsumptionRate(old.foodConsumptionBehavior);
            this.foodMaxStorage = new SimpleFoodMaxStorage(old.foodMaxStorage);
            this.foodConcern = new SimpleFoodConcern(old.foodConcern);

            this.moveAbility = new SimpleMove(old.moveAbility);

            this.pipeEnter = new SimplePipeEnter(old.pipeEnter);
            this.pipeLeave = new SimplePipeLeave(old.pipeLeave);
            this.pipeSurvival = new SimplePipeSurvival(old.pipeSurvival);

            this.callPipeProbability = new SimpleCallPipe(old.callPipeProbability);
        }


        public override double GetDivisionProbability()
        {
            return divideBehavior.CurrValue;
        }

        public override double GetDeathProbability()
        {
            return deathBehavior.CurrValue;
        }

        
        public override int GetPressureToleranceAtLocation()
        {
            return (int)pressureToleranceAtLocation.CurrValue;
        }

        public override int GetPressureToleranceAtNeighbors()
        {
            return (int)pressureToleranceAtNeighbors.CurrValue;
        }


        public override int GetMaxFoodStorageLevel()
        {
            return (int)foodMaxStorage.CurrValue;
        }

        public override int GetFoodConcernLevel()
        {
            return (int)foodConcern.CurrValue;
        }

        public override int GetFoodConsumptionRate()
        {
            return (int)foodConsumptionBehavior.CurrValue;
        }


        public override double GetMoveProbability()
        {
            return moveAbility.CurrValue;
        }

        public override double GetEnterPipeProbability()
        {
            return pipeEnter.CurrValue;
        }

        public override double GetLeavePipeProbability()
        {
            return pipeLeave.CurrValue;
        }

        public override double GetSurvivePipeProbability()
        {
            return pipeSurvival.CurrValue;
        }


        public override double GetCallPipeProbability()
        {
            return callPipeProbability.CurrValue;
        }

        public override void Mutate()
        {
            throw new NotImplementedException();
        }

        private void applyMutation(Mutation m)
        {
            switch (m.behavior)
            {
                case BehaviorKind.DivisionProbability:
                    divideBehavior.MutateTo(m.value);
                    Console.WriteLine("new division probability: " + divideBehavior.CurrValue);
                    break;
                case BehaviorKind.DeathProbability:
                    deathBehavior.MutateTo(m.value);
                    Console.WriteLine("new death probability: " + deathBehavior.CurrValue);
                    break;
                case BehaviorKind.MoveProbability:
                    moveAbility.MutateTo(m.value);
                    Console.WriteLine("new move probability: " + moveAbility.CurrValue);
                    break;
                case BehaviorKind.EnterPipeProbability:
                    pipeEnter.MutateTo(m.value);
                    Console.WriteLine("new enter pipe probability: " + pipeEnter.CurrValue);
                    break;
                case BehaviorKind.ExitPipeProbability:
                    pipeLeave.MutateTo(m.value);
                    Console.WriteLine("new leave pipe probability: " + pipeLeave.CurrValue);
                    break;
                case BehaviorKind.SurvivePipeProbability:
                    pipeSurvival.MutateTo(m.value);
                    Console.WriteLine("new pipe survive probability: " + pipeSurvival.CurrValue);
                    break;
                case BehaviorKind.CallPipeProbability:
                    callPipeProbability.MutateTo(m.value);
                    Console.WriteLine("new call pipe probability: " + callPipeProbability.CurrValue);
                    break;
                case BehaviorKind.PressureToleranceAtLocation:
                    pressureToleranceAtLocation.MutateTo(m.value);
                    Console.WriteLine("new pressure tolerance at location: " + pressureToleranceAtLocation.CurrValue);
                    break;
                case BehaviorKind.PressureToleranceAtNeighbors:
                    pressureToleranceAtNeighbors.MutateTo(m.value);
                    Console.WriteLine("new pressure tolerance at neighbs: " + pressureToleranceAtNeighbors.CurrValue);
                    break;
                case BehaviorKind.FoodConsumptionRate:
                    foodConsumptionBehavior.MutateTo(m.value);
                    Console.WriteLine("new food consumption rate: " + foodConsumptionBehavior.CurrValue);
                    break;
                case BehaviorKind.FoodMaxStorage:
                    foodMaxStorage.MutateTo(m.value);
                    Console.WriteLine("new food max storage: " + foodMaxStorage.CurrValue);
                    break;
                case BehaviorKind.FoodConcernLevel:
                    foodConcern.MutateTo(m.value);
                    Console.WriteLine("new food concern level: " + foodConcern.CurrValue);
                    break;
                default:
                    throw new Exception("unsupported mutation type");
            }
        }

        public override void ApplyMutations(List<Mutation> mutationList)
        {
            foreach (Mutation m in mutationList)
            {
                applyMutation(m);
            }
            Console.WriteLine("\n");
        }
    }
}
