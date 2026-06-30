using ClassLibrary.WCF;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
namespace Console.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class MessageService : IMessageService
    {
        public Task EnableAirplaneMode(string from, string to, string timeZoneId)
        {
            throw new NotImplementedException();
        }

        public Task EnableLockdownMode(string from, string to, string timeZoneId)
        {
            throw new NotImplementedException();
        }

        public void SendMessageToService(string message)
        {
            System.Console.WriteLine($"Received from client: {message}");

            // Example: send a response back to client
            var callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            callback.SendMessageToClient("Got your message: " + message);
        }
    }
}
