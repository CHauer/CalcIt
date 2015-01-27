// -----------------------------------------------------------------------
// <copyright file="GameClient.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - GameClient.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Server
{
    using System;
    using System.Collections.Generic;

    using CalcIt.Protocol.Client;

    /// <summary>
    /// The game client.
    /// </summary>
    public class GameClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameClient"/> class.
        /// </summary>
        public GameClient(string username, Guid sessionId)
        {
            this.AnswerQueue = new Queue<Answer>();

            this.UserName = username;
            this.SessionId = sessionId;
            this.JoinDate = DateTime.Now;
            this.Points = 0;
            this.GameCount = 0;
        }

        /// <summary>
        /// Gets or sets the answer queue.
        /// </summary>
        /// <value>
        /// The answer queue.
        /// </value>
        public Queue<Answer> AnswerQueue { get; private set; }

        /// <summary>
        /// Gets or sets the game count.
        /// </summary>
        /// <value>
        /// The game count.
        /// </value>
        public int GameCount { get; set; }

        /// <summary>
        /// Gets or sets the game end time.
        /// </summary>
        /// <value>
        /// The game end time.
        /// </value>
        public DateTime? GameEndTime { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is game running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is game running; otherwise, <c>false</c>.
        /// </value>
        public bool IsGameRunning
        {
            get
            {
                // ReSharper disable once ConvertPropertyToExpressionBody
                return (GameEndTime == null);
            }
        }

        /// <summary>
        /// Gets or sets the game start time.
        /// </summary>
        /// <value>
        /// The game start time.
        /// </value>
        public DateTime GameStartTime { get; set; }

        /// <summary>
        /// Gets or sets the join date.
        /// </summary>
        /// <value>
        /// The join date.
        /// </value>
        public DateTime JoinDate { get; private set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public Guid SessionId { get; private set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; private set; }
    }
}