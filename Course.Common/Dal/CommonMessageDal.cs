using BPM.Common.Data;
using BPM.Common.Provider;
using Course.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Common.Dal
{
    public class CommonMessageDal : BaseRepository<CommonMessageModel>
    {

        public static CommonMessageDal Instance
        {
            get { return SingletonProvider<CommonMessageDal>.Instance; }
        }
    }
}