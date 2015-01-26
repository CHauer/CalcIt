// -----------------------------------------------------------------------
// <copyright file="HighScoreItem.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - HighScoreItem.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The high score item.
    /// </summary>
    [DataContract]
    public class HighScoreItem
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
        /// Gets or sets the game play seconds.
        /// </summary>
        /// <value>
        /// The game play seconds.
        /// </value>
        [DataMember]
        public int GamePlaySeconds { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [DataMember]
        public int Username { get; set; }
    }
}