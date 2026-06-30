using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Downtime.Common;

namespace Downtime.Service.Events
{
    public class ApplicationSettingsChanged : EventArgs
    {
        public ApplicationSettingsChanged(GeneralSettings generalSettings)
        {
            GeneralSettings = generalSettings;
        }

        public GeneralSettings GeneralSettings { get; private set; }

        public ApplicationSettingsChanged(AppBlockerModeSettings appBlockerModeSettings)
        {
            AppBlockerModeSettings = appBlockerModeSettings;
        }

        public AppBlockerModeSettings AppBlockerModeSettings { get; private set; }
    }
}
