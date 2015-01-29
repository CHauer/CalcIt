// -----------------------------------------------------------------------
// <copyright file="CalcItMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - CalcItMessage.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol
{
    using System;
    using System.Runtime.Serialization;

    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;
    using CalcIt.Protocol.Server;

    /// <summary>
    /// The calc it message.
    /// </summary>
    [DataContract]
    [KnownType(typeof(CalcItClientMessage))]
    [KnownType(typeof(CalcItMonitorMessage))]
    [KnownType(typeof(CalcItServerMessage))]
    public abstract class CalcItMessage : ICalcItSession, IMessageControl
    {
        /// <summary>
        /// Gets or sets the message nr.
        /// </summary>
        /// <value>
        /// The message nr.
        /// </value>
        [DataMember]
        public int MessageNr { get; set; }

        /// <summary>
        /// Gets or sets the session endpoint.
        /// </summary>
        /// <value>
        /// The reconnect endpoint.
        /// </value>
        [DataMember]
        public ConnectionEndpoint ReconnectEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        [DataMember]
        public Guid? SessionId { get; set; }
    }
}