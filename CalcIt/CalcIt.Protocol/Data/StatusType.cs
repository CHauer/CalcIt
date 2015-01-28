// -----------------------------------------------------------------------
// <copyright file="StatusType.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - StatusType.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The status type.
    /// </summary>
    [DataContract]
    public enum StatusType
    {
        /// <summary>
        /// The ok value.
        /// </summary>
        [EnumMember]
        Ok, 

        /// <summary>
        /// The error value.
        /// </summary>
        [EnumMember]
        Error, 
    }
}