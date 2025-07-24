using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VSTAPP.Models;
using System.IO;

namespace VSTAPP.Views
{
    public partial class DesignationSelectionPage : UserControl
    {
        public event Action OnBackClicked;
        public event Action<TrainingSessionData> OnSOPSelected;

        private Dictionary<string, List<string>> designationsData;
        private List<SOPItem> sopData;
        private string selectedArea;
        private string selectedMine;

        public DesignationSelectionPage()
        {
            InitializeComponent();
        }

        public void Initialize(string area, string mine)
        {
            selectedArea = area;
            selectedMine = mine;
        }

        public void LoadData(DesignationSOPModel model)
        {
            designationsData = model.Designations;
            sopData = model.SOPs;

            // Populate designation dropdown
            DesignationComboBox.ItemsSource = designationsData.Keys.ToList();
        }

        private void DesignationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DesignationComboBox.SelectedItem is string selectedDesignation)
            {
                LoadSOPBlocks(selectedDesignation);
                SelectionStatusText.Text = $"Designation: {selectedDesignation} - Click on an SOP to start training";
            }
            else
            {
                ClearSOPBlocks();
                SelectionStatusText.Text = "Please select a designation first";
            }
        }

        private void LoadSOPBlocks(string designation)
        {
            SOPBlocksContainer.Children.Clear();
            SOPSelectionTitle.Visibility = Visibility.Visible;

            // Get SOPs available for the selected designation
            var availableSOPNames = designationsData[designation];

            foreach (var sopName in availableSOPNames)
            {
                // Find matching SOP in the SOPs list
                var sopItem = sopData.FirstOrDefault(s =>
                    s.name != null && (
                        s.name.Equals(sopName, StringComparison.OrdinalIgnoreCase) ||
                        s.name.Contains(sopName) ||
                        sopName.Contains(s.name)
                    ));

                string sopPath = sopItem?.path ?? "Path not configured";

                CreateSOPBlock(sopName, sopPath);
            }
        }

        private void CreateSOPBlock(string sopName, string sopPath)
        {
            var button = new Button
            {
                Style = (Style)this.Resources["SOPBlockStyle"],
                Width = 280,
                Height = 100,
                Tag = new { Name = sopName, Path = sopPath }
            };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            stackPanel.Children.Add(new TextBlock
            {
                Text = sopName,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = GetSOPDescription(sopName),
                FontSize = 12,
                FontStyle = FontStyles.Italic,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0),
                TextWrapping = TextWrapping.Wrap
            });

            // Add path indicator
            var pathIndicator = new TextBlock
            {
                Text = File.Exists(sopPath) ? "✓ Ready" : "⚠ Path not found",
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 3, 0, 0),
                Foreground = File.Exists(sopPath) ? System.Windows.Media.Brushes.LightGreen : System.Windows.Media.Brushes.Yellow
            };
            stackPanel.Children.Add(pathIndicator);

            button.Content = stackPanel;
            button.Click += SOPBlock_Click;

            SOPBlocksContainer.Children.Add(button);
        }

        private void SOPBlock_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                dynamic selectedSOP = button.Tag;

                // Create training session data
                var trainingData = new TrainingSessionData
                {
                    Area = selectedArea,
                    Mine = selectedMine,
                    Designation = DesignationComboBox.SelectedItem.ToString(),
                    SOPName = selectedSOP.Name,
                    SOPPath = selectedSOP.Path,
                    SessionStartTime = DateTime.Now
                };

                // Check if video file exists
                if (!File.Exists(selectedSOP.Path))
                {
                    var result = MessageBox.Show(
                        $"SOP video file not found at:\n{selectedSOP.Path}\n\nDo you want to continue anyway?",
                        "Video File Missing",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                        return;
                }

                // Trigger the SOP selection event
                OnSOPSelected?.Invoke(trainingData);
            }
        }

        private void ClearSOPBlocks()
        {
            SOPBlocksContainer.Children.Clear();
            SOPSelectionTitle.Visibility = Visibility.Collapsed;
        }

        private string GetSOPDescription(string sopName)
        {
            var descriptions = new Dictionary<string, string>
            {
                {"SOP1", "Drilling Operations Safety"},
                {"SOP2", "Equipment Maintenance"},
                {"Manager", "Management Protocols"},
                {"Loader Operator", "Loading Equipment Operation"},
                {"Drill Operator", "Drilling Equipment Operation"},
                {"Crusher Operator", "Crushing Equipment Operation"},
                {"Safety Officer", "Safety Inspection Procedures"},
                {"First Aid Responder", "Emergency Response Procedures"},
                {"Blasting SOP", "Blasting Safety Procedures"},
                {"Maintenance SOP", "Equipment Maintenance Guidelines"},
                {"Safety SOP", "General Safety Protocols"}
            };

            return descriptions.ContainsKey(sopName) ? descriptions[sopName] : "Standard Operating Procedure";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackClicked?.Invoke();
        }
    }
}