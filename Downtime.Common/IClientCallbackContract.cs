using System.ServiceModel;

namespace Downtime.Common
{
    [ServiceContract]
    public interface IClientCallbackContract
    {
        [OperationContract(IsOneWay = true)] 
        void SendMessageToClient(string message);
    }
}