using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasherBusiness
{
    public enum RequestCommand
    {
        Heart = 0x0000/*心跳同步*/,
        Sync = 0x00C9/*时间同步*/,
        Config = 0x00CA/*读取设置项*/,
        Upload = 0x00CB/*设备上传状态*/,
        Temp = 0x00CC/*设备上传的临时数据*/,
        Balance = 0x00CD/*消费结算*/,
        CardAndPassword = 0x0065/*卡号+密码验证*/,
        CardOnly = 0x0066/*刷卡验证*/,
        PhoneAndPassword = 0x0067/*手机号+密码验证*/
    }

    public class BoardRequestInfo : IRequestInfo
    {
        public long Ticks { get; internal set; }
        public string Key { get; internal set; }
        public RequestCommand Command { get; internal set; }
        public string BoardNumber { get; internal set; }

        public bool[] BitStatus { get; internal set; }
        public int[] ValueStatus { get; internal set; }
        public string Card { get; internal set; }
        public string Password { get; internal set; }
        public string Telphone { get; internal set; }
        public int BalanceId { get; internal set; }
        public int Payment { get; internal set; }
    }

    public class BoardIReceiveFilter : ReceiveFilterBase<BoardRequestInfo>
    {
        private long ByteToInt64(byte b)
        {
            return (b + 256) % 256;
        }

        private int ByteToInt32(byte b)
        {
            return (b + 256) % 256;
        }

        public override BoardRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            if (length == 1)
            {
                rest = length - 1;
                return new BoardRequestInfo() { Command = RequestCommand.Heart, Key = Enum.GetName(typeof(RequestCommand), RequestCommand.Heart) };
            }
            else
            {
                int dataLength = ByteToInt32(readBuffer[offset + 6]);
                //接收到的数据长度小于合法的数据长度
                if (4 + 2 + 1 + dataLength > length)
                {
                    rest = 0;
                    return null;
                }

                int cmdValue = (ByteToInt32(readBuffer[offset + 4]) << 8) + ByteToInt32(readBuffer[offset + 5]);
                //接收到的数据不合法，直接过滤掉
                if (!Enum.IsDefined(typeof(RequestCommand), cmdValue))
                {
                    rest = 0;
                    return null;
                }

                RequestCommand commad = (RequestCommand)Enum.ToObject(typeof(RequestCommand), cmdValue);

                rest = length - (4 + 2 + 1 + dataLength);
                long ticks = (ByteToInt32(readBuffer[offset]) << 24) +
                    (ByteToInt32(readBuffer[offset + 1]) << 16) +
                    (ByteToInt32(readBuffer[offset + 2]) << 8) +
                    ByteToInt32(readBuffer[offset + 3]);
                string boardNumber = string.Format("{0:000000}", (ByteToInt64(readBuffer[offset + 7]) << 24) +
                    (ByteToInt64(readBuffer[offset + 8]) << 16) +
                    (ByteToInt64(readBuffer[offset + 9]) << 8) +
                   ByteToInt64(readBuffer[offset + 10]));

                BoardRequestInfo requestInfo = new BoardRequestInfo()
                {
                    Ticks = ticks,
                    Command = commad,
                    Key = Enum.GetName(typeof(RequestCommand), commad),
                    BoardNumber = boardNumber
                };

                if (commad == RequestCommand.Upload)
                {
                    requestInfo.BitStatus = new bool[32];
                    requestInfo.ValueStatus = new int[10];

                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            requestInfo.BitStatus[i + j * 8] = (readBuffer[offset + 11 + j] & (1 << i)) == (1 << i);//低位在前
                            //requestInfo.BitStatus[i + j * 8] = (readBuffer[offset + 11 + j] & (1 << 7 - i)) == (1 << 7 - i);//高位在前
                        }
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        requestInfo.ValueStatus[i] = (ByteToInt32(readBuffer[offset + 15 + i * 2]) << 8) + ByteToInt32(readBuffer[offset + 15 + i * 2 + 1]);
                    }
                }
                else if (commad == RequestCommand.CardAndPassword)
                {
                    requestInfo.Card = string.Format("{0:0000000000}", (ByteToInt64(readBuffer[offset + 11]) << 24) +
                        (ByteToInt64(readBuffer[offset + 12]) << 16) +
                        (ByteToInt64(readBuffer[offset + 13]) << 8) +
                        ByteToInt64(readBuffer[offset + 14]));
                    requestInfo.Password = string.Format("{0:000000}", (ByteToInt64(readBuffer[offset + 15]) << 24) +
                        (ByteToInt64(readBuffer[offset + 16]) << 16) +
                        (ByteToInt64(readBuffer[offset + 17]) << 8) +
                        ByteToInt64(readBuffer[offset + 18]));
                }
                else if (commad == RequestCommand.CardOnly)
                {
                    requestInfo.Card = string.Format("{0:0000000000}", (ByteToInt64(readBuffer[offset + 11]) << 24) +
                        (ByteToInt64(readBuffer[offset + 12]) << 16) +
                        (ByteToInt64(readBuffer[offset + 13]) << 8) +
                        ByteToInt64(readBuffer[offset + 14]));
                }
                else if (commad == RequestCommand.PhoneAndPassword)
                {
                    requestInfo.Telphone = string.Format("{0:00000000000}", (ByteToInt64(readBuffer[offset + 11]) << 40) +
                        (ByteToInt64(readBuffer[offset + 12]) << 32) +
                        (ByteToInt64(readBuffer[offset + 13]) << 24) +
                        (ByteToInt64(readBuffer[offset + 14]) << 16) +
                        (ByteToInt64(readBuffer[offset + 15]) << 8) +
                        ByteToInt64(readBuffer[offset + 16]));
                    requestInfo.Password = string.Format("{0:000000}", (ByteToInt64(readBuffer[offset + 17]) << 24) +
                        (ByteToInt64(readBuffer[offset + 18]) << 16) +
                        (ByteToInt64(readBuffer[offset + 19]) << 8) +
                        ByteToInt64(readBuffer[offset + 20]));
                }
                else if (commad == RequestCommand.Balance || commad==RequestCommand.Temp)
                {
                    requestInfo.BalanceId = (ByteToInt32(readBuffer[offset + 11]) << 24) +
                        (ByteToInt32(readBuffer[offset + 12]) << 16) +
                        (ByteToInt32(readBuffer[offset + 13]) << 8) +
                        ByteToInt32(readBuffer[offset + 14]);
                    requestInfo.Payment = (ByteToInt32(readBuffer[offset + 15]) << 24) +
                        (ByteToInt32(readBuffer[offset + 16]) << 16) +
                        (ByteToInt32(readBuffer[offset + 17]) << 8) +
                        ByteToInt32(readBuffer[offset + 18]);
                }

                return requestInfo;
            }
        }
    }

    public class BoardAppSession : AppSession<BoardAppSession, BoardRequestInfo>
    {
        public int HeartBeatCount { get; set; }

        public string BoardNumber { get; set; }

        public BoardAppSession()
        {
            HeartBeatCount = 0;
        }
    }

    public class BoardAppServer : AppServer<BoardAppSession, BoardRequestInfo>
    {
        public int DepartmentId { get; private set; }
        public string DepartmentName { get; private set; }

        public BoardAppServer(int departmentId, string departmentName) : base(new DefaultReceiveFilterFactory<BoardIReceiveFilter, BoardRequestInfo>())
        {
            this.DepartmentId = departmentId;
            this.DepartmentName = departmentName;
        }
    }
}
