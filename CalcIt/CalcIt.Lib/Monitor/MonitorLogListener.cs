using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Protocol.Monitor;

namespace CalcIt.Lib.Monitor
{
    public class MonitorLogListener : CalcIt.Lib.Log.ILogListener
    {
        public CalcItNetworkServer<CalcItMonitorMessage> MonitorNetworkAccess
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public void WriteLogMessage(Protocol.Monitor.LogMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
