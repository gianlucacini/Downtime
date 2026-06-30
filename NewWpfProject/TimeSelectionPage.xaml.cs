using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewWpfProject
{
    /// <summary>
    /// Interaction logic for TimeSelectionPage.xaml
    /// </summary>
    public partial class TimeSelectionPage : Page
    {
        public TimeSelectionPage()
        {
            InitializeComponent();
            InitializeTimeSelectors();
            UpdateDurationDisplay();
        }

        private void InitializeTimeSelectors()
        {
            // Initialize hours (0-23)
            var hours = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i.ToString("00"));
            }
            StartHourComboBox.ItemsSource = hours;
            EndHourComboBox.ItemsSource = hours;

            // Initialize minutes (0-59, in 5-minute intervals)
            var minutes = new List<string>();
            for (int i = 0; i < 60; i += 5)
            {
                minutes.Add(i.ToString("00"));
            }
            StartMinuteComboBox.ItemsSource = minutes;
            EndMinuteComboBox.ItemsSource = minutes;

            // Set default values
            StartHourComboBox.SelectedIndex = 22; // 10 PM
            StartMinuteComboBox.SelectedIndex = 0;
            EndHourComboBox.SelectedIndex = 6; // 6 AM
            EndMinuteComboBox.SelectedIndex = 0;

            // Add event handlers
            StartHourComboBox.SelectionChanged += TimeSelection_Changed;
            StartMinuteComboBox.SelectionChanged += TimeSelection_Changed;
            EndHourComboBox.SelectionChanged += TimeSelection_Changed;
            EndMinuteComboBox.SelectionChanged += TimeSelection_Changed;
        }

        private void TimeSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            UpdateDurationDisplay();
        }

        private void UpdateDurationDisplay()
        {
            if (StartHourComboBox.SelectedItem == null || 
                StartMinuteComboBox.SelectedItem == null ||
                EndHourComboBox.SelectedItem == null || 
                EndMinuteComboBox.SelectedItem == null)
                return;

            string startHour = StartHourComboBox.SelectedItem.ToString();
            string startMinute = StartMinuteComboBox.SelectedItem.ToString();
            string endHour = EndHourComboBox.SelectedItem.ToString();
            string endMinute = EndMinuteComboBox.SelectedItem.ToString();

            int startTotalMinutes = int.Parse(startHour) * 60 + int.Parse(startMinute);
            int endTotalMinutes = int.Parse(endHour) * 60 + int.Parse(endMinute);

            int durationMinutes;
            if (endTotalMinutes > startTotalMinutes)
            {
                durationMinutes = endTotalMinutes - startTotalMinutes;
            }
            else
            {
                // Spans midnight
                durationMinutes = (24 * 60 - startTotalMinutes) + endTotalMinutes;
            }

            int hours = durationMinutes / 60;
            int minutes = durationMinutes % 60;

            DurationText.Text = $"{startHour}:{startMinute} - {endHour}:{endMinute} ({hours}h {minutes}m)";
        }

        private void SaveSchedule_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Schedule saved successfully!\n\n" +
                $"Block will be active from {StartHourComboBox.SelectedItem}:{StartMinuteComboBox.SelectedItem} " +
                $"to {EndHourComboBox.SelectedItem}:{EndMinuteComboBox.SelectedItem}",
                "Schedule Saved",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}

