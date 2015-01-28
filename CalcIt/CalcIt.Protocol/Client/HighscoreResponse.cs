// -----------------------------------------------------------------------
// <copyright file="HighscoreResponse.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - HighscoreResponse.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using CalcIt.Protocol.Data;

    /// <summary>
    /// The high score response.
    /// </summary>
    [DataContract]
    public class HighscoreResponse : CalcItClientMessage
    {
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