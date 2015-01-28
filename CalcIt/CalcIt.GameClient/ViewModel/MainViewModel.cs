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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
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
        /// <summary>
        /// The answer time left.
        /// </summary>
        private string answerTimeLeft;

        /// <summary>
        /// The calculation operator.
        /// </summary>
        private string calcOperator;

        /// <summary>
        /// The cancel counter token.
        /// </summary>
        private CancellationTokenSource cancelCounterToken;

        /// <summary>
        /// The command start game.
        /// </summary>
        private ICommand commandEndGame;

        /// <summary>
        /// The command refresh high score.
        /// </summary>
        private ICommand commandRefreshHighScore;

        /// <summary>
        /// The command send answer.
        /// </summary>
        private ICommand commandSendAnswer;

        /// <summary>
        /// The command start game.
        /// </summary>
        private ICommand commandStartGame;

        /// <summary>
        /// The connection endpoint.
        /// </summary>
        private ConnectionEndpoint connectionEndpoint;

        /// <summary>
        /// The connection error message.
        /// </summary>
        private string connectionErrorMessage;

        /// <summary>
        /// The hostname.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// The current answer.
        /// </summary>
        private string currentAnswer;

        /// <summary>
        /// The game client manager.
        /// </summary>
        private GameClientManager gameClientManager;

        /// <summary>
        /// The game command executor.
        /// </summary>
        private CommandExecutor<CalcItClientMessage> gameCommandExecutor;

        /// <summary>
        /// The game count.
        /// </summary>
        private int gameCount;

        /// <summary>
        /// The game network client.
        /// </summary>
        private CalcItNetworkClient<CalcItClientMessage> gameNetworkClient;

        /// <summary>
        /// The game play time.
        /// </summary>
        private int gamePlayTime;

        /// <summary>
        /// The high score.
        /// </summary>
        private ObservableCollection<HighScoreItem> highScore;

        /// <summary>
        /// The is game end.
        /// </summary>
        private bool isGameEnd;

        /// <summary>
        /// The is game running.
        /// </summary>
        private bool isGameRunning;

        /// <summary>
        /// The last log message.
        /// </summary>
        private string lastLogMessage;

        /// <summary>
        /// The logger.
        /// </summary>
        private Logger logger;

        /// <summary>
        /// The network access type.
        /// </summary>
        private NetworkAccessType networkAccessType;

        /// <summary>
        /// The new question.
        /// </summary>
        private bool newQuestion;

        /// <summary>
        /// The number a.
        /// </summary>
        private string numberA;

        /// <summary>
        /// The number b.
        /// </summary>
        private string numberB;

        /// <summary>
        /// The points.
        /// </summary>
        private int points;

        /// <summary>
        /// The client.
        /// </summary>
        private UdpClientConnector<CalcItClientMessage> udpClient;

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

            this.IsGameRunning = false;
            this.IsGameEnd = false;
        }

        /// <summary>
        /// The network access type enumeration.
        /// </summary>
        private enum NetworkAccessType
        {
            /// <summary>
            /// The Network access value.
            /// </summary>
            Udp, 

            /// <summary>
            /// The named pipes.
            /// </summary>
            NamedPipes, 
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
                this.RaisePropertyChanged(() => this.Answer);
            }
        }

        /// <summary>
        /// Gets or sets the answer time left.
        /// </summary>
        /// <value>
        /// The answer time left.
        /// </value>
        public string AnswerTimeLeft
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
        /// Gets or sets the calculation operator.
        /// </summary>
        /// <value>
        /// The calculation operator.
        /// </value>
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
        /// Gets the last log message.
        /// </summary>
        /// <value>
        /// The last log message.
        /// </value>
        public string ConnectionErrorMessage
        {
            get
            {
                return this.connectionErrorMessage;
            }

            private set
            {
                this.connectionErrorMessage = value;
                this.RaisePropertyChanged(() => this.ConnectionErrorMessage);
                this.RaisePropertyChanged(() => this.ConnectionErrorMessage);
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
        /// Gets or sets the game count.
        /// </summary>
        /// <value>
        /// The game count.
        /// </value>
        public int GameCount
        {
            get
            {
                return this.gameCount;
            }

            set
            {
                this.gameCount = value;
                this.RaisePropertyChanged(() => this.GameCount);
            }
        }

        /// <summary>
        /// Gets or sets the game play time.
        /// </summary>
        /// <value>
        /// The game play time.
        /// </value>
        public int GamePlayTime
        {
            get
            {
                return this.gamePlayTime;
            }

            set
            {
                this.gamePlayTime = value;
                this.RaisePropertyChanged(() => this.GamePlayTime);
            }
        }

        /// <summary>
        /// Gets the high score list.
        /// </summary>
        /// <value>
        /// The high score list.
        /// </value>
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
        /// Gets or sets a value indicating whether this instance is game end.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is game end; otherwise, <c>false</c>.
        /// </value>
        public bool IsGameEnd
        {
            get
            {
                return this.isGameEnd;
            }

            set
            {
                this.isGameEnd = value;
                this.RaisePropertyChanged(() => this.IsGameEnd);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is game running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is game running; otherwise, <c>false</c>.
        /// </value>
        public bool IsGameRunning
        {
            get
            {
                return this.isGameRunning;
            }

            set
            {
                this.isGameRunning = value;
                this.RaisePropertyChanged(() => this.IsGameRunning);
                this.RaisePropertyChanged(() => this.IsGameEnd);
            }
        }

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
                return this.lastLogMessage;
            }

            private set
            {
                this.lastLogMessage = value;
                this.RaisePropertyChanged(() => this.LastLogMessage);
                this.RaisePropertyChanged(() => this.LogMessageColor);
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
        /// Gets the number a.
        /// </summary>
        /// <value>
        /// The number a.
        /// </value>
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
        /// Gets the number b.
        /// </summary>
        /// <value>
        /// The number b.
        /// </value>
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
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public int Points
        {
            get
            {
                return this.points;
            }

            set
            {
                this.points = value;
                this.RaisePropertyChanged(() => this.Points);
            }
        }

        /// <summary>
        /// Gets or sets the refresh high score.
        /// </summary>
        /// <value>
        /// The refresh high score.
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
        /// <value>
        /// The username.
        /// </value>
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
        /// Determines whether this instance [can connect game].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> can connect value.
        /// </returns>
        private bool CanConnectGame()
        {
            if (string.IsNullOrEmpty(this.ConnectionString) || string.IsNullOrEmpty(this.Username))
            {
                return false;
            }

            if (this.connectionString.Contains(":"))
            {
                var parts = this.connectionString.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    this.connectionEndpoint = new IpConnectionEndpoint();

                    this.connectionEndpoint.Hostname = parts[0];

                    var port = 0;
                    try
                    {
                        port = Convert.ToInt32(parts[1]);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return false;
                    }

                    if (!(port > 0 && port < 65536))
                    {
                        return false;
                    }

                    (this.connectionEndpoint as IpConnectionEndpoint).Port = port;
                }
                else
                {
                    return false;
                }

                this.networkAccessType = NetworkAccessType.Udp;
            }
            else if (this.connectionString.Contains("@"))
            {
                // namedpipes
                this.networkAccessType = NetworkAccessType.NamedPipes;
            }

            return true;
        }

        /// <summary>
        /// Determines whether this instance can send answer.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> status.
        /// </returns>
        private bool CanSendAnswer()
        {
            if (!string.IsNullOrEmpty(this.currentAnswer))
            {
                try
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    Convert.ToInt32(this.currentAnswer);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Executes the connect and start game.
        /// </summary>
        private void ExecuteConnectAndStartGame()
        {
            if (this.networkAccessType == NetworkAccessType.Udp)
            {
                this.udpClient = new UdpClientConnector<CalcItClientMessage>()
                {
                    ConnectionSettings = this.connectionEndpoint, 
                    MessageTransformer = new DataContractTransformer<CalcItClientMessage>(), 
                    Logger = this.logger
                };

                this.gameNetworkClient = new CalcItNetworkClient<CalcItClientMessage>()
                {
                    ClientConnector = this.udpClient, 
                    Logger = this.logger
                };

                this.gameClientManager.NetworkAccess = this.gameNetworkClient;

                this.gameCommandExecutor = new CommandExecutor<CalcItClientMessage>()
                {
                    Logger = this.logger, 
                    MethodProvider = this.gameClientManager, 
                    NetworkAccess = this.gameNetworkClient
                };

                this.gameCommandExecutor.StartExecutor();
                this.gameNetworkClient.Connect();
            }
            else
            {
                // TODO named pipes
            }

            this.gameClientManager.ConnectClient(this.Username);
        }

        /// <summary>
        /// Executes the refresh high score.
        /// </summary>
        private void ExecuteRefreshHighScore()
        {
            this.gameNetworkClient.Send(new HighscoreRequest());
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
                Debug.WriteLine(ex.Message);
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
        /// The initialize game client.
        /// </summary>
        private void InitializeGameClient()
        {
            this.gameClientManager = new GameClientManager() { Logger = this.logger };
            this.gameClientManager.QuestionReceived += this.NetworkQuestionReceived;
            this.gameClientManager.HighScoreReceived += this.NetworkHighScoreReceived;
            this.gameClientManager.EndGameReceived += this.NetworkEndGameReceived;
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
        /// The logger handle message logged.
        /// </summary>
        /// <param name="sender">
        /// The sender parameter.
        /// </param>
        /// <param name="e">
        /// The e parameter.
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

            this.LastLogMessage = e.Message;

            Debug.WriteLine(e.Message);
        }

        /// <summary>
        /// Networks the end game received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MessageReceivedEventArgs{EndGame}"/> instance containing the event data.
        /// </param>
        private void NetworkEndGameReceived(object sender, MessageReceivedEventArgs<EndGame> e)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                    {
                        this.IsGameRunning = false;
                        this.IsGameEnd = true;
                    });

            if (this.cancelCounterToken != null)
            {
                try
                {
                    this.cancelCounterToken.Cancel();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            Application.Current.Dispatcher.Invoke(
                () =>
                    {
                        this.GameCount = e.Message.GameCount;
                        this.Points = e.Message.Points;
                        this.GamePlayTime = e.Message.GamePlaySeconds;
                    });
        }

        /// <summary>
        /// The network high score received.
        /// </summary>
        /// <param name="sender">
        /// The sender parameter.
        /// </param>
        /// <param name="e">
        /// The e parameter.
        /// </param>
        private void NetworkHighScoreReceived(object sender, MessageReceivedEventArgs<HighscoreResponse> e)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                    {
                        this.HighScoreList.Clear();
                        foreach (var item in e.Message.HighScoreList)
                        {
                            this.HighScoreList.Add(item);
                        }
                    });
        }

        /// <summary>
        /// Networks the question received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MessageReceivedEventArgs{Question}"/> instance containing the event data.
        /// </param>
        private void NetworkQuestionReceived(object sender, MessageReceivedEventArgs<Question> e)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                    {
                        this.IsGameRunning = true;
                        this.IsGameEnd = false;
                    });

            this.newQuestion = true;
            var question = e.Message as Question;

            if (this.cancelCounterToken != null)
            {
                try
                {
                    this.cancelCounterToken.Cancel();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            this.NumberA = question.NumberA.ToString();
            this.NumberB = question.NumberB.ToString();
            Application.Current.Dispatcher.Invoke(() => this.Answer = string.Empty);

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

            this.newQuestion = false;

            this.cancelCounterToken = new CancellationTokenSource();

            Task.Run(
                () => this.StartTimeCounter(question.TimeToAnswer, this.cancelCounterToken.Token), 
                this.cancelCounterToken.Token);
        }

        /// <summary>
        /// Starts the time counter.
        /// </summary>
        /// <param name="timeToAnswer">
        /// The time to answer.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        private void StartTimeCounter(int timeToAnswer, CancellationToken token)
        {
            while (timeToAnswer > 0 && !this.newQuestion)
            {
                this.AnswerTimeLeft = string.Format("{0} sec", timeToAnswer);
                Thread.Sleep(new TimeSpan(0, 0, 0, 0, 900));
                timeToAnswer--;

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}