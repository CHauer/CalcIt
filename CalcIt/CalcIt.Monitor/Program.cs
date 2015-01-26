using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcIt.Monitor
{
    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.Monitor;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Tcp;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Lib.NetworkAccess.Udp;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;

    public class Program
    {
        /// <summary>
        /// The monitor commmand executor.
        /// </summary>
        private static CommandExecutor<CalcItMonitorMessage> monitorCommmandExecutor;

        /// <summary>
        /// The monitor client
        /// </summary>
        private static CalcItNetworkClient<CalcItMonitorMessage> monitorNetworkClient;

        /// <summary>
        /// The monitor manager.
        /// </summary>
        private static MonitorManager monitorManager;

        /// <summary>
        /// The server hostname
        /// </summary>
        private static string serverHostname;

        /// <summary>
        /// The server port
        /// </summary>
        private static int serverPort;

        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger;

        public static void Main(string[] args)
        {
            bool exit = false;

            HandleArguments(args);

            InitializeLogger();

            monitorManager = new MonitorManager();

            InitializeNetworkAccess();
            InitializeCommandExecutor();

            monitorManager.NetworkAccess = monitorNetworkClient;
            monitorManager.ConnectToServer();

            while (!exit)
            {
                string input = Console.ReadLine();
                exit = input != null && input.ToLower().Trim().Equals("exit");
            }
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
        /// Initializes the command executor.
        /// </summary>
        private static void InitializeCommandExecutor()
        {
            monitorCommmandExecutor = new CommandExecutor<CalcItMonitorMessage>()
            {
                Logger = logger,
                MethodProvider = monitorManager,
                NetworkAccess = monitorNetworkClient
            };
        }

        /// <summary>
        /// Handles the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void HandleArguments(string[] args)
        {
            //Standard settings
            serverHostname = "localhost";
            serverPort = 50210;

            if (args.Length == 2)
            {
                serverHostname = args[0].Trim();

                try
                {
                    serverPort = Convert.ToInt32(args[1].Trim());
                }
                catch (Exception ex)
                {
                    serverPort = 50210;
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Initializes the network access.
        /// </summary>
        private static void InitializeNetworkAccess()
        {
            monitorNetworkClient = new CalcItNetworkClient<CalcItMonitorMessage>()
            {
                ClientConnector =
                     new TcpClientConnector<CalcItMonitorMessage>()
                     {
                         MessageTransformer = new DataContractTransformer<CalcItMonitorMessage>(),
                         ConnectionSettings =
                             new IpConnectionEndpoint()
                             {
                                 Hostname = serverHostname,
                                 Port = serverPort
                             }
                     }
            };

            monitorNetworkClient.Connect();
        }
    }
}
