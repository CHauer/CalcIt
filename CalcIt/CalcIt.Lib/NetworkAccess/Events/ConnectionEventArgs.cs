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
        public ConnectionEventArgs(Guid session)
        {
            this.Session = session;
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public Guid Session { get; private set; }
    }
}