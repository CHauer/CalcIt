// -----------------------------------------------------------------------
// <copyright file="ConnectServer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - ConnectServer.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Server
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The connect server.
    /// </summary>
    [DataContract]
    public class ConnectServer : CalcItServerMessage
    {
    }
}