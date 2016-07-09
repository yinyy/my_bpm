using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Bll
{
    public class WasherSendNewsBll
    {
        public static WasherSendNewsBll Instance
        {
            get { return SingletonProvider<WasherSendNewsBll>.Instance; }
        }
    }
}
