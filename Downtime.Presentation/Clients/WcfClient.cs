using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Downtime.Common;

namespace Downtime.Presentation.Clients
{
    public class WcfClient
    {
        public WcfClient() 
        {
            var callbackInstance = new InstanceContext(new ClientCallback());
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var endpoint = new EndpointAddress("net.pipe://localhost/UnplugServiceDontEvenWorryAboutIt");

            _factory = new DuplexChannelFactory<IComunicationServiceContract>(callbackInstance, binding, endpoint);
            _proxy = _factory.CreateChannel();

            System.Console.WriteLine("Client connected.");
        }
        
        private IComunicationServiceContract _proxy;
        private DuplexChannelFactory<IComunicationServiceContract> _factory;

        public async Task SaveGeneralSettings(GeneralSettings generalConfig)
        {
            await _proxy.SaveGeneralSettings(generalConfig);
        }
        public async Task SaveAppBlockerModeConfig(AppBlockerModeSettings appBlockerModeSettings)
        {
            await _proxy.SaveAppBlockerModeConfig(appBlockerModeSettings);
        }

        public void Stop()
        {
            if (_factory != null)
            {
                try { _factory.Close(); }
                catch { _factory.Abort(); }
            }
        }
    }
}
