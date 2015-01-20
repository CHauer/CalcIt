using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Monitor;

namespace CalcIt.Protocol.Monitor
{
    public class LogMessage : CalcItMonitorMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        public LogMessage() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LogMessage(string message) : this(message, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        public LogMessage(string message, bool isDebug)
        {
            Message = message;
            IsDebug = isDebug;
            Date = DateTime.Now;

            if (IsDebug)
            {
                this.StackTrace = GetStackTrance();
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is debug.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is debug; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebug { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        public CalcIt.Protocol.LogMessageType Type
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Servername
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (IsDebug)
            {
                return string.Format("{0:G} - {1}\nDEBUG:{2}", Date, Message);
            }

            return string.Format("{0:G} - {1}", Date, Message);
        }
    }
}
