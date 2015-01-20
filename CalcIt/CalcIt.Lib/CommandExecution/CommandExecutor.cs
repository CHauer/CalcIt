using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Protocol;
using CalcIt.Protocol.Server;

namespace CalcIt.Lib.CommandExecution
{
    public class CommandExecutor<T> where T : class, ICalcItSession
    {
        private Queue<T> commandQueue;

        public INetworkAccess<T> NetworkAccess { get; set; }

        public object MethodProvider
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        private MethodInfo FindExecutorMethod(T message)
        {
            throw new System.NotImplementedException();
        }

        public void StartExecutor()
        {
            throw new System.NotImplementedException();
        }

        public void StopExecutor()
        {
            throw new System.NotImplementedException();
        }

        private void RunExecutor()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateServerNetworkAccess(INetworkAccess<CalcItServerMessage> connection)
        {
            throw new System.NotImplementedException();
        }
    }
}
