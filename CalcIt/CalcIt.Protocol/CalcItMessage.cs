using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CalcIt.Protocol
{
    [DataContract]
    public abstract class CalcItMessage : ICalcItSession
    {

        public Guid SessionId
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }


        public Session.SessionEndpoint SessionEndpoint
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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
    }
}
