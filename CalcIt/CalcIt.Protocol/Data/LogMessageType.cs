// -----------------------------------------------------------------------
// <copyright file="LogMessageType.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - LogMessageType.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The log message type.
    /// </summary>
    [DataContract]
    public enum LogMessageType
    {
        /// <summary>
        /// The error.
        /// </summary>
        [EnumMember]
        Error, 

        /// <summary>
        /// The warning.
        /// </summary>
        [EnumMember]
        Warning, 

        /// <summary>
        /// The log.
        /// </summary>
        [EnumMember]
        Log, 

        /// <summary>
        /// The debug.
        /// </summary>
        [EnumMember]
        Debug, 
    }
}