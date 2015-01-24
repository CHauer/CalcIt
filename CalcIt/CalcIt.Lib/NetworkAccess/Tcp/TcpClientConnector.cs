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
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Session;

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
                // Todo log ex
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
            var tempClient = this.reconnectListener.EndAcceptTcpClient(result);

            var message = this.MessageTransformer.TransformFrom(tempClient.GetStream());

            if (message != null && message.SessionId.Equals(this.sessionId))
            {
                this.OnMessageReceived(new MessageReceivedEventArgs<T>(message, this.sessionId));
            }
            else
            {
                // TODO log wrong session id
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
            this.reconnectListener = new TcpListener(IPAddress.Any, this.reconnectPort);
        }

        /// <summary>
        /// Runs the message receiver.
        /// </summary>
        private void RunMessageReceiver()
        {
            while (this.client.Connected)
            {
                T message = this.MessageTransformer.TransformFrom(this.client.GetStream());

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
                        // TODO log wrong session id received
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
                    // reconnect endpoint
                    message.ConnectionEndpoint = new IpConnectionEndpoint()
                                                     {
                                                         Hostname = Dns.GetHostName(), 
                                                         Port = this.reconnectPort
                                                     };
                }

                try
                {
                    this.MessageTransformer.TransformTo(this.client.GetStream(), message);
                }
                catch (Exception ex)
                {
                    // TODO log ex
                }
            }
        }

        /// <summary>
        /// Runs the reconnect listener.
        /// </summary>
        private void RunReconnectListener()
        {
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

                this.reconnectListener.BeginAcceptTcpClient(this.HandleIncommingReconnect, null);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="args">
        /// The <see cref="MessageReceivedEventArgs{T}"/> instance containing the event data.
        /// </param>
        private void OnMessageReceived(MessageReceivedEventArgs<T> args)
        {
            // ReSharper disable once UseNullPropagation
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, args);
            }
        }
    }
}