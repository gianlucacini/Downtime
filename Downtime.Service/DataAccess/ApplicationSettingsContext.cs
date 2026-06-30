using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Downtime.Common;
using Downtime.Service.Events;

namespace Downtime.Service.DataAccess
{
    public class ApplicationSettingsContext
    {
        public ApplicationSettingsContext()
        {
            String dataAccessFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _fileConfigFullPath = System.IO.Path.Combine(dataAccessFolder, "Downtime.Data.xml");
        }

        public EventHandler<ApplicationSettingsChanged> OnApplicationSettingsChanged;

        private ApplicationSettings _applicationSettings;

        private string _fileConfigFullPath;

        private Boolean _fileConfigOpened = false;

        private System.IO.FileStream _fileConfig;

        void KeepFileOpen() => ToggleFileConfigOpen(FileConfigAction.Open);
        
        void CloseFile() => ToggleFileConfigOpen(FileConfigAction.Close);
        
        public ApplicationSettings Get()
        {
            return _applicationSettings;
        }

        ApplicationSettings RefreshSettings()
        {
            CloseFile();

            GeneralSettings generalSettings = new GeneralSettings();
            generalSettings.HardBlock = GetValue<bool>(nameof(GeneralSettings), "HardBlock");
            generalSettings.TimeZoneId = GetValue<string>(nameof(GeneralSettings), "TimeZoneId");


            AppBlockerModeSettings appBlockerMode = new AppBlockerModeSettings();

            appBlockerMode.ListOfAppExes = GetValue<String>(nameof(AppBlockerModeSettings), "ListOfAppExes")
                .Split(',')
                .ToList();

            appBlockerMode.From = GetValue<String>(nameof(AppBlockerModeSettings), "From");
            appBlockerMode.To = GetValue<String>(nameof(AppBlockerModeSettings), "To");

            _applicationSettings = new ApplicationSettings()
            {
                GeneralSettings = generalSettings,
                AppBlockerMode = appBlockerMode
            };

            KeepFileOpen();

            return Get();
        }

        public void SaveGeneralSettings(GeneralSettings settings)
        {
            CloseFile();

            SetValue(nameof(GeneralSettings), nameof(GeneralSettings.TimeZoneId), settings.TimeZoneId);

            SetValue(nameof(GeneralSettings), nameof(GeneralSettings.HardBlock), settings.HardBlock == true ? "1" : "0");

            KeepFileOpen();

            RefreshSettings();

            OnApplicationSettingsChanged?.Invoke(this, new ApplicationSettingsChanged(settings));
        }

        public void SaveAppBlockerModeSettings(AppBlockerModeSettings settings)
        {
            CloseFile();

            SetValue(nameof(AppBlockerModeSettings), nameof(AppBlockerModeSettings.ListOfAppExes), string.Join(",",settings.ListOfAppExes));
            SetValue(nameof(AppBlockerModeSettings), nameof(AppBlockerModeSettings.From), settings.From);
            SetValue(nameof(AppBlockerModeSettings), nameof(AppBlockerModeSettings.To), settings.To);
            SetValue(nameof(AppBlockerModeSettings), nameof(AppBlockerModeSettings.Enabled), settings.Enabled == true ? "1" : "0");

            KeepFileOpen();

            RefreshSettings();

            OnApplicationSettingsChanged?.Invoke(this, new ApplicationSettingsChanged(settings));
        }

        void ToggleFileConfigOpen(FileConfigAction fileConfigAction)
        {
            if (fileConfigAction == FileConfigAction.Close)
            {
                //close file config
                if (_fileConfigOpened == true && _fileConfig != null)
                {
                    _fileConfig.Close();
                    _fileConfigOpened = false;
                }
            }
            else
            {
                //open file config
                _fileConfig = System.IO.File.Open(_fileConfigFullPath, System.IO.FileMode.Open);
                _fileConfigOpened = true;
            }
        }

        public T GetValue<T>(string section, string key)
        {
            var xml = XDocument.Load(_fileConfigFullPath);
            var sectionElement = xml.Root?.Element(section);
            if (sectionElement == null)
                return default;

            var valueElement = sectionElement.Element(key);
            if (valueElement == null || string.IsNullOrWhiteSpace(valueElement.Value))
                return default;

            return (T)Convert.ChangeType(valueElement.Value, typeof(T));
        }

        public void SetValue(string section, string key, object value)
        {
            var xml = System.IO.File.Exists(_fileConfigFullPath)
                ? XDocument.Load(_fileConfigFullPath)
                : new XDocument(new XElement("Settings"));

            var root = xml.Root ?? new XElement("Settings");

            var sectionElement = root.Element(section);
            if (sectionElement == null)
            {
                sectionElement = new XElement(section);
                root.Add(sectionElement);
            }

            var keyElement = sectionElement.Element(key);
            if (keyElement != null)
                keyElement.Value = value?.ToString() ?? "";
            else
                sectionElement.Add(new XElement(key, value?.ToString() ?? ""));

            xml.Save(_fileConfigFullPath);
        }
    }

    enum FileConfigAction
    {
        Open,
        Close
    }
}
