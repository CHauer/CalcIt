// -----------------------------------------------------------------------
// <copyright file="ICalcItSession.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - ICalcItSession.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol
{
    using System;

    using CalcIt.Protocol.Endpoint;

    /// <summary>
    /// The CalcItSession interface.
    /// </summary>
    public interface ICalcItSession
    {
        /// <summary>
        /// Gets or sets the reconnect endpoint.
        /// </summary>
        ConnectionEndpoint ReconnectEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        Guid? SessionId { get; set; }
    }
}