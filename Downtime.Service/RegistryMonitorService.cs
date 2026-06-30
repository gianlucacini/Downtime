using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Downtime.Common;
using Downtime.Service.BusinessLogic;
using Downtime.Service.DataAccess;
using Downtime.Service.Events;
using Downtime.Service.Jobs;

namespace Downtime.Service
{
    partial class RegistryMonitorService : ServiceBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettingsContext _applicationSettingsContext;
        private readonly RegistryMonitorJob _registryMonitorJob;

        private Thread _workerThread;
        private readonly ManualResetEvent _settingsChangedEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);

        public RegistryMonitorService(
            ILogger logger,
            ApplicationSettingsContext applicationSettingsContext,
            RegistryMonitorJob registryMonitorJob)
        {
            InitializeComponent();
            _applicationSettingsContext = applicationSettingsContext;
            _logger = logger;
            _registryMonitorJob = registryMonitorJob;
        }

        internal void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            _logger.Information("OnStart Called");

            _workerThread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = nameof(RegistryMonitorService) + "Worker"
            };

            _workerThread.Start();

            _applicationSettingsContext.OnApplicationSettingsChanged += OnApplicationSettingsChanged;
        }

        private void WorkerLoop()
        {
            // Wait handles: 0 = settings changed, 1 = stop requested
            WaitHandle[] waitHandles = { _settingsChangedEvent, _stopEvent };

            while (true)
            {
                _registryMonitorJob.Stop();

                var appSettings = _applicationSettingsContext.Get();

                if (appSettings.GeneralSettings.HardBlock == true)
                {
                    try
                    {
                        _registryMonitorJob.Start();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, ex.Message);
                        // log error, but keep the loop alive
                        // e.g. EventLog.WriteEntry(ServiceName, ex.ToString(), EventLogEntryType.Error);
                    }
                }

                // 3) Reset the settings event so we’ll block until the next change
                _settingsChangedEvent.Reset();

                // 4) Wait for either a change or a stop request
                int signaled = WaitHandle.WaitAny(waitHandles);
                if (signaled == 1)    // _stopEvent
                    break;            // service is stopping
                                      // if signaled==0, loop again to re-load settings
            }
        }  

        private void OnApplicationSettingsChanged(object sender, ApplicationSettingsChanged e)
        {
            _settingsChangedEvent.Set();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            _logger.Information($"OnSessionChange Called, Reason = {changeDescription.Reason}");

            base.OnSessionChange(changeDescription);
        }

        protected override Boolean OnPowerEvent(PowerBroadcastStatus powerStatus)
        {

            switch (powerStatus)
            {
                case PowerBroadcastStatus.OemEvent:
                case PowerBroadcastStatus.Suspend:
                case PowerBroadcastStatus.ResumeSuspend:
                case PowerBroadcastStatus.QuerySuspend:
                case PowerBroadcastStatus.QuerySuspendFailed:
                case PowerBroadcastStatus.ResumeAutomatic:
                case PowerBroadcastStatus.ResumeCritical:

                    _logger.Information($"OnPowerEvent Called, PowerStatus = {powerStatus}");

                    break;
                default:
                    break;
            }

            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnShutdown()
        {
            _logger.Information("OnShutdown Called");

            StopService();

            base.OnShutdown();
        }

        protected override void OnStop()
        {
            _logger.Information("OnStop Called");

            StopService();

            base.OnStop();
        }

        void StopService()
        {
            // Signal the worker to exit
            _stopEvent.Set();

            // Unsubscribe and wait for thread to end
            _applicationSettingsContext.OnApplicationSettingsChanged -= OnApplicationSettingsChanged;
            if (!_workerThread.Join(TimeSpan.FromSeconds(5)))
            {
                _workerThread.Abort();    // last resort
            }

            _registryMonitorJob.Stop();
        }
    }
}
