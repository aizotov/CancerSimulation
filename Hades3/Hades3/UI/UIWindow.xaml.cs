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
        MutationCreationWindow mutationCreationWindow;

        ComboBoxItem selectedConfiguration;

        private ObservableDataSource<Point> finalCellNumberDataSource;
        private ObservableDataSource<Point> blastCellNumberDataSource;
        private ObservableDataSource<Point> mutationCountDataSource;
        private ObservableDataSource<Point> bloodVesselCountCountDataSource;

        private Dictionary<Button, List<Mutation>> mutationButtonMapping;

        private Dictionary<ComboBoxItem, Type> startConfigChoices;

        public UIWindow(SimulationCore displayModule)
        {
            ElementHost.EnableModelessKeyboardInterop(this);

            InitializeComponent();

            mutationButtonMapping = new Dictionary<Button, List<Mutation>>();

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
            mutationCreationButton.IsEnabled = false;
        }

        #region updateGraphs

        public void UpdateNumberFinalOfCells(int cellNumber, long timeStep)
        {
            Point p = new Point(timeStep, cellNumber);
            finalCellNumberDataSource.AppendAsync(Dispatcher, p);
        }

        public void UpdateNumberOfBlastCells(int cellNumber, long timeStep)
        {
            Point p = new Point(timeStep, cellNumber);
            blastCellNumberDataSource.AppendAsync(Dispatcher, p);
        }

        public void UpdateNumberOfMutations(int mutationNumber, long timeStep)
        {
            Point p = new Point(timeStep, mutationNumber);
            mutationCountDataSource.AppendAsync(Dispatcher, p);
        }

        public void UpdateNumberOfPipes(int hungryCellNumber, long timeStep)
        {
            Point p = new Point(timeStep, hungryCellNumber);
            bloodVesselCountCountDataSource.AppendAsync(Dispatcher, p);
        }

        #endregion

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

            mutationCreationButton.IsEnabled = true;
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
            mutationCreationWindow = new MutationCreationWindow(this);
            ElementHost.EnableModelessKeyboardInterop(mutationCreationWindow);
            mutationCreationWindow.Show();
        }

        public void CreateMutationButton(List<Mutation> mutations, string mutationName)
        {
            Button newButton = new Button();
            newButton.Content = mutationName;
            newButton.MinHeight = 50;
            newButton.MinWidth = 50;
            newButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            newButton.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            mutationButtonMapping.Add(newButton, new List<Mutation>(mutations));

            newButton.Click += delegate(Object sender, RoutedEventArgs e)
            {
                List<Mutation> theMutationList = mutationButtonMapping[(Button)sender];
                Console.WriteLine("just pushed a button for this!");
                foreach (Mutation m in mutations)
                    Console.WriteLine(m.ToString());
                SimulationCore.Instance.AcceptMutationBlueprint(theMutationList);
            };

            experimentPanel.Children.Add(newButton);
        }

        private void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.StopSimulation();
            ResetStartSimulationButton();
            stopSimulationButton.IsEnabled = false;
            mutationCreationButton.IsEnabled = false;
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

        private void useSelectionCheckBox_Toggle(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleUseSelection();
        }

        private void showBlastCellCount_Toggle(object sender, RoutedEventArgs e)
        {
            SimulationCore.Instance.ToggleShowBlastCellCount();
        }

    }
}
