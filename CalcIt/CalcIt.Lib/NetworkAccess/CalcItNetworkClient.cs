using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess
{
    public class CalcItNetworkClient<T> : INetworkAccess<T> where T : class, ICalcItSession
    {
        public CalcItNetworkClient()
        {
        }

        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public INetworkClientConnector<T> ClientConnector { get; set; }

        public void Connect()
        {
            throw new System.NotImplementedException();
        }

        public void Send(T message)
        {
            throw new System.NotImplementedException();
        }
        public void Close()
        {
            throw new System.NotImplementedException();
        }
    }
}
