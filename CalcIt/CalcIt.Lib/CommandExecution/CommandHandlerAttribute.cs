// -----------------------------------------------------------------------
// <copyright file="CommandHandlerAttribute.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - CommandHandlerAttribute.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.CommandExecution
{
    using System;

    /// <summary>
    /// The command handler attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerAttribute"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        public CommandHandlerAttribute(Type messageType)
        {
            this.MessageType = messageType;
        }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        /// <value>
        /// The message type.
        /// </value>
        public Type MessageType { get; set; }
    }
}