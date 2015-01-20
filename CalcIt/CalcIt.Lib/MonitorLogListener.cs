using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Lib
{
    public class MonitorLogListener : CalcIt.Lib.Log.ILogListener
    {
        public CalcItServerListener<CalcItMonitorMessage> MonitorNetworkAccess
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
