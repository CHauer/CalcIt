// -----------------------------------------------------------------------
// <copyright file="GameClientManager.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - GameClientManager.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Client
{
    using System;

    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The game client manager.
    /// </summary>
    public class GameClientManager
    {
        /// <summary>
        /// The client session identifier
        /// </summary>
        private Guid clientSessionId;

        /// <summary>
        /// The connect sent
        /// </summary>
        private bool connectSent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameClientManager"/> class.
        /// </summary>
        public GameClientManager()
        {
            this.connectSent = false;
        }

        /// <summary>
        /// The end game received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<EndGame>> EndGameReceived;

        /// <summary>
        /// Occurs when high score response was received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<HighscoreResponse>> HighScoreReceived;

        /// <summary>
        /// Occurs when a question was received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<Question>> QuestionReceived;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the network access.
        /// </summary>
        /// <value>
        /// The network access.
        /// </value>
        public INetworkAccess<CalcItClientMessage> NetworkAccess { get; set; }

        /// <summary>
        /// Connects the client.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        public void ConnectClient(string username)
        {
            if (NetworkAccess == null)
            {
                throw new InvalidOperationException();
            }

            this.NetworkAccess.Send(new ConnectClient() { Username = username });
            this.connectSent = true;
        }

        /// <summary>
        /// The handle client operation status.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        [CommandHandler(typeof(ClientOperationStatus))]
        public void HandleClientOperationStatus(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is ClientOperationStatus))
            {
                throw new ArgumentException("Message type invalid - required message is ClientOperationStatus.");
            }

            if (this.connectSent)
            {
                var clientOperationStatus = message as ClientOperationStatus;

                if (clientOperationStatus.ResponseToOperation.Equals(typeof(ConnectClient).Name))
                {
                    if (clientOperationStatus.SessionId != null)
                    {
                        this.clientSessionId = clientOperationStatus.SessionId.Value;
                        this.SendStartGame();
                    }
                }
            }
        }

        /// <summary>
        /// The handle end game message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [CommandHandler(typeof(EndGame))]
        public void HandleEndGameMessage(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is EndGame))
            {
                throw new ArgumentException("Message type invalid - required message is EndGame.");
            }


            this.OnEndGameReceived(new MessageReceivedEventArgs<EndGame>((EndGame)message));
        }

        /// <summary>
        /// The handle highscore response.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [CommandHandler(typeof(HighscoreResponse))]
        public void HandleHighscoreResponse(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is HighscoreResponse))
            {
                throw new ArgumentException("Message type invalid - required message is HighscoreResponse.");
            }


            this.OnHighScoreReceived(new MessageReceivedEventArgs<HighscoreResponse>((HighscoreResponse)message));
        }

        /// <summary>
        /// The handle question.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [CommandHandler(typeof(Question))]
        public void HandleQuestion(CalcItMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!(message is Question))
            {
                throw new ArgumentException("Message type invalid - required message is Question.");
            }

            this.OnQuestionReceived(new MessageReceivedEventArgs<Question>((Question)message));
        }

        /// <summary>
        /// Sends the answer.
        /// </summary>
        /// <param name="answerValue">
        /// The answer value.
        /// </param>
        public void SendAnswer(int answerValue)
        {
            if (NetworkAccess == null)
            {
                throw new InvalidOperationException();
            }

            this.NetworkAccess.Send(new Answer() { AnswerContent = answerValue });
        }

        /// <summary>
        /// Sends the highscore request.
        /// </summary>
        public void SendHighscoreRequest()
        {
            if (NetworkAccess == null)
            {
                throw new InvalidOperationException();
            }

            this.NetworkAccess.Send(new HighscoreRequest());
        }

        /// <summary>
        /// Sends the start game.
        /// </summary>
        public void SendStartGame()
        {
            if (NetworkAccess == null)
            {
                throw new InvalidOperationException();
            }

            this.NetworkAccess.Send(new StartGame() { SessionId = this.clientSessionId });
        }

        /// <summary>
        /// The on high score received.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnHighScoreReceived(MessageReceivedEventArgs<HighscoreResponse> e)
        {
            var onHighScoreReceived = this.HighScoreReceived;
            if (onHighScoreReceived != null)
            {
                onHighScoreReceived(this, e);
            }
        }

        /// <summary>
        /// The on question received.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnQuestionReceived(MessageReceivedEventArgs<Question> e)
        {
            var onQuestionReceived = this.QuestionReceived;

            // ReSharper disable once UseNullPropagation
            if (onQuestionReceived != null)
            {
                onQuestionReceived(this, e);
            }
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        private void LogMessage(LogMessage logMessage)
        {
            // ReSharper disable once UseNullPropagation
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(logMessage);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:EndGameReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="CalcIt.Lib.NetworkAccess.Events.MessageReceivedEventArgs{EndGame}"/>
        ///  instance containing the event data.
        /// </param>
        protected virtual void OnEndGameReceived(MessageReceivedEventArgs<EndGame> e)
        {
            var onEndGameReceived = this.EndGameReceived;
            if (onEndGameReceived != null)
            {
                onEndGameReceived(this, e);
            }
        }
    }
}