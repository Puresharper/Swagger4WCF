using System.ServiceModel;
using System.ServiceModel.Web;
using Swagger4WCF.Demo.Models;

namespace Swagger4WCF.Demo
{
    /// <summary>
    /// WCF service to create various messages.
    /// </summary>
    [ServiceContract]
    public interface IWCFService1
    {
        /// <summary>
        /// Create a message to say hello at particular time.
        /// </summary>
        /// <param name="person">Person concerned by the hello message</param>
        /// <param name="datetime">Date of the hello message</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        string CreateHelloMessage(Person person, string datetime);
    }
}
