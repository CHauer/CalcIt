﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Server;

namespace CalcIt.Protocol.Monitor
{
    public class LogServerMessage : CalcIt.Protocol.Monitor.LogMessage
    {
        public CalcItServerMessage ServerMessage
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
