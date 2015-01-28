// -----------------------------------------------------------------------
// <copyright file="CalcItServerMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - CalcItServerMessage.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Server
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The calc it server message.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Heartbeat))]
    [KnownType(typeof(Merge))]
    [KnownType(typeof(SyncMessage))]
    [KnownType(typeof(TunnelMessage))]
    [KnownType(typeof(ConnectServer))]
    public abstract class CalcItServerMessage : CalcItMessage
    {
    }
}