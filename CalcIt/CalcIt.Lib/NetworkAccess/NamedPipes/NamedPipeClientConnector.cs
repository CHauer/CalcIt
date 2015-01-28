// -----------------------------------------------------------------------
// <copyright file="NamedPipeClientConnector.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - NamedPipeClientConnector.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.NamedPipes
{
    using System;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Endpoint;

    /// <summary>
    /// The named pipe client connector.
    /// </summary>
    /// <typeparam name="T">
    /// Type of class and ICalcItSession implemented.
    /// </typeparam>
    public class NamedPipeClientConnector<T> : INetworkClientConnector<T>
        where T : class, ICalcItSession
    {
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
        /// Gets or sets a value indicating whether is connected.
        /// </summary>
        /// <value>
        /// The is connected.
        /// </value>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public bool IsConnected
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
        /// The close.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The connect.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Not Implemented Exception.
        /// </exception>
        public void Connect()
        {
            throw new NotImplementedException();
        }

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
        /// The on message received.
        /// </summary>
        /// <param name="e">
        /// The e parameter.
        /// </param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> e)
        {
            var onMessageReceived = this.MessageReceived;
            // ReSharper disable once UseNullPropagation
            if (onMessageReceived != null)
            {
                onMessageReceived.Invoke(this, e);
            }
        }
    }
}