// -----------------------------------------------------------------------
// <copyright file="ConnectionEndpoint.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - ConnectionEndpoint.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Endpoint
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The connection endpoint.
    /// </summary>
    [DataContract]
    [KnownType(typeof(IpConnectionEndpoint))]
    [KnownType(typeof(PipeConnectionEndpoint))]
    public abstract class ConnectionEndpoint
    {
        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        [DataMember]
        public string Hostname { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public abstract override string ToString();

    }
}