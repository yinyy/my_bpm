using BPM.Common.Provider;
using Exam.Core.Dal;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Bll
{
    public class ExamExamBll
    {
        public static ExamExamBll Instance
        {
            get { return SingletonProvider<ExamExamBll>.Instance; }
        }

        public int Insert(ExamExamModel model)
        {
            return ExamExamDal.Instance.Insert(model);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "KeyId", string order = "asc")
        {
            return ExamExamDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }


        public ExamExamModel Get(int keyId)
        {
            return ExamExamDal.Instance.Get(keyId);
        }

        public int Delete(int keyId)
        {
            return ExamExamDal.Instance.Delete(keyId);
        }

        public int Update(ExamExamModel model)
        {
            return ExamExamDal.Instance.Update(model);
        }

        public ExamExamModel[] GetAll()
        {
            return ExamExamDal.Instance.GetAll().OrderByDescending(m => m.Started).ToArray();
        }
    }
}
