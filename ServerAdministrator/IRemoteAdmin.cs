using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerAdministrator
{
    [ServiceContract]
    public interface IRemoteAdmin
    {
        [OperationContract]
        ICollection<string> GetLog();
        [OperationContract]
        ICollection<string> GetRanking();
        [OperationContract]
        ICollection<string> GetStatistics();
        [OperationContract]
        string AddPlayer(string username);
        [OperationContract]
        string ModifyPlayer(string oldUsername, string newUsername);
        [OperationContract]
        string DeletePlayer(string username);
    }
}
