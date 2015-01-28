// -----------------------------------------------------------------------
// <copyright file="ILogListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - ILogListener.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Log
{
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The Log Listener interface.
    /// </summary>
    public interface ILogListener
    {
        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void WriteLogMessage(LogMessage message);
    }
}