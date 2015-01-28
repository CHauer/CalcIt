// -----------------------------------------------------------------------
// <copyright file="IMessageTransformer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - IMessageTransformer.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.Transform
{
    using System.IO;

    /// <summary>
    /// The MessageTransformer interface.
    /// </summary>
    /// <typeparam name="T">
    /// The type generic placeholder.
    /// </typeparam>
    public interface IMessageTransformer<T>
    {
        /// <summary>
        /// The transform from.
        /// </summary>
        /// <param name="data">
        /// The data parameter.
        /// </param>
        /// <returns>
        /// The <see cref="T"/> type T.
        /// </returns>
        T TransformFrom(byte[] data);

        /// <summary>
        /// The transform from.
        /// </summary>
        /// <param name="streamFrom">
        /// The stream from.
        /// </param>
        /// <returns>
        /// The <see cref="T"/> type T.
        /// </returns>
        T TransformFrom(Stream streamFrom);

        /// <summary>
        /// The transform to.
        /// </summary>
        /// <param name="transformObject">
        /// The transform object.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/> data.
        /// </returns>
        byte[] TransformTo(T transformObject);

        /// <summary>
        /// The transform to.
        /// </summary>
        /// <param name="streamTo">
        /// The stream to.
        /// </param>
        /// <param name="transformObject">
        /// The transform object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> data.
        /// </returns>
        bool TransformTo(Stream streamTo, T transformObject);
    }
}