﻿using BPM.Common.Provider;
using System.Collections.Generic;
using System.Linq;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherDeviceBll
    {
        public static WasherDeviceBll Instance
        {
            get { return SingletonProvider<WasherDeviceBll>.Instance; }
        }

        public long Add(WasherDeviceModel model)
        {
            return WasherDeviceDal.Instance.Insert(model);
        }

        public int Update(WasherDeviceModel model)
        {
            return WasherDeviceDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return WasherDeviceDal.Instance.Delete(keyid);
        }

        public IEnumerable<WasherDeviceModel> GetAll()
        {
            return WasherDeviceDal.Instance.GetAll();
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherDeviceDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public WasherDeviceModel Get(int keyId)
        {
            return WasherDeviceDal.Instance.GetWhere(new { KeyId = keyId }).FirstOrDefault();
        }

        public WasherDeviceModel Get(string serialNumber)
        {
            return WasherDeviceDal.Instance.GetWhere(new { SerialNumber = serialNumber }).FirstOrDefault();
        }

        public WasherDeviceModel GetByBoardNumber(string deviceID)
        {
            return WasherDeviceDal.Instance.GetWhere(new { BoardNumber=deviceID}).FirstOrDefault();
        }
    }
}