using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcIt.Protocol.Session
{
    public class Session
    {
        public Guid SessionId { get; set; }

        public ConnectionEndpoint ConnectionEndpoint { get; set; }
    }
}
