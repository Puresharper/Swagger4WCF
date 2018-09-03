using System.Runtime.Serialization;

namespace Swagger4WCF.Demo.Models
{
    /// <summary>
    /// Gender enum
    /// </summary>
    [DataContract]
    public enum Gender
    {
        /// <summary>
        /// Unknown Gender 
        /// </summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// The male
        /// </summary>
        [EnumMember]
        Male = 1,
        /// <summary>
        /// The female
        /// </summary>
        [EnumMember]
        Female = 2
    }
}
