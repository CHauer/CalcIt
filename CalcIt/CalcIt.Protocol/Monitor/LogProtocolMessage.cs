using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Server;

namespace CalcIt.Protocol.Monitor
{
    using System.Runtime.Serialization;
    using System.Reflection;

    [DataContract]
    public class LogProtocolMessage : LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogProtocolMessage"/> class.
        /// </summary>
        public LogProtocolMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProtocolMessage"/> class.
        /// </summary>
        /// <param name="protocolMessage">The protocol message.</param>
        public LogProtocolMessage(CalcItMessage protocolMessage)
            : this(protocolMessage, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProtocolMessage" /> class.
        /// </summary>
        /// <param name="protocolMessage">The protocol message.</param>
        /// <param name="message">The message.</param>
        public LogProtocolMessage(CalcItMessage protocolMessage, string message)
            : base(message)
        {
            this.ProtocolMessage = protocolMessage;
        }

        /// <summary>
        /// Gets or sets the server message.
        /// </summary>
        /// <value>
        /// The server message.
        /// </value>
        [DataMember]
        public CalcItMessage ProtocolMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is outgoing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is outgoing; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsOutgoing { get; set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        public override LogMessage Copy()
        {
            return new LogProtocolMessage()
            {
                SessionId = this.SessionId,
                Message = this.Message,
                MessageNr = this.MessageNr,
                ReconnectEndpoint = this.ReconnectEndpoint,
                Type = this.Type,
                ProtocolMessage = this.ProtocolMessage,
                IsOutgoing = this.IsOutgoing
            };
        }

    }
}
