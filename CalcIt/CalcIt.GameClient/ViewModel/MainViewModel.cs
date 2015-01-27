// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.GameClient - MainViewModel.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.GameClient.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;

    using CalcIt.Lib.Client;
    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Lib.NetworkAccess.Transform;
    using CalcIt.Lib.NetworkAccess.Udp;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;

    using GalaSoft.MvvmLight;

    using Mutzl.MvvmLight;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private UdpClientConnector<CalcItClientMessage> udpClient;

        private CalcItNetworkClient<CalcItClientMessage> gameNetworkClient;

        private ConnectionEndpoint connectionEndpoint;

        private NetworkAccessType networkAccessType;

        /// <summary>
        /// 
        /// </summary>
        private enum NetworkAccessType
        {
            Udp,
            NamedPipes,
        }

        /// <summary>
        /// The is game running
        /// </summary>
        private bool isGameRunning;

        /// <summary>
        /// The connection error message
        /// </summary>
        private string connectionErrorMessage;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is game running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is game running; otherwise, <c>false</c>.
        /// </value>
        public bool IsGameRunning
        {
            get { return isGameRunning; }
            set
            {
                isGameRunning = value;
                RaisePropertyChanged(() => IsGameRunning);
                RaisePropertyChanged(() => IsChangeConnectionEnabled);
            }
        }

        /// <summary>
        /// The is change connection enabled
        /// </summary>
        private bool isChangeConnectionEnabled = true;

        /// <summary>
        /// Gets a value indicating whether [change connection enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [change connection enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool IsChangeConnectionEnabled
        {
            get { return isChangeConnectionEnabled; }
            set
            {
                this.isChangeConnectionEnabled = value;
                RaisePropertyChanged(() => IsChangeConnectionEnabled);
            }
        }

        /// <summary>
        /// The answer time left.
        /// </summary>
        private string answerTimeLeft;

        /// <summary>
        /// The calc operator.
        /// </summary>
        private string calcOperator;

        /// <summary>
        /// The command start game
        /// </summary>
        private ICommand commandEndGame;

        /// <summary>
        /// The command refresh high score
        /// </summary>
        private ICommand commandRefreshHighScore;

        /// <summary>
        /// The command send answer
        /// </summary>
        private ICommand commandSendAnswer;

        /// <summary>
        /// The command start game
        /// </summary>
        private ICommand commandStartGame;

        /// <summary>
        /// The current answer
        /// </summary>
        private string currentAnswer;

        /// <summary>
        /// The game client manager
        /// </summary>
        private GameClientManager gameClientManager;

        /// <summary>
        /// The game command executor
        /// </summary>
        private CommandExecutor<CalcItClientMessage> gameCommandExecutor;

        /// <summary>
        /// The high score.
        /// </summary>
        private ObservableCollection<HighScoreItem> highScore;

        /// <summary>
        /// The hostname.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// The last log message.
        /// </summary>
        private string lastLogMessage;

        /// <summary>
        /// The logger
        /// </summary>
        private Logger logger;

        /// <summary>
        /// The number a.
        /// </summary>
        private string numberA;

        /// <summary>
        /// The number b.
        /// </summary>
        private string numberB;

        /// <summary>
        /// The username.
        /// </summary>
        private string username;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            this.InitializeLogger();
            this.InitializeCommands();

            this.InitializeGameClient();
        }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public string Answer
        {
            get
            {
                return this.currentAnswer;
            }

            set
            {
                this.currentAnswer = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the answer time left.
        /// </summary>
        public String AnswerTimeLeft
        {
            get
            {
                return this.answerTimeLeft;
            }

            set
            {
                this.answerTimeLeft = value;
                this.RaisePropertyChanged(() => this.AnswerTimeLeft);
            }
        }

        /// <summary>
        /// Gets or sets the calc operator.
        /// </summary>
        public string CalcOperator
        {
            get
            {
                return this.calcOperator;
            }

            set
            {
                this.calcOperator = value;
                this.RaisePropertyChanged(() => this.CalcOperator);
            }
        }

        /// <summary>
        /// Gets or sets the end game.
        /// </summary>
        /// <value>
        /// The end game.
        /// </value>
        public ICommand EndGame
        {
            get
            {
                return this.commandEndGame;
            }

            set
            {
                this.commandEndGame = value;
                this.RaisePropertyChanged(() => this.EndGame);
            }
        }

        /// <summary>
        /// Gets the high score list.
        /// </summary>
        public ObservableCollection<HighScoreItem> HighScoreList
        {
            get
            {
                return this.highScore;
            }

            private set
            {
                this.highScore = value;
                this.RaisePropertyChanged(() => this.HighScoreList);
            }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }

            set
            {
                this.connectionString = value;
                this.RaisePropertyChanged(() => this.ConnectionString);
            }
        }

        /// <summary>
        /// Gets the color of the log message.
        /// </summary>
        /// <value>
        /// The color of the log message.
        /// </value>
        public Color LogMessageColor { get; private set; }

        /// <summary>
        /// Gets the last log message.
        /// </summary>
        /// <value>
        /// The last log message.
        /// </value>
        public string LastLogMessage
        {
            get
            {
                return lastLogMessage;
            }
            private set
            {
                this.lastLogMessage = value;
                RaisePropertyChanged(() => LastLogMessage);
                RaisePropertyChanged(() => LogMessageColor);
            }
        }

        /// <summary>
        /// Gets the last log message.
        /// </summary>
        /// <value>
        /// The last log message.
        /// </value>
        public string ConnectionErrorMessage
        {
            get
            {
                return connectionErrorMessage;
            }
            private set
            {
                this.connectionErrorMessage = value;
                RaisePropertyChanged(() => ConnectionErrorMessage);
                RaisePropertyChanged(() => ConnectionErrorMessage);
            }
        }

        /// <summary>
        /// Gets or sets the number a.
        /// </summary>
        public string NumberA
        {
            get
            {
                return this.numberA;
            }

            private set
            {
                this.numberA = value;
                this.RaisePropertyChanged(() => this.NumberA);
            }
        }

        /// <summary>
        /// Gets or sets the number b.
        /// </summary>
        public string NumberB
        {
            get
            {
                return this.numberB;
            }

            private set
            {
                this.numberB = value;
                this.RaisePropertyChanged(() => this.NumberB);
            }
        }

        /// <summary>
        /// Gets or sets the refresh highscore.
        /// </summary>
        /// <value>
        /// The refresh highscore.
        /// </value>
        public ICommand RefreshHighscore
        {
            get
            {
                return this.commandRefreshHighScore;
            }

            set
            {
                this.commandRefreshHighScore = value;
                this.RaisePropertyChanged(() => this.RefreshHighscore);
            }
        }

        /// <summary>
        /// Gets or sets the send answer.
        /// </summary>
        /// <value>
        /// The send answer.
        /// </value>
        public ICommand SendAnswer
        {
            get
            {
                return this.commandSendAnswer;
            }

            set
            {
                this.commandSendAnswer = value;
                this.RaisePropertyChanged(() => this.SendAnswer);
            }
        }

        /// <summary>
        /// Gets or sets the start game.
        /// </summary>
        /// <value>
        /// The start game.
        /// </value>
        public ICommand StartGame
        {
            get
            {
                return this.commandStartGame;
            }

            set
            {
                this.commandStartGame = value;
                this.RaisePropertyChanged(() => this.StartGame);
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value;
                this.RaisePropertyChanged(() => this.Username);
            }
        }

        /// <summary>
        /// Determines whether this instance [can send answer].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanSendAnswer()
        {
            if (!string.IsNullOrEmpty(this.currentAnswer))
            {
                try
                {
                    Convert.ToInt32(this.currentAnswer);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Executes the send answer.
        /// </summary>
        private void ExecuteSendAnswer()
        {
            int answer;

            try
            {
                answer = Convert.ToInt32(this.currentAnswer);
            }
            catch (Exception ex)
            {
                return;
            }

            this.gameClientManager.SendAnswer(answer);
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.StartGame = new DependentRelayCommand(
                this.ExecuteConnectAndStartGame,
                this.CanConnectGame,
                this,
                () => this.Username,
                () => this.ConnectionString,
                () => this.IsGameRunning);

            this.RefreshHighscore = new DependentRelayCommand(
                this.ExecuteRefreshHighScore,
                () => this.IsGameRunning,
                this,
                () => this.IsGameRunning);

            this.SendAnswer = new DependentRelayCommand(
                this.ExecuteSendAnswer,
                this.CanSendAnswer,
                this,
                () => this.Answer);
        }

        /// <summary>
        /// Executes the refresh high score.
        /// </summary>
        private void ExecuteRefreshHighScore()
        {
            this.gameNetworkClient.Send(new HighscoreRequest());
        }

        /// <summary>
        /// Determines whether this instance [can connect game].
        /// </summary>
        /// <returns></returns>
        private bool CanConnectGame()
        {
            if (String.IsNullOrEmpty(this.ConnectionString) || string.IsNullOrEmpty(this.Username))
            {
                return false;
            }

            if (connectionString.Contains(":"))
            {
                var parts = connectionString.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    connectionEndpoint = new IpConnectionEndpoint();


                    connectionEndpoint.Hostname = parts[0];

                    var port = 0;
                    try
                    {
                        port = Convert.ToInt32(parts[1]);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    if (!(port > 0 && port < 65536))
                    {
                        return false;
                    }

                    (connectionEndpoint as IpConnectionEndpoint).Port = port;

                }
                else
                {
                    return false;
                }
                networkAccessType = NetworkAccessType.Udp;
            }
            else if (connectionString.Contains("@"))
            {
                //namedpipes
                networkAccessType = NetworkAccessType.NamedPipes;
            }

            return true;
        }

        /// <summary>
        /// Executes the connect and start game.
        /// </summary>
        private void ExecuteConnectAndStartGame()
        {
            if (networkAccessType == NetworkAccessType.Udp)
            {
                udpClient = new UdpClientConnector<CalcItClientMessage>()
                {
                    ConnectionSettings = connectionEndpoint,
                    MessageTransformer = new DataContractTransformer<CalcItClientMessage>(),
                    Logger = logger
                };

                gameNetworkClient = new CalcItNetworkClient<CalcItClientMessage>()
                {
                    ClientConnector = udpClient,
                    Logger = logger
                };

                gameClientManager.NetworkAccess = gameNetworkClient;

                this.gameCommandExecutor = new CommandExecutor<CalcItClientMessage>()
                {
                    Logger = logger,
                    MethodProvider = gameClientManager,
                    NetworkAccess = gameNetworkClient
                };

                gameCommandExecutor.StartExecutor();
                gameNetworkClient.Connect();
            }
            else
            {
                // TODO named pipes
            }

            gameClientManager.ConnectClient(Username);
            this.IsChangeConnectionEnabled = false;
        }

        private void NetworkEndGameReceived(object sender, MessageReceivedEventArgs<EndGame> e)
        {
            this.IsChangeConnectionEnabled = true;

            //TODO
            //e.Message.GameCount
            //e.Message.Points
        }

        private bool newQuestion;


        private void NetworkHighScoreReceived(object sender, MessageReceivedEventArgs<HighscoreResponse> e)
        {
            HighScoreList.Clear();
            foreach (var item in e.Message.HighScoreList)
            {
                HighScoreList.Add(item);
            }
        }

        /// <summary>
        /// Networks the question received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs{Question}"/> instance containing the event data.</param>
        private void NetworkQuestionReceived(object sender, MessageReceivedEventArgs<Question> e)
        {
            newQuestion = true;
            var question = e.Message as Question;

            this.NumberA = question.NumberA.ToString();
            this.NumberB = question.NumberB.ToString();
            this.Answer = string.Empty;

            switch (question.Operator)
            {
                case OperatorType.Plus:
                    this.CalcOperator = "+";
                    break;
                case OperatorType.Multiply:
                    this.CalcOperator = "x";
                    break;
                case OperatorType.Minus:
                    this.CalcOperator = "-";
                    break;
            }

            newQuestion = false;
            Task.Run(() => StartTimeCounter(question.TimeToAnswer));
        }

        /// <summary>
        /// Starts the time counter.
        /// </summary>
        /// <param name="timeToAnswer">The time to answer.</param>
        private void StartTimeCounter(int timeToAnswer)
        {
            while (timeToAnswer > 0 && !newQuestion)
            {
                this.AnswerTimeLeft = String.Format("{0} sec", timeToAnswer);
                Thread.Sleep(new TimeSpan(0, 0, 0, 0, 900));
                timeToAnswer--;
            }
        }

        /// <summary>
        /// The initialize game client.
        /// </summary>
        private void InitializeGameClient()
        {
            this.gameClientManager = new GameClientManager() { Logger = this.logger };
            gameClientManager.QuestionReceived += this.NetworkQuestionReceived;
            gameClientManager.HighScoreReceived += this.NetworkHighScoreReceived;
            gameClientManager.EndGameReceived += this.NetworkEndGameReceived;
        }

        /// <summary>
        /// The initialize logger.
        /// </summary>
        private void InitializeLogger()
        {
            this.logger = new Logger();
            this.logger.MessageLogged += this.LoggerHandleMessageLogged;
        }

        /// <summary>
        /// The logger_ handle message logged.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LoggerHandleMessageLogged(object sender, LogMessage e)
        {
            switch (e.Type)
            {
                case LogMessageType.Error:
                    this.LogMessageColor = Colors.Red;
                    break;
                case LogMessageType.Warning:
                    this.LogMessageColor = Colors.Gold;
                    break;
                default:
                    this.LogMessageColor = Colors.Black;
                    break;
            }

            LastLogMessage = e.Message;

            Debug.WriteLine(e.Message);
        }
    }
}