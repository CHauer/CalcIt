// -----------------------------------------------------------------------
// <copyright file="TcpServerListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - TcpServerListener.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Session;

    /// <summary>
    /// The tcp server listener.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class TcpServerListener<T> : INetworkServerConnector<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// The TCP sockets.
        /// </summary>
        private Dictionary<Guid, TcpClient> clientConnections;

        /// <summary>
        /// The tcp listener.
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// The message send queue.
        /// </summary>
        private Queue<T> messageSendQueue;

        /// <summary>
        /// The calculation step sleep time.
        /// </summary>
        private TimeSpan taskWaitSleepTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerListener{T}"/> class.
        /// </summary>
        public TcpServerListener()
        {
            this.Initialize();
        }

        /// <summary>
        /// Occurs when incoming connection occured.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> IncomingConnectionOccured;

        /// <summary>
        /// Occurs when message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets the listen port.
        /// </summary>
        /// <value>
        /// The listen port.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Connection Settings are invalid or missing.</exception>
        public int ListenPort
        {
            get
            {
                if (ConnectionSettings == null || !(ConnectionSettings is IpConnectionEndpoint))
                {
                    throw new InvalidOperationException("Connection Settings are invalid or missing.");
                }

                return (ConnectionSettings as IpConnectionEndpoint).Port;
            }
        }

        /// <summary>
        /// Gets or sets the connection settings.
        /// </summary>
        /// <value>
        /// The connection settings.
        /// </value>
        public ConnectionEndpoint ConnectionSettings { get; set; }

        /// <summary>
        /// Gets or sets the message transformer.
        /// </summary>
        /// <value>
        /// The message transformer.
        /// </value>
        public IMessageTransformer<T> MessageTransformer { get; set; }

        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public List<Session> Sessions { get; private set; }

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
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (this.MessageTransformer == null)
            {
                throw new InvalidOperationException("Message transformer has to be initialized!");
            }

            this.InitializeListener();

            this.IsRunning = true;

            Task.Run(() => this.RunListener());
            Task.Run(() => this.RunSendQueue());
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;

            this.clientConnections.Values.ToList().ForEach(client => client.Close());
        }

        /// <summary>
        /// Checks the session exists.
        /// </summary>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private void CheckSessionExists(Guid sessionId, T message)
        {
            // if session id not exists in session table
            if (!this.Sessions.Any(s => s.SessionId.Equals(sessionId)))
            {
                // and message has valid session "reconnect" endpoint
                if (message != null && message.SessionId == null && message.ConnectionEndpoint != null)
                {
                    var createdSession = new Session()
                                             {
                                                 SessionId = sessionId, 
                                                 ConnectionEndpoint = message.ConnectionEndpoint
                                             };

                    // add session to table
                    this.Sessions.Add(createdSession);

                    this.OnIncomingConnectionOccured(createdSession);
                }
            }
        }

        /// <summary>
        /// Handles the incomming connection.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private void HandleIncommingConnection(IAsyncResult result)
        {
            TcpClient client = this.listener.EndAcceptTcpClient(result);

            Guid newSessionId = Guid.NewGuid();
            this.clientConnections.Add(newSessionId, client);

            Task.Run(() => this.RunHandleClient(newSessionId, client));
        }

        /// <summary>
        /// Initializes the listener.
        /// </summary>
        private void InitializeListener()
        {
            if (this.ListenPort == -1)
            {
                // Standardport
                this.ListenPort = 3105;
            }

            try
            {
                this.listener = new TcpListener(IPAddress.Any, this.ListenPort);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
            this.Sessions = new List<Session>();
            this.clientConnections = new Dictionary<Guid, TcpClient>();
        }

        /// <summary>
        /// Raises the <see cref="E:IncomingConnectionOccured"/> event.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        private void OnIncomingConnectionOccured(Session session)
        {
            // ReSharper disable once UseNullPropagation
            if (this.IncomingConnectionOccured != null)
            {
                this.IncomingConnectionOccured(this, new ConnectionEventArgs(session));
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

        /// <summary>
        /// Runs the handle client.
        /// </summary>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        private void RunHandleClient(Guid sessionId, TcpClient client)
        {
            while (client.Connected && this.IsRunning)
            {
                T message = this.MessageTransformer.TransformFrom(client.GetStream());

                this.CheckSessionExists(sessionId, message);

                this.OnMessageReceived(new MessageReceivedEventArgs<T>(message, sessionId));
            }
        }

        /// <summary>
        /// Runs the listener.
        /// </summary>
        private void RunListener()
        {
            while (this.IsRunning)
            {
                while (!this.listener.Pending())
                {
                    Thread.Sleep(this.taskWaitSleepTime);

                    if (!this.IsRunning)
                    {
                        return;
                    }
                }

                this.listener.BeginAcceptTcpClient(this.HandleIncommingConnection, null);
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

                if (message.SessionId != null)
                {
                    if (this.clientConnections.ContainsKey(message.SessionId.Value))
                    {
                        this.MessageTransformer.TransformTo(
                            this.clientConnections[message.SessionId.Value].GetStream(), 
                            message);
                    }
                }
            }
        }
    }
}