using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Lib.NetworkAccess.Transform
{
    using System.IO;
    using System.Runtime.Serialization;

    public class DataContractTransformer<T> : IMessageTransformer<T> where T : class
    {
      
        public bool TransformTo(Stream streamTo, T transformObject)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            try
            {
                serializer.WriteObject(streamTo, transformObject);
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        public T TransformFrom(Stream streamFrom)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            try
            {
                return (T)serializer.ReadObject(streamFrom);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
