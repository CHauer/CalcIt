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

    using CalcIt.Protocol.Session;

    /// <summary>
    /// The calc it message.
    /// </summary>
    [DataContract]
    public abstract class CalcItMessage : ICalcItSession
    {
        /// <summary>
        /// Gets or sets the message nr.
        /// </summary>
        [DataMember]
        public int MessageNr { get; set; }

        /// <summary>
        /// Gets or sets the session endpoint.
        /// </summary>
        [DataMember]
        public ConnectionEndpoint ConnectionEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        [DataMember]
        public Guid? SessionId { get; set; }
    }
}