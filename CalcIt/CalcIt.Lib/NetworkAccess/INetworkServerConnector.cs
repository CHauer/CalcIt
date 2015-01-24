using System;
using System.Collections.Generic;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;
using CalcIt.Protocol.Session;

namespace CalcIt.Lib.NetworkAccess
{
    public interface INetworkServerConnector<T> where T : class, ICalcItSession
    {
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        event EventHandler<ConnectionEventArgs> IncomingConnectionOccured;

        IMessageTransformer<T> MessageTransformer
        {
            get;
            set;
        }

        bool IsRunning
        {
            get;

        }

        List<Session> Sessions
        {
            get;
        }

        ConnectionEndpoint ConnectionSettings
        {
            get;
            set;
        }

        void Send(T message);

        void Start();

        void Stop();
    }
}
