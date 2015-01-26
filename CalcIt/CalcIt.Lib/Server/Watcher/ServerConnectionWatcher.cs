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
    /// Clients are used to send mesages to servers.
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

            IntializeServerConnections();
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
        /// Gets the server connections.
        /// </summary>
        /// <value>
        /// The server connections.
        /// </value>
        public List<CalcItNetworkServer<CalcItServerMessage>> DisconnectedServers { get; private set; }

        /// <summary>
        /// Gets the server client connections.
        /// </summary>
        /// <value>
        /// The server client connections.
        /// </value>
        public List<CalcItNetworkClient<CalcItServerMessage>> DisconnectedClients { get; private set; }

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
        /// Intializes the server connections.
        /// </summary>
        private void IntializeServerConnections()
        {
            this.OpenClients = new List<CalcItNetworkClient<CalcItServerMessage>>();
            this.OpenServers = new List<CalcItNetworkServer<CalcItServerMessage>>();

            this.DisconnectedClients = new List<CalcItNetworkClient<CalcItServerMessage>>();
            this.DisconnectedServers = new List<CalcItNetworkServer<CalcItServerMessage>>();

            foreach (string connection in Configuration.ServerConnections)
            {
                ConnectionEndpoint endpoint = this.CreateEndpoint(connection);

                if (endpoint != null)
                {
                    DisconnectedClients.Add(new CalcItNetworkClient<CalcItServerMessage>()
                    {
                        Logger = this.Logger,
                        ClientConnector = new TcpClientConnector<CalcItServerMessage>()
                        {
                            ConnectionSettings = endpoint,
                            Logger = Logger,
                            MessageTransformer = new DataContractTransformer<CalcItServerMessage>()
                        },
                    });
                }
            }

            foreach (int listenerPort in Configuration.ServerListeners)
            {
                DisconnectedServers.Add(new CalcItNetworkServer<CalcItServerMessage>()
                {
                    Logger = this.Logger,
                    ServerConnector = new TcpServerListener<CalcItServerMessage>()
                    {
                        // only listener port needed
                        ConnectionSettings = new IpConnectionEndpoint() { Port = listenerPort },
                        Logger = Logger,
                        MessageTransformer = new DataContractTransformer<CalcItServerMessage>()
                    },
                });
            }
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

            Task.Run(() => RunWatcher());
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
            DisconnectedServers.ForEach(server =>
            {
                Task.Run(() => this.RunHandleServer(server));
            });

            // run client to server connections
            DisconnectedClients.ForEach(client =>
            {
                Task.Run(() => this.RunHandleClient(client));
            });
        }

        /// <summary>
        /// Handles the client side.
        /// </summary>
        /// <param name="client">The client.</param>
        private async void RunHandleClient(CalcItNetworkClient<CalcItServerMessage> client)
        {
            bool connectionOpen = true;

            while (IsRunning)
            {
                while (!await this.InitialClientSync(client))
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5, 0));
                }

                // Connection synced and open
                DisconnectedClients.Remove(client);
                OpenClients.Add(client);

                // forward received messages - except heartbeat and sync
                client.MessageReceived += HandleMessageReceived;

                // heartbeat keep alive
                while (connectionOpen)
                {
                    connectionOpen = await ExchangeHeartbeat(client);
                }

                // Connection broken
                OpenClients.Remove(client);
                DisconnectedClients.Add(client);

                CheckConnectionLost();

                // forward received messages - except heartbeat and sync
                client.MessageReceived -= HandleMessageReceived;
            }
        }

        /// <summary>
        /// Checks the connection lost.
        /// </summary>
        private void CheckConnectionLost()
        {
            if (this.OpenServers.Count == 0 && this.OpenClients.Count == 0)
            {
                //connection lost
                OnAllConnectionsLost();

                ServerManagerInstance.UpdateServerConnectionLost();
            }
        }

        /// <summary>
        /// Exchanges the heartbeat.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>The status of the send and receive or the heartbeat message.</returns>
        private async Task<bool> ExchangeHeartbeat(CalcItNetworkClient<CalcItServerMessage> client)
        {
            Heartbeat returnHeartbeat;
            int token = GenerateCheckToken();

            client.Send(new Heartbeat() { CheckToken = token });

            try
            {
                var receivedMessage = await client.Receive(typeof(Heartbeat), new TimeSpan(0, 0, 0, this.Configuration.AnswerTimeout));

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
        /// Initials the client synchronize.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        private async Task<bool> InitialClientSync(CalcItNetworkClient<CalcItServerMessage> client)
        {
            Heartbeat returnHeartbeat;
            SyncMessage syncMessage;
            int token = GenerateCheckToken();

            client.Connect();

            client.Send(new Heartbeat() { CheckToken = token });

            try
            {
                var receivedMessage = await client.Receive(typeof(Heartbeat), new TimeSpan(0, 0, 0, this.Configuration.AnswerTimeout));

                if (receivedMessage != null)
                {
                    returnHeartbeat = (Heartbeat)receivedMessage;
                }
                else
                {
                    LogMessage(new LogMessage(LogMessageType.Debug, "Initial Sync - Heartbeat Timeout reached!"));
                    client.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                client.Close();
                return false;
            }

            if (returnHeartbeat.CheckToken != token)
            {
                client.Close();
                return false;
            }

            client.Send(new SyncMessage() { RandomNumber = ServerManagerInstance.RandomToken, ServerStartTime = ServerManagerInstance.StartTime });

            try
            {
                syncMessage = (SyncMessage)await client.Receive(typeof(SyncMessage), new TimeSpan(0, 0, 0, this.Configuration.AnswerTimeout));
            }
            catch (Exception ex)
            {
                client.Close();
                return false;
            }

            ServerManagerInstance.UpdateServerState(syncMessage);

            return true;
        }

        /// <summary>
        /// Runs the handle server.
        /// </summary>
        /// <param name="server">The server.</param>
        private async void RunHandleServer(CalcItNetworkServer<CalcItServerMessage> server)
        {
            Guid sessionId;
            bool connectionOpen = true;

            while (IsRunning)
            {
                while (!await this.InitialServerSync(server))
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }

                //Connection synced and open
                DisconnectedServers.Remove(server);
                OpenServers.Add(server);

                // forward received messages - except heartbeat and sync
                server.MessageReceived += HandleMessageReceived;

                //heartbeat keep alive
                while (connectionOpen)
                {
                    connectionOpen = await ExchangeHeartbeat(server);
                }

                //Connection broken
                DisconnectedServers.Remove(server);
                DisconnectedServers.Add(server);

                CheckConnectionLost();

                // forward received messages - except heartbeat and sync
                server.MessageReceived -= HandleMessageReceived;
            }

        }

        /// <summary>
        /// Exchanges the heartbeat.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <returns></returns>
        private async Task<bool> ExchangeHeartbeat(CalcItNetworkServer<CalcItServerMessage> server)
        {
            Heartbeat heartbeat;

            heartbeat = (Heartbeat)await server.Receive(typeof(Heartbeat));

            server.Send(heartbeat);

            return true;
        }

        /// <summary>
        /// Initials the server synchronize.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <returns></returns>
        private async Task<bool> InitialServerSync(CalcItNetworkServer<CalcItServerMessage> server)
        {
            Heartbeat heartbeat;
            SyncMessage syncMessage;
            int token = GenerateCheckToken();

            server.Start();

            try
            {
                heartbeat = (Heartbeat)await server.Receive(typeof(Heartbeat));

                server.Send(heartbeat);
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
                server.Stop();

                return false;
            }

            try
            {
                syncMessage = (SyncMessage)await server.Receive(typeof(SyncMessage));

                // server state update not here - time between received and send important

                server.Send(new SyncMessage()
                {
                    RandomNumber = ServerManagerInstance.RandomToken,
                    ServerStartTime = ServerManagerInstance.StartTime,
                    SessionId = syncMessage.SessionId
                });
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
                server.Stop();
                return false;
            }

            ServerManagerInstance.UpdateServerState(syncMessage);

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
            if (e.Message is SyncMessage || e.Message is Heartbeat)
                return;

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
