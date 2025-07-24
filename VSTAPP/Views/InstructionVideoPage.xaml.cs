using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VSTAPP.Models;

namespace VSTAPP.Views
{
    public partial class InstructionVideoPage : UserControl
    {
        public event Action OnBackClicked;
        public event Action OnInstructionComplete;

        private TrainingSessionData sessionData;
        private string instructionVideoPath;

        public InstructionVideoPage()
        {
            InitializeComponent();
        }

        public void StartInstructionVideo(TrainingSessionData data, string instructionPath)
        {
            sessionData = data;
            instructionVideoPath = instructionPath;

            if (File.Exists(instructionVideoPath))
            {
                LoadingText.Visibility = Visibility.Collapsed;
                InstructionPlayer.Source = new Uri(Path.GetFullPath(instructionVideoPath));
                InstructionPlayer.Play();
            }
            else
            {
                LoadingText.Text = $"Instruction video not found:\n{instructionVideoPath}";
                LoadingText.Visibility = Visibility.Visible;
            }
        }

        private void InstructionPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Instruction video completed, proceed to SOP with webcam
            ProceedToSOP();
        }

        private void ProceedToSOP()
        {
            InstructionPlayer.Stop();
            OnInstructionComplete?.Invoke();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            InstructionPlayer.Stop();
            ProceedToSOP();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            InstructionPlayer.Stop();
            OnBackClicked?.Invoke();
        }
    }
}