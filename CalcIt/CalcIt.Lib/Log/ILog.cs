using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Monitor;

namespace CalcIt.Lib.Log
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddLogMessage(LogMessage message);

    }
}
