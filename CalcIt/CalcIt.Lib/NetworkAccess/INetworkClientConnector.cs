using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess
{
    public interface INetworkClientConnector<T> where T : class, ICalcItSession
    {
        event System.EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        int IsConnected
        {
            get;
            set;
        }

        IMessageTransformer<T> MessageTransformer
        {
            get;
            set;
        }

        bool Send(T message);

        void Connect();
    }
}
