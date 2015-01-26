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
    using System.Windows.Input;

    using CalcIt.Lib.Client;
    using CalcIt.Lib.Log;
    using CalcIt.Protocol.Data;
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
        private TimeSpan answerTimeLeft;

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
        /// The high score.
        /// </summary>
        private ObservableCollection<HighScoreItem> highScore;

        /// <summary>
        /// The hostname.
        /// </summary>
        private string hostname;

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
        /// The port.
        /// </summary>
        private int port;

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
        public TimeSpan AnswerTimeLeft
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
        /// Gets or sets the hostname.
        /// </summary>
        public string Hostname
        {
            get
            {
                return this.hostname;
            }

            set
            {
                this.hostname = value;
                this.RaisePropertyChanged(() => this.Hostname);
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

            set
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

            set
            {
                this.numberB = value;
                this.RaisePropertyChanged(() => this.NumberB);
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                this.port = value;
                this.RaisePropertyChanged(() => this.Port);
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
            if (string.IsNullOrEmpty(this.currentAnswer))
            {
                try
                {
                    Convert.ToInt32(this.currentAnswer);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
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

            // gameClientManager.QuestionReceived
            // gameClientManager.HighScoreReceived
        }

        /// <summary>
        /// The initialize logger.
        /// </summary>
        private void InitializeLogger()
        {
            this.logger = new Logger();
            this.logger.MessageLogged += this.Logger_HandleMessageLogged;
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
        private void Logger_HandleMessageLogged(object sender, LogMessage e)
        {
            // TODO log somewhere on UI

            Debug.WriteLine(e.Message);
        }
    }
}