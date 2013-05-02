using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;

namespace Hades3
{
    /// <summary>
    /// Interaction logic for ExperimentCreation.xaml
    /// </summary>
    public partial class ExperimentCreationWindow : Window
    {
        UIWindow parentWindow;

        Dictionary<BehaviorKind, double> behaviorValueMap;
        List<Mutation> mutations;

        public ExperimentCreationWindow(UIWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;

            mutations = new List<Mutation>();
            behaviorValueMap = new Dictionary<BehaviorKind, double>();
        }

        private void createExperimentButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(BehaviorKind m in behaviorValueMap.Keys)
            {
                mutations.Add(new Mutation(m, behaviorValueMap[m]));
            }
            parentWindow.CreateMutationBlueprint(mutations);
            mutations.Clear();
        }

        private void pressureToleranceAtLocationMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "pressure tolerance at location";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingPressureToleranceAtLocation;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxPressureToleranceAtLocation;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinPressureToleranceAtLocation;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("pressure tolerance @ loc val: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.PressureToleranceAtLocation] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            pressureToleranceAtLocationMutationButton.IsEnabled = false;
        }

        private void selfDeathMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label sdLabel = new Label();
            sdLabel.Content = "self death";
            mutationList.Children.Add(sdLabel);

            Slider sdSlider = new Slider();
            sdSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingDeathRate;
            sdSlider.Maximum = 1.0;
            sdSlider.Minimum = 0.0;

            sdSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("death rate val: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.DeathProbability] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(sdSlider);
            selfDeathMutationButton.IsEnabled = false;

        }

        private void divisionRateMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label drLabel = new Label();
            drLabel.Content = "division rate";
            mutationList.Children.Add(drLabel);

            Slider drSlider = new Slider();
            drSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingDivideRate;
            drSlider.Maximum = 1.0;
            drSlider.Minimum = 0.0;

            drSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("division rate val: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.DivisionProbability] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(drSlider);
            divisionRateMutationButton.IsEnabled = false;
        }

        private void pressureToleranceAtNeighborsMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "pressure tolerance at neighbors";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingPressureToleranceAtNeighbors;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxPressureToleranceAtNeighbors;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinPressureToleranceAtNeighbors;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("pressure tolerance @ neighbors val: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.PressureToleranceAtNeighbors] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            pressureToleranceAtNeighborsMutationButton.IsEnabled = false;
        }

        private void enterPipeMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "enter pipe probability";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.EnterPipeProbability;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("enter pipe probability: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.EnterPipeProbability] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            enterPipeMutationButton.IsEnabled = false;
        }

        private void exitPipeMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "exit pipe probability";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.LeavePipeProbability;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("exit pipe probability: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.ExitPipeProbability] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            exitPipeMutationButton.IsEnabled = false;
        }

        private void survivePipeMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "survive pipe probability";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.SurvivePipeProbability;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("survive pipe probability: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.SurvivePipeProbability] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            survivePipeMutationButton.IsEnabled = false;
        }

        private void moveProbabilityButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "move probability";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.MoveProbability;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("move probability: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.MoveProbability] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            moveProbabilityButton.IsEnabled = false;
        }

        private void foodConsumptionRateMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "food consumption rate";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.FoodConsumptionRate;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodConsumptionRate;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodConsumptionRate;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("food consumption rate: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.FoodConsumptionRate] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            foodConsumptionRateMutationButton.IsEnabled = false;
        }

        private void foodMaxStorageMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "food max storage";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.FoodMaxStorage;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodStorage;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodStorage;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("food max storage: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.FoodMaxStorage] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            foodMaxStorageMutationButton.IsEnabled = false;
        }

        private void foodConcernLevelMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            ptLabel.Content = "food concern level";
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = SimulationCore.Instance.SimulationParams.FinalCellConfig.FoodConcernLevel;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodConcernLevel;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodConcernLevel;

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                Console.WriteLine("food concern level: " + valueChangedEvent.NewValue);
                behaviorValueMap[BehaviorKind.FoodConcernLevel] = valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            foodConcernLevelMutationButton.IsEnabled = false;
        }

    }
}
