// -----------------------------------------------------------------------
// <copyright file="IMessageControl.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - IMessageControl.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol
{
    /// <summary>
    /// The MessageControl interface.
    /// </summary>
    public interface IMessageControl
    {
        /// <summary>
        /// Gets or sets the message nr.
        /// </summary>
        int MessageNr { get; set; }
    }
}