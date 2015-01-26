using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Lib.Client
{
    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Client;
    using CalcIt.Protocol.Monitor;

    public class GameClientManager
    {
        public event EventHandler<MessageReceivedEventArgs<HighscoreResponse>> HighScoreReceived;

        public event EventHandler<MessageReceivedEventArgs<Question>> QuestionReceived;

        public INetworkAccess<CalcItClientMessage> NetworkAccess { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILog Logger { get; set; }

        [CommandHandler(typeof(Question))]
        public void HandleQuestion(CalcItMessage message)
        {
            OnQuestionReceived(new MessageReceivedEventArgs<Question>((Question)message));
        }

        [CommandHandler(typeof(HighscoreResponse))]
        public void HandleHighscoreResponse(CalcItMessage message)
        {
            OnHighScoreReceived(new MessageReceivedEventArgs<HighscoreResponse>((HighscoreResponse)message));
        }

        [CommandHandler(typeof(ClientOperationStatus))]
        public void HandleClientOperationStatus(CalcItMessage message)
        {
           
        }

        [CommandHandler(typeof(EndGame))]
        public void HandleEndGameMessage(CalcItMessage message)
        {

        }

        public void ConnectClient(string username)
        {

        }

        public void SendStartGame()
        {

        }

        public void SendAnswer(int answerValue)
        {
            if (NetworkAccess == null)
            {
                return;
            }

            NetworkAccess.Send(new Answer()
            {
                AnswerContent = answerValue
            });
        }

        public void SendHighscoreRequest()
        {
            if (NetworkAccess == null)
            {
                return;
            }

            NetworkAccess.Send(new HighscoreRequest());
        }

        protected virtual void OnHighScoreReceived(MessageReceivedEventArgs<HighscoreResponse> e)
        {
            var onHighScoreReceived = this.HighScoreReceived;
            if (onHighScoreReceived != null)
            {
                onHighScoreReceived(this, e);
            }
        }

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
        /// <param name="logMessage">The log message.</param>
        private void LogMessage(LogMessage logMessage)
        {
            // ReSharper disable once UseNullPropagation
            if (Logger != null)
            {
                Logger.AddLogMessage(logMessage);
            }
        }
    }
}
