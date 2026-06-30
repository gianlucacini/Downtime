using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Downtime.Common;
using Downtime.Service.DataAccess;

namespace Downtime.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ComunicationServiceContract : IComunicationServiceContract
    {
        public ComunicationServiceContract(ApplicationSettingsContext applicationSettingsContext)
        {
            _applicationSettingsContext = applicationSettingsContext;
        }

        private readonly ApplicationSettingsContext _applicationSettingsContext;
      
        public void SendMessageToService(string message)
        {
            System.Console.WriteLine($"Received from client: {message}");

            // Example: send a response back to client
            var callback = OperationContext.Current.GetCallbackChannel<IClientCallbackContract>();
            callback.SendMessageToClient("Got your message: " + message);
        }

        public async Task SaveAppBlockerModeConfig(AppBlockerModeSettings appBlockerModeSettings)
        {
            await Task.Run(() =>
            {
                _applicationSettingsContext.SaveAppBlockerModeSettings(appBlockerModeSettings);
            });
        }

        public async Task SaveGeneralSettings(GeneralSettings generalSettings)
        {
            await Task.Run(() =>
            {
                _applicationSettingsContext.SaveGeneralSettings(generalSettings);
            });
        }
    }
}
