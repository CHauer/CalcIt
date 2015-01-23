using System;
using System.Collections.Generic;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess
{
    public class CalcItNetworkServer<T> : INetworkAccess<T> where T : class, ICalcItSession
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;
    
        public INetworkServerConnector<T> ServerConnector
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public List<Guid> Sessions
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        private void RunServer()
        {
            throw new System.NotImplementedException();
        }

        public void Send(T message)
        {
            throw new System.NotImplementedException();
        }
    }
}
