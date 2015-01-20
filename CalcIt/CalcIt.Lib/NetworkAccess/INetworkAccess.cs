using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess
{
    public interface INetworkAccess<T> where T : class, ICalcItSession
    {
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        void Send(T message);
    }
}
