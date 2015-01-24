// -----------------------------------------------------------------------
// <copyright file="ConnectionEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - ConnectionEventArgs.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.Events
{
    using System;

    using CalcIt.Protocol.Session;

    /// <summary>
    /// The connection event args.
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public ConnectionEventArgs(Session session)
        {
            this.Session = session;
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public Session Session { get; private set; }
    }
}