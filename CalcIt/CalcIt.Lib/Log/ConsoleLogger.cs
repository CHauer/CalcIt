using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Monitor;

namespace CalcIt.Lib.Log
{
    using CalcIt.Protocol.Data;

    /// <summary>
    /// 
    /// </summary>
    public class ConsoleLogListener : ILogListener
    {
        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteLogMessage(LogMessage message)
        {
            if(message.IsDebug && !Debugger.IsAttached)
            {
                //donst display Debug DetailMessage 
                return;
            }

            switch (message.Type)
            {
                case LogMessageType.Log:
                    Console.ResetColor();
                    break;
                case LogMessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogMessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogMessageType.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine(message.ToString());
            Console.ResetColor();
        }
    }
}

