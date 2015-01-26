// -----------------------------------------------------------------------
// <copyright file="TcpClientConnector.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - TcpClientConnector.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// Opens a Tcp Client to the given hostname:port.
    /// Creates a tcp listener with a random port between 10000-65535 on localhost address. (ipadress.any)
    /// The tcp listener is for reconnect purposes when communicating with the passive game server.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class TcpClientConnector<T> : INetworkClientConnector<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// The client.
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// The first receive.
        /// </summary>
        private bool firstReceiveHandled;

        /// <summary>
        /// The first send.
        /// </summary>
        private bool firstSendHandled;

        /// <summary>
        /// The message send queue
        /// </summary>
        private Queue<T> messageSendQueue;

        /// <summary>
        /// The reconnect listener.
        /// </summary>
        private TcpListener reconnectListener;

        /// <summary>
        /// The reconnect port
        /// </summary>
        private int reconnectPort;

        /// <summary>
        /// The reconnect running indicator.
        /// </summary>
        private bool reconnectRunning;

        /// <summary>
        /// The session identifier
        /// </summary>
        private Guid sessionId;

        /// <summary>
        /// The calculation step sleep time.
        /// </summary>
        private TimeSpan taskWaitSleepTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientConnector{T}"/> class.
        /// </summary>
        public TcpClientConnector()
        {
            this.Initialize();
        }

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
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Connection Settings are invalid or missing.</exception>
        public string Hostname
        {
            get
            {
                // ReSharper disable once MergeSequentialChecks
                if (this.ConnectionSettings == null || !(this.ConnectionSettings is IpConnectionEndpoint))
                {
                    throw new InvalidOperationException("Connection Settings are invalid or missing.");
                }

                return this.ConnectionSettings.Hostname;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (this.client != null)
                {
                    return this.client.Connected;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the message transformer.
        /// </summary>
        public IMessageTransformer<T> MessageTransformer { get; set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Connection Settings are invalid or missing.</exception>
        public int Port
        {
            get
            {
                if (this.ConnectionSettings == null || !(this.ConnectionSettings is IpConnectionEndpoint))
                {
                    throw new InvalidOperationException("Connection Settings are invalid or missing.");
                }

                return (this.ConnectionSettings as IpConnectionEndpoint).Port;
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
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.reconnectRunning = false;
            this.client.Close();
        }

        /// <summary>
        /// The connect.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Connection Settings missing.</exception>
        public void Connect()
        {
            try
            {
                this.client = new TcpClient(this.Hostname, this.Port);
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
            }

            this.reconnectRunning = true;

            Task.Run(() => this.RunMessageReceiver());
            Task.Run(() => this.RunMessageSender());
            Task.Run(() => this.RunReconnectListener());
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Send(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.messageSendQueue.Enqueue(message);
        }

        /// <summary>
        /// Gets the random reconnect port between 10000 and 65535.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetRandomReconnectPort()
        {
            return new Random(DateTime.Now.Millisecond).Next(10000, 65535);
        }

        /// <summary>
        /// Handles the incomming reconnect.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private void HandleIncommingReconnect(IAsyncResult result)
        {
            TcpClient tempClient = null;

            try
            {
                tempClient = this.reconnectListener.EndAcceptTcpClient(result);
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
                return;
            }

            T message = null;

            try
            {
                message = this.MessageTransformer.TransformFrom(tempClient.GetStream());
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
            }

            if (message != null && message.SessionId != null && message.SessionId.Value.Equals(this.sessionId))
            {
                this.OnMessageReceived(new MessageReceivedEventArgs<T>(message, this.sessionId));
            }
            else
            {
                LogMessage(new LogMessage(LogMessageType.Error, "Invalid Session Id provided."));
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.firstReceiveHandled = false;
            this.firstSendHandled = false;
            this.taskWaitSleepTime = new TimeSpan(0, 0, 0, 0, 100);
            this.messageSendQueue = new Queue<T>();
            this.reconnectPort = this.GetRandomReconnectPort();
        }

        /// <summary>
        /// Runs the message receiver.
        /// </summary>
        private void RunMessageReceiver()
        {
            NetworkStream networkStream = null;
            T message = null;

            try
            {
                networkStream = client.GetStream();
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
                networkStream = null;
            }

            while (this.client.Connected && networkStream != null)
            {
                // while no data on network stream available and connection not closed
                while (client.Available == 0 && client.Connected)
                {
                    Thread.Sleep(taskWaitSleepTime);
                }

                try
                {
                    //// direct deserializing from networkstream ends in endless loop
                    //// because networkStream does not support seek/readtoend 

                    var buffer = new byte[client.Available];
                    networkStream.Read(buffer, 0, client.Available);
                    var memoryStream = new MemoryStream(buffer);

                    // Receive message
                    message = this.MessageTransformer.TransformFrom(memoryStream);
                }
                catch (Exception ex)
                {
                    LogMessage(new LogMessage(ex));
                }

                if (message != null && message.SessionId != null)
                {
                    // first message received from server - session id set
                    if (!this.firstReceiveHandled)
                    {
                        this.firstReceiveHandled = true;
                        this.sessionId = message.SessionId.Value;
                    }

                    if (message.SessionId.Value.Equals(this.sessionId))
                    {
                        this.OnMessageReceived(new MessageReceivedEventArgs<T>(message, this.sessionId));
                    }
                    else
                    {
                        LogMessage(new LogMessage(LogMessageType.Error, "Invalid Session Id provided."));
                    }
                }
            }
        }

        /// <summary>
        /// Runs the message sender.
        /// </summary>
        private void RunMessageSender()
        {
            while (this.client.Connected)
            {
                while (this.messageSendQueue.Count == 0)
                {
                    Thread.Sleep(this.taskWaitSleepTime);
                }

                T message = this.messageSendQueue.Dequeue();

                if (!this.firstSendHandled)
                {
                    message.SessionId = null;
                }

                // reconnect endpoint
                message.ReconnectEndpoint = new IpConnectionEndpoint()
                {
                    Hostname = Dns.GetHostName(),
                    Port = this.reconnectPort
                };


                try
                {
                    this.MessageTransformer.TransformTo(this.client.GetStream(), message);
                }
                catch (Exception ex)
                {
                    LogMessage(new LogMessage(ex));
                }
            }
        }

        /// <summary>
        /// Runs the reconnect listener.
        /// </summary>
        private void RunReconnectListener()
        {
            try
            {
                this.reconnectListener = new TcpListener(IPAddress.Any, this.reconnectPort);
                this.reconnectListener.Start();
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
            }

            while (this.reconnectRunning)
            {
                while (!this.reconnectListener.Pending())
                {
                    Thread.Sleep(this.taskWaitSleepTime);

                    if (!this.reconnectRunning)
                    {
                        return;
                    }
                }

                try
                {
                    this.reconnectListener.BeginAcceptTcpClient(this.HandleIncommingReconnect, null);
                }
                catch (Exception ex)
                {
                    LogMessage(new LogMessage(ex));
                }
            }

            try
            {
                this.reconnectListener.Stop();
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="args">
        /// The <see cref="MessageReceivedEventArgs{T}"/> instance containing the event data.
        /// </param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> args)
        {
            // ReSharper disable once UseNullPropagation
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, args);
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