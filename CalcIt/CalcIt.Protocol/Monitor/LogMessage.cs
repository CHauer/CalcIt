// -----------------------------------------------------------------------
// <copyright file="LogMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - LogMessage.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Monitor
{
    using System;
    using System.Runtime.Serialization;

    using CalcIt.Protocol.Data;

    /// <summary>
    /// The log message.
    /// </summary>
    [DataContract]
    [KnownType(typeof(LogProtocolMessage))]
    public class LogMessage : CalcItMonitorMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        public LogMessage()
            : this(LogMessageType.Log, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public LogMessage(string message)
            : this(LogMessageType.Log, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="ex">
        /// The exception.
        /// </param>
        public LogMessage(Exception ex)
            : this(LogMessageType.Error, ex.Message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="logType">
        /// Type of the log.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public LogMessage(LogMessageType logType, string message)
        {
            this.Type = logType;
            this.Message = message;
            this.Date = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date value.
        /// </value>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is debug.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debug; otherwise, <c>false</c>.
        /// </value>
        [IgnoreDataMember]
        public bool IsDebug
        {
            get
            {
                return this.Type == LogMessageType.Debug;
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type value.
        /// </value>
        [DataMember]
        public LogMessageType Type { get; set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="LogMessage"/>.
        /// </returns>
        public virtual LogMessage Copy()
        {
            return new LogMessage()
            {
                SessionId = this.SessionId, 
                Message = this.Message, 
                MessageNr = this.MessageNr, 
                ReconnectEndpoint = this.ReconnectEndpoint, 
                Type = this.Type
            };
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.IsDebug)
            {
                return string.Format("{0:G} - DEBUG:{1}", this.Date, this.Message);
            }

            return string.Format("{0:G} - {1}", this.Date, this.Message);
        }
    }
}