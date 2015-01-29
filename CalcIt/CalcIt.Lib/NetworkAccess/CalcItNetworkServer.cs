// -----------------------------------------------------------------------
// <copyright file="CalcItNetworkServer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - CalcItNetworkServer.cs</summary>
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
    /// The calc it network server.
    /// </summary>
    /// <typeparam name="T">
    /// Type of class and ICalcItSession implemented.
    /// </typeparam>
    public class CalcItNetworkServer<T> : INetworkAccess<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// The receive queues.
        /// </summary>
        private Dictionary<Guid, Queue<T>> receiveQueues = new Dictionary<Guid, Queue<T>>();

        /// <summary>
        /// The listen types.
        /// </summary>
        private List<Type> listenTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcItNetworkServer{T}"/> class.
        /// </summary>
        public CalcItNetworkServer()
        {
            this.Sessions = new List<Guid>();
        }

        /// <summary>
        /// The message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// The message send.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<T>> MessageSend;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the server connector.
        /// </summary>
        /// <value>
        /// The server connector.
        /// </value>
        public INetworkServerConnector<T> ServerConnector { get; set; }

        /// <summary>
        /// Gets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public List<Guid> Sessions { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// ServerConnector has to be initialized!.
        /// </exception>
        public void Start()
        {
            if (this.ServerConnector == null)
            {
                throw new InvalidOperationException("ServerConnector has to be initialized!");
            }

            this.ServerConnector.MessageReceived += (sender, e) => { this.OnMessageReceived(e); };
            this.ServerConnector.IncomingConnectionOccured += (sender, e) => { this.Sessions.Add(e.Session); };

            this.ServerConnector.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            // ReSharper disable once UseNullPropagation
            if (this.ServerConnector == null)
            {
                return;
            }

            this.ServerConnector.Stop();
        }

        /// <summary>
        /// The send method.
        /// </summary>
        /// <param name="message">
        /// The message parameter.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// ServerConnector has to be initialized!.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Message exception.
        /// </exception>
        public void Send(T message)
        {
            if (this.ServerConnector == null)
            {
                throw new InvalidOperationException("ServerConnector has to be initialized!");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.ServerConnector.Send(message);

            this.OnMessageSend(new MessageReceivedEventArgs<T>(message));
        }

        /// <summary>
        /// Receives this instance.
        /// </summary>
        /// <param name="messageType">
        /// Type of the message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> the received message.
        /// </returns>
        [Obsolete]
        public async Task<T> Receive(Type messageType)
        {
            Queue<T> input = new Queue<T>();

            EventHandler<MessageReceivedEventArgs<T>> handler = (sender, e) =>
                {
                    if (e.Message.GetType() == messageType)
                    {
                        input.Enqueue(e.Message);
                    }
                };

            this.MessageReceived += handler;

            await Task.Run(
                () =>
                    {
                        while (input.Count == 0)
                        {
                            Thread.Sleep(100);
                        }
                    });

            this.MessageReceived -= handler;

            if (input.Count == 0)
            {
                return null;
            }

            return input.Dequeue();
        }

        /// <summary>
        /// Clears the receive queue.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void ClearReceiveQueue(Guid sessionId)
        {
            if (this.receiveQueues.ContainsKey(sessionId))
            {
                this.receiveQueues[sessionId].Clear();
            }
        }

        /// <summary>
        /// Starts the queue receiver.
        /// </summary>
        /// <param name="types">
        /// The types.
        /// </param>
        public void StartQueueReceiver(params Type[] types)
        {
            this.listenTypes = types.ToList();

            this.MessageReceived += this.HandleQueueReceiveMessage;
        }

        /// <summary>
        /// Stops the queue receiver.
        /// </summary>
        public void StopQueueReceiver()
        {
            this.MessageReceived -= this.HandleQueueReceiveMessage;
        }

        /// <summary>
        /// Receives this instance.
        /// </summary>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> received message.
        /// </returns>
        public async Task<T> Receive(Guid sessionId, TimeSpan timeout)
        {
            DateTime start = DateTime.Now;

            await Task.Run(
                () =>
                    {
                        while (this.receiveQueues[sessionId].Count == 0)
                        {
                            Thread.Sleep(100);

                            if ((DateTime.Now - start) >= timeout)
                            {
                                return;
                            }
                        }
                    });

            if (this.receiveQueues[sessionId].Count == 0)
            {
                return null;
            }

            return this.receiveQueues[sessionId].Dequeue();
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="CalcIt.Lib.NetworkAccess.Events.MessageReceivedEventArgs{T}"/> instance containing the event data.
        /// </param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> e)
        {
            var onMessageReceived = this.MessageReceived;

            // ReSharper disable once UseNullPropagation
            if (onMessageReceived != null)
            {
                onMessageReceived(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MessageSend"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MessageReceivedEventArgs{T}"/> instance containing the event data.
        /// </param>
        protected virtual void OnMessageSend(MessageReceivedEventArgs<T> e)
        {
            var onMessageSend = this.MessageSend;

            // ReSharper disable once UseNullPropagation
            if (onMessageSend != null)
            {
                onMessageSend(this, e);
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
            if (this.listenTypes == null)
            {
                return;
            }

            if (e.Message.SessionId == null)
            {
                return;
            }

            if (!this.receiveQueues.ContainsKey(e.Message.SessionId.Value))
            {
                this.receiveQueues.Add(e.Message.SessionId.Value, new Queue<T>());
            }

            if (this.listenTypes.Contains(e.Message.GetType()))
            {
                // ReSharper disable once PossibleInvalidOperationException
                // ReSharper disable once AssignNullToNotNullAttribute
                if (this.receiveQueues.ContainsKey(e.Message.SessionId.Value))
                {
                    this.receiveQueues[e.Message.SessionId.Value].Enqueue(e.Message);
                }
            }
        }
    }
}