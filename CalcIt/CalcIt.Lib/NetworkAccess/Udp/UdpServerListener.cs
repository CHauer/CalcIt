using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;
using CalcIt.Protocol.Session;

namespace CalcIt.Lib.NetworkAccess.Udp
{
    public class UdpServerListener<T> : INetworkServerConnector<T> where T : class, ICalcItSession
    {
        /// <summary>
        /// The message send queue.
        /// </summary>
        private Queue<T> messageSendQueue;

        /// <summary>
        /// The calculation step sleep time.
        /// </summary>
        private TimeSpan taskWaitSleepTime;

        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public event EventHandler<ConnectionEventArgs> IncomingConnectionOccured;

        public int ListenPort
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

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public IMessageTransformer<T> MessageTransformer { get; set; }

        public int IsRunning { get; set; }

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


        event EventHandler<ConnectionEventArgs> INetworkServerConnector<T>.IncomingConnectionOccured
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        bool INetworkServerConnector<T>.IsRunning
        {
            get { throw new NotImplementedException(); }
        }

        public ConnectionEndpoint ConnectionSettings
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

        void INetworkServerConnector<T>.Send(T message)
        {
            throw new NotImplementedException();
        }
    }
}
