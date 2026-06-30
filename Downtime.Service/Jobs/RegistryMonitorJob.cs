using Serilog;
using Downtime.Service.BusinessLogic;
using System;

namespace Downtime.Service.Jobs
{
    internal class RegistryMonitorJob
    {
        internal RegistryMonitorJob(ILogger logger)
        {
            _logger = logger;
        }

        private RegistryMonitor RegMonitor = null;
        private readonly ILogger _logger;
        private string serviceName = "Downtime Service";
        private string ActualServicePath = "";

        void RestoreDefaultSettings()
        {
             _logger.Information("Restoring Default Registry Settings");

            //open registry and read metadata of installed service

            //service user changed

            string objectName = GetRegistryKey<string>(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ObjectName");

            if (objectName != "LocalSystem")
            {
                _logger.Information($"Restoring Registry Default Settings: Changing {objectName} back to LocalSystem");

                SetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ObjectName", "LocalSystem");
            }

            //service start type

            int startType = GetRegistryKey<int>(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "Start");

            if (startType != 2)
            {
                _logger.Information($"Restoring Registry Default Settings: Changing {startType} start type back to 2");

                SetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "Start", 2);

            }

            //service path changed

            string modifiedServicePath = GetRegistryKey<string>(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ImagePath");

            if (modifiedServicePath != ActualServicePath)
            {
                _logger.Information($"Restoring Registry Default Settings: Changing {objectName} back to LocalSystem");

                SetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ImagePath", ActualServicePath);
            }
        }


        internal void Stop()
        {
            _logger.Information("Stopping Registry Monitor");

            if (RegMonitor == null || RegMonitor.IsMonitoring == false)
            {
                _logger.Information("Registry Monitor is null or already not monitoring");

                return;
            }

            RegMonitor.Stop();

            RestoreDefaultSettings();

            _logger.Information($"Registry monitor is monitoring = {RegMonitor.IsMonitoring}");
        }

        internal void Start()
        {
            _logger.Information("Starting Registry Monitor");

            Microsoft.Win32.RegistryKey browserKeys = null;
            try
            {
                browserKeys = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, true);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while starting the registry monitor");
                return;
            }

            if (browserKeys == null)
            {
                _logger.Error($"No key found for service named {serviceName}");
                return;
            }

            ActualServicePath = GetRegistryKey<string>(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ImagePath");

            RegMonitor = new RegistryMonitor(browserKeys);
            RegMonitor.Error += RegMonitor_Error;
            RegMonitor.RegChanged += RegMonitor_RegChanged;
            RegMonitor.RegChangeNotifyFilter = RegChangeNotifyFilter.Value;
            RegMonitor.Start();

            _logger.Information($"Registry monitor is monitoring = {RegMonitor.IsMonitoring}");
        }

        private void RegMonitor_RegChanged(Object sender, EventArgs e)
        {
            _logger.Information("Registry Key Change Detected For Service " + serviceName);

            RestoreDefaultSettings();
        }

        private void RegMonitor_Error(Object sender, System.IO.ErrorEventArgs e)
        {
            _logger.Error(e.GetException(), "An error occurred while executing the registry monitor");
        }

        T GetRegistryKey<T>(String subKey, String keyName)
        {
            Microsoft.Win32.RegistryKey browserKeys = Microsoft.Win32.Registry
                   .LocalMachine.OpenSubKey(subKey, true);

            if (browserKeys is null)
            {
                _logger.Error($"GetRegistryKey Error: while trying to retrieve a registry key in HKLM: subkey = {subKey}, keyname = {keyName}. key is null");
                return default(T);
            }

            return (T)browserKeys.GetValue(keyName);
        }


        /// <summary>
        /// Update Or Create a new registry key-value pair
        /// </summary>
        /// <param name="subKey">Ex: SOFTWARE\StrictParent </param>
        /// <param name="keyName">registry key name</param>
        /// <param name="keyValue">registry key value</param>
        void SetRegistryKey(String subKey, String keyName, Object keyValue)
        {
            Microsoft.Win32.RegistryKey browserKeys = null;
            browserKeys = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKey, true);

            if (browserKeys == null)
            {
                _logger.Error($"SetRegistryKey Error: while trying to open a subkey in HKLM: subkey = {subKey}, keyname = {keyName}, keyvalue = {keyValue}. key does not exist.");

                Microsoft.Win32.Registry.LocalMachine.CreateSubKey(subKey, true);

                browserKeys = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKey, true);

                browserKeys.SetValue(keyName, keyValue);

                browserKeys.Close();

            }
            else
            {
                browserKeys.SetValue(keyName, keyValue);
                browserKeys.Close();
            }
        }
    }
}