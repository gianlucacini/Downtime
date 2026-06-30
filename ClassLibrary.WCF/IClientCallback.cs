using System.ServiceModel;

namespace ClassLibrary.WCF
{
    [ServiceContract]
    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)] 
        void SendMessageToClient(string message);
    }
}