using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Protocol.Client
{
    public abstract class CalcItClientMessage : CalcItMessage
    {

        public Guid ClientId
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int MessageNr
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Date
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
