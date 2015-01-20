using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Protocol.Data;

namespace CalcIt.Protocol.Client
{
    public class HighscoreResponse : CalcItClientMessage
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
    }
}
