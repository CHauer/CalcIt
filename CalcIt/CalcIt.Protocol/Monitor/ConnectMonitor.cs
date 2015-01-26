// -----------------------------------------------------------------------
// <copyright file="ConnectMonitor.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - ConnectMonitor.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Monitor
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The connect monitor.
    /// </summary>
    [DataContract]
    public class ConnectMonitor : CalcItMonitorMessage
    {
    }
}