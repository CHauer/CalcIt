using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcIt.Lib.NetworkAccess.Transform
{
    public class DataContractTransformer<T> : IMessageTransformer<T> where T : class
    {
        public bool TransformTo(System.IO.Stream streamTo, T transformObject)
        {
            throw new NotImplementedException();
        }

        public T TransformFrom(System.IO.Stream streamFrom)
        {
            throw new NotImplementedException();
        }
    }
}
