using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.Server.Configuration;
using CalcIt.Lib.Server.Watcher;

namespace CalcIt.Lib.Server
{
    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Monitor;
    using CalcIt.Protocol.Server;

    public class ServerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerManager"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <exception cref="System.ArgumentNullException">configurationManager</exception>
        public ServerManager(IConfigurationManager<ServerConfiguration> configurationManager)
        {
            this.IsActiveServer = true;

            StartTime = DateTime.Now;
            RandomToken = GenerateCheckToken();

            if (configurationManager == null)
            {
                throw new ArgumentNullException("configurationManager");
            }

            this.ConfigurationManager = configurationManager;
            InitializeConfiguration();
        }

        /// <summary>
        /// Generates the check token.
        /// </summary>
        /// <returns></returns>
        private int GenerateCheckToken()
        {
            return new Random(DateTime.Now.Millisecond).Next(1, 1000000);
        }

        /// <summary>
        /// Initializes the configuration.
        /// </summary>
        private void InitializeConfiguration()
        {
            try
            {
                Configuration = ConfigurationManager.LoadConfiguration();
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));

                //Standarad Configuration
                Configuration = new ServerConfiguration();

                // save standard configuration to file
                ConfigurationManager.SaveConfiguration(Configuration);
            }
        }

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        /// <value>
        /// The configuration manager.
        /// </value>
        public IConfigurationManager<ServerConfiguration> ConfigurationManager { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ServerConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the game clients.
        /// </summary>
        /// <value>
        /// The game clients.
        /// </value>
        public List<GameClient> GameClients { get; private set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the random token.
        /// </summary>
        /// <value>
        /// The random token.
        /// </value>
        public int RandomToken { get; private set; }

        /// <summary>
        /// Gets or sets the network access.
        /// </summary>
        /// <value>
        /// The network access.
        /// </value>
        public INetworkAccess<CalcItClientMessage> GameClientNetworkAccess { get; set; }

        /// <summary>
        /// Gets or sets the monitor client network access.
        /// </summary>
        /// <value>
        /// The monitor client network access.
        /// </value>
        public INetworkAccess<CalcItMonitorMessage> MonitorClientNetworkAccess { get; set; }

        /// <summary>
        /// Gets or sets the server network access.
        /// </summary>
        /// <value>
        /// The server network access.
        /// </value>
        public INetworkAccess<CalcItServerMessage> ServerNetworkAccess { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is active server.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is active server; otherwise, <c>false</c>.
        /// </value>
        public bool IsActiveServer { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILog Logger { get; set; }

        [CommandHandler(typeof(ConnectClient))]
        public void AddGameClient(CalcItMessage message)
        {
            throw new System.NotImplementedException();
        }

        [CommandHandler(typeof(StartGame))]
        public void StartClientGame(CalcItMessage message)
        {
            throw new System.NotImplementedException();
        }

        [CommandHandler(typeof(Answer))]
        public void HandleAnswer(CalcItMessage message)
        {
            throw new System.NotImplementedException();
        }

        [CommandHandler(typeof(HighscoreRequest))]
        public void HandleHighscoreRequest(CalcItMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void SendHighscoreRespone()
        {

        }

        public void SendQuestion()
        {

        }

        public void SendEndGame()
        {

        }

        [CommandHandler(typeof(TunnelMessage))]
        public void HandleTunnelMessage(CalcItMessage tunnelMessage)
        {
        }

        [CommandHandler(typeof(Merge))]
        public void HandleMergeMessage(CalcItMessage mergeMessage)
        {
        }

        [CommandHandler(typeof(ConnectMonitor))]
        public void HandleConnectMonitor(CalcItMessage message)
        {
            MonitorClientNetworkAccess.Send(new MonitorOperationStatus()
            {
                Status = Protocol.Data.StatusType.Ok,
                Message = "Monitor connected!",
                SessionId = message.SessionId
            });

            LogMessage(new LogMessage("Monitor connected!"));
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
        /// Updates the state of the server.
        /// </summary>
        /// <param name="syncMessage">The synchronize message.</param>
        public void UpdateServerState(SyncMessage syncMessage)
        {
            if (syncMessage == null)
            {
                return;
            }

            if (syncMessage.ServerStartTime > this.StartTime)
            {
                this.IsActiveServer = false;
            }
            else if (syncMessage.ServerStartTime == this.StartTime)
            {
                if (syncMessage.RandomNumber > this.RandomToken)
                {
                    this.IsActiveServer = false;
                }
                else
                {
                    this.IsActiveServer = true;
                }
            }
            else
            {
                this.IsActiveServer = true;
            }

            LogMessage(new LogMessage(string.Format("Sync Message received - server state is {0}. Received StartTime:{1:g} This Server StartTime: {2:g}",
                this.IsActiveServer ? "Active" : "Passiv", syncMessage.ServerStartTime, this.StartTime)));

        }

        /// <summary>
        /// Updates the server connection lost.
        /// </summary>
        public void UpdateServerConnectionLost()
        {
            this.IsActiveServer = true;

            LogMessage(new LogMessage(string.Format("Alle Server connections lost. - Current server state is {0}.", this.IsActiveServer ? "Active" : "Passiv")));
        }
    }
}
