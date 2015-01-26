// -----------------------------------------------------------------------
// <copyright file="ClientOperationStatus.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - ClientOperationStatus.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Runtime.Serialization;

    using CalcIt.Protocol.Data;

    /// <summary>
    /// The client operation status.
    /// </summary>
    [DataContract]
    public class ClientOperationStatus : CalcItClientMessage
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember]
        public StatusType Status { get; set; }
    }
}