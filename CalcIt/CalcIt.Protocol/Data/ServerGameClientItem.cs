// -----------------------------------------------------------------------
// <copyright file="ServerGameClientItem.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - ServerGameClientItem.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The server game client item.
    /// </summary>
    [DataContract]
    public class ServerGameClientItem
    {
        /// <summary>
        /// Gets or sets the game end time.
        /// </summary>
        /// <value>
        /// The game end time.
        /// </value>
        [DataMember]
        public DateTime? GameEndTime { get; set; }

        /// <summary>
        /// Gets or sets the game start time.
        /// </summary>
        /// <value>
        /// The game start time.
        /// </value>
        [DataMember]
        public DateTime GameStartTime { get; set; }

        /// <summary>
        /// Gets or sets the join date.
        /// </summary>
        /// <value>
        /// The join date.
        /// </value>
        [DataMember]
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        [DataMember]
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        [DataMember]
        public Guid SessionId { get; set; }

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