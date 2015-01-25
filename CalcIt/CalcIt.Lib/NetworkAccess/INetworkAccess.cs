// -----------------------------------------------------------------------
// <copyright file="INetworkAccess.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - INetworkAccess.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess
{
    using System;

    using CalcIt.Lib.NetworkAccess.Events;
    using CalcIt.Protocol;

    /// <summary>
    /// The NetworkAccess interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface INetworkAccess<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// The message received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Send(T message);
    }
}