// -----------------------------------------------------------------------
// <copyright file="DataContractTransformer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - DataContractTransformer.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.NetworkAccess.Transform
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// The data contract transformer.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class DataContractTransformer<T> : IMessageTransformer<T>
        where T : class
    {
        /// <summary>
        /// The transform from.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T TransformFrom(byte[] data)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// The transform from.
        /// </summary>
        /// <param name="streamFrom">
        /// The stream from.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T TransformFrom(Stream streamFrom)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            try
            {
                return (T)serializer.ReadObject(streamFrom);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// The transform to.
        /// </summary>
        /// <param name="transformObject">
        /// The transform object.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] TransformTo(T transformObject)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, transformObject);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

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
        /// The <see cref="bool"/>.
        /// </returns>
        public bool TransformTo(Stream streamTo, T transformObject)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            try
            {
                serializer.WriteObject(streamTo, transformObject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}