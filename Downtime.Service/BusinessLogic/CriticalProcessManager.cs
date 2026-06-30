using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Downtime.Common;
using Downtime.Common.Models;
using Downtime.Service.DataAccess;

namespace Downtime.Service.BusinessLogic
{
    public class CriticalProcessManager
    {
        public CriticalProcessManager(ILogger logger, ApplicationSettingsContext applicationSettingsContext)
        {
            _logger = logger;
            _applicationSettingsContext = applicationSettingsContext;
        }

        private readonly ApplicationSettingsContext _applicationSettingsContext;
        private ILogger _logger;
        [System.Runtime.InteropServices.DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);

        public void StatusChanged()
        {
            var settings = _applicationSettingsContext.Get();

            if (settings.AnyModeIsRunning)
            {
                SetProcessAsCritical(settings);
            }
            else
            {
                SetProcessAsNotCritical(settings);
            }
        }
        private Boolean ProcessIsCritical = false;

        private void SetProcessAsCritical(ApplicationSettings settings)
        {
            if (ProcessIsCritical == false)
            {
                ProcessIsCritical = true;

                _logger.Information("PROCESS SET AS CRITICAL");
#if !DEBUG
                System.Diagnostics.Process.EnterDebugMode();
                RtlSetProcessIsCritical(1, 0, 0);
#endif
            }
        }

        /// <summary>
        /// renders the application stoppable and uninstallable
        /// </summary>
        /// <param name="settings"></param>
        private void SetProcessAsNotCritical(ApplicationSettings settings)
        {
            if (ProcessIsCritical)
            {
                ProcessIsCritical = false;

                _logger.Information("PROCESS SET AS NOT CRITICAL");

#if !DEBUG
                RtlSetProcessIsCritical(0, 0, 0);
#endif
            }
        }
    }
}
