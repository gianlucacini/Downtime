using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Downtime.Presentation.ViewModel.Windows
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public string ApplicationTitle { get { return "App Title"; } }
        public TimeSpan SelectedTime { get { return TimeSpan.Zero; } }

      
        public ObservableCollection<NavigationViewItem> MenuItems { get { return _menuItems; } }

        private ObservableCollection<NavigationViewItem> _menuItems = new ObservableCollection<NavigationViewItem>()
        {
            new NavigationViewItem()
            {
                Content = "Dashboard",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home16, FontSize = 22 },
                TargetPageType = typeof(Views.Pages.TestPage1),
                
            },
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings16, FontSize = 22 },
                TargetPageType = typeof(Views.Pages.TestPage2)
            }
        };


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
