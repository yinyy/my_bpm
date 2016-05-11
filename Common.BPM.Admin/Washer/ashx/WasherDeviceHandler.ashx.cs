using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;
using Omu.ValueInjecter;
using BPM.Core.Dal;
using ZXing.QrCode;
using ZXing;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Configuration;

namespace BPM.Admin.Washer.ashx
{
    //设备当前的状态，0表示状态位置，1表示当前正在工作，2表示空闲

    /// <summary>
    /// WasherDeviceHandler 的摘要说明
    /// </summary>
    public class WasherDeviceHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherDeviceModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherDeviceModel>>(json);
                rpm.CurrentContext = context;
            }

            WasherDeviceModel model;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;

                    model.BoardNumber = "";
                    model.Province = "";
                    model.City = "";
                    model.Region = "";
                    model.Address = "";
                    model.Status = "";
                    model.IpAddress = "";
                    model.Memo2 = "";
                    model.Setting = string.Format("{{\"Coin\": 0, \"Params\":[{0}]}}", ConfigurationManager.AppSettings["board_default_setting"]);
                  
                    context.Response.Write(WasherDeviceBll.Instance.Add(model));
                    break;
                case "users":
                    context.Response.Write(JSONhelper.ToJson(UserDal.Instance.GetAll().Select(u => new { KeyId = u.KeyId, Title = u.TrueName + "()" })));
                    break;
                case "dpts":
                    context.Response.Write(JSONhelper.ToJson(DepartmentDal.Instance.GetAll().Select(d => new { KeyId = d.KeyId, Title = d.DepartmentName })));
                    break;
                case "edit":
                    model = WasherDeviceBll.Instance.Get(rpm.KeyId);
                    model.SerialNumber = rpm.Entity.SerialNumber;
                    model.BoardNumber = rpm.Entity.BoardNumber;
                    model.DeliveryTime = rpm.Entity.DeliveryTime;
                    model.ProductionTime = rpm.Entity.ProductionTime;
                    model.DepartmentId = rpm.Entity.DepartmentId;
                    model.Memo = rpm.Entity.Memo;
        
                    context.Response.Write(WasherDeviceBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(WasherDeviceBll.Instance.Delete(rpm.KeyId));
                    break;
                case "set":
                    model = WasherDeviceBll.Instance.Get(rpm.KeyId);
                    model.Province = rpm.Entity.Province.Substring(rpm.Entity.Province.IndexOf('_')+1);
                    model.City = rpm.Entity.City.Substring(rpm.Entity.City.IndexOf('_') + 1);
                    model.Region= rpm.Entity.Region.Substring(rpm.Entity.Region.IndexOf('_') + 1);
                    model.Address = rpm.Entity.Address;
                    model.Setting = rpm.Entity.Setting;
                    model.BoardNumber = string.Format("{0:000000000}", rpm.Entity.BoardNumber);
                    model.Memo2 = rpm.Entity.Memo2;

                    context.Response.Write(WasherDeviceBll.Instance.Update(model));
                    break;
                case "device_qrcode":
                    model = WasherDeviceBll.Instance.Get(rpm.KeyId);
                    string url = string.Format("/qrcode/{0}", model.KeyId);
                    string filename = context.Server.MapPath(url);

                    if (!Directory.Exists(filename))
                    {
                        Directory.CreateDirectory(filename);
                    }
                    
                    url = string.Format("{0}/{1}.jpg", url, model.SerialNumber);
                    filename = context.Server.MapPath(url);

                    if (File.Exists(filename))
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = true, Url = url }));
                    }
                    else {
                        try
                        {
                            BarcodeWriter writer = new BarcodeWriter();
                            writer.Format = BarcodeFormat.QR_CODE;
                            writer.Options = new QrCodeEncodingOptions()
                            {
                                CharacterSet = "utf-8",
                                DisableECI = true,
                                Width = 2048,
                                Height = 2048
                            };
                            writer.Write(model.SerialNumber).Save(filename, ImageFormat.Jpeg);
                            context.Response.Write(JSONhelper.ToJson(new { Success = true, Url = url }));
                        }
                        catch (Exception e)
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = e.Message }));
                        }
                    }
                    break;
                case "edit2":
                    model = WasherDeviceBll.Instance.Get(rpm.KeyId);

                    

                    context.Response.Write(WasherDeviceBll.Instance.Update(model));
                    break;
                case "del2":
                    model = WasherDeviceBll.Instance.Get(rpm.KeyId);
                    

                    context.Response.Write(WasherDeviceBll.Instance.Update(model));
                    break;
                case "dept":
                    context.Response.Write(JSONhelper.ToJson(DepartmentDal.Instance.GetAll().OrderBy(a => a.DepartmentName).Select(a => new { KeyId = a.KeyId, DepartmentName = a.DepartmentName })));
                    break;
                case "list2":
                    string filter = string.Format("{{'groupOp':'AND','rules':[{{'field':'DepartmentId','op':'eq','data':{0}}}, {{'field':'Deleted','op':'eq','data':0}}],'groups':[{1}]}}", departmentId, rpm.Filter);
                    context.Response.Write(WasherDeviceBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                case "start":
                    IPAddress address = IPAddress.Parse("192.168.31.135");
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        socket.Connect(address, 6000);
                    }catch(Exception exp)
                    {
                        Console.WriteLine(exp.Message);
                    }

                    byte[] bs = { 0x00, 0x00, 0x00, 0x00, 0x07, 0xd1, 0x05, 0x01, 0x02, 0x03, 0x04, 0x01 };
                    socket.Send(bs, SocketFlags.None);

                    byte[] buffer = new byte[1024];
                    int len = socket.Receive(buffer);

                    foreach(byte b in buffer){
                        Console.Write(string.Format("{0:x2}", b));
                    }
                    Console.WriteLine();

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    context.Response.Write("1");
                    break;
                default:
                    context.Response.Write(WasherDeviceBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}