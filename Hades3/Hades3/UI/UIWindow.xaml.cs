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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Forms.Integration;
using System.IO;

namespace Hades3
{
    /// <summary>
    /// Interaction logic for UIWindow.xaml
    /// </summary>
    /// 
    public partial class UIWindow : Window
    {
        ExperimentCreationWindow experimentCreationWindow;

        ComboBoxItem selectedConfiguration;

        private ObservableDataSource<Point> finalCellNumberDataSource;
        private ObservableDataSource<Point> blastCellNumberDataSource;
        private ObservableDataSource<Point> mutationCountDataSource;
        private ObservableDataSource<Point> bloodVesselCountCountDataSource;

        private int finalCellNumberGraphTimeCounter = 0;
        private int blastCellNumberGraphTimeCounter = 0;
        private int mutationNumberGraphTimeCounter = 0;
        private int bloodVesselNumberGraphTimeCounter = 0;

        private Dictionary<Button, Experiment> experimentButtonMapping;

        private Dictionary<ComboBoxItem, Type> startConfigChoices;

        public UIWindow(SimulationCore displayModule)
        {
            ElementHost.EnableModelessKeyboardInterop(this);

            InitializeComponent();

            experimentButtonMapping = new Dictionary<Button, Experiment>();

            startConfigChoices = new Dictionary<ComboBoxItem, Type>();
            IEnumerable<String> paramFileNames = Directory.EnumerateFiles("Content");          

            foreach (String f in paramFileNames)
            {
                if(System.IO.Path.GetExtension(f) == ".json"){
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Content = System.IO.Path.GetFileName(f);
                    startConfigurationComboBox.Items.Add(newItem);
                }
            }

            finalCellNumberDataSource = new ObservableDataSource<Point>();
            finalCellNumberDataSource.SetXYMapping(p => p);
            blastCellNumberDataSource = new ObservableDataSource<Point>();
            blastCellNumberDataSource.SetXYMapping(p => p);
            cellCount.AddLineGraph(finalCellNumberDataSource, Colors.Green, 2, "final cells");
            cellCount.AddLineGraph(blastCellNumberDataSource, Colors.Red, 2, "blast cells");
            cellCount.LegendVisible = false;

            mutationCountDataSource = new ObservableDataSource<Point>();
            mutationCountDataSource.SetXYMapping(p => p);
            mutationCount.AddLineGraph(mutationCountDataSource, Colors.Black, 2, "mutations");
            mutationCount.LegendVisible = false;

            bloodVesselCountCountDataSource = new ObservableDataSource<Point>();
            bloodVesselCountCountDataSource.SetXYMapping(p => p);
            bloodVesselCount.AddLineGraph(bloodVesselCountCountDataSource, Colors.Purple, 2, "blood vessels");
            bloodVesselCount.LegendVisible = false;

            stopSimulationButton.IsEnabled = false;
            experimentCreateButton.IsEnabled = false;
        }

        public void UpdateNumberFinalOfCells(int cellNumber)
        {
            Point p = new Point(finalCellNumberGraphTimeCounter++, cellNumber);
            finalCellNumberDataSource.AppendAsync(Dispatcher, p);
        }

        public void UpdateNumberOfBlastCells(int cellNumber)
        {
            Point p = new Point(blastCellNumberGraphTimeCounter++, cellNumber);
            blastCellNumberDataSource.AppendAsync(Dispatcher, p);
        }

        public void UpdateNumberOfMutations(int mutationNumber)
        {
            Point p = new Point(mutationNumberGraphTimeCounter++, mutationNumber);
            mutationCountDataSource.AppendAsync(Dispatcher, p);
        }

        public void UpdateNumberOfPipes(int hungryCellNumber)
        {
            Point p = new Point(bloodVesselNumberGraphTimeCounter++, hungryCellNumber);
            bloodVesselCountCountDataSource.AppendAsync(Dispatcher, p);
        }

        public void ResetStartSimulationButton()
        {
            startSimulationButton.IsEnabled = true;
            startConfigurationComboBox.IsEnabled = true;
            stopSimulationButton.IsEnabled = false;
        }

