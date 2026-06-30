using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downtime.Common 
{
    public class ApplicationSettings
    {
        public ApplicationSettings() 
        {
            GeneralSettings = new GeneralSettings();
            AppBlockerMode = new AppBlockerModeSettings();
        }

        public AppBlockerModeSettings AppBlockerMode;
        public GeneralSettings GeneralSettings;

        public bool AnyModeIsRunning 
        {
            get
            {
                return AppBlockerMode.Enabled; //AirplaneMode.Enabled || LockDownMode.Enabled;
            }
        }
    }

    public class GeneralSettings
    {
        public bool HardBlock { get; set; } = false;
        public string TimeZoneId { get; set; } = TimeZoneInfo.Local.Id;
    }

    public class AirplaneModeSettings
    {
        public bool Enabled { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    public class LockDownModeSettings
    {
        public bool Enabled { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    public class AppBlockerModeSettings
    {
        public List<string> ListOfAppExes { get; set; } = new List<string>();
        public bool Enabled { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
