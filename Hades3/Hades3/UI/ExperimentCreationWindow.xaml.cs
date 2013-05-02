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
        //Experiment theExperiment;
        //List<Experiment.GeneType> mutations;

        //Dictionary<ListBoxItem, Experiment.Selection> selectionMapping;
        //Dictionary<ListBoxItem, Experiment.GeneType> mutationMapping;

        UIWindow parentWindow;

        Dictionary<BehaviorKind, double> behaviorValueMap;
        List<Mutation> mutations;

        public ExperimentCreationWindow(UIWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;

            mutations = new List<Mutation>();
            behaviorValueMap = new Dictionary<BehaviorKind, double>();

            /*
            selectionMapping = new Dictionary<ListBoxItem, Experiment.Selection>();
            mutationMapping = new Dictionary<ListBoxItem, Experiment.GeneType>();

            

            //List<ComboBoxItem> cboxItems = new List<ComboBoxItem>();

            ComboBoxItem selectAll = new ComboBoxItem();
            selectAll.Content = "select all";
            selectAll.Name = "selectAll";
            cellSelectionComboBox.Items.Add(selectAll);
            selectionMapping.Add(selectAll, Experiment.Selection.All);

            ComboBoxItem selectAllBlast = new ComboBoxItem();
            selectAllBlast.Content = "select all blast cells";
            cellSelectionComboBox.Items.Add(selectAllBlast);
            selectionMapping.Add(selectAllBlast, Experiment.Selection.AllBlast);

            ComboBoxItem selectAllFinal = new ComboBoxItem();
            selectAllFinal.Content = "select all final cells";
            cellSelectionComboBox.Items.Add(selectAllFinal);
            selectionMapping.Add(selectAllFinal, Experiment.Selection.AllFinal);

            ComboBoxItem selectOneBlast = new ComboBoxItem();
            selectOneBlast.Content = "select one blast cell";
            cellSelectionComboBox.Items.Add(selectOneBlast);
            selectionMapping.Add(selectOneBlast, Experiment.Selection.OneBlast);

            ComboBoxItem selectOneFinal = new ComboBoxItem();
            selectOneFinal.Content = "select one final cell";
            cellSelectionComboBox.Items.Add(selectOneFinal);
            selectionMapping.Add(selectOneFinal, Experiment.Selection.OneFinal);

            cellSelectionComboBox.SelectedItem = selectAll;

            Console.WriteLine("there are " + cellSelectionComboBox.Items.Count + " items in the combobox");
             * */

            //mutations = new List<Experiment.GeneType>();
        }

        /*
        private void spaceToleranceMutationButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem newMutation = new ListBoxItem();
            newMutation.Content = "space tolerance";
            mutationList.Children.Add(newMutation);
            mutations.Add(Experiment.GeneType.SpaceTolerance);
            spaceToleranceMutationButton.IsEnabled = false;
        }
         * */

        /*
        private void selfRepairMutationButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem newMutation = new ListBoxItem();
            newMutation.Content = "self repair";
            mutationsListBox.Items.Add(newMutation);
            mutations.Add(Experiment.GeneType.SelfRepair);
            selfRepairMutationButton.IsEnabled = false;
        }
         * */

        

        private void createExperimentButton_Click(object sender, RoutedEventArgs e)
        {

            /*
            foreach (Mutation m in mutations)
            {
                mutations.Add(new Mutation(m.behavior, behaviorValueMap[m.behavior]));
            }
             * */
            

            foreach(BehaviorKind m in behaviorValueMap.Keys)
            {
                mutations.Add(new Mutation(m, behaviorValueMap[m]));
            }
            parentWindow.CreateMutationBlueprint(mutations);
            mutations.Clear();
            
            /*
            Experiment.Selection select = selectionMapping[(ListBoxItem)cellSelectionComboBox.SelectedItem];

            if (mutations.Count == 0)
            {
                MessageBox.Show("invalid mutation selection");
                return;
            }

            int mutationValue;
            int.TryParse(modifierValueTextBox.Text, out mutationValue);
            if (mutationValue == 0)
            {
                MessageBox.Show("invalid mutation modifier");
                return;
            }

            string experimentName = experimentNameTextBox.Text;

            theExperiment = new Experiment(select, mutations, mutationValue, experimentName);
            Console.WriteLine("Created experiment. Selection: " + select + ", mutations: " + mutations.ToString() + ", mutate by: " + mutationValue + ", name: " + experimentName);
            parentWindow.CreateExperiment(theExperiment);
            */ 
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

            //mutations.Add(new Mutation(BehaviorKind.PressureTolerance));
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
