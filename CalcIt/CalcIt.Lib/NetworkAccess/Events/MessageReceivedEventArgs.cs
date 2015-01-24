// -----------------------------------------------------------------------
// <copyright file="MessageReceivedEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - MessageReceivedEventArgs.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.Events
{
    using System;

    using CalcIt.Protocol;

    /// <summary>
    /// The message received event args.
    /// </summary>
    /// <typeparam name="T">
    /// Type should implement ICalcItSession interface.
    /// </typeparam>
    public class MessageReceivedEventArgs<T> : EventArgs
        where T : class, ICalcItSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        public MessageReceivedEventArgs(T message, Guid sessionId)
        {
            this.Message = message;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public T Message { get; private set; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public Guid SessionId { get; private set; }
    }
}