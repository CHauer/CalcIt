using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;
using CalcIt.Protocol.Endpoint;

namespace CalcIt.Lib.NetworkAccess.NamedPipes
{
    using CalcIt.Lib.Log;

    public class NamedPipeServerConnector<T> : INetworkServerConnector<T> where T : class, ICalcItSession
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public event EventHandler<ConnectionEventArgs> IncomingConnectionOccured;

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

        public bool IsRunning
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

        public List<Guid> Sessions { get; set; }

        public int PipeName
        {
            get
            {
                throw new System.NotImplementedException();
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

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


        public ConnectionEndpoint ConnectionSettings { get; set; }
    }
}
