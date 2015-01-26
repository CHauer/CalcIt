// -----------------------------------------------------------------------
// <copyright file="StartGame.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Protocol - StartGame.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The start game.
    /// </summary>
    [DataContract]
    public class StartGame : CalcItClientMessage
    {
    }
}