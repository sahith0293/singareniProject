using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using VSTAPP.Models;
using VSTAPP.Views;
using System;
using System.Collections.Generic;


namespace VSTAPP
{
    public partial class MainWindow : Window
    {
        private string selectedArea;
        private string selectedMine;
        private string selectedDesignation;
        private string selectedSOP;

        public MainWindow()
        {
            InitializeComponent();
            ShowWelcomePage();
        }

        private void ShowWelcomePage()
        {
            var welcomePage = new WelcomePage();
            welcomePage.OnStartClicked += ShowAreaSelectionPage;
            welcomePage.OnWatchRecordsClicked += ShowRecordings;
            MainGrid.Children.Clear();
            MainGrid.Children.Add(welcomePage);
        }

        private void ShowAreaSelectionPage()
        {
            var areaSelectionPage = new AreaSelectionPage();
            areaSelectionPage.OnNextClicked += (area, mine) =>
            {
                selectedArea = area;
                selectedMine = mine;
                ShowDesignationSelectionPage();
            };
            areaSelectionPage.OnBackClicked += ShowWelcomePage;

            try
            {
                var json = File.ReadAllText("Config/areas_mines.json");
                var model = JsonConvert.DeserializeObject<AreaMineModel>(json);
                areaSelectionPage.LoadData(model);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"డేటా లోడ్ చేయడంలో లోపం: {ex.Message}", "లోపం",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MainGrid.Children.Clear();
            MainGrid.Children.Add(areaSelectionPage);
        }

        private void ShowDesignationSelectionPage()
        {
            var designationPage = new DesignationSelectionPage();
            designationPage.Initialize(selectedArea, selectedMine);
            designationPage.OnBackClicked += ShowAreaSelectionPage;
            designationPage.OnSOPSelected += (trainingData) =>
            {
                var message = $"Training Session Details:\n\n" +
                             $"Area: {trainingData.Area}\n" +
                             $"Mine: {trainingData.Mine}\n" +
                             $"Designation: {trainingData.Designation}\n" +
                             $"SOP: {trainingData.SOPName}\n\n" +
                             $"Ready to start training session?";

                var result = MessageBox.Show(message, "Start Training Session",
                                           MessageBoxButton.OKCancel,
                                           MessageBoxImage.Information);

                if (result == MessageBoxResult.OK)
                {
                    StartSOPTrainingSession(trainingData);
                }
            };

            try
            {
                // Simple file path - check multiple locations
                string json = "";
                string[] possiblePaths = {
                    "Config/designations_sops.json",
                    "designations_sops.json",

                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "designations_sops.json"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "designations_sops.json")

                };
        

                string usedPath = "";
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        json = File.ReadAllText(path);
                        usedPath = path;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(json))
                {
                    // Create default data if file doesn't exist
                    var defaultData = new DesignationSOPModel
                    {
                        Designations = new Dictionary<string, List<string>>
                {
                    {"Engineer", new List<string> {"SOP1", "SOP2", "Manager"}},
                    {"Operator", new List<string> {"Loader Operator", "Drill Operator", "Crusher Operator"}},
                    {"Safety", new List<string> {"Safety Officer", "First Aid Responder"}}
                },
                        SOPs = new List<SOPItem>
                        {
                            new SOPItem {name = "SOP1", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "Sop_Video1.mp4")},
                            new SOPItem {name = "SOP2", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "Sop_Video2.mp4")},
                            new SOPItem {name = "Manager", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "Management.mp4")},
                            new SOPItem {name = "Loader Operator", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "LoaderOperation.mp4")},
                            new SOPItem {name = "Drill Operator", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "DrillOperation.mp4")},
                            new SOPItem {name = "Crusher Operator", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "CrusherOperation.mp4")},
                            new SOPItem {name = "Safety Officer", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "SafetyInspection.mp4")},
                            new SOPItem {name = "First Aid Responder", path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Sop_Videos", "FirstAid.mp4")}
                        }

                    };

                    designationPage.LoadData(defaultData);

                    MessageBox.Show($"Config file not found. Using default data.\n\nSearched paths:\n{string.Join("\n", possiblePaths)}",
                                   "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var model = JsonConvert.DeserializeObject<DesignationSOPModel>(json);
                    designationPage.LoadData(model);

                    MessageBox.Show($"Data loaded successfully from: {usedPath}", "Success",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error details: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                               "Detailed Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MainGrid.Children.Clear();
            MainGrid.Children.Add(designationPage);
        }
        private void StartSOPTrainingSession(TrainingSessionData trainingData)
        {
            selectedDesignation = trainingData.Designation;
            selectedSOP = trainingData.SOPName;

            // Path to instruction video - FIXED PATH
            string instructionVideoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos", "Instruction_videos", "instruction_video.mp4");

            // Debug: Show the actual path being checked
            MessageBox.Show($"Looking for instruction video at:\n{instructionVideoPath}", "Debug Path", MessageBoxButton.OK, MessageBoxImage.Information);

            // Check if instruction video exists
            if (!File.Exists(instructionVideoPath))
            {
                MessageBox.Show($"Instruction video not found at:\n{instructionVideoPath}\n\nPlease place instruction_video.mp4 in Videos/Instruction_videos/ folder",
                               "Instruction Video Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // First show instruction video page
            var instructionPage = new InstructionVideoPage();
            instructionPage.OnBackClicked += ShowDesignationSelectionPage;
            instructionPage.OnInstructionComplete += () => {
                // After instruction video, show SOP with webcam
                ShowSOPWithWebcam(trainingData);
            };

            instructionPage.StartInstructionVideo(trainingData, instructionVideoPath);

            MainGrid.Children.Clear();
            MainGrid.Children.Add(instructionPage);
        }
        private void ShowSOPWithWebcam(TrainingSessionData trainingData)
        {
            var sopWebcamPage = new SOPWebcamPage();
            sopWebcamPage.OnBackClicked += ShowDesignationSelectionPage;
            sopWebcamPage.OnTrainingComplete += () => {
                MessageBox.Show($"Training session completed!\n\nSession ID: {trainingData.SessionId}\nRecording saved.",
                               "Training Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowWelcomePage();
            };

            sopWebcamPage.StartSOPWithWebcam(trainingData);

            MainGrid.Children.Clear();
            MainGrid.Children.Add(sopWebcamPage);
        }
        private void ShowRecordings()
        {
            MessageBox.Show("రికార్డుల వీక్షణ ఫంక్షనాలిటీ అమలు చేయబడుతుంది.", "రికార్డులు",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}