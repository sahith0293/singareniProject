using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VSTAPP.Models;

namespace VSTAPP.Views
{
    public partial class SOPWebcamPage : UserControl
    {
        public event Action OnBackClicked;
        public event Action OnTrainingComplete;

        private TrainingSessionData sessionData;
        private bool isRecording = false;

        public SOPWebcamPage()
        {
            InitializeComponent();
        }

        public void StartSOPWithWebcam(TrainingSessionData data)
        {
            sessionData = data;
            SOPTitle.Text = $"SOP Training: {sessionData.SOPName}";

            // Start webcam recording
            StartWebcamRecording();

            // Start SOP video
            if (File.Exists(sessionData.SOPPath))
            {
                LoadingText.Visibility = Visibility.Collapsed;
                SOPPlayer.Source = new Uri(Path.GetFullPath(sessionData.SOPPath));
                SOPPlayer.Play();
                StatusText.Text = "🔴 Recording... SOP video playing";
            }
            else
            {
                LoadingText.Text = $"SOP video not found:\n{sessionData.SOPPath}";
                StatusText.Text = "🔴 Recording... No SOP video";
            }
        }

        private void StartWebcamRecording()
        {
            isRecording = true;
            WebcamStatus.Text = "🔴 RECORDING\nLive Feed";

            // TODO: Initialize webcam here
            // This is where you'd add webcam initialization code
            MessageBox.Show("Webcam recording started!\n\nWebcam feed will be shown in top-right corner.",
                           "Recording Started", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StopWebcamRecording()
        {
            isRecording = false;
            WebcamStatus.Text = "⏹ STOPPED\nRecording Saved";
            StatusText.Text = "Recording stopped and saved";

            // TODO: Stop webcam and save recording
            MessageBox.Show($"Recording saved!\n\nSession: {sessionData.SessionId}\nDuration: {DateTime.Now - sessionData.SessionStartTime:mm\\:ss}",
                           "Recording Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SOPPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            CompleteTraining();
        }

        private void CompleteTraining()
        {
            SOPPlayer.Stop();
            StopWebcamRecording();
            OnTrainingComplete?.Invoke();
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteTraining();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRecording)
            {
                var result = MessageBox.Show("Recording is in progress. Stop recording and go back?",
                                           "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SOPPlayer.Stop();
                    StopWebcamRecording();
                    OnBackClicked?.Invoke();
                }
            }
            else
            {
                OnBackClicked?.Invoke();
            }
        }
    }
}