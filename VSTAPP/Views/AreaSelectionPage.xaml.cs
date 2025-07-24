using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VSTAPP.Models;


namespace VSTAPP.Views
{
    public partial class AreaSelectionPage : UserControl
    {
        public event Action<string, string> OnNextClicked;
        public event Action OnBackClicked;
        private Dictionary<string, List<string>> areasData;

        public AreaSelectionPage()
        {
            InitializeComponent();
        }

        public void LoadData(AreaMineModel model)
        {
            areasData = model.Areas;
            AreaListBox.ItemsSource = areasData.Keys;
        }

        private void AreaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AreaListBox.SelectedItem is string selectedArea)
            {
                MineListBox.ItemsSource = areasData[selectedArea];
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackClicked?.Invoke();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreaListBox.SelectedItem is string area && MineListBox.SelectedItem is string mine)
            {
                OnNextClicked?.Invoke(area, mine);
            }
            else
            {
                MessageBox.Show("దయచేసి ప్రాంతం మరియు గని రెండింటినీ ఎంచుకోండి.", "ఎంపిక అవసరం",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}