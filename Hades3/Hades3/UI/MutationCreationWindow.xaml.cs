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
    public partial class MutationCreationWindow : Window
    {
        UIWindow parentWindow;

        Dictionary<BehaviorKind, double> behaviorValueMap;
        List<Mutation> mutations;

        public MutationCreationWindow(UIWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;

            mutations = new List<Mutation>();
            behaviorValueMap = new Dictionary<BehaviorKind, double>();
        }

        private void createExperimentButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(BehaviorKind m in behaviorValueMap.Keys)
                mutations.Add(new Mutation(m, behaviorValueMap[m]));

            parentWindow.CreateMutationButton(mutations, mutationNameTextBox.Text);
            mutations.Clear();
        }


        private void divisionRateMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label drLabel = new Label();
            double value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingDivideRate;
            drLabel.Content = "division prob: " + value;
            mutationList.Children.Add(drLabel);

            Slider drSlider = new Slider();
            drSlider.Value = value;
            drSlider.Maximum = 1.0;
            drSlider.Minimum = 0.0;

            behaviorValueMap.Add(BehaviorKind.DivisionProbability, value);

            drSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {                
                behaviorValueMap[BehaviorKind.DivisionProbability] = valueChangedEvent.NewValue;
                drLabel.Content = "division prob: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(drSlider);
            divisionRateMutationButton.IsEnabled = false;
        }

        private void selfDeathMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label sdLabel = new Label();
            double value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingDeathRate;
            sdLabel.Content = "self death prob: " + value;
            mutationList.Children.Add(sdLabel);

            Slider sdSlider = new Slider();
            sdSlider.Value = value;
            sdSlider.Maximum = 1.0;
            sdSlider.Minimum = 0.0;

            behaviorValueMap.Add(BehaviorKind.DeathProbability, value);

            sdSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.DeathProbability] = valueChangedEvent.NewValue;
                sdLabel.Content = "self death prob: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(sdSlider);
            selfDeathMutationButton.IsEnabled = false;

        }

        private void moveProbabilityButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            double value = SimulationCore.Instance.SimulationParams.FinalCellConfig.MoveProbability;
            ptLabel.Content = "move prob: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = value;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            behaviorValueMap.Add(BehaviorKind.MoveProbability, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.MoveProbability] = valueChangedEvent.NewValue;
                ptLabel.Content = "move prob: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            moveProbabilityButton.IsEnabled = false;
        }

        private void enterPipeMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            double value = SimulationCore.Instance.SimulationParams.FinalCellConfig.EnterPipeProbability;
            ptLabel.Content = "enter pipe prob: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = value;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            behaviorValueMap.Add(BehaviorKind.EnterPipeProbability, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.EnterPipeProbability] = valueChangedEvent.NewValue;
                ptLabel.Content = "enter pipe prob: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            enterPipeMutationButton.IsEnabled = false;
        }

        private void exitPipeMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            double value = SimulationCore.Instance.SimulationParams.FinalCellConfig.LeavePipeProbability;
            ptLabel.Content = "exit pipe prob: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = value;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            behaviorValueMap.Add(BehaviorKind.ExitPipeProbability, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.ExitPipeProbability] = valueChangedEvent.NewValue;
                ptLabel.Content = "exit pipe prob: " + valueChangedEvent.NewValue;

            };

            mutationList.Children.Add(ptSlider);
            exitPipeMutationButton.IsEnabled = false;
        }

        private void survivePipeMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            double value = SimulationCore.Instance.SimulationParams.FinalCellConfig.SurvivePipeProbability;
            ptLabel.Content = "survive pipe prob: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.Value = value;
            ptSlider.Maximum = 1.0;
            ptSlider.Minimum = 0.0;

            behaviorValueMap.Add(BehaviorKind.SurvivePipeProbability, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.SurvivePipeProbability] = valueChangedEvent.NewValue;
                ptLabel.Content = "survive pipe prob: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            survivePipeMutationButton.IsEnabled = false;
        }

        private void pressureToleranceAtLocationMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            int value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingPressureToleranceAtLocation;
            ptLabel.Content = "pressure tolerance at location: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.IsSnapToTickEnabled = true;
            ptSlider.TickFrequency = 1;
            ptSlider.Value = value;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxPressureToleranceAtLocation;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinPressureToleranceAtLocation;

            behaviorValueMap.Add(BehaviorKind.PressureToleranceAtLocation, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.PressureToleranceAtLocation] = valueChangedEvent.NewValue;
                ptLabel.Content = "pressure tolerance at location: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            pressureToleranceAtLocationMutationButton.IsEnabled = false;
        }

        private void pressureToleranceAtNeighborsMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            int value = SimulationCore.Instance.SimulationParams.FinalCellConfig.startingPressureToleranceAtNeighbors;
            ptLabel.Content = "pressure tolerance at neighbors: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.IsSnapToTickEnabled = true;
            ptSlider.TickFrequency = 1;
            ptSlider.Value = value;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxPressureToleranceAtNeighbors;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinPressureToleranceAtNeighbors;

            behaviorValueMap.Add(BehaviorKind.PressureToleranceAtNeighbors, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.PressureToleranceAtNeighbors] = valueChangedEvent.NewValue;
                ptLabel.Content = "pressure tolerance at neighbors: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            pressureToleranceAtNeighborsMutationButton.IsEnabled = false;
        }

        private void foodConsumptionRateMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            int value = SimulationCore.Instance.SimulationParams.FinalCellConfig.FoodConsumptionRate;
            ptLabel.Content = "food consumption rate: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.IsSnapToTickEnabled = true;
            ptSlider.TickFrequency = 1;
            ptSlider.Value = value;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodConsumptionRate;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodConsumptionRate;

            behaviorValueMap.Add(BehaviorKind.FoodConsumptionRate, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.FoodConsumptionRate] = valueChangedEvent.NewValue;
                ptLabel.Content = "food consumption rate: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            foodConsumptionRateMutationButton.IsEnabled = false;
        }

        private void foodMaxStorageMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            int value = SimulationCore.Instance.SimulationParams.FinalCellConfig.FoodMaxStorage;
            ptLabel.Content = "food max storage: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.IsSnapToTickEnabled = true;
            ptSlider.TickFrequency = 1;
            ptSlider.Value = value;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodStorage;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodStorage;

            behaviorValueMap.Add(BehaviorKind.FoodMaxStorage, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.FoodMaxStorage] = valueChangedEvent.NewValue;
                ptLabel.Content = "food max storage: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            foodMaxStorageMutationButton.IsEnabled = false;
        }

        private void foodConcernLevelMutationButton_Click(object sender, RoutedEventArgs e)
        {
            Label ptLabel = new Label();
            int value = SimulationCore.Instance.SimulationParams.FinalCellConfig.FoodConcernLevel;
            ptLabel.Content = "food concern level: " + value;
            mutationList.Children.Add(ptLabel);

            Slider ptSlider = new Slider();
            ptSlider.IsSnapToTickEnabled = true;
            ptSlider.TickFrequency = 1;
            ptSlider.Value = value;
            ptSlider.Maximum = SimulationCore.Instance.SimulationParams.cellParameters.MaxFoodConcernLevel;
            ptSlider.Minimum = SimulationCore.Instance.SimulationParams.cellParameters.MinFoodConcernLevel;

            behaviorValueMap.Add(BehaviorKind.FoodConcernLevel, value);

            ptSlider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> valueChangedEvent)
            {
                behaviorValueMap[BehaviorKind.FoodConcernLevel] = valueChangedEvent.NewValue;
                ptLabel.Content = "food concern level: " + valueChangedEvent.NewValue;
            };

            mutationList.Children.Add(ptSlider);
            foodConcernLevelMutationButton.IsEnabled = false;
        }

    }
}
