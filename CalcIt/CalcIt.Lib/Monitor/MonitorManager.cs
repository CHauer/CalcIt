using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Lib.Monitor
{
    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Monitor;

    public class MonitorManager
    {
        /// <summary>
        /// Gets or sets the network access.
        /// </summary>
        /// <value>
        /// The network access.
        /// </value>
        public INetworkAccess<CalcItMonitorMessage> NetworkAccess { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Handles the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        [CommandHandler(typeof(LogMessage))]
        public void HandleLogMessage(CalcItMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (message == null)
            {
                return;
            }

            if (message is LogMessage)
            {
                // ReSharper disable once TryCastAlwaysSucceeds
                LogMessage(message as LogMessage);
            }
        }

        /// <summary>
        /// Connects to server.
        /// </summary>
        public void ConnectToServer()
        {
            if (NetworkAccess == null)
            {
                LogMessage(new LogMessage(LogMessageType.Error, "Network Access has to be initialized!"));
                return;
            }

            NetworkAccess.Send(new ConnectMonitor());
        }

        /// <summary>
        /// Handles the operation status.
        /// </summary>
        /// <param name="message">The message.</param>
        [CommandHandler(typeof(MonitorOperationStatus))]
        public void HandleOperationStatus(CalcItMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (message == null)
            {
                return;
            }

            if (message is MonitorOperationStatus)
            {
                MonitorOperationStatus status = message as MonitorOperationStatus;
                // ReSharper disable once TryCastAlwaysSucceeds

                if (status.Status == StatusType.Error)
                {
                    LogMessage(new LogMessage(LogMessageType.Error, status.Message));
                }
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