        private void startSimulation_Click(object sender, RoutedEventArgs e)
        {
            blastCellNumberDataSource.Collection.Clear();
            finalCellNumberDataSource.Collection.Clear();
            mutationCountDataSource.Collection.Clear();
            bloodVesselCountCountDataSource.Collection.Clear();

            blastCellNumberGraphTimeCounter = 0;
            finalCellNumberGraphTimeCounter = 0;
            mutationNumberGraphTimeCounter = 0;
            bloodVesselNumberGraphTimeCounter = 0;

            experimentCreateButton.IsEnabled = true;
            startSimulationButton.IsEnabled = false;
            startConfigurationComboBox.IsEnabled = false;
            stopSimulationButton.IsEnabled = true;

            /*
            int numberOfStartingCells;
            int.TryParse(startingNumberTextBox.Text, out numberOfStartingCells);
            if (numberOfStartingCells == 0)
            {
                MessageBox.Show("invalid number of starting cells, please enter an integer");
                return;
            }
             * */

            //if (startConfigChoices[(ComboBoxItem)startConfigurationComboBox.SelectedItem].Equals(typeof(BlastCell)))
            //displayModule.StartSimulation(typeof(BlastCell));
            //else if (startConfigChoices[(ComboBoxItem)startConfigurationComboBox.SelectedItem].Equals(typeof(FinalCell)))
            //  displayModule.StartSimulation(typeof(FinalCell));
            
            
            //SimulationCore.Instance.StartSimulation("during the week");
            SimulationCore.Instance.StartSimulation(((ComboBoxItem)startConfigurationComboBox.SelectedItem).Content.ToString());
            
            /*
            else
            {
                Console.WriteLine("attempted to select: " + selectedConfiguration.Name);
                throw new Exception("unsupported start configuration");
            }
             * */
        }

        private void startConfigurationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedConfiguration = (ComboBoxItem)startConfigurationComboBox.SelectedItem;
        }

        private void experimentCreateButton_Click(object sender, RoutedEventArgs e)
        {
            experimentCreationWindow = new ExperimentCreationWindow(this);
            ElementHost.EnableModelessKeyboardInterop(experimentCreationWindow);
            experimentCreationWindow.Show();
        }

        public void CreateExperiment(Experiment newExperiment)
        {
            Button newButton = new Button();
            newButton.Content = newExperiment.Name;
            newButton.MinHeight = 50;
            newButton.MinWidth = 50;
            newButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            newButton.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            experimentButtonMapping.Add(newButton, newExperiment);
            newButton.Click += delegate(Object sender, RoutedEventArgs e)
            {
                Experiment myExperiment = experimentButtonMapping[(Button)sender];
                Console.WriteLine("starting experiment: " + myExperiment.Name);
                SimulationCore.Instance.AcceptExperiment(myExperiment);
            };

            experimentPanel.Children.Add(newButton);
        }

        public void CreateMutationBlueprint(List<Mutation> mutations)
        {
            Console.WriteLine("created mutation blueprint!");
            foreach (Mutation m in mutations)
            {
                Console.WriteLine(m.behavior + "  :  " + m.value);
            }
            Console.WriteLine("\n");

            SimulationCore.Instance.AcceptMutationBlueprint(mutations);
        }

        private void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.StopSimulation();
            ResetStartSimulationButton();
            stopSimulationButton.IsEnabled = false;
            experimentCreateButton.IsEnabled = false;
        }

        private void drawSimulationTextBox_Checked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawSimulation();
        }
        private void drawSimulationTextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawSimulation();
        }

        private void drawCellsTextBox_Checked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawCells();
        }
        private void drawCellsTextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawCells();
        }

        private void drawBloodVesselsTextBox_Checked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawBloodVessels();
        }
        private void drawBloodVesselsTextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawBloodVessels();
        }

        private void drawSignalsTextBox_Checked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawSignals();
        }
        private void drawSignalsTextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawSignals();
        }

        private void hideLegendCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            cellCount.LegendVisible = false;
            mutationCount.LegendVisible = false;
            bloodVesselCount.LegendVisible = false;
        }

        private void hideLegendCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            cellCount.LegendVisible = true;
            mutationCount.LegendVisible = true;
            bloodVesselCount.LegendVisible = true;
        }

        private void drawPressureCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawPressure();
        }

        private void drawPressureCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleDrawPressure();
        }

    }
}
