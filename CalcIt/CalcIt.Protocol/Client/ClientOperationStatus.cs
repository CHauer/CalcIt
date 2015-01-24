using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CalcIt.Protocol.Data;

namespace CalcIt.Protocol.Client
{
    public class ClientOperationStatus : CalcItClientMessage
    {
        public StatusType Status
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
            }
        }

        public int Message
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
            }
        }
    }
}