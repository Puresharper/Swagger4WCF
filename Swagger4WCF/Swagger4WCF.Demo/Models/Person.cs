using System.Runtime.Serialization;

namespace Swagger4WCF.Demo.Models
{
    /// <summary>
    /// Person
    /// </summary>
    [DataContract]
    public class Person
    {
        /// <summary>
        /// First name
        /// </summary>
        [DataMember]
        public string Firstname { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [DataMember]
        public string Lastname { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        [DataMember]
        public Gender Gender { get; set; }
    }
}
