// -----------------------------------------------------------------------
// <copyright file="IpConnectionEndpoint.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - IpConnectionEndpoint.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Endpoint
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The ip connection endpoint.
    /// </summary>
    [DataContract]
    public class IpConnectionEndpoint : ConnectionEndpoint
    {
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [DataMember]
        public int Port { get; set; }
    }
}