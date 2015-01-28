// -----------------------------------------------------------------------
// <copyright file="ConsoleLogListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - ConsoleLogListener.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Log
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The console log listener class.
    /// </summary>
    public class ConsoleLogListener : ILogListener
    {
        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void WriteLogMessage(LogMessage message)
        {
            if (message.IsDebug && !Debugger.IsAttached)
            {
                // donst display Debug DetailMessage 
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

            if (message is LogProtocolMessage)
            {
                var protocolMessage = message as LogProtocolMessage;

                if (protocolMessage.IsOutgoing)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine(this.CreateProtocolMessageOutput(message as LogProtocolMessage));
            }
            else
            {
                Console.WriteLine(message.ToString());
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Creates the protocol message output.
        /// </summary>
        /// <param name="logProtocolMessage">
        /// The log protocol message.
        /// </param>
        /// <returns>
        /// The protocol message in text format.
        /// </returns>
        private string CreateProtocolMessageOutput(LogProtocolMessage logProtocolMessage)
        {
            StringBuilder builder = new StringBuilder();

            if (logProtocolMessage == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(logProtocolMessage.Message))
            {
                builder.AppendLine(
                    string.Format("{0} - MessageType: {0}", logProtocolMessage.ProtocolMessage.GetType().Name));
            }
            else
            {
                builder.AppendLine(
                    string.Format(
                        "{0}\nMessageType: {1}", 
                        logProtocolMessage.ToString(), 
                        logProtocolMessage.ProtocolMessage.GetType().Name));
            }

            try
            {
                logProtocolMessage.ProtocolMessage.GetType()
                    .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                    .ToList()
                    .ForEach(
                        prop =>
                            {
                                builder.AppendLine(
                                    string.Format(
                                        "{0} : {1}", 
                                        prop.Name, 
                                        prop.GetValue(logProtocolMessage.ProtocolMessage).ToString()));
                            });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return builder.ToString();
        }
    }
}