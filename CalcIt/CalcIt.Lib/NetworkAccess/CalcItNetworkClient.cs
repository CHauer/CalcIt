// -----------------------------------------------------------------------
// <copyright file="CalcItNetworkClient.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - CalcItNetworkClient.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The calc it network client.
    /// </summary>
    /// <typeparam name="T">
    /// Type of class and ICalcItSession implemented.
    /// </typeparam>
    public class CalcItNetworkClient<T> : INetworkAccess<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// Indicates the session is established.
        /// </summary>
        private bool isSessionEstablished;

        /// <summary>
        /// The session identifier.
        /// </summary>
        private Guid sessionId;

        /// <summary>
        /// The receive queue.
        /// </summary>
        private Queue<T> receiveQueue;

        /// <summary>
        /// The listen types.
        /// </summary>
        private List<Type> listenTypes;

        /// <summary>
        /// The is queue receiver.
        /// </summary>
        private bool isQueueReceiver = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcItNetworkClient{T}"/> class.
        /// </summary>
        public CalcItNetworkClient()
        {
            this.isSessionEstablished = false;
        }

        /// <summary>
        /// The message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets or sets the client connector.
        /// </summary>
        /// <value>
        /// The client connector.
        /// </value>
        public INetworkClientConnector<T> ClientConnector { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            // ReSharper disable once UseNullPropagation
            if (this.ClientConnector == null)
            {
                return;
            }

            this.ClientConnector.Close();
        }

        /// <summary>
        /// The connect.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// ClientConnector has to be initialized!.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void Connect()
        {
            if (this.ClientConnector == null)
            {
                throw new InvalidOperationException("ClientConnector has to be initialized!");
            }

            this.ClientConnector.MessageReceived += (sender, e) =>
                {
                    if (!this.isSessionEstablished && e.Message.SessionId != null)
                    {
                        this.sessionId = e.Message.SessionId.Value;
                        this.isSessionEstablished = true;
                    }

                    this.OnMessageReceived(e);
                };

            this.ClientConnector.Logger = this.Logger;
            this.ClientConnector.Connect();
        }

        /// <summary>
        /// The send method.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// ClientConnector has to be initialized!.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Invalid Operation Exception.
        /// </exception>
        public void Send(T message)
        {
            if (this.ClientConnector == null)
            {
                throw new InvalidOperationException("ClientConnector has to be initialized!");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (this.isSessionEstablished)
            {
                message.SessionId = this.sessionId;
            }

            this.ClientConnector.Send(message);
        }

        /// <summary>
        /// Starts the queue receiver.
        /// </summary>
        /// <param name="types">
        /// The types.
        /// </param>
        public void StartQueueReceiver(params Type[] types)
        {
            this.receiveQueue = new Queue<T>();
            this.listenTypes = types.ToList();

            if (this.isQueueReceiver)
            {
                return;
            }

            this.isQueueReceiver = true;
            this.MessageReceived += this.HandleQueueReceiveMessage;
        }

        /// <summary>
        /// Stops the queue receiver.
        /// </summary>
        public void StopQueueReceiver()
        {
            this.isQueueReceiver = false;

            this.MessageReceived -= this.HandleQueueReceiveMessage;
        }

        /// <summary>
        /// Receives this instance.
        /// </summary>
        /// <param name="messageType">
        /// The message Type.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> status.
        /// </returns>
        public async Task<T> Receive(Type messageType, TimeSpan timeout)
        {
            DateTime start = DateTime.Now;

            await Task.Run(
                () =>
                    {
                        while (this.receiveQueue.Count == 0)
                        {
                            Thread.Sleep(100);

                            if ((DateTime.Now - start) >= timeout)
                            {
                                return;
                            }
                        }
                    });

            if (this.receiveQueue.Count == 0)
            {
                return null;
            }

            return this.receiveQueue.Dequeue();
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MessageReceivedEventArgs{T}"/> instance containing the event data.
        /// </param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> e)
        {
            var onMessageReceived = this.MessageReceived;
            if (onMessageReceived != null)
            {
                onMessageReceived(this, e);
            }
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        private void LogMessage(LogMessage logMessage)
        {
            // ReSharper disable once UseNullPropagation
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(logMessage);
            }
        }

        /// <summary>
        /// Handles the queue receive message.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MessageReceivedEventArgs{T}"/> instance containing the event data.
        /// </param>
        private void HandleQueueReceiveMessage(object sender, MessageReceivedEventArgs<T> e)
        {
            if (this.listenTypes == null || this.receiveQueue == null)
            {
                return;
            }

            if (this.listenTypes.Contains(e.Message.GetType()))
            {
                this.receiveQueue.Enqueue(e.Message);
            }
        }
    }
}