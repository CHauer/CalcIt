using System;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess.Events
{
    public class MessageReceivedEventArgs<T> : EventArgs where T : class, ICalcItSession
    {
        public T Message { get; set; }

        public int SessionId { get; set; }

    }
}
