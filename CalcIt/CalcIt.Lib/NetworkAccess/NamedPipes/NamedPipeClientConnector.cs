using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess.NamedPipes
{
    using CalcIt.Lib.Log;
    using CalcIt.Protocol.Endpoint;

    public class NamedPipeClientConnector<T> : INetworkClientConnector<T> where T : class, ICalcItSession
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public bool IsConnected
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IMessageTransformer<T> MessageTransformer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        public void Send(T message)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }


        public ConnectionEndpoint ConnectionSettings { get; set; }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
