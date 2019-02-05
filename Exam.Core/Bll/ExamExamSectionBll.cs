using BPM.Common.Provider;
using Exam.Core.Dal;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Bll
{
    public class ExamExamSectionBll
    {
        public static ExamExamSectionBll Instance
        {
            get { return SingletonProvider<ExamExamSectionBll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return ExamExamSectionDal.Instance.GetJson(pageindex, pagesize, filter, sort, order);
        }

        public int Insert(ExamExamSectionModel entity)
        {
            return ExamExamSectionDal.Instance.Insert(entity);
        }

        public int Delete(int keyId)
        {
            return ExamExamSectionDal.Instance.Delete(keyId);
        }

        public int Update(ExamExamSectionModel model)
        {
            return ExamExamSectionDal.Instance.Update(model);
        }

        public ExamExamSectionModel Get(int keyId)
        {
            return ExamExamSectionDal.Instance.Get(keyId);
        }
    }
}
