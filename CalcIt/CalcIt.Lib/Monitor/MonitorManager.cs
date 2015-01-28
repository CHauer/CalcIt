// -----------------------------------------------------------------------
// <copyright file="MonitorManager.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - MonitorManager.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Monitor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.CommandExecution;
    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The monitor manager.
    /// </summary>
    public class MonitorManager
    {
        /// <summary>
        /// The ok received.
        /// </summary>
        private bool receivedStatus = false;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the network access.
        /// </summary>
        /// <value>
        /// The network access.
        /// </value>
        public INetworkAccess<CalcItMonitorMessage> NetworkAccess { get; set; }

        /// <summary>
        /// Handles the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
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
                this.LogMessage(message as LogMessage);
            }
        }

        /// <summary>
        /// The handle log protocol message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [CommandHandler(typeof(LogProtocolMessage))]
        public void HandleLogProtocolMessage(CalcItMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (message == null)
            {
                return;
            }

            if (message is LogMessage)
            {
                // ReSharper disable once TryCastAlwaysSucceeds
                this.LogMessage(message as LogMessage);
            }
        }

        /// <summary>
        /// Connects to server.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> status.
        /// </returns>
        public async Task<bool> ConnectToServer()
        {
            if (this.NetworkAccess == null)
            {
                this.LogMessage(new LogMessage(LogMessageType.Error, "Network Access has to be initialized!"));
                return false;
            }

            this.NetworkAccess.Send(new ConnectMonitor());

            return await Task.Run<bool>(
                () =>
                    {
                        DateTime start = DateTime.Now;
                        TimeSpan wait = new TimeSpan(0, 0, 15);

                        while (!this.receivedStatus)
                        {
                            Thread.Sleep(100);

                            if ((DateTime.Now - start) > wait)
                            {
                                return false;
                            }
                        }

                        return true;
                    });
        }

        /// <summary>
        /// Handles the operation status.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
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
                    this.LogMessage(new LogMessage(LogMessageType.Error, status.Message));
                }
                else
                {
                    this.receivedStatus = true;
                    this.LogMessage(new LogMessage(LogMessageType.Log, status.Message));
                }
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
    }
}