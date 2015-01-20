using System;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess
{
    public interface INetworkServerConnector<T> where T : class, ICalcItSession
    {
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        event EventHandler IncomingConnectionOccured;

        IMessageTransformer<T> MessageTransformer
        {
            get;
            set;
        }

        int IsRunning
        {
            get;
            set;
        }

        List<System.Guid> Sessions
        {
            get;
            set;
        }

        bool Send(T message);

        void Start();

        void Stop();
    }
}
