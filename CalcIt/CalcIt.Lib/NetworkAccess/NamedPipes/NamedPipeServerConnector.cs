// -----------------------------------------------------------------------
// <copyright file="NamedPipeServerConnector.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - NamedPipeServerConnector.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.NamedPipes
{
    using System;
    using System.Collections.Generic;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Endpoint;

    /// <summary>
    /// The named pipe server connector.
    /// </summary>
    /// <typeparam name="T">
    /// Type of class and ICalcItSession implemented.
    /// </typeparam>
    public class NamedPipeServerConnector<T> : INetworkServerConnector<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// The incoming connection.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> IncomingConnectionOccured;

        /// <summary>
        /// The message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets or sets the connection settings.
        /// </summary>
        /// <value>
        /// The connection settings.
        /// </value>
        public ConnectionEndpoint ConnectionSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is running.
        /// </summary>
        /// <value>
        /// The is running.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public bool IsRunning
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the message transformer.
        /// </summary>
        /// <value>
        /// The message transformer.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public IMessageTransformer<T> MessageTransformer
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the pipe name.
        /// </summary>
        /// <value>
        /// The pipe name.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public int PipeName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public List<Guid> Sessions { get; set; }

        /// <summary>
        /// The send method.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public void Send(T message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public void Start()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The stop method.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public void Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Raises the <see cref="E:IncomingConnectionOccured" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ConnectionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIncomingConnectionOccured(ConnectionEventArgs e)
        {
            var onIncomingConnectionOccured = this.IncomingConnectionOccured;
            if (onIncomingConnectionOccured != null)
            {
                onIncomingConnectionOccured.Invoke(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MessageReceivedEventArgs{T}"/> instance containing the event data.</param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> e)
        {
            var onMessageReceived = this.MessageReceived;
            if (onMessageReceived != null)
            {
                onMessageReceived.Invoke(this, e);
            }
        }
    }
}