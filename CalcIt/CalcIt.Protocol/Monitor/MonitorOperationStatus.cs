﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CalcIt.Protocol.Data;

namespace CalcIt.Protocol.Monitor
{
    public class MonitorOperationStatus : CalcItMonitorMessage
    {
        public StatusType Status
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Message
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