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
    public class WasherVcodeDal : BaseRepository<WasherVcodeModel>
    {
        public static WasherVcodeDal Instance
        {
            get { return SingletonProvider<WasherVcodeDal>.Instance; }
        }

        public WasherVcodeModel Get(string telphone)
        {
            return GetWhere(new { Telphone = telphone }).OrderByDescending(v => v.Created).FirstOrDefault();
        }
    }
}
