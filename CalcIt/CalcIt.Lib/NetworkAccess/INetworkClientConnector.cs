// -----------------------------------------------------------------------
// <copyright file="INetworkClientConnector.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - INetworkClientConnector.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess
{
    using System;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Endpoint;

    /// <summary>
    /// The NetworkClientConnector interface.
    /// </summary>
    /// <typeparam name="T">
    /// Parameter is Class which implements ICalcItSession
    /// </typeparam>
    public interface INetworkClientConnector<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// The message received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets or sets the connection settings.
        /// </summary>
        /// <value>
        /// The connection settings.
        /// </value>
        ConnectionEndpoint ConnectionSettings { get; set; }

        /// <summary>
        /// Gets a value indicating whether is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets or sets the message transformer.
        /// </summary>
        IMessageTransformer<T> MessageTransformer { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        ILog Logger
        {
            get;
            set;
        }

        /// <summary>
        /// The close.
        /// </summary>
        void Close();

        /// <summary>
        /// The connect.
        /// </summary>
        void Connect();

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Send(T message);
    }
}