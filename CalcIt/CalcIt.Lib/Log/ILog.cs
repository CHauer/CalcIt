// -----------------------------------------------------------------------
// <copyright file="ILog.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - ILog.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Log
{
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The Log interface.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void AddLogMessage(LogMessage message);
    }
}