using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess.NamedPipes
{
    public class NamedPipeServerConnector<T> : INetworkServerConnector<T> where T : class, ICalcItSession
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public event EventHandler IncomingConnectionOccured;

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

        public int IsRunning
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

        public bool Send(T message)
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
    }
}
