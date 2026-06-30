using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Downtime.Presentation.Clients;
using Downtime.Presentation.ViewModel.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Downtime.Presentation
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            this.ExtendsContentIntoTitleBar = true;
            InitializeComponent();
        }
      

        private void SelectApps_Button_Click(object sender, RoutedEventArgs e)
        {
            Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox()
            {
                Title = "Save your work?",
                Content = "Save your Project and all your setting?",
                CloseButtonText = "Cancel",
                SecondaryButtonText = "Don't Save",
                PrimaryButtonText = "Save"
            };

            var result = messageBox.ShowDialogAsync();

        }

      
    }
}
