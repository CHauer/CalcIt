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
    /// </typeparam>
    public class CalcItNetworkClient<T> : INetworkAccess<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// Indicates the session is established
        /// </summary>
        private bool isSessionEstablished;

        /// <summary>
        /// The session identifier
        /// </summary>
        private Guid sessionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcItNetworkClient{T}" /> class.
        /// </summary>
        public CalcItNetworkClient()
        {
            isSessionEstablished = false;
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
        /// <exception cref="System.InvalidOperationException">ClientConnector has to be initialized!</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Connect()
        {
            if (this.ClientConnector == null)
            {
                throw new InvalidOperationException("ClientConnector has to be initialized!");
            }

            this.ClientConnector.MessageReceived += (sender, e) =>
            {
                if (!isSessionEstablished && e.Message.SessionId != null)
                {
                    this.sessionId = e.Message.SessionId.Value;
                    isSessionEstablished = true;
                }

                this.OnMessageReceived(e);
            };

            this.ClientConnector.Logger = Logger;
            this.ClientConnector.Connect();
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.InvalidOperationException">ClientConnector has to be initialized!</exception>
        /// <exception cref="InvalidOperationException"></exception>
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

            if (isSessionEstablished)
            {
                message.SessionId = sessionId;
            }

            this.ClientConnector.Send(message);
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MessageReceivedEventArgs{T}" /> instance containing the event data.</param>
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
        /// <param name="logMessage">The log message.</param>
        private void LogMessage(LogMessage logMessage)
        {
            // ReSharper disable once UseNullPropagation
            if (Logger != null)
            {
                Logger.AddLogMessage(logMessage);
            }
        }

        /// <summary>
        /// Receives this instance.
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public async Task<T> Receive(Type messageType, TimeSpan timeout)
        {
            DateTime start = DateTime.Now;
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

                    if ((DateTime.Now - start) >= timeout)
                    {
                        this.MessageReceived -= handler;
                        return;
                    }
                }
            });

            this.MessageReceived -= handler;

            if (input.Count == 0)
            {
                return null;
            }

            return input.Dequeue();
        }
    }
}