using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherWeChatConsumeDal : BaseRepository<WasherWeChatConsumeModel>
    {
        public static WasherWeChatConsumeDal Instance
        {
            get { return SingletonProvider<WasherWeChatConsumeDal>.Instance; }
        }
    }
}

