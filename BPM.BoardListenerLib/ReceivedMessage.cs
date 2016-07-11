using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BoardListenerLib
{
    public class ReceivedMessageBase : TcpMessageBase
    {
        public byte[] Meta;

        //private ReplyMessageBase CreateReplyMessage()
        //{
        //    ReplyMessageBase mo = null;
        //    switch (Command)
        //    {
        //        case CommandType.HeartBeat:
        //            mo = new ReplyHeartBeatMessage();
        //            break;
        //        case CommandType.ValidateCardAndPassword:
        //        case CommandType.ValidateCard:
        //        case CommandType.ValidatePhoneAndPassword:
        //            mo = new ReplyValidateMessage();
        //            break;
        //        case CommandType.TimeSync:
        //            mo = new ReplyTimeSyncMessage();
        //            break;
        //        case CommandType.ReaderSetting:
        //            mo = new ReplyReadSettingMessage();
        //            break;
        //        case CommandType.UploadStatus:
        //            mo = new ReplyUploadStatusMessage();
        //            break;
        //        case CommandType.Account:
        //            mo = new ReplyAccountMessage();
        //            break;
        //        case CommandType.Send:
        //            mo = new ReplySendMessage(Meta);
        //            break;
        //        default:
        //            mo = new ReplyUnknownMessage(Meta);
        //            break;
        //    }

        //    mo.Command = Command;
        //    mo.BoardNumber = BoardNumber;
        //    mo.Ticks = (int)DateTime.Now.Ticks;

        //    return mo;
        //}

        public static ReceivedMessageBase Parse(byte[] bs)
        {
            ReceivedMessageBase mo = null;
            if (bs.Length == 1)
            {
                mo = new ReceivedHeartBeatMessage(bs.Length) { Command = CommandType.HeartBeat };
            }
            else if (bs.Length >= 6)
            {
                if (bs[4] == 0xff)
                {
                    mo = new ReceivedSendMessage() { Command = CommandType.Send };
                }
                else
                {
                    int command = (bs[4] << 8) + bs[5];
                    switch (command)
                    {
                        case 201:
                            mo = new ReceivedTimeSyncMessage() { Command = CommandType.TimeSync };
                            break;
                        case 202:
                            mo = new ReceivedReadSettingMessage() { Command = CommandType.ReaderSetting };
                            break;
                        case 203:
                            mo = new ReceivedUploadStatusMessage() { Command = CommandType.UploadStatus };
                            break;
                        case 101:
                            mo = new ReceivedValidateCardAndPasswordMessage() { Command = CommandType.ValidateCardAndPassword };
                            break;
                        case 102:
                            mo = new ReceivedValidateCardMessage() { Command = CommandType.ValidateCard };
                            break;
                        case 103:
                            mo = new ReceivedValidatePhoneAndPasswordMessage() { Command = CommandType.ValidatePhoneAndPassword };
                            break;
                        case 205:
                            mo = new ReceivedAccountMessage() { Command = CommandType.Account };
                            break;
                        default:
                            mo = new ReceivedUnknownMessage() { Command = CommandType.Unknown };
                            break;
                    }
                }
            }
            else
            {
                mo = new ReceivedUnknownMessage() { Command = CommandType.Unknown };
            }
        
            mo.Meta = bs;

            if (mo.Command !=CommandType.Unknown)
            {
                if (mo.Command != CommandType.HeartBeat)
                {
                    mo.Ticks = (bs[0] << 24) + (bs[1] << 16) + (bs[2] << 8) + bs[3];
                    mo.Length = bs[6];
                }

                if (mo.Meta.Length >= 11)
                {
                    mo.BoardNumber = string.Format("{0:000000}", (bs[7] << 24) + (bs[8] << 16) + (bs[9] << 8) + bs[10]);
                }

                switch (mo.Command)
                {
                    case CommandType.ValidateCardAndPassword:
                        ReceivedValidateCardAndPasswordMessage rvcpm = mo as ReceivedValidateCardAndPasswordMessage;

                        rvcpm.CardNo = string.Format("{0:0000000000}", ((long)bs[11] << 24) + ((long)bs[12] << 16) + (bs[13] << 8) + bs[14]);
                        rvcpm.Password = string.Format("{0:000000}", (bs[15] << 24) + (bs[16] << 16) + (bs[17] << 8) + bs[18]);
                        break;
                    case CommandType.ValidateCard:
                        ReceivedValidateCardMessage rvcm = mo as ReceivedValidateCardMessage;
                        rvcm.CardNo = string.Format("{0:0000000000}", ((long) bs[11] << 24) + ((long)bs[12] << 16) + (bs[13] << 8) + bs[14]);
                        break;
                    case CommandType.ValidatePhoneAndPassword:
                        ReceivedValidatePhoneAndPasswordMessage rvppm = mo as ReceivedValidatePhoneAndPasswordMessage;

                        rvppm.Phone = string.Format("{0:00000000000}", ((long)bs[11] << 40) + ((long)bs[12] << 32) + (bs[13] << 24) + (bs[14] << 16) + (bs[15] << 8) + bs[16]);
                        rvppm.Password = string.Format("{0:000000}", (bs[17] << 24) + (bs[18] << 16) + (bs[19] << 8) + bs[20]);
                        break;
                    case CommandType.TimeSync:
                        break;
                    case CommandType.ReaderSetting:
                        break;
                    case CommandType.UploadStatus:
                        ReceivedUploadStatusMessage rmus = mo as ReceivedUploadStatusMessage;

                        rmus.BitStatus = (bs[11] << 24) + (bs[12] << 16) + (bs[13] << 8) + bs[14];
                        for (int i = 0; i < rmus.ValueStatus.Length; i++)
                        {
                            rmus.ValueStatus[i] = (bs[i * 2 + 15] << 8) + bs[i * 2 + 1 + 15];
                        }
                        break;
                    case CommandType.Account:
                        ReceivedAccountMessage rma = mo as ReceivedAccountMessage;

                        rma.BalanceId = (bs[11] << 24) + (bs[12] << 16) + (bs[13] << 8) + bs[14];
                        rma.Payment = (bs[15] << 24) + (bs[16] << 16) + (bs[17] << 8) + bs[18];
                        break;
                    default:
                        break;
                }
            }

            return mo;
        }
    }

    public class ReceivedHeartBeatMessage : ReceivedMessageBase
    {
        public ReceivedHeartBeatMessage(int Length)
        {
            this.Length = Length;
        }

        public override string ToString()
        {
            return string.Format("方向：接收；命令：心跳同步。");
        }
    }

    public class ReceivedTimeSyncMessage : ReceivedMessageBase
    {
        public override string ToString()
        {
            return string.Format("方向：接收；命令：时间同步；设备号：{0}。", this.BoardNumber);
        }
    }

    public class ReceivedReadSettingMessage : ReceivedMessageBase
    {
        public override string ToString()
        {
            return string.Format("方向：接收；命令：读取设置；设备号：{0}。", this.BoardNumber);
        }
    }

    public class ReceivedUploadStatusMessage : ReceivedMessageBase
    {
        public int BitStatus;
        public int[] ValueStatus = new int[10];

        public override string ToString()
        {
            return string.Format("方向：接收；命令：上传状态；设备号：{0}；泡沫箱液位：{1}；输出1：{2}；输出2：{3}；输出3：{4}；输出4：{5}；输出5：{6}；输出6：{7}；输出7：{8}；输出8：{9}；输出9：{10}；温度值1：{11}；温度值2：{12}。",
                this.BoardNumber,
                (this.BitStatus & 0x80000000) == 0x80000000,
                (this.BitStatus & 0x00008000) == 0x00008000,
                (this.BitStatus & 0x00004000) == 0x00004000,
                (this.BitStatus & 0x00002000) == 0x00002000,
                (this.BitStatus & 0x00001000) == 0x00001000,
                (this.BitStatus & 0x00000800) == 0x00000800,
                (this.BitStatus & 0x00000400) == 0x00000400,
                (this.BitStatus & 0x00000200) == 0x00000200,
                (this.BitStatus & 0x00000100) == 0x00000100,
                (this.BitStatus & 0x00800000) == 0x00800000,
                this.ValueStatus[0] - 40,
                this.ValueStatus[1] - 40);
        }
    }

    public class ReceivedValidateCardAndPasswordMessage : ReceivedMessageBase
    {
        public string CardNo;
        public string Password;

        public override string ToString()
        {
            return string.Format("方向：接收；命令：验证卡密；设备号：{0}；卡号：{1}；密码：{2}。",
                this.BoardNumber,
                this.CardNo,
                this.Password);
        }
    }

    public class ReceivedValidateCardMessage : ReceivedMessageBase
    {
        public string CardNo;

        public override string ToString()
        {
            return string.Format("方向：接收；命令：验证刷卡；设备号：{0}；卡号：{1}。",
                this.BoardNumber,
                this.CardNo);
        }
    }

    public class ReceivedValidatePhoneAndPasswordMessage : ReceivedMessageBase
    {
        public string Phone;
        public string Password;

        public override string ToString()
        {
            return string.Format("方向：接收；命令：验证手密；设备号：{0}；手机号：{1}；密码：{2}。",
                this.BoardNumber,
                this.Phone,
                this.Password);
        }
    }

    public class ReceivedAccountMessage : ReceivedMessageBase
    {
        public int BalanceId;
        public int Payment;

        public override string ToString()
        {
            return string.Format("方向：接收；命令：消费结算；设备号：{0}；记录序号：{1}；扣款金额：{2:#.00}。",
                this.BoardNumber,
                this.BalanceId,
                this.Payment/100.0);
        }
    }

    public class ReceivedSendMessage : ReceivedMessageBase
    {
        public override string ToString()
        {
            return string.Format("方向：接收；命令：直接发送；设备号：{0}。",
                this.BoardNumber);
        }
    }

    public class ReceivedUnknownMessage : ReceivedMessageBase
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in Meta)
            {
                sb.Append(string.Format("{0:x2} ", b));
            }

            return string.Format("方向：接收；命令：未知；数据：{0}。", sb.ToString().Trim().ToUpper());
        }
    }
}
