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
    /// </typeparam>
    public class CalcItNetworkServer<T> : INetworkAccess<T>
        where T : class, ICalcItSession
    {
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
        /// Gets or sets the server connector.
        /// </summary>
        /// <value>
        /// The server connector.
        /// </value>
        public INetworkServerConnector<T> ServerConnector { get; set; }

        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public List<Guid> Sessions { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">ServerConnector has to be initialized!</exception>
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
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// ServerConnector has to be initialized!
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// message
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
        }

        /// <summary>
        /// Receives this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<T> Receive(Type messageType)
        {
            // DateTime start = DateTime.Now;
            Queue<T> input = new Queue<T>();

            EventHandler<MessageReceivedEventArgs<T>> handler = (sender, e) =>
            {
                if (e.Message.GetType() == messageType)
                {
                    input.Enqueue(e.Message);
                }
            };

            this.MessageReceived += handler;

            await Task.Run(() =>
            {
                while (input.Count == 0)
                {
                    Thread.Sleep(100);

                    // if ((DateTime.Now - start) >= timeout)
                    // {
                    //    this.MessageReceived -= handler;
                    //    throw new TimeoutException();
                    // }
                }
            });

            this.MessageReceived -= handler;

            return input.Dequeue();
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
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        private void LogMessage(LogMessage logMessage)
        {
            // ReSharper disable once UseNullPropagation
            if (Logger != null)
            {
                Logger.AddLogMessage(logMessage);
            }
        }
    }
}