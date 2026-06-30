using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AppSelectionPage.xaml
    /// </summary>
    public partial class AppSelectionPage : Page
    {
        private ObservableCollection<AppItem> apps;

        private CollectionViewSource appsViewSource;

        public AppSelectionPage()
        {
            InitializeComponent();
            LoadMockApps();
            
            // Set up filtering
            appsViewSource = new CollectionViewSource();
            appsViewSource.Source = apps;
            appsViewSource.Filter += AppsViewSource_Filter;
            
            AppsListControl.ItemsSource = appsViewSource.View;
        }

        private void AppsViewSource_Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text) || SearchTextBox.Text == "Search applications...")
            {
                e.Accepted = true;
                return;
            }

            AppItem app = e.Item as AppItem;
            if (app != null)
            {
                string searchText = SearchTextBox.Text.ToLower();
                e.Accepted = app.Name.ToLower().Contains(searchText) || 
                            app.Path.ToLower().Contains(searchText);
            }
        }

        private void LoadMockApps()
        {
            apps = new ObservableCollection<AppItem>
            {
                new AppItem { Name = "Steam", Path = "C:\\Program Files (x86)\\Steam\\steam.exe", IsSelected = false },
                new AppItem { Name = "Epic Games Launcher", Path = "C:\\Program Files (x86)\\Epic Games\\Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe", IsSelected = false },
                new AppItem { Name = "Discord", Path = "C:\\Users\\User\\AppData\\Local\\Discord\\app-1.0.9002\\Discord.exe", IsSelected = false },
                new AppItem { Name = "Valorant", Path = "C:\\Riot Games\\VALORANT\\live\\VALORANT.exe", IsSelected = false },
                new AppItem { Name = "Counter-Strike 2", Path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Counter-Strike Global Offensive\\game\\bin\\win64\\cs2.exe", IsSelected = false },
                new AppItem { Name = "League of Legends", Path = "C:\\Riot Games\\League of Legends\\Game\\League of Legends.exe", IsSelected = false },
                new AppItem { Name = "Minecraft", Path = "C:\\Users\\User\\AppData\\Roaming\\.minecraft\\Minecraft.exe", IsSelected = false },
                new AppItem { Name = "Battle.net", Path = "C:\\Program Files (x86)\\Battle.net\\Battle.net Launcher.exe", IsSelected = false },
                new AppItem { Name = "Overwatch 2", Path = "C:\\Program Files (x86)\\Overwatch\\_retail_\\Overwatch.exe", IsSelected = false },
                new AppItem { Name = "Call of Duty: Warzone", Path = "C:\\Program Files\\Call of Duty\\Modern Warfare\\ModernWarfare.exe", IsSelected = false },
                new AppItem { Name = "Apex Legends", Path = "C:\\Program Files (x86)\\Origin Games\\Apex\\r5apex.exe", IsSelected = false },
                new AppItem { Name = "Fortnite", Path = "C:\\Program Files\\Epic Games\\Fortnite\\FortniteClient-Win64-Shipping.exe", IsSelected = false }
            };
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search applications...")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search applications...";
                SearchTextBox.Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (appsViewSource != null)
            {
                appsViewSource.View.Refresh();
            }
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var app in apps)
            {
                app.IsSelected = true;
            }
            UpdateSelectedCount();
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var app in apps)
            {
                app.IsSelected = false;
            }
            UpdateSelectedCount();
        }

        private void AppCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSelectedCount();
        }

        private void AppCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            int count = apps.Count(a => a.IsSelected);
            SelectedCountText.Text = $"Selected: {count}";
        }

        private void AppItem_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                border.Background = (SolidColorBrush)FindResource("SelectedBrush");
            }
        }

        private void AppItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                border.Background = Brushes.Transparent;
            }
        }
    }

    public class AppItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsSelected { get; set; }
    }
}

