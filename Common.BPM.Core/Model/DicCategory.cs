﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Common;
using BPM.Common.Data;

namespace BPM.Core.Model
{
    [TableName("Sys_DicCategory")]
    public class DicCategory
    {
        public int KeyId{get;set;}

        public string Title { get; set; }

        public string Code { get; set; }

        public int Sortnum { get; set; }

        public string Remark { get; set; }

        public override string ToString()
        {
            return JSONhelper.ToJson(this);
        }

    }
}
