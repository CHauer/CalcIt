// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Monitor - Program.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Monitor
{
    using System;
    using System.Linq;

    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.Monitor;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Tcp;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Monitor.Properties;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The exit flag.
        /// </summary>
        private static bool exit = false;

        /// <summary>
        /// The logger.
        /// </summary>
        private static Logger logger;

        /// <summary>
        /// The monitor command executor.
        /// </summary>
        private static CommandExecutor<CalcItMonitorMessage> monitorCommmandExecutor;

        /// <summary>
        /// The monitor manager.
        /// </summary>
        private static MonitorManager monitorManager;

        /// <summary>
        /// The monitor client.
        /// </summary>
        private static CalcItNetworkClient<CalcItMonitorMessage> monitorNetworkClient;

        /// <summary>
        /// The server hostname.
        /// </summary>
        private static string serverHostname;

        /// <summary>
        /// The server port.
        /// </summary>
        private static int serverPort;

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">
        /// The args array.
        /// </param>
        public static void Main(string[] args)
        {
            HandleArguments(args);
            PrepareConsoleWindow();

            InitializeLogger();

            monitorManager = new MonitorManager() { Logger = logger };

            ConnectMonitor();

            while (!exit)
            {
                string input = Console.ReadLine();
                exit = input != null && input.ToLower().Trim().Equals("exit");
            }

            EndMonitor();
        }

        /// <summary>
        /// Connects the monitor.
        /// </summary>
        private static async void ConnectMonitor()
        {
            bool connected = false;

            while (!connected)
            {
                InitializeNetworkAccess();
                InitializeCommandExecutor();

                monitorCommmandExecutor.StartExecutor();

                monitorManager.NetworkAccess = monitorNetworkClient;
                connected = await monitorManager.ConnectToServer();
            }
        }

        /// <summary>
        /// Ends the monitor.
        /// </summary>
        private static void EndMonitor()
        {
            monitorNetworkClient.Close();
            monitorCommmandExecutor.StopExecutor();
        }

        /// <summary>
        /// Handles the arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        private static void HandleArguments(string[] args)
        {
            // Standard settings
            serverHostname = "localhost";
            serverPort = 50210;

            if (args.Any(arg => arg.Equals("/h")))
            {
                Console.WriteLine(Resources.Help);
            }

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
        /// Initializes the logger.
        /// </summary>
        private static void InitializeLogger()
        {
            logger = new Logger();
            logger.AddListener(new ConsoleLogListener());
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
                        ConnectionSettings = new IpConnectionEndpoint() { Hostname = serverHostname, Port = serverPort }
                    }
            };

            monitorNetworkClient.Connect();
        }

        /// <summary>
        /// Prepares the console window.
        /// </summary>
        private static void PrepareConsoleWindow()
        {
            Console.Title = Resources.ConsoleHeader;
            Console.WriteLine(Resources.StartUpText);
        }
    }
}