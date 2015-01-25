// -----------------------------------------------------------------------
// <copyright file="UdpServerListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - UdpServerListener.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.Udp
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
    /// The udp server listener.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class UdpServerListener<T> : INetworkServerConnector<T>
        where T : class, ICalcItSession, IMessageControl
    {
        /// <summary>
        /// The client connections
        /// </summary>
        private Dictionary<Guid, IPEndPoint> clientConnections;

        /// <summary>
        /// The client message control numbers
        /// </summary>
        private Dictionary<Guid, int> clientMessageControlNumbers;

        /// <summary>
        /// The message send queue.
        /// </summary>
        private Queue<T> messageSendQueue;

        /// <summary>
        /// The receiver
        /// </summary>
        private UdpClient receiver;

        /// <summary>
        /// The calculation step sleep time.
        /// </summary>
        private TimeSpan taskWaitSleepTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpServerListener{T}"/> class.
        /// </summary>
        public UdpServerListener()
        {
            this.Initialize();
        }

        /// <summary>
        /// An incoming connection or session was received.
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
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the listen port.
        /// </summary>
        /// <value>
        /// The listen port.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Connection Settings are invalid or missing.</exception>
        public int ListenPort
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
        /// Gets or sets the message transformer.
        /// </summary>
        /// <value>
        /// The message transformer.
        /// </value>
        public IMessageTransformer<T> MessageTransformer { get; set; }

        /// <summary>
        /// Gets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public List<Guid> Sessions { get; private set; }

        /// <summary>
        /// The start.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Message transformer has to be initialized!</exception>
        public void Start()
        {
            if (this.MessageTransformer == null)
            {
                throw new InvalidOperationException("Message transformer has to be initialized!");
            }

            this.IsRunning = true;

            Task.Run(() => this.RunReceiver());
            Task.Run(() => this.RunSendQueue());
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Handles the received message.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="receivedEndpoint">
        /// The received endpoint.
        /// </param>
        private void HandleReceivedMessage(byte[] data, IPEndPoint receivedEndpoint)
        {
            Guid currentSessionId;
            T message = null;

            try
            {
                message = this.MessageTransformer.TransformFrom(data);
            }
            catch (Exception ex)
            {
                // TODO Log ex
            }

            if (message == null)
            {
                return;
            }

            if (message.SessionId == null)
            {
                // create new session id
                Guid newSessionId = Guid.NewGuid();

                currentSessionId = newSessionId;

                this.clientConnections.Add(newSessionId, receivedEndpoint);
                this.Sessions.Add(newSessionId);

                message.SessionId = currentSessionId;
                this.clientMessageControlNumbers.Add(currentSessionId, 0);

                this.OnIncomingConnectionOccured(newSessionId);
            }

            // if session does not exists in session table
            if (!this.Sessions.Contains(message.SessionId.Value))
            {
                // invalid message - connection close
                // TODO log invalid message
                return;
            }

            currentSessionId = message.SessionId.Value;

            //message tranmition control
            if (message.MessageNr != (clientMessageControlNumbers[currentSessionId] + 1))
            {
                // invalid message - connection close
                // TODO log invalid message
                return;
            }

            //if valid message - messagenr increment
            clientMessageControlNumbers[currentSessionId]++;

            this.OnMessageReceived(new MessageReceivedEventArgs<T>(message, currentSessionId));
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.taskWaitSleepTime = new TimeSpan(0, 0, 0, 0, 100);

            this.messageSendQueue = new Queue<T>();

            this.InitializeSessions();
        }

        /// <summary>
        /// Initializes the sessions.
        /// </summary>
        private void InitializeSessions()
        {
            this.Sessions = new List<Guid>();
            this.clientConnections = new Dictionary<Guid, IPEndPoint>();
            this.clientMessageControlNumbers = new Dictionary<Guid, int>();
        }

        /// <summary>
        /// Runs the receiver.
        /// </summary>
        private void RunReceiver()
        {
            this.receiver = new UdpClient(this.ListenPort);

            while (this.IsRunning)
            {
                var remoteEp = new IPEndPoint(IPAddress.Any, this.ListenPort);
                var data = this.receiver.Receive(ref remoteEp);

                Task.Run(() => this.HandleReceivedMessage(data, remoteEp));
            }
        }

        /// <summary>
        /// Runs the send queue.
        /// </summary>
        private void RunSendQueue()
        {
            while (this.IsRunning)
            {
                while (this.messageSendQueue.Count == 0)
                {
                    Thread.Sleep(this.taskWaitSleepTime);
                }

                T message = this.messageSendQueue.Dequeue();

                if (message.SessionId == null)
                {
                    continue;
                }

                if (this.Sessions.Contains(message.SessionId.Value))
                {
                    // session is in table - use cached ipendpoint connection

                    clientMessageControlNumbers[message.SessionId.Value]++;
                    message.MessageNr = clientMessageControlNumbers[message.SessionId.Value];

                    try
                    {
                        using (var client = new UdpClient(this.clientConnections[message.SessionId.Value]))
                        {
                            byte[] buffer = this.MessageTransformer.TransformTo(message);
                            client.Send(buffer, buffer.Length);
                            client.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Todo Log ex
                    }
                }
                else
                {
                    if (message.ReconnectEndpoint != null && message.ReconnectEndpoint is IpConnectionEndpoint)
                    {
                        message.MessageNr++;

                        var reconEp = message.ReconnectEndpoint as IpConnectionEndpoint;
                        try
                        {
                            using (var client = new UdpClient(reconEp.Hostname, reconEp.Port))
                            {
                                byte[] buffer = this.MessageTransformer.TransformTo(message);
                                client.Send(buffer, buffer.Length);
                                client.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Todo Log ex
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// message
        /// </exception>
        public void Send(T message)
        {
            // ReSharper disable once MergeSequentialChecks
            if (message == null || message.SessionId == null)
            {
                throw new ArgumentNullException("message");
            }

            this.messageSendQueue.Enqueue(message);
        }

        /// <summary>
        /// Raises the <see cref="E:IncomingConnectionOccured"/> event.
        /// </summary>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        protected virtual void OnIncomingConnectionOccured(Guid sessionId)
        {
            // ReSharper disable once UseNullPropagation
            if (this.IncomingConnectionOccured != null)
            {
                this.IncomingConnectionOccured(this, new ConnectionEventArgs(sessionId));
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
    }
}