// -----------------------------------------------------------------------
// <copyright file="ConnectClient.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - ConnectClient.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The connect client.
    /// </summary>
    [DataContract]
    public class ConnectClient : CalcItClientMessage
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [DataMember]
        public string Username { get; set; }
    }
}