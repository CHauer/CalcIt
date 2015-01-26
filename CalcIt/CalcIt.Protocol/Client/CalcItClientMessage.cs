// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalcItClientMessage.cs" company="FH Wr.Neustadt">
//   Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// --------------------------------------------------------------------------------------------------------------------
namespace CalcIt.Protocol.Client
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The calc it client message.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Answer))]
    [KnownType(typeof(ClientOperationStatus))]
    [KnownType(typeof(ConnectClient))]
    [KnownType(typeof(EndGame))]
    [KnownType(typeof(HighscoreRequest))]
    [KnownType(typeof(HighscoreResponse))]
    [KnownType(typeof(Question))]
    [KnownType(typeof(StartGame))]
    public abstract class CalcItClientMessage : CalcItMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalcItClientMessage"/> class.
        /// </summary>
        protected CalcItClientMessage()
        {
            this.Date = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }
    }
}