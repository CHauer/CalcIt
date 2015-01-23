// -----------------------------------------------------------------------
// <copyright file="TcpServerListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - TcpServerListener.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;
using CalcIt.Protocol.Session;

namespace CalcIt.Lib.NetworkAccess.Tcp
{
    public class TcpServerListener<T> : INetworkServerConnector<T> where T : class, ICalcItSession
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public event EventHandler IncomingConnectionOccured;

        public int ListenPort
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public IMessageTransformer<T> MessageTransformer
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int IsRunning
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Send(T message)
        {
            throw new NotImplementedException();
        }

        public List<Session> Sessions
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}