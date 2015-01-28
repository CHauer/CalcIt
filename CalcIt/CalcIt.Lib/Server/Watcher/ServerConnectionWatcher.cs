using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.CommandExecution;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Protocol.Server;

namespace CalcIt.Lib.Server.Watcher
{
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Tcp;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Lib.Server.Configuration;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The Server Connection watcher.
    /// Server are used to receive messages.
    /// Clients are used to send messages to servers.
    /// </summary>
    public class ServerConnectionWatcher : INetworkAccess<CalcItServerMessage>
    {
        /// <summary>
        /// The message forward queue
        /// </summary>
        private Queue<CalcItServerMessage> messageForwardQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnectionWatcher" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="logger">The logger.</param>
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
        /// Gets the server manager instance.
        /// </summary>
        /// <value>
        /// The server manager instance.
        /// </value>
        public ServerManager ServerManagerInstance { get; private set; }

        /// <summary>
        /// Gets the server connections.
        /// </summary>
        /// <value>
        /// The server connections.
        /// </value>
        public List<CalcItNetworkServer<CalcItServerMessage>> OpenServers { get; private set; }

        /// <summary>
        /// Gets the server client connections.
        /// </summary>
        /// <value>
        /// The server client connections.
        /// </value>
        public List<CalcItNetworkClient<CalcItServerMessage>> OpenClients { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is logging received messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is log received messages; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogReceivedMessages { get; set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ServerConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Occurs when all connections lost.
        /// </summary>
        public event EventHandler AllConnectionsLost;

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
        /// Creates the endpoint.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        private ConnectionEndpoint CreateEndpoint(string connection)
        {
            //string def;
            ConnectionEndpoint endpoint = null;

            //if (connection.StartsWith("ip:"))
            //{
            //(def = connection.Replace("ip:", string.Empty);
            string[] parts = connection.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                endpoint = new IpConnectionEndpoint() { Hostname = parts[0], Port = Convert.ToInt32(parts[1]) };
            }
            else
            {
                this.LogMessage(
                    new LogMessage(LogMessageType.Error, String.Format("Connection definition {0} is invalid!", connection)));
            }
            //}
            //else if (connection.StartsWith("pipe:"))
            //{
            //    def = connection.Replace("pipe:", string.Empty);
            //    string[] parts = def.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            //    if (parts.Length == 2)
            //    {
            //        endpoint = new PipeConnectionEndpoint() { Hostname = parts[0], PipeName = parts[1] };
            //    }
            //    else
            //    {
            //        this.LogMessage(
            //            new LogMessage(LogMessageType.Error, String.Format("Connection definition {0} is invalid!", connection)));
            //    }
            //}
            //else
            //{
            //    this.LogMessage(
            //        new LogMessage(LogMessageType.Error, String.Format("Connection definition {0} is invalid!", connection)));
            //}

            return endpoint;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            IsRunning = true;

            Task.Run(() => this.RunWatcher());
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            OpenClients.ForEach(con => con.Close());
            OpenServers.ForEach(server => server.Stop());
        }

        /// <summary>
        /// Runs the watcher.
        /// </summary>
        private void RunWatcher()
        {
            //Start all servers
            if (Configuration.ServerListeners != null && Configuration.ServerListeners.Count > 0)
            {
                foreach (int listenerPort in Configuration.ServerListeners)
                {
                    ConnectionEndpoint endpoint = new IpConnectionEndpoint() { Port = listenerPort };

                    Task.Run(() => this.RunHandleServer(endpoint));
                }
            }

            // run client to server connections
            if (Configuration.ServerConnections != null && Configuration.ServerConnections.Count > 0)
            {
                foreach (string connection in Configuration.ServerConnections)
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
        /// Handles the client side.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        private async void RunHandleClient(ConnectionEndpoint endpoint)
        {
            int heartbeatCounter;

            bool connectionSynced = false;
            CalcItNetworkClient<CalcItServerMessage> client = null;

            while (this.IsRunning)
            {
                while (!connectionSynced)
                {
                    client = CreateClient(endpoint);
                    connectionSynced = await this.InitialClientSync(client);
                    Thread.Sleep(new TimeSpan(0, 0, Configuration.ReconnectServerConnectionTime, 0));
                }

                // Connection synced and open
                OpenClients.Add(client);
                LogMessage(new LogMessage(LogMessageType.Log, String.Format("Connection {0} is now open.", endpoint.ToString())));

                // forward received messages - except heartbeat and sync
                client.MessageReceived += HandleMessageReceived;

                heartbeatCounter = Configuration.HeartbeatRetryCounter;

                // heartbeat keep alive
                while (connectionSynced)
                {
                    if (!await ExchangeHeartbeat(client))
                    {
                        heartbeatCounter--;
                    }
                    else
                    {
                        heartbeatCounter = Configuration.HeartbeatRetryCounter;
                        Thread.Sleep(new TimeSpan(0, 0, 0, Configuration.HeartbeatTime));
                    }

                    if (heartbeatCounter == 0)
                    {
                        connectionSynced = false;
                    }
                }

                // Connection broken
                OpenClients.Remove(client);
                LogMessage(new LogMessage(LogMessageType.Warning, String.Format("Connection {0} is now closed.", endpoint.ToString())));

                CheckConnectionLost();

                // forward received messages - except heartbeat and sync
                client.MessageReceived -= HandleMessageReceived;
            }
        }

        /// <summary>
        /// Creates the network client.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>The new generated CalcItNetworkClient</returns>
        private CalcItNetworkClient<CalcItServerMessage> CreateClient(ConnectionEndpoint endpoint)
        {
            return new CalcItNetworkClient<CalcItServerMessage>()
            {
                Logger = this.Logger,
                ClientConnector = new TcpClientConnector<CalcItServerMessage>()
                {
                    ConnectionSettings = endpoint,
                    Logger = this.Logger,
                    MessageTransformer = new DataContractTransformer<CalcItServerMessage>()
                },
            };
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
        /// Initials the client synchronize.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        private async Task<bool> InitialClientSync(CalcItNetworkClient<CalcItServerMessage> client)
        {
            ConnectServer returnedConnect;
            SyncMessage syncMessage;
            int token = GenerateCheckToken();

            // client connect
            client.Connect();

            // send connect server
            client.Send(new ConnectServer());

            try
            {
                var receivedMessage = await client.Receive(typeof(ConnectServer), new TimeSpan(0, 0, 0, this.Configuration.SyncTimeOut * 2));

                if (receivedMessage != null)
                {
                    returnedConnect = (ConnectServer)receivedMessage;
                }
                else
                {
                    LogMessage(new LogMessage(LogMessageType.Debug, "Initial Sync - Connect Server Timeout reached!"));
                    client.Close();

                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
                client.Close();

                return false;
            }

            if (returnedConnect.SessionId == null)
            {
                LogMessage(new LogMessage(LogMessageType.Debug, "Intial Sync - Connect Server SendBack - Session ID invalid."));
                client.Close();

                return false;
            }


            // Sync message exchange
            client.Send(new SyncMessage()
            {
                RandomNumber = ServerManagerInstance.RandomToken,
                ServerStartTime = ServerManagerInstance.StartTime,
                SessionId = returnedConnect.SessionId
            });

            try
            {
                var receivedMessage = await client.Receive(typeof(SyncMessage), new TimeSpan(0, 0, 0, this.Configuration.SyncTimeOut));

                if (receivedMessage != null)
                {
                    syncMessage = (SyncMessage)receivedMessage;
                }
                else
                {
                    LogMessage(new LogMessage(LogMessageType.Debug, "Initial Sync - SyncMessage Timeout reached!"));
                    client.Close();

                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
                client.Close();

                return false;
            }

            this.ServerManagerInstance.UpdateServerState(syncMessage);

            return true;
        }

        /// <summary>
        /// Runs the handle server.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        private void RunHandleServer(ConnectionEndpoint endpoint)
        {
            Queue<CalcItServerMessage> messageInputQueue = new Queue<CalcItServerMessage>();
            CalcItNetworkServer<CalcItServerMessage> server = null;

            server = CreateServer(endpoint);

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

                Task.Run(() => HandleServerConnection(server, (ConnectServer)messageInputQueue.Dequeue()));
            }

            server.Stop();
        }

        /// <summary>
        /// Handles the server connection.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="message">The message.</param>
        private async void HandleServerConnection(CalcItNetworkServer<CalcItServerMessage> server, ConnectServer message)
        {
            int heartbeatCounter;

            if (message.SessionId == null)
            {
                LogMessage(new LogMessage(LogMessageType.Debug, "Server Connect - SessionId invalid!"));
                return;
            }

            var clientSessionId = message.SessionId.Value;
            bool connectionOpen = false;

            connectionOpen = await this.InitialServerSync(server, clientSessionId);

            if (!connectionOpen)
            {
                LogMessage(new LogMessage(LogMessageType.Log, String.Format("Connection {0} inital sync failed.",
                    server.ServerConnector.ConnectionSettings.ToString())));
                return;

            }

            //Connection synced and open - session id present
            OpenServers.Add(server);

            LogMessage(new LogMessage(LogMessageType.Log, String.Format("Connection {0} is now open.",
                server.ServerConnector.ConnectionSettings.ToString())));

            // forward received messages - except heartbeat and sync
            server.MessageReceived += HandleMessageReceived;

            heartbeatCounter = Configuration.HeartbeatRetryCounter;

            //heartbeat keep alive
            while (connectionOpen)
            {
                if (!await ExchangeHeartbeat(server, clientSessionId))
                {
                    heartbeatCounter--;
                }
                else
                {
                    //reset counter
                    heartbeatCounter = Configuration.HeartbeatRetryCounter;
                    Thread.Sleep(new TimeSpan(0, 0, 0, Configuration.HeartbeatTime));
                }

                if (heartbeatCounter == 0)
                {
                    connectionOpen = false;
                }
            }

            // Connection broken
            OpenServers.Remove(server);

            LogMessage(new LogMessage(LogMessageType.Warning, String.Format("Connection {0} is now closed.",
                server.ServerConnector.ConnectionSettings.ToString())));

            CheckConnectionLost();

            // forward received messages - except heartbeat and sync
            server.MessageReceived -= HandleMessageReceived;
        }

        /// <summary>
        /// Initials the server synchronize.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>The status of the inital sync.</returns>
        private async Task<bool> InitialServerSync(CalcItNetworkServer<CalcItServerMessage> server, Guid sessionId)
        {
            SyncMessage syncMessage;
            int token = GenerateCheckToken();

            // heartbeat exchange
            try
            {
                server.Send(new ConnectServer()
                {
                    SessionId = sessionId,
                });
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));

                return false;
            }

            // sync message exchange
            try
            {
                syncMessage = (SyncMessage)await server.Receive(typeof(SyncMessage), new TimeSpan(0, 0, 0, Configuration.SyncTimeOut * 2));

                if (syncMessage == null)
                {
                    return false;
                }

                //// server state update not here - time between received and send important

                server.Send(new SyncMessage()
                {
                    RandomNumber = ServerManagerInstance.RandomToken,
                    ServerStartTime = ServerManagerInstance.StartTime,
                    SessionId = sessionId
                });
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));

                return false;
            }

            this.ServerManagerInstance.UpdateServerState(syncMessage);

            return true;
        }

        /// <summary>
        /// Creates a new server network listener with the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>The new generated network server.</returns>
        private CalcItNetworkServer<CalcItServerMessage> CreateServer(ConnectionEndpoint endpoint)
        {
            return new CalcItNetworkServer<CalcItServerMessage>()
            {
                Logger = this.Logger,
                ServerConnector = new TcpServerListener<CalcItServerMessage>()
                {
                    // only listener port needed
                    ConnectionSettings = endpoint,
                    Logger = Logger,
                    MessageTransformer = new DataContractTransformer<CalcItServerMessage>()
                },
            };
        }

        /// <summary>
        /// Exchanges the heartbeat.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>The status of the send and receive or the heartbeat message.</returns>
        private async Task<bool> ExchangeHeartbeat(CalcItNetworkClient<CalcItServerMessage> client)
        {
            Heartbeat returnHeartbeat;
            int token = this.GenerateCheckToken();

            client.Send(new Heartbeat() { CheckToken = token });

            try
            {
                var receivedMessage = await client.Receive(typeof(Heartbeat), new TimeSpan(0, 0, 0, this.Configuration.HeartbeatTime * 2));

                if (receivedMessage != null)
                {
                    returnHeartbeat = (Heartbeat)receivedMessage;
                }
                else
                {
                    LogMessage(new LogMessage(LogMessageType.Debug, "Heartbeat Exchange - Heartbeat Timeout reached!"));
                    client.Close();

                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
                client.Close();

                return false;
            }

            if (returnHeartbeat.CheckToken != token)
            {
                client.Close();

                return false;
            }

            return true;
        }


        /// <summary>
        /// Exchanges the heartbeat.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>The status of the hreatbeat exchange.</returns>
        private async Task<bool> ExchangeHeartbeat(CalcItNetworkServer<CalcItServerMessage> server, Guid sessionId)
        {
            var receive = await server.Receive(typeof(Heartbeat), new TimeSpan(0, 0, 0, Configuration.HeartbeatTime * 2));

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
        /// <returns></returns>
        private int GenerateCheckToken()
        {
            return new Random(DateTime.Now.Millisecond).Next(1, 10000);
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
        /// The message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<CalcItServerMessage>> MessageReceived;

        /// <summary>
        /// Handles the message received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs{CalcItServerMessage}"/> instance containing the event data.</param>
        private void HandleMessageReceived(object sender, MessageReceivedEventArgs<CalcItServerMessage> e)
        {
            // log protocol message 
            LogMessage(new LogProtocolMessage(e.Message));

            // do not forward heartbeat and syncmessage
            if (e.Message is SyncMessage || e.Message is Heartbeat)
            {
                return;
            }

            OnMessageReceived(sender, e);
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.InvalidOperationException">No Server Connection available!</exception>
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs{CalcItServerMessage}"/> instance containing the event data.</param>
        protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs<CalcItServerMessage> e)
        {
            var onMessageReceived = this.MessageReceived;

            // ReSharper disable once UseNullPropagation
            if (onMessageReceived != null)
            {
                onMessageReceived(sender, e);
            }
        }
    }
}
