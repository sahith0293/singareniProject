using System;
using System.Windows;
using System.Windows.Controls;

namespace VSTAPP.Views
{
    public partial class WelcomePage : UserControl
    {
        public event Action OnStartClicked;
        public event Action OnWatchRecordsClicked;

        public WelcomePage()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            OnStartClicked?.Invoke();
        }

        private void WatchRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            OnWatchRecordsClicked?.Invoke();
        }
    }
}