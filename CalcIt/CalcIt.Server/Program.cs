using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalcIt.Lib;
using CalcIt.Lib.CommandExecution;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Lib.NetworkAccess.Tcp;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Lib.NetworkAccess.Udp;
using CalcIt.Lib.Server;
using CalcIt.Lib.Server.Configuration;
using CalcIt.Lib.Server.Watcher;
using CalcIt.Protocol;
using CalcIt.Protocol.Client;
using CalcIt.Protocol.Monitor;
using CalcIt.Protocol.Server;

namespace CalcIt.Server
{
    public class Program
    {
        private static ServerManager _serverManager;

        private static ServerConnectionWatcher _connectionWatcher;

        private static CommandExecutor<CalcItServerMessage> _serverCommmandExecutor;

        private static CommandExecutor<CalcItClientMessage> _gameClientCommmandExecutor;

        private static CommandExecutor<CalcItMonitorMessage> _monitorClientCommmandExecutor;

        private static CalcItNetworkServer<CalcItClientMessage> _gameClientConnectionServer;

        private static CalcItNetworkServer<CalcItMonitorMessage> _monitorClientConnectionServer;

        public static void Main(string[] args)
        {
            _serverManager = new ServerManager(new XmlConfigurationSerializer<ServerConfiguration>()
            {
                ConfigurationFile = "ServerConfiguration.xml"
            });

            InitializeNetworkAccess();
            InitializeCommandExecutor();

            _connectionWatcher = new ServerConnectionWatcher(_serverManager.Configuration);

            //the command executor for server to server messages
            _connectionWatcher.ServerCommandExecutor = _serverCommmandExecutor;
            _connectionWatcher.Start();

            //Start Command execution - handling input
            _gameClientCommmandExecutor.StartExecutor();
            _monitorClientCommmandExecutor.StartExecutor();

            //start receiving input
            _gameClientConnectionServer.Start();
            _monitorClientConnectionServer.Start();

            //Server end
            Console.Write("Enter for Server End");
            Console.ReadLine();

            EndServer();
        }

        /// <summary>
        /// Ends the server.
        /// </summary>
        private static void EndServer()
        {
            _gameClientConnectionServer.Stop();
            _monitorClientConnectionServer.Stop();

            _gameClientCommmandExecutor.StopExecutor();
            _monitorClientCommmandExecutor.StopExecutor();

            _connectionWatcher.Stop();
        }

        private static void InitializeNetworkAccess()
        {
            _gameClientConnectionServer = new CalcItNetworkServer<CalcItClientMessage>()
            {
                ServerConnector = new UdpServerListener<CalcItClientMessage>()
                {
                    MessageTransformer = new DataContractTransformer<CalcItClientMessage>(),
                    ListenPort = _serverManager.Configuration.GameClientServerPort
                }
            };

            _monitorClientConnectionServer = new CalcItNetworkServer<CalcItMonitorMessage>()
            {
                ServerConnector = new TcpServerListener<CalcItMonitorMessage>()
                {
                    MessageTransformer = new DataContractTransformer<CalcItMonitorMessage>(),
                    ListenPort = _serverManager.Configuration.MonitorClientServerPort
                }
            };
        }

        private static void InitializeCommandExecutor()
        {
            _gameClientCommmandExecutor = new CommandExecutor<CalcItClientMessage>()
            {
                MethodProvider = _serverManager,
                NetworkAccess = _gameClientConnectionServer
            };

            _monitorClientCommmandExecutor = new CommandExecutor<CalcItMonitorMessage>()
            {
                MethodProvider = _serverManager,
                NetworkAccess = _monitorClientConnectionServer
            };

            // No network access set - gets set by connection watcher 
            // after inital server active/passive sync
            _serverCommmandExecutor = new CommandExecutor<CalcItServerMessage>()
            {
                MethodProvider = _serverManager
            };

        }
    }
}
