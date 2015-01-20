using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Data;

namespace CalcIt.Protocol.Server
{
    public class Merge : CalcItServerMessage
    {
        public List<HighScoreItem> HighScoreList
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public List<ServerGameClientItem> GameClientList
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
