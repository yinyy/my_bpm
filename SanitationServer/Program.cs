
using BPM.Core.Dal;
using Sanitation.Bll;
using Sanitation.Model;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;

namespace SanitationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("启动服务器...");

            var app = new MyAppServer();
            if (!app.Setup(3001))
            {
                Console.WriteLine("服务器端口被占用。");
                return;
            }

            Console.WriteLine("服务器创建成功。");

            if (!app.Start())
            {
                Console.WriteLine("服务器启动失败。");
                return;
            }

            Console.WriteLine("服务器启动成功。");
            Console.WriteLine("停止服务请按‘Q’键。");

            app.NewSessionConnected += App_NewSessionConnected;
            app.NewRequestReceived += App_NewRequestReceived;

            while (Console.ReadKey().KeyChar != 'Q')
            {
                Console.WriteLine("停止服务请按‘Q’键。");
            }

            app.Stop();
            Console.WriteLine("服务器已经停止。");
        }

        private static void App_NewRequestReceived(MySession session, UploadDataRequestInfo requestInfo)
        {
            //SanitationDispatchModel model = new SanitationDispatchModel();
            //model.DriverId = requestInfo.Person;
            //model.TrunkId = requestInfo.Trunk;
            //model.Volumn = requestInfo.Volumn/10.0f;
            //model.Time = requestInfo.Created;
            //model.Kind = 1;
            //model.Address = DicDal.Instance.GetWhere(new { Code = requestInfo.Address }).FirstOrDefault().Title;
            //model.Potency = requestInfo.Potency;
            //model.Status = 1;
            //model.Memo = "";

            //SanitationDispatchBll.Instance.Add(model);
        }

        private static void App_NewSessionConnected(MySession session)
        {
            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss}，与客户端建立连接。", DateTime.Now);
        }
    }

    class MySession: AppSession<MySession, UploadDataRequestInfo>
    {
        private Timer timer;
        private byte[] command = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x00, 0x03, 0x00, 0x0a };

        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();

            timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            Send(command, 0, command.Length);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            timer.Stop();
            timer.Elapsed -= Timer_Elapsed;
        }
    }

    class MyAppServer: AppServer<MySession, UploadDataRequestInfo>
    {
        public MyAppServer() : base(new DefaultReceiveFilterFactory<FixedLengthDataReceiverFilter, UploadDataRequestInfo>())
        {
        }
    }

    class BeginEndMarkDataReceiverFilter : BeginEndMarkReceiveFilter<UploadDataRequestInfo>
    {
        private readonly static byte[] BeginMark = new byte[]{ (byte)'A', (byte)'A' };
        private readonly static byte[] EndMark = new byte[] { (byte)'B', (byte)'B' };

        public BeginEndMarkDataReceiverFilter() : base(BeginMark, EndMark)
        {

        }

        protected override UploadDataRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            string data = Encoding.UTF8.GetString(readBuffer, offset, length);
            data = data.Substring(2);
            data = data.Substring(0, data.Length - 2);

            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss}，获取请求数据：\"{1}\"。",DateTime.Now, data);

            string[] datas = data.Split(',');

            UploadDataRequestInfo requestInfo = new UploadDataRequestInfo();
            requestInfo.Address = datas[0];
            requestInfo.Created = DateTime.ParseExact(datas[1], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            requestInfo.Person = Convert.ToInt32(datas[2]);
            requestInfo.Trunk = Convert.ToInt32(datas[3]);
            requestInfo.Volumn = Convert.ToInt32(datas[4]);
            requestInfo.Potency = Convert.ToInt32(datas[5]);

            return requestInfo;
        }
    }

    class FixedLengthDataReceiverFilter : FixedSizeReceiveFilter<UploadDataRequestInfo>
    {
        public FixedLengthDataReceiverFilter() : base(29)
        {

        }

        protected override UploadDataRequestInfo ProcessMatchedRequest(byte[] buffer, int offset, int length, bool toBeCopied)
        {
            for(int i = offset; i < offset + length; i++)
            {
                Console.Write("{0:x2} ", buffer[i]);
            }




            string data = ""; Encoding.UTF8.GetString(buffer, offset, length);
            //data = data.Substring(2);
            //data = data.Substring(0, data.Length - 2);

            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss}，获取请求数据：\"{1}\"。", DateTime.Now, data);

            string[] datas = data.Split(',');

            UploadDataRequestInfo requestInfo = new UploadDataRequestInfo();
            //requestInfo.Address = datas[0];
            //requestInfo.Created = DateTime.ParseExact(datas[1], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            //requestInfo.Person = Convert.ToInt32(datas[2]);
            //requestInfo.Trunk = Convert.ToInt32(datas[3]);
            //requestInfo.Volumn = Convert.ToInt32(datas[4]);
            //requestInfo.Potency = Convert.ToInt32(datas[5]);

            return requestInfo;
        }
    }

    class UploadDataRequestInfo : IRequestInfo
    {
        public string Key { get; set; }

        public DateTime Created { get; set; }
        public int Person { get; set; }
        public int Trunk { get; set; }
        public int Volumn { get; set; }
        public string Address { get; set; }
        public int Potency { get; set; }
    }
}
