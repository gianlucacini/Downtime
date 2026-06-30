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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppSelectionPage appSelectionPage;
        private TimeSelectionPage timeSelectionPage;

        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize pages
            appSelectionPage = new AppSelectionPage();
            timeSelectionPage = new TimeSelectionPage();
            
            // Navigate to apps page by default
            ContentFrame.Navigate(appSelectionPage);
            UpdateNavigationButtons("apps");
        }

        private void BtnAppsPage_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(appSelectionPage);
            UpdateNavigationButtons("apps");
        }

        private void BtnTimePage_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(timeSelectionPage);
            UpdateNavigationButtons("time");
        }

        private void UpdateNavigationButtons(string activePage)
        {
            if (activePage == "apps")
            {
                BtnAppsPage.Style = (Style)FindResource("ModernButton");
                BtnTimePage.Style = (Style)FindResource("SecondaryButton");
            }
            else
            {
                BtnAppsPage.Style = (Style)FindResource("SecondaryButton");
                BtnTimePage.Style = (Style)FindResource("ModernButton");
            }
        }
    }
}
