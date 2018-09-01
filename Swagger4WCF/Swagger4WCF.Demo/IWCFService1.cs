using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Swagger4WCF.Demo
{
    /// <summary>
    /// Person
    /// </summary>
    [DataContract]
    public class Person
    {
        /// <summary>
        /// Firstname
        /// </summary>
        [DataMember]
        public string Firstname { get; set; }

        /// <summary>
        /// Lastname
        /// </summary>
        [DataMember]
        public string Lastname { get; set; }
    }

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
