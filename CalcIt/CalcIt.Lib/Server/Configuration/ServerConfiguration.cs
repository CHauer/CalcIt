﻿// -----------------------------------------------------------------------
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
        /// Gets or sets the answer timeout in seconds.
        /// </summary>
        public int AnswerTimeout { get; set; }

        /// <summary>
        /// Gets or sets the game client server port.
        /// </summary>
        public int GameServerPort { get; set; }

        /// <summary>
        /// Gets or sets the maximal points.
        /// </summary>
        public int MaximalPoints { get; set; }

        /// <summary>
        /// Gets or sets the minimal points.
        /// </summary>
        public int MinimalPoints { get; set; }

        /// <summary>
        /// Gets or sets the monitor client server port.
        /// </summary>
        public int MonitorServerPort { get; set; }

        /// <summary>
        /// Gets or sets the server connections.
        /// </summary>
        public List<int> ServerListeners { get; set; }

        /// <summary>
        /// Gets or sets the server connections.
        /// </summary>
        /// <value>
        /// The server connections.
        /// </value>
        public List<string> ServerConnections { get; set; }

        /// <summary>
        /// The initialize standard configuration.
        /// </summary>
        private void InitializeStandardConfiguration()
        {
            this.AnswerTimeout = 10;
            this.GameServerPort = 3105;
            this.MaximalPoints = 20;
            this.MinimalPoints = 0;
            this.MonitorServerPort = 50210;
        }
    }
}