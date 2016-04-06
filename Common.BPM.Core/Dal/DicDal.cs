using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Core.Model;
using BPM.Common.Data;
using BPM.Common.Provider;

namespace BPM.Core.Dal
{
    public class DicDal : BaseRepository<Dic>
    {
        public static DicDal Instance
        {
            get { return SingletonProvider<DicDal>.Instance; }
        }

       
        public IEnumerable<Dic>  GetListBy(int categoryId)
        {
            return GetWhere(new {CategoryId = categoryId});
        }

        public IEnumerable<Dic> GetListBy(int categoryid, int parentId)
        {
            var list = GetListBy(categoryid);
            return from n in list
                   where n.ParentId == parentId
                   select n;
        }

        /// <summary>
        /// 更新字典类别的默认值
        /// </summary>
        /// <param name="dicid">字典Id</param>
        public void UpdateDefaultState(int dicid)
        {
            var dic = Get(dicid);
            if (dic != null)
            {
                var _cateid = dic.CategoryId;

                DbUtils.ExecuteNonQuery("update sys_dics set isdefault=0 where categoryid=@cateid and keyid<>@Keyid", new
                    {
                        cateid = _cateid,
                        Keyid = dicid
                    });
            }
        }
    }
}
