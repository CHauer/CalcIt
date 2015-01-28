// -----------------------------------------------------------------------
// <copyright file="IpConnectionEndpoint.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - IpConnectionEndpoint.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Endpoint
{
    using System;
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Hostname))
            {
                return "Listened Port: " + this.Port.ToString();
            }

            return string.Format("{0}:{1}", this.Hostname, this.Port);
        }
    }
}