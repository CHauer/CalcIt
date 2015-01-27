// -----------------------------------------------------------------------
// <copyright file="EndGame.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - EndGame.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The end game.
    /// </summary>
    [DataContract]
    public class EndGame : CalcItClientMessage
    {
        /// <summary>
        /// Gets or sets the game count.
        /// </summary>
        /// <value>
        /// The game count.
        /// </value>
        [DataMember]
        public int GameCount { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        [DataMember]
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the game play seconds.
        /// </summary>
        /// <value>
        /// The game play seconds.
        /// </value>
        [DataMember]
        public int GamePlaySeconds { get; set; }
    }
}