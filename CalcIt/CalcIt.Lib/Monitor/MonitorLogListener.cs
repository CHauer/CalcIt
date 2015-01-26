// -----------------------------------------------------------------------
// <copyright file="MonitorLogListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - MonitorLogListener.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Monitor
{
    using System;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The monitor log listener.
    /// </summary>
    public class MonitorLogListener : ILogListener
    {
        /// <summary>
        /// Gets or sets the monitor network access.
        /// </summary>
        public CalcItNetworkServer<CalcItMonitorMessage> MonitorNetworkAccess { get; set; }

        /// <summary>
        /// The write log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void WriteLogMessage(LogMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (this.MonitorNetworkAccess != null)
            {
                foreach (Guid sessionId in this.MonitorNetworkAccess.Sessions)
                {
                    LogMessage copyMessage = message.Copy();

                    copyMessage.SessionId = sessionId;

                    this.MonitorNetworkAccess.Send(copyMessage);
                }
            }
        }
    }
}