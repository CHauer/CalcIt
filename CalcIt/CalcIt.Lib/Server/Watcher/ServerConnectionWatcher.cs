// -----------------------------------------------------------------------
// <copyright file="ServerConnectionWatcher.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - ServerConnectionWatcher.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Server.Watcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Tcp;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Lib.Server.Configuration;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;
    using CalcIt.Protocol.Server;

    /// <summary>
    /// The Server Connection watcher.
    /// Server are used to receive messages.
    /// Clients are used to send messages to servers.
    /// </summary>
    public class ServerConnectionWatcher : INetworkAccess<CalcItServerMessage>
    {
        /// <summary>
        /// The message forward queue.
        /// </summary>
        private Queue<CalcItServerMessage> messageForwardQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnectionWatcher"/> class.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ServerConnectionWatcher(ServerManager server, ILog logger)
        {
            this.ServerManagerInstance = server;
            this.Configuration = server.Configuration;
            this.Logger = logger;
            this.messageForwardQueue = new Queue<CalcItServerMessage>();
            this.IsLogReceivedMessages = true;

            this.InitializeServerConnections();
        }

        /// <summary>
        /// Occurs when all connections lost.
        /// </summary>
        public event EventHandler AllConnectionsLost;

        /// <summary>
        /// The message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<CalcItServerMessage>> MessageReceived;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ServerConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is logging received messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is log received messages; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogReceivedMessages { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets the server client connections.
        /// </summary>
        /// <value>
        /// The server client connections.
        /// </value>
        public List<CalcItNetworkClient<CalcItServerMessage>> OpenClients { get; private set; }

        /// <summary>
        /// Gets the server connections.
        /// </summary>
        /// <value>
        /// The server connections.
        /// </value>
        public List<CalcItNetworkServer<CalcItServerMessage>> OpenServers { get; private set; }

        /// <summary>
        /// Gets the server manager instance.
        /// </summary>
        /// <value>
        /// The server manager instance.
        /// </value>
        public ServerManager ServerManagerInstance { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.IsRunning = true;

            Task.Run(() => this.RunWatcher());
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;
            this.OpenClients.ForEach(con => con.Close());
            this.OpenServers.ForEach(server => server.Stop());
        }

        /// <summary>
        /// The send method.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// No Server Connection available!.
        /// </exception>
        public void Send(CalcItServerMessage message)
        {
            if (this.OpenClients.Count > 0)
            {
                this.OpenClients[0].Send(message);
            }
            else if (this.OpenServers.Count > 0)
            {
                message.SessionId = this.OpenServers[0].Sessions[0];
                this.OpenServers[0].Send(message);
            }
            else
            {
                throw new InvalidOperationException("No Server Connection available!");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MessageReceivedEventArgs{CalcItServerMessage}"/> instance containing the event data.
        /// </param>
        protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs<CalcItServerMessage> e)
        {
            var onMessageReceived = this.MessageReceived;

            // ReSharper disable once UseNullPropagation
            if (onMessageReceived != null)
            {
                onMessageReceived(sender, e);
            }
        }

        /// <summary>
        /// Called when all connections lost.
        /// </summary>
        protected virtual void OnAllConnectionsLost()
        {
            var onAllConnectionsLost = this.AllConnectionsLost;
            if (onAllConnectionsLost != null)
            {
                onAllConnectionsLost(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Checks the connection lost.
        /// </summary>
        private void CheckConnectionLost()
        {
            if (this.OpenServers.Count == 0 && this.OpenClients.Count == 0)
            {
                // connection lost
                this.OnAllConnectionsLost();

                this.ServerManagerInstance.UpdateServerConnectionLost();
            }
        }

        /// <summary>
        /// Creates the network client.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <returns>
        /// The new generated CalcItNetworkClient.
        /// </returns>
        private CalcItNetworkClient<CalcItServerMessage> CreateClient(ConnectionEndpoint endpoint)
        {
            return new CalcItNetworkClient<CalcItServerMessage>()
            {
                Logger = this.Logger,
                ClientConnector =
                    new TcpClientConnector<CalcItServerMessage>()
                    {
                        ConnectionSettings = endpoint,
                        Logger = this.Logger,
                        MessageTransformer = new DataContractTransformer<CalcItServerMessage>()
                    },
            };
        }

        /// <summary>
        /// Creates the endpoint.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="ConnectionEndpoint"/>.
        /// </returns>
        private ConnectionEndpoint CreateEndpoint(string connection)
        {
            // string def;
            ConnectionEndpoint endpoint = null;

            // if (connection.StartsWith("ip:"))
            // {
            // (def = connection.Replace("ip:", string.Empty);
            string[] parts = connection.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                endpoint = new IpConnectionEndpoint() { Hostname = parts[0], Port = Convert.ToInt32(parts[1]) };
            }
            else
            {
                this.LogMessage(
                    new LogMessage(
                        LogMessageType.Error,
                        string.Format("Connection definition {0} is invalid!", connection)));
            }

            // }
            // else if (connection.StartsWith("pipe:"))
            // {
            // def = connection.Replace("pipe:", string.Empty);
            // string[] parts = def.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            // if (parts.Length == 2)
            // {
            // endpoint = new PipeConnectionEndpoint() { Hostname = parts[0], PipeName = parts[1] };
            // }
            // else
            // {
            // this.LogMessage(
            // new LogMessage(LogMessageType.Error, String.Format("Connection definition {0} is invalid!", connection)));
            // }
            // }
            // else
            // {
            // this.LogMessage(
            // new LogMessage(LogMessageType.Error, String.Format("Connection definition {0} is invalid!", connection)));
            // }
            return endpoint;
        }

        /// <summary>
        /// Creates a new server network listener with the given endpoint.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <returns>
        /// The new generated network server.
        /// </returns>
        private CalcItNetworkServer<CalcItServerMessage> CreateServer(ConnectionEndpoint endpoint)
        {
            var server = new CalcItNetworkServer<CalcItServerMessage>()
            {
                Logger = this.Logger,
                ServerConnector = new TcpServerListener<CalcItServerMessage>()
                {
                    // only listener port needed
                    ConnectionSettings = endpoint,
                    Logger = this.Logger,
                    MessageTransformer = new DataContractTransformer<CalcItServerMessage>()
                },
            };

            server.StartQueueReceiver(typeof(Heartbeat), typeof(ConnectServer), typeof(SyncMessage));

            return server;
        }

        /// <summary>
        /// Exchanges the heartbeat.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <returns>
        /// The status of the send and receive or the heartbeat message.
        /// </returns>
        private async Task<bool> ExchangeHeartbeat(CalcItNetworkClient<CalcItServerMessage> client)
        {
            Heartbeat returnHeartbeat;
            int token = this.GenerateCheckToken();

            client.StartQueueReceiver(typeof(Heartbeat));

            client.Send(new Heartbeat() { CheckToken = token });

            try
            {
                var receivedMessage =
                    await
                    client.Receive(typeof(Heartbeat), new TimeSpan(0, 0, 0, this.Configuration.HeartbeatTime + 10));

                if (receivedMessage != null)
                {
                    returnHeartbeat = (Heartbeat)receivedMessage;
                }
                else
                {
                    this.LogMessage(
                        new LogMessage(LogMessageType.Debug, "Heartbeat Exchange - Heartbeat Timeout reached!"));

                    client.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(new LogMessage(ex));
                client.Close();

                return false;
            }

            if (returnHeartbeat.CheckToken != token)
            {
                client.Close();

                return false;
            }

            client.StopQueueReceiver();
            return true;
        }

        /// <summary>
        /// Exchanges the heartbeat.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <returns>
        /// The status of the heartbeat exchange.
        /// </returns>
        private async Task<bool> ExchangeHeartbeat(CalcItNetworkServer<CalcItServerMessage> server, Guid sessionId)
        {
            var receive = await server.Receive(sessionId, new TimeSpan(0, 0, 0, this.Configuration.HeartbeatTime + 10));

            if (receive == null)
            {
                return false;
            }

            if (receive is Heartbeat && receive.SessionId == sessionId)
            {
                server.Send(receive);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generates the check token.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/> token.
        /// </returns>
        private int GenerateCheckToken()
        {
            return new Random(DateTime.Now.Millisecond).Next(1, 10000);
        }

        /// <summary>
        /// Handles the server connection.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private async void HandleServerConnection(
            CalcItNetworkServer<CalcItServerMessage> server,
            ConnectServer message)
        {
            int heartbeatCounter;

            if (message.SessionId == null)
            {
                this.LogMessage(new LogMessage(LogMessageType.Debug, "Server Connect - SessionId invalid!"));
                return;
            }

            var clientSessionId = message.SessionId.Value;
            bool connectionOpen = false;

            // clear receive queue - incomming connect message
            server.ClearReceiveQueue(clientSessionId);

            connectionOpen = await this.InitialServerSync(server, clientSessionId);

            if (!connectionOpen)
            {
                this.LogMessage(
                    new LogMessage(
                        LogMessageType.Log,
                        string.Format("Connection {0} inital sync failed.", server.ServerConnector.ConnectionSettings.ToString())));
                return;
            }

            // Connection synced and open - session id present
            this.OpenServers.Add(server);

            this.LogMessage(
                new LogMessage(
                    LogMessageType.Log,
                    string.Format("Connection {0} is now open.", server.ServerConnector.ConnectionSettings.ToString())));

            // forward received messages - except heartbeat and sync
            server.MessageReceived += this.HandleMessageReceived;

            heartbeatCounter = this.Configuration.HeartbeatRetryCounter;

            // heartbeat keep alive
            while (connectionOpen)
            {
                if (!await this.ExchangeHeartbeat(server, clientSessionId))
                {
                    heartbeatCounter--;
                }
                else
                {
                    // reset counter
                    heartbeatCounter = this.Configuration.HeartbeatRetryCounter;

                    // Thread.Sleep(new TimeSpan(0, 0, 0, this.Configuration.HeartbeatTime));
                }

                if (heartbeatCounter == 0)
                {
                    connectionOpen = false;
                }
            }

            // Connection broken
            this.OpenServers.Remove(server);

            this.LogMessage(
                new LogMessage(
                    LogMessageType.Warning,
                    string.Format("Connection {0} is now closed.", server.ServerConnector.ConnectionSettings.ToString())));

            this.CheckConnectionLost();

            // forward received messages - except heartbeat and sync
            server.MessageReceived -= this.HandleMessageReceived;
        }

        /// <summary>
        /// Initials the client synchronize.
        /// </summary>
        /// <param name="client">
        /// The client parameter.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> status.
        /// </returns>
        private async Task<bool> InitialClientSync(CalcItNetworkClient<CalcItServerMessage> client)
        {
            ConnectServer returnedConnect;
            SyncMessage syncMessage;
            int token = this.GenerateCheckToken();

            // client connect
            client.Connect();

            client.StartQueueReceiver(typeof(ConnectServer), typeof(SyncMessage));

            // send connect server
            client.Send(new ConnectServer());

            try
            {
                var receivedMessage =
                    await
                    client.Receive(typeof(ConnectServer), new TimeSpan(0, 0, 0, this.Configuration.SyncTimeOut * 2));

                if (receivedMessage != null)
                {
                    returnedConnect = (ConnectServer)receivedMessage;
                }
                else
                {
                    this.LogMessage(
                        new LogMessage(LogMessageType.Debug, "Initial Sync - Connect Server Timeout reached!"));
                    client.Close();

                    return false;
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(new LogMessage(ex));
                client.Close();

                return false;
            }

            if (returnedConnect.SessionId == null)
            {
                this.LogMessage(
                    new LogMessage(LogMessageType.Debug, "Intial Sync - Connect Server SendBack - Session ID invalid."));
                client.Close();

                return false;
            }

            // Sync message exchange
            client.Send(
                new SyncMessage()
                {
                    RandomNumber = this.ServerManagerInstance.RandomToken,
                    ServerStartTime = this.ServerManagerInstance.StartTime,
                    SessionId = returnedConnect.SessionId
                });

            try
            {
                var receivedMessage =
                    await client.Receive(typeof(SyncMessage), new TimeSpan(0, 0, 0, this.Configuration.SyncTimeOut));

                if (receivedMessage != null)
                {
                    syncMessage = (SyncMessage)receivedMessage;
                }
                else
                {
                    this.LogMessage(new LogMessage(LogMessageType.Debug, "Initial Sync - SyncMessage Timeout reached!"));
                    client.Close();

                    return false;
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(new LogMessage(ex));
                client.Close();

                return false;
            }

            client.StopQueueReceiver();

            this.ServerManagerInstance.UpdateServerState(syncMessage);

            return true;
        }

        /// <summary>
        /// Initializes the server connections.
        /// </summary>
        private void InitializeServerConnections()
        {
            this.OpenClients = new List<CalcItNetworkClient<CalcItServerMessage>>();
            this.OpenServers = new List<CalcItNetworkServer<CalcItServerMessage>>();

            // this.DisconnectedClients = new List<CalcItNetworkClient<CalcItServerMessage>>();
            // this.DisconnectedServers = new List<CalcItNetworkServer<CalcItServerMessage>>();
        }

        /// <summary>
        /// Initials the server synchronize.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <returns>
        /// The status of the initial sync.
        /// </returns>
        private async Task<bool> InitialServerSync(CalcItNetworkServer<CalcItServerMessage> server, Guid sessionId)
        {
            SyncMessage syncMessage;
            int token = this.GenerateCheckToken();

            // heartbeat exchange
            try
            {
                server.Send(new ConnectServer() { SessionId = sessionId, });
            }
            catch (Exception ex)
            {
                this.LogMessage(new LogMessage(ex));

                return false;
            }

            // sync message exchange
            try
            {
                syncMessage =
                    (SyncMessage)
                    await server.Receive(sessionId, new TimeSpan(0, 0, 0, this.Configuration.SyncTimeOut * 2));

                if (syncMessage == null)
                {
                    return false;
                }

                //// server state update not here - time between received and send important
                server.Send(
                    new SyncMessage()
                    {
                        RandomNumber = this.ServerManagerInstance.RandomToken,
                        ServerStartTime = this.ServerManagerInstance.StartTime,
                        SessionId = sessionId
                    });
            }
            catch (Exception ex)
            {
                this.LogMessage(new LogMessage(ex));

                return false;
            }

            this.ServerManagerInstance.UpdateServerState(syncMessage);

            return true;
        }

        /// <summary>
        /// Handles the client side.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        private async void RunHandleClient(ConnectionEndpoint endpoint)
        {
            int heartbeatCounter;

            bool connectionSynced = false;
            CalcItNetworkClient<CalcItServerMessage> client = null;

            while (this.IsRunning)
            {
                while (!connectionSynced)
                {
                    client = this.CreateClient(endpoint);
                    connectionSynced = await this.InitialClientSync(client);

                    if (!connectionSynced)
                    {
                        Thread.Sleep(new TimeSpan(0, 0, this.Configuration.ReconnectServerConnectionTime, 0));
                    }
                }

                // Connection synced and open
                this.OpenClients.Add(client);
                this.LogMessage(
                    new LogMessage(
                        LogMessageType.Log,
                        string.Format("Connection {0} is now open.", endpoint.ToString())));

                // forward received messages - except heartbeat and sync
                client.MessageReceived += this.HandleMessageReceived;

                heartbeatCounter = this.Configuration.HeartbeatRetryCounter;

                // heartbeat keep alive
                while (connectionSynced)
                {
                    if (!await this.ExchangeHeartbeat(client))
                    {
                        heartbeatCounter--;
                    }
                    else
                    {
                        heartbeatCounter = this.Configuration.HeartbeatRetryCounter;

                        // Thread.Sleep(new TimeSpan(0, 0, 0, this.Configuration.HeartbeatTime));
                    }

                    if (heartbeatCounter == 0)
                    {
                        connectionSynced = false;
                    }
                }

                // Connection broken
                this.OpenClients.Remove(client);
                this.LogMessage(
                    new LogMessage(
                        LogMessageType.Warning,
                        string.Format("Connection {0} is now closed.", endpoint.ToString())));

                this.CheckConnectionLost();

                // forward received messages - except heartbeat and sync
                client.MessageReceived -= this.HandleMessageReceived;
            }
        }

        /// <summary>
        /// Runs the handle server.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        private void RunHandleServer(ConnectionEndpoint endpoint)
        {
            Queue<CalcItServerMessage> messageInputQueue = new Queue<CalcItServerMessage>();
            CalcItNetworkServer<CalcItServerMessage> server = null;

            server = this.CreateServer(endpoint);

            server.MessageReceived += (sender, e) =>
                {
                    if (e.Message is ConnectServer)
                    {
                        messageInputQueue.Enqueue(e.Message);
                    }
                };

            server.Start();

            while (this.IsRunning)
            {
                while (messageInputQueue.Count == 0)
                {
                    Thread.Sleep(100);
                }

                Task.Run(() => this.HandleServerConnection(server, (ConnectServer)messageInputQueue.Dequeue()));
            }

            server.Stop();
        }

        /// <summary>
        /// Runs the watcher.
        /// </summary>
        private void RunWatcher()
        {
            // Start all servers
            if (this.Configuration.ServerListeners != null && this.Configuration.ServerListeners.Count > 0)
            {
                foreach (int listenerPort in this.Configuration.ServerListeners)
                {
                    ConnectionEndpoint endpoint = new IpConnectionEndpoint() { Port = listenerPort };

                    Task.Run(() => this.RunHandleServer(endpoint));
                }
            }

            // run client to server connections
            if (this.Configuration.ServerConnections != null && this.Configuration.ServerConnections.Count > 0)
            {
                foreach (string connection in this.Configuration.ServerConnections)
                {
                    ConnectionEndpoint endpoint = this.CreateEndpoint(connection);

                    if (endpoint != null)
                    {
                        Task.Run(() => this.RunHandleClient(endpoint));
                    }
                }
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
        /// Handles the message received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MessageReceivedEventArgs{CalcItServerMessage}"/> instance containing the event data.
        /// </param>
        private void HandleMessageReceived(object sender, MessageReceivedEventArgs<CalcItServerMessage> e)
        {
            // log protocol message 
            this.LogMessage(new LogProtocolMessage(e.Message));

            // do not forward heartbeat and syncmessage
            if (e.Message is SyncMessage || e.Message is Heartbeat || e.Message is ConnectServer)
            {
                Debug.WriteLine("{0} received.", e.Message.GetType().Name);
                return;
            }

            this.OnMessageReceived(sender, e);
        }
    }
}