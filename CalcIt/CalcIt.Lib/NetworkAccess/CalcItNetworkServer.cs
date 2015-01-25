using System;
using System.Collections.Generic;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess
{
    public class CalcItNetworkServer<T> : INetworkAccess<T> where T : class, ICalcItSession
    {
        public CalcItNetworkServer()
        {

        }

        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public INetworkServerConnector<T> ServerConnector { get; set; }

        public List<Guid> Sessions { get; set; }
    
        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void Send(T message)
        {
            throw new System.NotImplementedException();
        }
    }
}
