using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Protocol
{
    public interface IMessageControl
    {
        int MessageNr
        {
            get;
            set;
        }
    }
}
