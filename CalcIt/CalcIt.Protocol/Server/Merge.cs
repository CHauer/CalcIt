// -----------------------------------------------------------------------
// <copyright file="Merge.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - Merge.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Server
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using CalcIt.Protocol.Data;

    /// <summary>
    /// The merge operation.
    /// </summary>
    [DataContract]
    public class Merge : CalcItServerMessage
    {
        /// <summary>
        /// Gets or sets the game client list.
        /// </summary>
        /// <value>
        /// The game client list.
        /// </value>
        [DataMember]
        public List<ServerGameClientItem> GameClientList { get; set; }

        /// <summary>
        /// Gets or sets the high score list.
        /// </summary>
        /// <value>
        /// The high score list.
        /// </value>
        [DataMember]
        public List<HighScoreItem> HighScoreList { get; set; }
    }
}