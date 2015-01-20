using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Monitor;

namespace CalcIt.Lib.Log
{
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
                //donst display Debug Message 
                return;
            }

            //if (message.
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //}
            //else if (message is WarningLogMessage)
            //{
            //    Console.ForegroundColor = ConsoleColor.DarkYellow;
            //}

            Console.WriteLine(message.ToString());
            Console.ResetColor();
        }
    }
}

