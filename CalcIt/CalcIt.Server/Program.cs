// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Server - Program.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Server
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using CalcIt.Lib;
    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.Monitor;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Tcp;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Lib.NetworkAccess.Udp;
    using CalcIt.Lib.Server;
    using CalcIt.Lib.Server.Configuration;
    using CalcIt.Lib.Server.Watcher;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Monitor;
    using CalcIt.Protocol.Server;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Server.Properties;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The configuration file
        /// </summary>
        private static string configurationFile;

        /// <summary>
        /// The _connection watcher.
        /// </summary>
        private static ServerConnectionWatcher connectionWatcher;

        /// <summary>
        /// The _game client commmand executor.
        /// </summary>
        private static CommandExecutor<CalcItClientMessage> gameCommmandExecutor;

        /// <summary>
        /// The _game client connection server.
        /// </summary>
        private static CalcItNetworkServer<CalcItClientMessage> gameClientConnectionServer;

        /// <summary>
        /// The _monitor client commmand executor.
        /// </summary>
        private static CommandExecutor<CalcItMonitorMessage> monitorCommmandExecutor;

        /// <summary>
        /// The _monitor client connection server.
        /// </summary>
        private static CalcItNetworkServer<CalcItMonitorMessage> monitorClientConnectionServer;

        /// <summary>
        /// The _server commmand executor.
        /// </summary>
        private static CommandExecutor<CalcItServerMessage> serverCommmandExecutor;

        /// <summary>
        /// The _server manager.
        /// </summary>
        private static ServerManager serverManager;

        /// <summary>
        /// The logger instance.
        /// </summary>
        private static Logger logger;

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            bool exit = false; 

            HandleArguments(args);

            // create server logger
            InitializeLogger();

            // Create server manager - logic
            InitializeServerManager();

            // Create servers for monitor and game clients and server connection watcher
            InitializeNetworkAccess();

            // Join logger and Listener for Monitors
            InitializeMonitorLogger();

            // Link server manager with send back network access instances
            serverManager.GameClientNetworkAccess = gameClientConnectionServer;
            serverManager.MonitorClientNetworkAccess = monitorClientConnectionServer;
            serverManager.ServerNetworkAccess = connectionWatcher;

            InitializeCommandExecutor();

            // Start Command execution - handling input
            gameCommmandExecutor.StartExecutor();
            monitorCommmandExecutor.StartExecutor();
            serverCommmandExecutor.StartExecutor();

            // start receiving input
            gameClientConnectionServer.Start();
            monitorClientConnectionServer.Start();

            // the connection watcher for server to server messages
             connectionWatcher.Start();

            // Server end
            PrepareConsoleWindow();
            while (!exit)
            {
                string input = Console.ReadLine();
                exit = input != null && input.ToLower().Trim().Equals("exit");
            }

            EndServer();
        }

        private static void PrepareConsoleWindow()
        {
            Console.Title = Resources.ConsoleHeader;
            Console.WriteLine(Resources.Enter_for_Server_End);
        }

        /// <summary>
        /// Initializes the monitor logger.
        /// </summary>
        private static void InitializeMonitorLogger()
        {
            logger.AddListener(new MonitorLogListener()
            {
                MonitorNetworkAccess = monitorClientConnectionServer
            });
        }

        /// <summary>
        /// Handles the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void HandleArguments(string[] args)
        {
            if (args.Any(a => a.Equals("-h") || a.Equals("/h")))
            {
                Console.WriteLine(Resources.Help);
                return;
            }

            if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    configurationFile = args[0];
                    return;
                }
            }

            // load standard config file
            configurationFile = ConfigurationManager.AppSettings["configuration"];
        }

        /// <summary>
        /// Initializes the server manager.
        /// </summary>
        private static void InitializeServerManager()
        {
            serverManager = new ServerManager(new XmlConfigurationSerializer<ServerConfiguration>()
                                            {
                                                ConfigurationFile = configurationFile
                                            })
            {
                Logger = logger
            };
        }

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        private static void InitializeLogger()
        {
            logger = new Logger();
            logger.AddListener(new ConsoleLogListener());
        }

        /// <summary>
        /// Ends the server.
        /// </summary>
        private static void EndServer()
        {
            gameClientConnectionServer.Stop();
            monitorClientConnectionServer.Stop();

            gameCommmandExecutor.StopExecutor();
            monitorCommmandExecutor.StopExecutor();

            connectionWatcher.Stop();
        }

        /// <summary>
        /// The initialize command executor.
        /// </summary>
        private static void InitializeCommandExecutor()
        {
            gameCommmandExecutor = new CommandExecutor<CalcItClientMessage>()
            {
                MethodProvider = serverManager,
                NetworkAccess = gameClientConnectionServer
            };
            serverManager.OnTunneldMessageReceived += (sender, e) => gameCommmandExecutor.HandleTunneldMessage(e.Message);

            monitorCommmandExecutor = new CommandExecutor<CalcItMonitorMessage>()
            {
                MethodProvider = serverManager,
                NetworkAccess = monitorClientConnectionServer
            };

            serverCommmandExecutor = new CommandExecutor<CalcItServerMessage>()
            {
                MethodProvider = serverManager,
                NetworkAccess = connectionWatcher
            };

        }

        /// <summary>
        /// The initialize network access.
        /// </summary>
        private static void InitializeNetworkAccess()
        {
            gameClientConnectionServer = new CalcItNetworkServer<CalcItClientMessage>()
            {
                ServerConnector =
                    new UdpServerListener<CalcItClientMessage>()
                    {
                        MessageTransformer = new DataContractTransformer<CalcItClientMessage>(),
                        ConnectionSettings =
                            new IpConnectionEndpoint() { Port = serverManager.Configuration.GameServerPort }
                    }
            };

            monitorClientConnectionServer = new CalcItNetworkServer<CalcItMonitorMessage>()
            {
                ServerConnector =
                    new TcpServerListener<CalcItMonitorMessage>()
                    {
                        MessageTransformer = new DataContractTransformer<CalcItMonitorMessage>(),
                        ConnectionSettings =
                            new IpConnectionEndpoint() { Port = serverManager.Configuration.MonitorServerPort }
                    }
            };

            gameClientConnectionServer.MessageReceived += LogMessageFromGameClientServer;
            gameClientConnectionServer.MessageSend += LogMessageSendFromGameClientServer;

            connectionWatcher = new ServerConnectionWatcher(serverManager, logger);
        }

        /// <summary>
        /// Logs the message from game client server.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs{CalcItClientMessage}"/> instance containing the event data.</param>
        private static void LogMessageFromGameClientServer(object sender, MessageReceivedEventArgs<CalcItClientMessage> e)
        {
            if (logger == null)
            {
                return;
            }

            logger.AddLogMessage(new LogProtocolMessage(e.Message, "Message received from GameClient."));
        }

        /// <summary>
        /// Logs the message send from game client server.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs{CalcItClientMessage}"/> instance containing the event data.</param>
        private static void LogMessageSendFromGameClientServer(object sender, MessageReceivedEventArgs<CalcItClientMessage> e)
        {
            if (logger == null)
            {
                return;
            }

            logger.AddLogMessage(new LogProtocolMessage(e.Message, "Message sent to GameClient.") { IsOutgoing = true });
        }

    }
}