using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Downtime.Common
{
   
    [ServiceContract(CallbackContract = typeof(IClientCallbackContract))]
    public interface IComunicationServiceContract
    {
        [OperationContract(IsOneWay = true)]
        Task SaveAppBlockerModeConfig(AppBlockerModeSettings appBlockerModeSettings);

        [OperationContract(IsOneWay = true)]
        Task SaveGeneralSettings(GeneralSettings generalSettings);
    }
}
