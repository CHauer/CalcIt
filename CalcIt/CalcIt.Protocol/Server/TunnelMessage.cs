﻿// -----------------------------------------------------------------------
// <copyright file="TunnelMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - TunnelMessage.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Server
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The tunnel message.
    /// </summary>
    [DataContract]
    public class TunnelMessage : CalcItServerMessage
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [DataMember]
        public CalcItMessage Content { get; set; }
    }
}