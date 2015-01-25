// -----------------------------------------------------------------------
// <copyright file="INetworkServerConnector.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - INetworkServerConnector.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess
{
    using System;
    using System.Collections.Generic;

    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Session;

    /// <summary>
    /// The NetworkServerConnector interface.
    /// </summary>
    /// <typeparam name="T">
    /// Class with ICalcItSession interface implementation.
    /// </typeparam>
    public interface INetworkServerConnector<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// An incoming connection or session was received.
        /// </summary>
        event EventHandler<ConnectionEventArgs> IncomingConnectionOccured;

        /// <summary>
        /// The message received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets or sets the connection settings.
        /// </summary>
        ConnectionEndpoint ConnectionSettings { get; set; }

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets or sets the message transformer.
        /// </summary>
        IMessageTransformer<T> MessageTransformer { get; set; }

        /// <summary>
        /// Gets the sessions.
        /// </summary>
        List<Guid> Sessions { get; }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Send(T message);

        /// <summary>
        /// The start.
        /// </summary>
        void Start();

        /// <summary>
        /// The stop.
        /// </summary>
        void Stop();
    }
}