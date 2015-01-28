// -----------------------------------------------------------------------
// <copyright file="CalcItMonitorMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - CalcItMonitorMessage.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Monitor
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The calc it monitor message.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ConnectMonitor))]
    [KnownType(typeof(LogMessage))]
    [KnownType(typeof(LogProtocolMessage))]
    [KnownType(typeof(MonitorOperationStatus))]
    public abstract class CalcItMonitorMessage : CalcItMessage
    {
    }
}