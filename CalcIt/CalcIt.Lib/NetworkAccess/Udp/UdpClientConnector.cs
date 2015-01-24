﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess.Udp
{
    public class UdpClientConnector<T> : INetworkClientConnector<T> where T : class, ICalcItSession
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

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

        public bool IsConnected
        {
            get { throw new NotImplementedException(); }
        }

        public Protocol.Session.ConnectionEndpoint ConnectionSettings
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

        public void Send(T message)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
