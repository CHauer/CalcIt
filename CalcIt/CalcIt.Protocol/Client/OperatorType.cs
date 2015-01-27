// -----------------------------------------------------------------------
// <copyright file="OperatorType.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - OperatorType.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The operator type.
    /// </summary>
    [DataContract]
    public enum OperatorType : int
    {
        /// <summary>
        /// The plus operator.
        /// </summary>
        [EnumMember]
        Plus = 1, 

        /// <summary>
        /// The minus operator.
        /// </summary>
        [EnumMember]
        Minus = 2, 

        /// <summary>
        /// The multiply operator.
        /// </summary>
        [EnumMember]
        Multiply = 3, 

        // /// <summary>
        // /// The divide operator.
        // /// </summary>
        // [EnumMember]
        // Divide = 4, 
    }
}