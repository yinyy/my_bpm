using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BoardListenerLib
{
    public class ReceivedMessageBase : MessageBase
    {
        public byte[] Meta;

        public ReplyMessageBase CreateReplyMessage()
        {
            ReplyMessageBase mo = null;
            switch (Command)
            {
                case CommandType.HeartBeat:
                    mo = new ReplyHeartBeatMessage();
                    break;
                case CommandType.ValidateCardAndPassword:
                case CommandType.ValidateCard:
                case CommandType.ValidatePhoneAndPassword:
                    mo = new ReplyValidateMessage();
                    break;
                case CommandType.TimeSync:
                    mo = new ReplyTimeSyncMessage();
                    break;
                case CommandType.ReaderSetting:
                    mo = new ReplyReadSettingMessage();
                    break;
                case CommandType.UploadStatus:
                    mo = new ReplyUploadStatusMessage();
                    break;
                case CommandType.Account:
                    mo = new ReplyAccountMessage();
                    break;
                case CommandType.Operation:
                    mo = new ReplyOperationMessage();
                    break;
                default:
                    break;
            }

            if (mo != null)
            {
                mo.Command = Command;
                mo.BoardNumber = BoardNumber;
                mo.Ticks = DateTime.Now.Ticks;
            }

            return mo;
        }

        public static ReceivedMessageBase Parse(byte[] bs)
        {
            ReceivedMessageBase mo = null;

            if (bs.Length == 1)
            {
                mo = new ReceivedHeartBeatMessage(bs.Length);
                mo.Meta = bs;
                mo.Command = 0;

                return mo;
            }

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
                case 2001:
                    mo = new ReceivedOperationMessage() { Command = CommandType.Operation };
                    break;
                default:
                    mo = null;
                    break;
            }

            if (mo == null)
            {
                return null;
            }

            mo.Meta = bs;
            mo.Ticks = (bs[0] << 24) + (bs[1] << 16) + (bs[2] << 8) + bs[3];
            mo.Length = bs[6];
            mo.BoardNumber = string.Format("{0:000000000}", (bs[7] << 24) + (bs[8] << 16) + (bs[9] << 8) + bs[10]);

            switch (mo.Command)
            {
                case CommandType.ValidateCardAndPassword:
                    ReceivedValidateCardAndPasswordMessage rvcpm = mo as ReceivedValidateCardAndPasswordMessage;

                    rvcpm.CardNo = string.Format("{0:000000000}", (bs[11] << 24) + (bs[12] << 16) + (bs[13] << 8) + bs[14]);
                    rvcpm.Password = string.Format("{0:000000}", (bs[15] << 24) + (bs[16] << 16) + (bs[17] << 8) + bs[18]);
                    break;
                case CommandType.ValidateCard:
                    ReceivedValidateCardMessage rvcm = mo as ReceivedValidateCardMessage;
                    rvcm.CardNo = string.Format("{0:000000000}", (bs[11] << 24) + (bs[12] << 16) + (bs[13] << 8) + bs[14]);
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

                    rma.CardId = (bs[11] << 24) + (bs[12] << 16) + (bs[13] << 8) + bs[14];
                    rma.Payment = (bs[15] << 24) + (bs[16] << 16) + (bs[17] << 8) + bs[18];
                    break;
                case CommandType.Operation:
                    ReceivedOperationMessage rom = mo as ReceivedOperationMessage;
                    if (bs[11] == 1)
                    {
                        rom.Operation = OperationType.Start;
                    }
                    else if (bs[11] == 2)
                    {
                        rom.Operation = OperationType.Stop;
                    }
                    break;
                default:
                    break;
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
    }

    public class ReceivedTimeSyncMessage : ReceivedMessageBase
    {
    }

    public class ReceivedReadSettingMessage : ReceivedMessageBase
    {
    }

    public class ReceivedUploadStatusMessage : ReceivedMessageBase
    {

        public int BitStatus;
        public int[] ValueStatus = new int[10];
    }

    public class ReceivedValidateCardAndPasswordMessage : ReceivedMessageBase
    {
        public string CardNo;
        public string Password;
    }

    public class ReceivedValidateCardMessage : ReceivedMessageBase
    {
        public string CardNo;
    }

    public class ReceivedValidatePhoneAndPasswordMessage : ReceivedMessageBase
    {
        public string Phone;
        public string Password;
    }

    public class ReceivedAccountMessage : ReceivedMessageBase
    {
        public int CardId;
        public int Payment;
    }

    public class ReceivedOperationMessage : ReceivedMessageBase
    {
        public OperationType Operation;
    }
}
