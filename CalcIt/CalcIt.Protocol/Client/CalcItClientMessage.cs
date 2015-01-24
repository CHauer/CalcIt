// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalcItClientMessage.cs" company="FH Wr.Neustadt">
//   Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// --------------------------------------------------------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System;

    /// <summary>
    /// The calc it client message.
    /// </summary>
    public abstract class CalcItClientMessage : CalcItMessage
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public int Date
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