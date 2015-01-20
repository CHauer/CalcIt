using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Protocol.Server
{
    public class Heartbeat : CalcItServerMessage
    {
        public int CheckToken
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
