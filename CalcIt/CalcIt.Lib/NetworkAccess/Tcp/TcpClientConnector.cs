using System;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess.Tcp
{
    public class TcpClientConnector<T> : INetworkClientConnector<T> where T : class, ICalcItSession
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public int Hostname
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

        public int Port
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

        public int IsConnected
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

        public void Connect()
        {
            throw new NotImplementedException();
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

        public bool Send(T message)
        {
            throw new NotImplementedException();
        }
    }
}
