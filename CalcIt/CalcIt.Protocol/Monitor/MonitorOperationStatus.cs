// -----------------------------------------------------------------------
// <copyright file="MonitorOperationStatus.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - MonitorOperationStatus.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Monitor
{
    using System.Runtime.Serialization;

    using CalcIt.Protocol.Data;

    /// <summary>
    /// The monitor operation status.
    /// </summary>
    [DataContract]
    public class MonitorOperationStatus : CalcItMonitorMessage
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