using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ClassLibrary.WCF
{
   
    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IMessageService
    {
        [OperationContract(IsOneWay = true)]
        Task EnableAirplaneMode(String from, String to, String timeZoneId);

        [OperationContract(IsOneWay = true)]
        Task EnableLockdownMode(String from, String to, String timeZoneId);
    }
}
