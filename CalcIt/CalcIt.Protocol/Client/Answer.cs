// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Answer.cs" company="FH Wr.Neustadt">
//   Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>
//   CalcIt.Protocol - Answer.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System;

    /// <summary>
    /// The answer.
    /// </summary>
    public class Answer : CalcItClientMessage
    {
        /// <summary>
        /// Gets or sets the content of the answer.
        /// </summary>
        /// <value>
        /// The content of the answer.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int AnswerContent
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