// -----------------------------------------------------------------------
// <copyright file="PipeConnectionEndpoint.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - PipeConnectionEndpoint.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Endpoint
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The pipe connection endpoint.
    /// </summary>
    [DataContract]
    public class PipeConnectionEndpoint : ConnectionEndpoint
    {
        /// <summary>
        /// Gets or sets the pipe name.
        /// </summary>
        /// <value>
        /// The name of the pipe.
        /// </value>
        [DataMember]
        public string PipeName { get; set; }
    }
}