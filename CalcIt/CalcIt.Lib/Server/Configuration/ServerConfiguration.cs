// -----------------------------------------------------------------------
// <copyright file="ServerConfiguration.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - ServerConfiguration.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Server.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The server configuration.
    /// </summary>
    [Serializable]
    public class ServerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfiguration"/> class.
        /// </summary>
        public ServerConfiguration()
        {
            this.InitializeStandardConfiguration();
        }

        /// <summary>
        /// Gets or sets the answer time out.
        /// </summary>
        /// <value>
        /// The answer time out.
        /// </value>
        public int AnswerTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the game client server port.
        /// </summary>
        /// <value>
        /// The game server port.
        /// </value>
        public int GameServerPort { get; set; }

        /// <summary>
        /// Gets or sets the heartbeat retry counter.
        /// </summary>
        /// <value>
        /// The heartbeat retry counter.
        /// </value>
        public int HeartbeatRetryCounter { get; set; }

        /// <summary>
        /// Gets or sets the heartbeat time.
        /// </summary>
        /// <value>
        /// The heartbeat time.
        /// </value>
        public int HeartbeatTime { get; set; }

        /// <summary>
        /// Gets or sets the maximal points.
        /// </summary>
        /// <value>
        /// The maximal points.
        /// </value>
        public int MaximalPoints { get; set; }

        /// <summary>
        /// Gets or sets the minimal points.
        /// </summary>
        /// <value>
        /// The minimal points.
        /// </value>
        public int MinimalPoints { get; set; }

        /// <summary>
        /// Gets or sets the monitor client server port.
        /// </summary>
        /// <value>
        /// The monitor server port.
        /// </value>
        public int MonitorServerPort { get; set; }

        /// <summary>
        /// Gets or sets the reconnect server connection time in minutes.
        /// </summary>
        /// <value>
        /// The reconnect server connection time.
        /// </value>
        public int ReconnectServerConnectionTime { get; set; }

        /// <summary>
        /// Gets or sets the server connections.
        /// </summary>
        /// <value>
        /// The server connections.
        /// </value>
        public List<string> ServerConnections { get; set; }

        /// <summary>
        /// Gets or sets the server connections.
        /// </summary>
        /// <value>
        /// The server listeners.
        /// </value>
        public List<int> ServerListeners { get; set; }

        /// <summary>
        /// Gets or sets the answer timeout in seconds.
        /// </summary>
        /// <value>
        /// The sync time out.
        /// </value>
        public int SyncTimeOut { get; set; }

        /// <summary>
        /// The initialize standard configuration.
        /// </summary>
        private void InitializeStandardConfiguration()
        {
            this.SyncTimeOut = 10;
            this.GameServerPort = 3105;
            this.MaximalPoints = 20;
            this.MinimalPoints = 0;
            this.MonitorServerPort = 50210;
            this.AnswerTimeOut = 60;
            this.ReconnectServerConnectionTime = 5;
            this.HeartbeatTime = 60;
            this.HeartbeatRetryCounter = 3;
        }
    }
}