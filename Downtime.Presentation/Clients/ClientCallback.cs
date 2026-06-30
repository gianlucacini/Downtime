using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Downtime.Common;

namespace Downtime.Presentation.Clients
{
    public class ClientCallback : IClientCallbackContract
    {
        public void SendMessageToClient(string message)
        {
            System.Console.WriteLine($"[Server -> Client] {message}");
        }
    }
}
