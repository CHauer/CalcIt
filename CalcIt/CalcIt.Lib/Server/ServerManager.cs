using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.Server.Configuration;
using CalcIt.Lib.Server.Watcher;

namespace CalcIt.Lib.Server
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Monitor;
    using CalcIt.Protocol.Server;

    public class ServerManager
    {

        /// <summary>
        /// The calculation step sleep time.
        /// </summary>
        private TimeSpan taskWaitSleepTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerManager"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <exception cref="System.ArgumentNullException">configurationManager</exception>
        public ServerManager(IConfigurationManager<ServerConfiguration> configurationManager)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException("configurationManager");
            }

            this.Initialize();

            this.ConfigurationManager = configurationManager;
            InitializeConfiguration();

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.IsActiveServer = true;
            this.taskWaitSleepTime = new TimeSpan(0, 0, 0, 0, 100);
            this.HighScores = new List<HighScoreItem>();
            this.GameClients = new List<GameClient>();
            this.StartTime = DateTime.Now;
            this.RandomToken = this.GenerateCheckToken();
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
        /// Gets or sets the high scores.
        /// </summary>
        /// <value>
        /// The high scores.
        /// </value>
        public List<HighScoreItem> HighScores { get; private set; }

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

        /// <summary>
        /// Occurs when on tunneld message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<CalcItClientMessage>> OnTunneldMessageReceived;

        /// <summary>
        /// Adds the game client.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
        /// <exception cref="System.ArgumentException">
        /// Wrong message type - ConnectClient message required.
        /// or
        /// DetailMessage session Id is not set.
        /// </exception>
        [CommandHandler(typeof(ConnectClient))]
        public void HandleAddGameClient(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is ConnectClient))
            {
                throw new ArgumentException("Wrong message type - ConnectClient message required.");
            }

            if (message.SessionId == null)
            {
                throw new ArgumentException("DetailMessage session Id is not set.");
            }

            ConnectClient clientMessage = message as ConnectClient;

            if (!string.IsNullOrEmpty(clientMessage.Username))
            {
                var gameclient = GameClients.FirstOrDefault(gl => gl.UserName.Equals(clientMessage.Username));

                if (gameclient == null)
                {
                    // gameclient and username do no exist
                    this.AddGameClient(clientMessage);
                }
                else
                {
                    // game running
                    if (gameclient.GameEndTime == null)
                    {
                        GameClientNetworkAccess.Send(new ClientOperationStatus()
                        {
                            ResponseToOperation = message.GetType().Name,
                            SessionId = message.SessionId,
                            DetailMessage = String.Format("Game for username {0} running.", gameclient.UserName),
                            Status = StatusType.Error
                        });

                        LogMessage(new LogMessage(LogMessageType.Warning, String.Format("Game for Username {0} already running.", gameclient.UserName)));

                    }
                    else
                    {
                        // if username/gameclient exists and game is over - new game
                        this.GameClients.Remove(gameclient);
                        this.AddGameClient(clientMessage);
                    }
                }
            }
            else
            {
                GameClientNetworkAccess.Send(new ClientOperationStatus()
                {
                    ResponseToOperation = message.GetType().Name,
                    DetailMessage = "Username required!",
                    SessionId = message.SessionId,
                    Status = StatusType.Error
                });
            }
        }

        /// <summary>
        /// Adds the game client.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.InvalidOperationException">Message SessionId is null or invalid.</exception>
        private void AddGameClient(ConnectClient message)
        {
            if (message.SessionId == null)
            {
                throw new InvalidOperationException("Message SessionId is null or invalid.");
            }

            this.GameClients.Add(new GameClient(message.Username, message.SessionId.Value));

            LogMessage(new LogMessage(String.Format("{0} Game started.", message.Username)));

            this.GameClientNetworkAccess.Send(new ClientOperationStatus()
                {
                    ResponseToOperation = message.GetType().Name,
                    SessionId = message.SessionId,
                    Status = StatusType.Ok,
                    DetailMessage = "Game client connection successfull."
                });
        }

        /// <summary>
        /// Starts the client game.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
        /// <exception cref="System.ArgumentException">
        /// Wrong message type - StartGame message required.
        /// or
        /// DetailMessage session Id is not set.
        /// </exception>
        [CommandHandler(typeof(StartGame))]
        public void StartClientGame(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is StartGame))
            {
                throw new ArgumentException("Wrong message type - StartGame message required.");
            }

            if (message.SessionId == null)
            {
                throw new ArgumentException("Message session Id is not set.");
            }

            // no started game exists
            var gameclient = GameClients.FirstOrDefault(gl => gl.SessionId.Equals(message.SessionId) && gl.IsGameRunning);

            if (gameclient != null)
            {
                //Start Question-Sender
                Task.Run(() => RunQuestionSender(gameclient));
            }
            else
            {
                GameClientNetworkAccess.Send(new ClientOperationStatus()
                {
                    ResponseToOperation = message.GetType().Name,
                    DetailMessage = "Game state is invalid - No game found or already started.",
                    SessionId = message.SessionId,
                    Status = StatusType.Error
                });
            }
        }

        /// <summary>
        /// Runs the question sender.
        /// </summary>
        /// <param name="gameClient">The game client.</param>
        private void RunQuestionSender(GameClient gameClient)
        {
            while (gameClient.Points > Configuration.MinimalPoints && gameClient.Points < Configuration.MaximalPoints)
            {
                Question currentQuestion = GenerateQuestion(gameClient);
                GameClientNetworkAccess.Send(currentQuestion);
                gameClient.GameCount++;

                DateTime sendtime = DateTime.Now;
                bool answerTimeout = false;

                while (gameClient.AnswerQueue.Count == 0 && !answerTimeout)
                {
                    Thread.Sleep(taskWaitSleepTime);

                    if ((DateTime.Now - sendtime).TotalSeconds > Configuration.AnswerTimeOut)
                    {
                        answerTimeout = true;
                    }
                }

                if (gameClient.AnswerQueue.Count > 0)
                {
                    var receivedAnswer = gameClient.AnswerQueue.Dequeue();
                    // clear all other answers
                    gameClient.AnswerQueue.Clear();

                    // check answer input
                    if (GetAnswer(currentQuestion) == receivedAnswer.AnswerContent)
                    {
                        gameClient.Points++;
                    }
                    else
                    {
                        gameClient.Points--;
                    }
                }
                else
                {
                    //minus point
                    gameClient.Points--;
                }
            }

            gameClient.GameEndTime = DateTime.Now;

            int gameplaySeconds = 0;

            try
            {
                gameplaySeconds = (int)(gameClient.GameEndTime.Value - gameClient.GameStartTime).TotalSeconds;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //Send endgame message
            GameClientNetworkAccess.Send(new EndGame()
            {
                GameCount = gameClient.GameCount,
                GamePlaySeconds = gameplaySeconds,
                Points = gameClient.Points,
                SessionId = gameClient.SessionId,
                Username = gameClient.UserName
            });

            HighScores.Add(new HighScoreItem()
            {
                GameCount = gameClient.GameCount,
                GamePlaySeconds = gameplaySeconds,
                Score = gameplaySeconds * gameplaySeconds,
                Username = gameClient.UserName
            });
        }

        /// <summary>
        /// Gets the answer.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns></returns>
        private int GetAnswer(Question question)
        {
            switch (question.Operator)
            {
                case OperatorType.Minus:
                    return question.NumberA - question.NumberB;
                case OperatorType.Plus:
                    return question.NumberA + question.NumberB;
                case OperatorType.Multiply:
                    return question.NumberA * question.NumberB;
            }
            return 0;
        }

        /// <summary>
        /// Generates the question.
        /// </summary>
        /// <param name="gameClient">The game client.</param>
        /// <returns></returns>
        private Question GenerateQuestion(GameClient gameClient)
        {
            Random number = new Random(DateTime.Now.Millisecond);

            return new Question()
            {
                NumberA = number.Next(1, 100),
                NumberB = number.Next(1, 100),

                Operator = (OperatorType)number.Next(1, (int)Enum.GetValues(typeof(OperatorType))
                                                                 .Cast<OperatorType>().Max()),
                SessionId = gameClient.SessionId,
                TimeToAnswer = Configuration.AnswerTimeOut
            };
        }

        /// <summary>
        /// Handles the answer.
        /// </summary>
        /// <param name="message">The message.</param>
        [CommandHandler(typeof(Answer))]
        public void HandleAnswer(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is Answer))
            {
                throw new ArgumentException("Wrong message type - Answer message required.");
            }

            if (message.SessionId == null)
            {
                throw new ArgumentException("Message session Id is not set.");
            }

            var gameclient = GameClients.FirstOrDefault(gc => gc.SessionId == message.SessionId.Value && gc.IsGameRunning);

            if (gameclient != null)
            {
                gameclient.AnswerQueue.Enqueue(message as Answer);
            }

            // else ignore answer message
        }

        /// <summary>
        /// Handles the highscore request.
        /// </summary>
        /// <param name="message">The message.</param>
        [CommandHandler(typeof(HighscoreRequest))]
        public void HandleHighscoreRequest(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is HighscoreRequest))
            {
                throw new ArgumentException("Wrong message type - HighscoreRequest message required.");
            }

            if (message.SessionId == null)
            {
                throw new ArgumentException("Message session Id is not set.");
            }

            var gameclient = GameClients.FirstOrDefault(gc => gc.SessionId == message.SessionId.Value && gc.IsGameRunning);

            if (gameclient != null)
            {
                SendHighscoreRespone(gameclient.SessionId);
            }
        }

        /// <summary>
        /// Sends the highscore respone.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void SendHighscoreRespone(Guid sessionId)
        {
            GameClientNetworkAccess.Send(new HighscoreResponse()
            {
                SessionId = sessionId,
                HighScoreList = HighScores
            });
        }

        /// <summary>
        /// Handles the tunnel message.
        /// </summary>
        /// <param name="tunnelMessage">The tunnel message.</param>
        [CommandHandler(typeof(TunnelMessage))]
        public void HandleTunnelMessage(CalcItMessage tunnelMessage)
        {
            if (tunnelMessage == null)
            {
                throw new ArgumentNullException("tunnelMessage");
            }

            if (!(tunnelMessage is TunnelMessage))
            {
                throw new ArgumentException("Wrong message type - TunnelMessage message required.");
            }

            if ((tunnelMessage as TunnelMessage).Content == null)
            {
                throw new ArgumentException("Tunnel Message content is not set.");
            }

            var innermessage = (tunnelMessage as TunnelMessage).Content as CalcItClientMessage;

            OnOnTunneldMessageReceived(new MessageReceivedEventArgs<CalcItClientMessage>(innermessage));
        }

        /// <summary>
        /// Handles the merge message.
        /// </summary>
        /// <param name="mergeMessage">The merge message.</param>
        [CommandHandler(typeof(Merge))]
        public void HandleMergeMessage(CalcItMessage mergeMessage)
        {
            if (mergeMessage == null)
            {
                throw new ArgumentNullException("mergeMessage");
            }

            if (!(mergeMessage is Merge))
            {
                throw new ArgumentException("Wrong message type - Merge message required.");
            }

            if (mergeMessage.SessionId == null)
            {
                throw new ArgumentException("Message session Id is not set.");
            }

            if (!IsActiveServer)
            {
                //TODO override server data
            }
        }

        /// <summary>
        /// Handles the connect monitor.
        /// </summary>
        /// <param name="message">The message.</param>
        [CommandHandler(typeof(ConnectMonitor))]
        public void HandleConnectMonitor(CalcItMessage message)
        {
            MonitorClientNetworkAccess.Send(new MonitorOperationStatus()
            {
                Status = StatusType.Ok,
                Message = "Monitor connected!",
                SessionId = message.SessionId
            });
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

            if (syncMessage.ServerStartTime < this.StartTime)
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
                // syncMessage.ServerStartTime > this.StartTime - later started
                this.IsActiveServer = true;
            }

            LogMessage(new LogMessage(string.Format("SyncMessage received - server state is {0}.\nReceived StartTime:{1:g}\nThis Server StartTime: {2:g}",
                this.IsActiveServer ? "Active" : "Passiv", syncMessage.ServerStartTime, this.StartTime)));

        }

        /// <summary>
        /// Updates the server connection lost.
        /// </summary>
        public void UpdateServerConnectionLost()
        {
            this.IsActiveServer = true;

            LogMessage(new LogMessage(string.Format("All Server connections lost. - Current server state is {0}.", this.IsActiveServer ? "Active" : "Passiv")));
        }

        /// <summary>
        /// Raises the <see cref="E:OnTunneldMessageReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="CalcIt.Lib.NetworkAccess.Events.MessageReceivedEventArgs{TunnelMessage}"/> instance containing the event data.</param>
        protected virtual void OnOnTunneldMessageReceived(MessageReceivedEventArgs<CalcItClientMessage> e)
        {
            var onOnTunneldMessageReceived = this.OnTunneldMessageReceived;
            if (onOnTunneldMessageReceived != null)
            {
                onOnTunneldMessageReceived(this, e);
            }
        }
    }
}
