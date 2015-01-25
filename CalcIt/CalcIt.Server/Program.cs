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

    using CalcIt.Lib;
    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Tcp;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Lib.NetworkAccess.Udp;
    using CalcIt.Lib.Server;
    using CalcIt.Lib.Server.Configuration;
    using CalcIt.Lib.Server.Watcher;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Monitor;
    using CalcIt.Protocol.Server;
    using CalcIt.Protocol.Session;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The _connection watcher.
        /// </summary>
        private static ServerConnectionWatcher connectionWatcher;

        /// <summary>
        /// The _game client commmand executor.
        /// </summary>
        private static CommandExecutor<CalcItClientMessage> gameClientCommmandExecutor;

        /// <summary>
        /// The _game client connection server.
        /// </summary>
        private static CalcItNetworkServer<CalcItClientMessage> gameClientConnectionServer;

        /// <summary>
        /// The _monitor client commmand executor.
        /// </summary>
        private static CommandExecutor<CalcItMonitorMessage> monitorClientCommmandExecutor;

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
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            serverManager =
                new ServerManager(
                    new XmlConfigurationSerializer<ServerConfiguration>()
                    {
                        ConfigurationFile = "ServerConfiguration.xml"
                    });

            InitializeNetworkAccess();
            InitializeCommandExecutor();

            connectionWatcher = new ServerConnectionWatcher(serverManager.Configuration);

            // the command executor for server to server messages
            connectionWatcher.ServerCommandExecutor = serverCommmandExecutor;
            connectionWatcher.Start();

            // Start Command execution - handling input
            gameClientCommmandExecutor.StartExecutor();
            monitorClientCommmandExecutor.StartExecutor();

            // start receiving input
            gameClientConnectionServer.Start();
            monitorClientConnectionServer.Start();

            // Server end
            Console.Write("Enter for Server End");
            Console.ReadLine();

            EndServer();
        }

        /// <summary>
        /// Ends the server.
        /// </summary>
        private static void EndServer()
        {
            gameClientConnectionServer.Stop();
            monitorClientConnectionServer.Stop();

            gameClientCommmandExecutor.StopExecutor();
            monitorClientCommmandExecutor.StopExecutor();

            connectionWatcher.Stop();
        }

        /// <summary>
        /// The initialize command executor.
        /// </summary>
        private static void InitializeCommandExecutor()
        {
            gameClientCommmandExecutor = new CommandExecutor<CalcItClientMessage>()
            {
                MethodProvider = serverManager, 
                NetworkAccess = gameClientConnectionServer
            };

            monitorClientCommmandExecutor = new CommandExecutor<CalcItMonitorMessage>()
            {
                MethodProvider = serverManager, 
                NetworkAccess = monitorClientConnectionServer
            };

            // No network access set - gets set by connection watcher 
            // after inital server active/passive sync
            serverCommmandExecutor = new CommandExecutor<CalcItServerMessage>() { MethodProvider = serverManager };
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
                            new IpConnectionEndpoint() { Port = serverManager.Configuration.GameClientServerPort }
                    }
            };

            monitorClientConnectionServer = new CalcItNetworkServer<CalcItMonitorMessage>()
            {
                ServerConnector =
                    new TcpServerListener<CalcItMonitorMessage>()
                    {
                        MessageTransformer = new DataContractTransformer<CalcItMonitorMessage>(), 
                        ConnectionSettings =
                            new IpConnectionEndpoint() { Port = serverManager.Configuration.MonitorClientServerPort }
                    }
            };
        }
    }
}