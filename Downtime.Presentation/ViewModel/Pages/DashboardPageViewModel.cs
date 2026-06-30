using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Downtime.Presentation.ViewModel.Pages
{
    internal class DashboardPageViewModel : INotifyPropertyChanged
    {
        public TimeSpan SelectedTime { get { return TimeSpan.Zero; } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
