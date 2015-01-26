// -----------------------------------------------------------------------
// <copyright file="SyncMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - SyncMessage.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Server
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The sync message.
    /// </summary>
    [DataContract]
    public class SyncMessage : CalcItServerMessage
    {
        /// <summary>
        /// Gets or sets the random number.
        /// </summary>
        /// <value>
        /// The random number.
        /// </value>
        [DataMember]
        public int RandomNumber { get; set; }

        /// <summary>
        /// Gets or sets the server start time.
        /// </summary>
        /// <value>
        /// The server start time.
        /// </value>
        [DataMember]
        public DateTime ServerStartTime { get; set; }
    }
}