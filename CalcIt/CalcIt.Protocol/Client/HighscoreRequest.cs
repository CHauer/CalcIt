// -----------------------------------------------------------------------
// <copyright file="HighscoreRequest.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - HighscoreRequest.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The highscore request.
    /// </summary>
    [DataContract]
    public class HighscoreRequest : CalcItClientMessage
    {
    }
}