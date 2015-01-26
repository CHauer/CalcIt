using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Server;

namespace CalcIt.Protocol.Monitor
{
    using System.Runtime.Serialization;

    [DataContract]
    public class LogServerMessage : LogMessage
    {
        /// <summary>
        /// Gets or sets the server message.
        /// </summary>
        /// <value>
        /// The server message.
        /// </value>
        [DataMember]
        public CalcItServerMessage ServerMessage { get; set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        public override LogMessage Copy()
        {
            return new LogServerMessage()
            {
                SessionId = this.SessionId,
                Message = this.Message,
                MessageNr = this.MessageNr,
                ReconnectEndpoint = this.ReconnectEndpoint,
                Type = this.Type,
                ServerMessage  = this.ServerMessage
            };
        }
    }
}
