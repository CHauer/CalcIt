using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcIt.Lib.CommandExecution
{
    public class CommandHandlerAttribute : Attribute
    {

        public Type MessageType
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
