using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using ClassLibrary.WCF;

namespace Console.WCF
{
    internal class Program
    {

        static ServiceHost _host = null;
        static void Main(string[] args)
        {
            var baseAddress = new Uri("net.pipe://localhost/UnplugServiceDontEvenWorryAboutIt");

            _host = new ServiceHost(typeof(MessageService), baseAddress);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

            _host.AddServiceEndpoint(typeof(IMessageService), binding, "");

            _host.Open();

            System.Console.WriteLine("Service is running using Named Pipes...");

            Thread.Sleep(-1);
        }
    }
}
