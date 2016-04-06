using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherReceivedMessageDal : BaseRepository<WasherReceivedMessageModel>
    {
        public static WasherReceivedMessageDal Instance
        {
            get { return SingletonProvider<WasherReceivedMessageDal>.Instance; }
        }
    }
}
