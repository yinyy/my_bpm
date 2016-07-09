using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BoardListenerLib
{
    public abstract class ReplyMessageBase : TcpMessageBase
    {
        public ReplyMessageBase()
        {
            this.Ticks = (int)DateTime.Now.Ticks;
        }

        public ReplyMessageBase(string boardNumber)
        {
            this.BoardNumber = boardNumber;
            this.Ticks = (int)DateTime.Now.Ticks;
        }

        protected byte[] buffer;

        public virtual byte[] ToByteArray()
        {
            return buffer;
        }

        protected void InitBuffer(bool withDeviceId = true)
        {
            buffer = new byte[Length + 7];

            long ticks = DateTime.Now.Ticks;

            buffer[0] = (byte)((ticks >> 24) & 0xff);
            buffer[1] = (byte)(((ticks >> 16) & 0xff));
            buffer[2] = (byte)(((ticks >> 8) & 0xff));
            buffer[3] = (byte)(ticks & 0xff);

            buffer[4] = (byte)(((int)Command >> 8) & 0xff);
            buffer[5] = (byte)((int)Command & 0xff);

            buffer[6] = (byte)Length;

            if (withDeviceId)
            {
                long deviceID = Convert.ToInt32(BoardNumber);

                buffer[7] = (byte)((deviceID >> 24) & 0xff);
                buffer[8] = (byte)((deviceID >> 16) & 0xff);
                buffer[9] = (byte)((deviceID >> 8) & 0xff);
                buffer[10] = (byte)(deviceID & 0xff);
            }
        }
    }

    public class ReplyTimeSyncMessage : ReplyMessageBase
    {
        public ReplyTimeSyncMessage(string boardNumber):base(boardNumber)
        {
            this.Command = TcpMessageBase.CommandType.TimeSync;
        }

        public override byte[] ToByteArray()
        {
            Length = 6;
            InitBuffer(false);

            DateTime now = DateTime.Now;
            buffer[7] = (byte)(now.Year - 2000);
            buffer[8] = (byte)now.Month;
            buffer[9] = (byte)now.Day;
            buffer[10] = (byte)now.Hour;
            buffer[11] = (byte)now.Minute;
            buffer[12] = (byte)now.Second;

            return buffer;
        }

        public override string ToString()
        {
            return string.Format("方向：返回；命令：时间同步；设备号：{0}；时间戳：{1}。",
                this.BoardNumber,
                this.Ticks);
        }
    }

    public class ReplyReadSettingMessage : ReplyMessageBase
    {
        public int ErrorCode = 0;
        public int[] Values = new int[32];

        public ReplyReadSettingMessage(string boardNumber):base(boardNumber)
        {
            this.Command = CommandType.ReaderSetting;
        }
        
        public override byte[] ToByteArray()
        {
            if (ErrorCode == 0)
            {
                Length = 68;

                InitBuffer();

                for (int i = 0; i < Values.Length; i++)
                {
                    buffer[i * 2 + 11] = (byte)((Values[i] >> 8) & 0xff);
                    buffer[i * 2 + 11 + 1] = (byte)(Values[i] & 0xff);
                }
            }
            else
            {
                Length = 1;

                InitBuffer(false);

                buffer[7] = (byte)ErrorCode;
            }

            return buffer;
        }

        public override string ToString()
        {
            if (ErrorCode == 1)
            {
                return string.Format("方向：返回；命令：读取设置；设备号：{0}；错误。",
                    this.BoardNumber);
            }
            else
            {
                int index = 1;

                return string.Format("方向：返回；命令：读取设置；设备号：{0}；{1}。",
                    this.BoardNumber,
                    this.Values.Select(v => string.Format("参数{0:00}：{1}", index++, v)).Aggregate((r, v) =>
                    {
                        if (string.IsNullOrWhiteSpace(r))
                        {
                            r = v;
                        }
                        else
                        {
                            r += "；" + v;
                        }
                        return r;
                    }));
            }
        }
    }

    public class ReplyUploadStatusMessage : ReplyMessageBase
    {
        public int Status;

        public ReplyUploadStatusMessage(string boardNumber, int Status=1):base(boardNumber)
        {
            this.Command = CommandType.UploadStatus;
            this.Status = Status;
        }

        public override byte[] ToByteArray()
        {
            Length = 1;
            InitBuffer(false);

            buffer[7] = (byte)Status;

            return buffer;
        }

        public override string ToString()
        {
            return string.Format("方向：返回；命令：上传状态；设备号：{0}；状态：{1}。",
                    this.BoardNumber,
                    this.Status);
        }
    }

    public class ReplyAccountMessage : ReplyMessageBase
    {
        public int BalanceId;
        public int Remain;

        public ReplyAccountMessage(string boardNumber):base(boardNumber)
        {
            this.Command = CommandType.Account;
        }

        public override byte[] ToByteArray()
        {
            Length = 12;
            InitBuffer();

            buffer[11] = (byte)((BalanceId >> 24) & 0xff);
            buffer[12] = (byte)((BalanceId >> 16) & 0xff);
            buffer[13] = (byte)((BalanceId >> 8) & 0xff);
            buffer[14] = (byte)(BalanceId & 0xff);

            buffer[15] = (byte)((Remain >> 24) & 0xff);
            buffer[16] = (byte)((Remain >> 16) & 0xff);
            buffer[17] = (byte)((Remain >> 8) & 0xff);
            buffer[18] = (byte)(Remain & 0xff);

            return buffer;
        }

        public override string ToString()
        {
            return string.Format("方向：返回；命令：消费结算；设备号：{0}；记录序号：{1}；实际金额：{2:#.00}。",
                    this.BoardNumber,
                    this.BalanceId,
                    this.Remain/100.0);
        }
    }

    public class ReplyValidateMessage : ReplyMessageBase
    {
        public int BalanceId;
        public CardKind Kind;
        public CardStatus Status;
        public int Money;

        public ReplyValidateMessage(string boardNumber):base(boardNumber)
        {
            this.Command = CommandType.ValidateCardAndPassword;
        }

        public override byte[] ToByteArray()
        {
            Command = CommandType.ValidateCardAndPassword;//不接受其它的返回值

            if (Status == CardStatus.Regular)
            {
                Length = 14;
                InitBuffer();

                buffer[11] = (byte)((BalanceId >> 24) & 0xff);
                buffer[12] = (byte)((BalanceId >> 16) & 0xff);
                buffer[13] = (byte)((BalanceId >> 8) & 0xff);
                buffer[14] = (byte)(BalanceId & 0xff);

                buffer[15] = (byte)Kind;
                buffer[16] = (byte)Status;

                buffer[17] = (byte)((Money >> 24) & 0xff);
                buffer[18] = (byte)((Money >> 16) & 0xff);
                buffer[19] = (byte)((Money >> 8) & 0xff);
                buffer[20] = (byte)(Money & 0xff);
            }
            else
            {
                Length = 10;
                InitBuffer();

                buffer[11] = (byte)((BalanceId >> 24) & 0xff);
                buffer[12] = (byte)((BalanceId >> 16) & 0xff);
                buffer[13] = (byte)((BalanceId >> 8) & 0xff);
                buffer[14] = (byte)(BalanceId & 0xff);

                buffer[15] = (byte)Kind;
                buffer[16] = (byte)Status;
            }

            return buffer;
        }

        public override string ToString()
        {
            return string.Format("方向：返回；命令：验证账户；设备号：{0}；记录序号：{1}；类型：{2}；状态：{3}；金额：{4:#.00}。",
                    this.BoardNumber,
                    this.BalanceId,
                    this.Kind.ToString(),
                    this.Status.ToString(),
                    this.Money / 100.0);
        }
    }

    public class ReplyHeartBeatMessage : ReplyMessageBase
    {
        public string Address="0.0.0.0";

        public ReplyHeartBeatMessage() : base(null)
        {

        }
        
        public override byte[] ToByteArray()
        {
            buffer = new byte[1];
            buffer[0] = 0x01;

            return buffer;
        }

        public override string ToString()
        {
            return string.Format("方向：返回；命令：心跳同步，IP地址：{0}。",
                    this.Address);
        }
    }

    public class ReplySendMessage : ReplyMessageBase
    {
        public ReplySendMessage(byte[] buffer):base()
        {
            this.buffer = buffer;
            ClearHighBits();
        }

        public override string ToString()
        {
            return string.Format("方向：返回；命令：直接发送；设备号：{0}。",
                    this.BoardNumber);
        }

        private void ClearHighBits()
        {
            buffer[4] = 0x00;
        }
    }

    public class ReplyUnknownMessage : ReplyMessageBase
    {
        public ReplyUnknownMessage(byte[] bs):base(null)
        {
            this.buffer = bs;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
            {
                sb.Append(string.Format("{0:x2} ", b));
            }

            return string.Format("方向：返回；命令：未知，数据：{0}。", sb.ToString().Trim().ToUpper());
        }
    }
}
