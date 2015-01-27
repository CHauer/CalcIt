// -----------------------------------------------------------------------
// <copyright file="Question.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - Question.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The question.
    /// </summary>
    [DataContract]
    public class Question : CalcItClientMessage
    {
        /// <summary>
        /// Gets or sets the number a.
        /// </summary>
        /// <value>
        /// The number a.
        /// </value>
        [DataMember]
        public int NumberA { get; set; }

        /// <summary>
        /// Gets or sets the number b.
        /// </summary>
        /// <value>
        /// The number b.
        /// </value>
        [DataMember]
        public int NumberB { get; set; }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        [DataMember]
        public OperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds to answer.
        /// </summary>
        /// <value>
        /// The time to answer.
        /// </value>
        [DataMember]
        public int TimeToAnswer { get; set; }
    }
}