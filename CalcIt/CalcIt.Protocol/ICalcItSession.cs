using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Protocol
{
    using CalcIt.Protocol.Session;

    /// <summary>
    /// 
    /// </summary>
    public interface ICalcItSession
    {
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        Guid? SessionId
        {
            get;
            set;
        }

        ConnectionEndpoint ConnectionEndpoint
        {
            get;
            set;
        }
    }
}
