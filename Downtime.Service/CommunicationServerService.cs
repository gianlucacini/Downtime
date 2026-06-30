using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Downtime.Common;

namespace Downtime.Service
{
    partial class CommunicationServerService : ServiceBase
    {
        public CommunicationServerService(ILogger logger)
        {
            InitializeComponent();

            _logger = logger;

        }

        private readonly ILogger _logger;
        static ServiceHost _host = null;

        internal void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            var baseAddress = new Uri("net.pipe://localhost/UnplugServiceDontEvenWorryAboutIt");

            _host = new ServiceHost(typeof(ComunicationServiceContract), baseAddress);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

            _host.AddServiceEndpoint(typeof(IComunicationServiceContract), binding, "");

            _host.Open();
        }

        protected override void OnStop()
        {
            _host?.Close();
        }
    }
}
