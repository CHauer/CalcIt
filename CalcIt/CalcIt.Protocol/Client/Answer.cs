// -----------------------------------------------------------------------
// <copyright file="Answer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - Answer.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The answer.
    /// </summary>
    [DataContract]
    public class Answer : CalcItClientMessage
    {
        /// <summary>
        /// Gets or sets the content of the answer.
        /// </summary>
        /// <value>
        /// The content of the answer.
        /// </value>
        [DataMember]
        public int AnswerContent { get; set; }
    }
}