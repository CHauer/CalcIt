// -----------------------------------------------------------------------
// <copyright file="Heartbeat.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - Heartbeat.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Server
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The heartbeat.
    /// </summary>
    [DataContract]
    public class Heartbeat : CalcItServerMessage
    {
        /// <summary>
        /// Gets or sets the check token.
        /// </summary>
        [DataMember]
        public int CheckToken { get; set; }
    }
}