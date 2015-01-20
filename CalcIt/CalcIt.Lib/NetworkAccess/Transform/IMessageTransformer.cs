using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CalcIt.Lib.NetworkAccess.Transform
{
    public interface IMessageTransformer<T>
    {
        bool TransformTo(Stream streamTo, T transformObject);

        T TransformFrom(Stream streamFrom);
    }
}
