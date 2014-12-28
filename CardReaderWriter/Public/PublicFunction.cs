using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace RFID.Public
{
    class PublicFunction
    {
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 全局变量        Global Variable
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte addr = 0;

        public static byte len = 0;
        //public static a

        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 枚举变量        Enum Variable
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// 通信波特率 Baud
        public enum eBaud
        {
            BAUD_9600 = 0x00,
            BAUD_19200 = 0x02,
            BAUD_57600 = 0x04,
            BAUD_115200 = 0x06,
        };

        /// 输出功率 Power
        public enum ePower
        {
            POWER_10 = 10,
            POWER_11 = 11,
            POWER_12 = 12,
            POWER_13 = 13,
            POWER_14 = 14,
            POWER_15 = 15,
            POWER_16 = 16,
            POWER_17 = 17,
            POWER_18 = 18,
            POWER_19 = 19,
            POWER_20 = 20,
            POWER_21 = 21,
            POWER_22 = 22,
            POWER_23 = 23,
            POWER_24 = 24,
            POWER_25 = 25,
            POWER_26 = 26,
            POWER_27 = 27,
            POWER_28 = 28
        };

        /// CRC校验 CRC16
        enum eCRC16
        {
            CRC_FALSE = 0,
            CRC_TURE = 1
        };

        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// JRM 操作函数     The JRM Control Function
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// /////////////////////////////////////////////////////////////////
        /// 选择通信波特率    Select the Connect Baud
        /// 参数说明：
        ///     
        ///     Intput:
        ///             long lBaud, 波特率参数，取值范围为：9600/19200/57600/115200/750000
        ///                         作用：选择通信波特率
        /// 
        ///     Output: 无
        /// 
        ///     return: 1
        /// 
        /// ////////////////////////////////////////////////////////////////
        public static bool SelectConnectBaud(long lBaud)
        {
            byte bBaud;
            if (lBaud == 9600)
                bBaud = 0x00;
            else if (lBaud == 19200)
                bBaud = 0x02;
            else if (lBaud == 57600)
                bBaud = 0x04;
            else
                bBaud = 0x06;

            DemoPublic.flag = (byte)(DemoPublic.flag & 0x01);

            DemoPublic.flag = (byte)(DemoPublic.flag | bBaud);

            return true;
        }

        /// /////////////////////////////////////////////////////////////////
        /// 选择CRC校验    Select CRC16 Function
        /// 参数说明：
        ///     
        ///     Intput:
        ///             bool CRC_flag, CRC16校验标志，取值范围为：0,1,
        ///                            作用：选择是否使用CRC16校验算法对发送数据进行校验
        ///                            对应值：0－否，1－是
        /// 
        ///     Output: 无
        /// 
        ///     return: 1
        /// 
        /// ////////////////////////////////////////////////////////////////
        public static bool SelectCRC(bool CRC_flag)
        {
            if (CRC_flag)
                DemoPublic.flag = (byte)(DemoPublic.flag | 1);
            else
                DemoPublic.flag = (byte)(DemoPublic.flag | 0);

            return true;
        }

        /// /////////////////////////////////////////////////////////////////
        /// 连接命令  Connect Function
        /// 参数说明：
        ///     
        ///     Intput: 无
        ///                            
        ///     Output: 
        ///             ref IntPtr hCom, COM口设备参数，
        ///                              作用：用于后续通信的COM参数
        /// 
        ///     return: 0/1
        /// 
        /// /////////////////////////////////////////////////////////////////
        public static bool ConnectJRM()
        {
            return DemoPublic.UhfReaderConnect(ref RFID.Public.DemoPublic.hCom, RFID.Public.DemoPublic.sPort, DemoPublic.flag);
        }

        /// //////////////////////////////////////////////////////////////////
        /// 断开连接命令   DisConnect Function
        /// 参数说明：
        ///     
        ///     Intput:
        ///             ref IntPtr hCom, COM口设备参数,
        ///                              作用：用于释放当前所占用的COM口
        /// 
        ///     Output: 无
        /// 
        ///     return: 0/1
        /// 
        /// /////////////////////////////////////////////////////////////////
        public static bool DisConnectJRM(ref IntPtr hCom)
        {
            return DemoPublic.UhfReaderDisconnect(ref hCom, DemoPublic.flag);
        }

        /// /////////////////////////////////////////////////////////////////
        /// 询问连接状态   Get the JRM Status of connect
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: 0/1
        /// 
        /// /////////////////////////////////////////////////////////////////
        public static bool GetConnectStatus()
        {
            Byte[] bStatus = new byte[1];
            return DemoPublic.UhfGetPaStatus(DemoPublic.hCom, bStatus, DemoPublic.flag);
        }

        /// /////////////////////////////////////////////////////////////////
        /// 询问JRM工作状态    Get the JRM Status of working
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: 0
        /// 
        /// /////////////////////////////////////////////////////////////////
        public static bool GetWorkStatus()
        {
            return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取当前输出功率   Get the Power
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,读取成功; false,读取失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        //public static bool GetPower()
        //{
        //     string bAPower="";
        //   //  byte[] bAPower = new byte[1];
        //    if (DemoPublic.UhfGetPower(DemoPublic.hCom, bAPower, DemoPublic.flag))
        //    {
        //        DemoPublic.Power =int.Parse(bAPower);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public static bool GetPower()
        {
            byte[] bAPower = new byte[4];
            if (DemoPublic.UhfGetPower(DemoPublic.hCom, bAPower, DemoPublic.flag))
            {
                string pow = Encoding.ASCII.GetString(bAPower,0,2);
                DemoPublic.Power =int.Parse(pow);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// //////////////////////////////////////////////////////////////////
        /// 设置输出功率   Set the Power
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte bPower, 输出功率，取值范围：10~30，
        ///                          作用：设置/改变当前工作输出功率
        /// 
        ///     Output: 无
        /// 
        ///     return:true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetPower(byte bPower)
        {
            byte bAOption;

            bAOption = 0x01;

            if (DemoPublic.UhfSetPower(DemoPublic.hCom, bAOption, bPower, DemoPublic.flag))
                return true;
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取当前工作频率   Get the Frequency
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output:
        ///             byte FreMode, 频率工作模式，取值范围：0x00~0x04
        ///                           对应值：中国标准920-925/中国标准840-845/ETSI标准/定频模式/用户自定义模式
        ///             byte FreBase, 频率基数，取值范围：0、1
        ///                           对应值：0-50KHz,1-125KHz
        ///             int FreI,     起始频率整数部分，取值范围：840～960
        ///                           对应值：840～960
        ///             int FreD,     起始频率的小数部分，取值：0～950
        ///                           对应值：0～950，该值必须能被【频率基数】整除，即能被50或125整除
        ///             byte FreChannel, 频道数，取值范围：0～0xFF
        ///                              对应值：0～0xFF
        ///             byte FreSpc,  频道带宽，取值范围：0～15
        ///                           对应值：0～15
        ///             byte FreHop,  频率跳换方式，取值范围：0~2
        ///                           对应值：随机跳换/从高往低顺序跳换/从低往高顺序跳换
        /// 
        ///     return: true,读取成功; false,读取失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool GetFrequency()
        {
            byte[] bFreMode = new byte[1];
            byte[] bFreBase = new byte[1];
            byte[] bFre = new byte[2];
            byte[] bFreChannel = new byte[1];
            byte[] bFreSpc = new byte[1];
            byte[] bFreHop = new byte[1];
            int iFreBase = 0;
            int iFreN = 0;

            if (DemoPublic.UhfGetFrequency(DemoPublic.hCom, bFreMode, bFreBase, bFre, bFreChannel, bFreSpc, bFreHop, DemoPublic.flag))
            {
                DemoPublic.FreMode = (byte)(bFreMode[0] & 0x07);
                DemoPublic.FreBase = (byte)(bFreBase[0] & 0x01);
                DemoPublic.sFreI = (int)((bFre[0] << 3) + (bFre[1] >> 5));
                if (DemoPublic.FreBase == 0)
                    iFreBase = 50;
                else
                    iFreBase = 125;

                DemoPublic.sFreD = (int)((bFre[1] & 0x1F) * iFreBase);
                DemoPublic.FreChannel = bFreChannel[0];
                DemoPublic.FreSpc = bFreSpc[0];
                DemoPublic.FreHop = bFreHop[0];

                iFreN = (int)((DemoPublic.FreChannel - 1) * DemoPublic.FreSpc * iFreBase + DemoPublic.sFreD) / 1000;
                DemoPublic.eFreD = (int)(((DemoPublic.FreChannel - 1) * DemoPublic.FreSpc * iFreBase + DemoPublic.sFreD) - (iFreN * 1000));
                DemoPublic.eFreI = DemoPublic.sFreI + iFreN;

                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置工作频率   Set the Frequency
        /// 参数说明：
        ///     
        ///     Intput:        
        ///             byte FreMode, 频率工作模式，取值范围：0x00~0x04
        ///                           对应值：中国标准920-925/中国标准840-845/ETSI标准/定频模式/用户自定义模式
        ///             byte FreBase, 频率基数，取值范围：0、1
        ///                           对应值：0-50KHz,1-125KHz
        ///             int FreI,     起始频率整数部分，取值范围：840～960
        ///                           对应值：840～960
        ///             int FreD,     起始频率的小数部分，取值：0～950
        ///                           对应值：0～950，该值必须能被【频率基数】整除，即能被50或125整除
        ///             byte FreChannel, 频道数，取值范围：0～0xFF
        ///                              对应值：0～0xFF
        ///             byte FreSpc,  频道带宽，取值范围：0～15
        ///                           对应值：0～15
        ///             byte FreHop,  频率跳换方式，取值范围：0~2
        ///                           对应值：随机跳换/从高往低顺序跳换/从低往高顺序跳换
        /// 
        ///     Output: 无
        /// 
        ///     return: true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetFrequency(byte FreMode, byte FreBase, int FreI, int FreD, byte FreChannel, byte FreSpc, byte FreHop)
        {
            byte bFreD;
            byte[] bFre = new byte[2];

            if ((FreBase & 0x01) == 0x01)
                bFreD = (byte)(FreD / 125);
            else
                bFreD = (byte)(FreD / 50);

            bFre[0] = (byte)((FreI << 5)>>8);
            bFre[1] = (byte)((FreI << 5) | (bFreD & 0x1F));

            if (DemoPublic.UhfSetFrequency(DemoPublic.hCom, FreMode, FreBase, bFre, FreChannel, FreSpc, FreHop, DemoPublic.flag))
            {
                return true;
            }
            else
                return true;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置工作频率 中国标准 920－925MHz   Set the Frequency, Chinese Standard 920-925MHz
        /// 参数说明：
        ///     
        ///     Intput: 无 
        /// 
        ///     Output: 无
        /// 
        ///     return: true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetFrequencyChineseStandard920()
        {
            byte[] bFre = new byte[2];

            bFre[0] = 0;
            bFre[1] = 0;

            if (DemoPublic.UhfSetFrequency(DemoPublic.hCom, 0x04, 0, bFre, 0x01, 0, 0, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置工作频率 中国标准 840－845MHz   Set the Frequency, Chinese Standard 840-845MHz
        /// 参数说明：
        ///     
        ///     Intput: 无 
        /// 
        ///     Output: 无
        /// 
        ///     return: true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetFrequencyChineseStandard840()
        {
            byte[] bFre = new byte[2];

            bFre[0] = 0;
            bFre[1] = 0;

            if (DemoPublic.UhfSetFrequency(DemoPublic.hCom, 0x01, 0, bFre, 0x02, 0, 0, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置工作频率 ETSI标准 865－868MHz   Set the Frequency, ETSI Standard 865-868MHz
        /// 参数说明：
        ///     
        ///     Intput: 无 
        /// 
        ///     Output: 无
        /// 
        ///     return: true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetFrequencyETSI()
        {
            byte[] bFre = new byte[2];

            bFre[0] = 0;
            bFre[1] = 0;

            if (DemoPublic.UhfSetFrequency(DemoPublic.hCom, 0x02, 0, bFre, 0x03, 0, 0, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置工作频率 美国标准 902-928MHz   Set the Frequency, US Standard
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetFrequencyUS()
        {
            if (SetFrequency(0x04, 0x01, 902, 0, 0x69, 02, 0))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置工作频率 定频模式 915MHz   Set the Frequency, Fixed Frequency 915MHz
        /// 参数说明：
        ///     
        ///     Intput: 无 
        /// 
        ///     Output: 无
        /// 
        ///     return: true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetFixedFrequencyUS()
        {
            byte[] bFre = new byte[2];

            bFre[0] = 0;
            bFre[1] = 0;

            if (DemoPublic.UhfSetFrequency(DemoPublic.hCom, 0x02, 0, bFre, 0, 0, 0, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置工作频率 定频模式 任意定频   Set the Frequency, Fixed Frequency
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte FreBase, 频率基数，取值范围：0、1
        ///                           对应值：0-50KHz,1-125KHz
        ///             int FreI,     频率整数部分，取值范围：840～960
        ///                           对应值：840～960
        ///             int FreD      频率小数部分，取值范围：0～950
        ///                           对应值：0～950，该值必须能被【频率基数】整除，即能被50或125整除
        /// 
        ///     Output: 无
        /// 
        ///     return: true,设置成功; false,设置失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetFixedFrequency(byte FreBase, int FreI, int FreD)
        {
            if (SetFrequency(0x04, FreBase, FreI, FreD, 1, 2, 0))
            {
                return true;
            }
            else
                return false;
        }
        // 把字节型转换成十六进制字符串  
        public static string ByteToString(byte[] InBytes)  
        {  
            string StringOut = "";  
            foreach (byte InByte in InBytes)  
            {  
                StringOut = StringOut + String.Format("{0:X2} ",InByte);  
            }  
            return StringOut;  
        }   
        public string ByteToString(byte[] InBytes,int len)  
        {  
            string StringOut = "";  
            for (int i = 0; i < len;i++)  
            {  
                StringOut = StringOut + String.Format("{0:X2} ",InBytes[i]);  
            }  
            return StringOut;  
        }  
            // 把十六进制字符串转换成字节型  
        public static byte[] StringToByte(string InString)  
        {  
            string[] ByteStrings;  
            ByteStrings = InString.Split(" ".ToCharArray());  
            byte[] ByteOut;  
            ByteOut = new byte[ByteStrings.Length - 1];  
            for (int i = 0; i == ByteStrings.Length - 1; i++)  
            {  
                ByteOut[i] = Convert.ToByte(("0x" + ByteStrings[i]));  
            }  
            return ByteOut;  
        }   
        /// //////////////////////////////////////////////////////////////////
        /// 获取JRM版本信息   Get the Version
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,获取成功; false,获取失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool GetVersion()
        {
            byte[] bSerial = new byte[6];
            byte[] bVersion = new byte[3];
            if (DemoPublic.UhfGetVersion(DemoPublic.hCom, bSerial, bVersion, DemoPublic.flag))
            {
                DemoPublic.Serial = bSerial;
                DemoPublic.Version[0] = bVersion[0];
                DemoPublic.Version[1] = bVersion[1];
                DemoPublic.Version[2] = bVersion[2];
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取产品Uid   Read the pruduct uid
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: uid
        ///             
        ///     return: true, 获取成功：0-空闲;1-忙碌; false, 操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ReadUID()
        {
            if (DemoPublic.UhfGetReaderUID(DemoPublic.hCom, DemoPublic.Uid, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }
        public static bool UhfSaveConfig()
        {
            if (DemoPublic.UhfSaveConfig(DemoPublic.hCom, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }
        /// //////////////////////////////////////////////////////////////////
        /// 识别标签    Inventory Tags Function
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte flgAnti, 防碰撞标志，取值范围：0、1
        ///                           对应值：0-不启动防碰撞功能;1-启动防碰撞功能
        ///             byte Q.       防碰撞Q值，取值范围：0～15
        /// 
        ///     Output: 无
        /// 
        ///     return: true,启动识别标签功能成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool UhfSetMode(byte mode)
        {
            if (DemoPublic.UhfSetMode(DemoPublic.hCom, mode, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }
        public static bool UhfSelectQ(byte mode)
        {
            if (DemoPublic.UhfSelectQ(DemoPublic.hCom, mode, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }
        /// //////////////////////////////////////////////////////////////////
        /// 单步识别标签    Inventory Tags Single Function
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 
        ///             byte[] UiiLen, 标签UII长度，数组大小：1个byte
        ///             byte[] Uii,    标签UII数据，包含PC值
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static int InventorySingle(byte[] UiiLend, byte[] Uiid)
        {
            int aaa = DemoPublic.UhfInventorySingleTag(DemoPublic.hCom, UiiLend, Uiid, DemoPublic.flag);
            if(aaa!=0)
            {
               
                return aaa;
            }
            else
                return 0;
        }
        /// //////////////////////////////////////////////////////////////////
        /// 标签tid识别    Inventory Tags TID Read Function
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 
        ///             byte[] UiiLen, 标签UII长度，数组大小：1个byte
        ///             byte[] Uii,    标签UII数据，包含PC值
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static int UhfReadDataByTID(int sa,int dl, byte[] TID)
        {
            int aaa=DemoPublic.UhfReadDataByTID(DemoPublic.hCom,sa,dl, TID, DemoPublic.flag);
            if (aaa!=0)
            {
                return aaa;
            }
            else
                return 0;
        } 

        /// //////////////////////////////////////////////////////////////////
        /// 获取标签数据线程处理函数    Get data of uii Thread Function
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: 无
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static void GetUiiThread()
        {
            string Uii_str = "";
            int i;

            byte[] blen = new byte[1];
            byte[] buii = new byte[255];

            do
            {
                Uii_str = "";
             //   if (DemoPublic.UhfReadInventory(DemoPublic.hCom, blen, buii))
                if (InventorySingle(blen, buii)!=0)
                {

                    for (i = 1; i <= (int)blen[0]; i++)
                    {
                        DemoPublic.TagArray[i] = buii[i - 1];
                        Uii_str += (buii[i - 1] >> 4).ToString("X");
                        Uii_str += (buii[i - 1] & 0x0F).ToString("X");
                    }

                    //在主窗口中显示数据                  
                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                    //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "识别", "", Uii_str, "", "1", Convert.ToString(blen[0]), "", "" });
                }
                else { Console.WriteLine("请将标签置于天线场区"); }

            } while (DemoPublic.LoopEnable);

            //Stop();

            if (DemoPublic.EPCThread != null)
            {
                DemoPublic.EPCThread.Abort();
            }
        }
        public static int UhfReadInventory(byte[]ulen, byte[] eUii)
        {
            int aaa = DemoPublic.UhfReadInventory(DemoPublic.hCom,ulen, eUii);
            if (aaa != 0)
            {
                return aaa;
            }
            else
                return 0;
        }
        public static int UhfRecvData(byte[] eUii)
        {
            int aaa = DemoPublic.UhfRecvData(DemoPublic.hCom, eUii);
            if (aaa != 0)
            {
                return aaa;
            }
            else
                return 0;
        } 
        /// //////////////////////////////////////////////////////////////////
        /// 停止操作   Stop command
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static int Stop()
        {
            int aab = DemoPublic.UhfStopOperation(DemoPublic.hCom, DemoPublic.flag);
            if (aab!=0)
            {
                return aab;
            }
            else
                return 0;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取数据   Read data from tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     读取数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] uii,   所要进行读取数据操作的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] readdata, 读取到的数据,数组大小:255个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static int ReadData(string Pwd, int bank, int ptr, int cnt,  byte[] readdata, byte error)
        {
            int epcdd = DemoPublic.UhfReadDataByEPC(DemoPublic.hCom, Pwd, bank, ptr, cnt, readdata, DemoPublic.flag);
            if (epcdd!=0)
            {
               // DemoPublic.sData = CharToCString(readdata, (int)cnt * 2);

                return epcdd;
            }
            else
            {
                //if (error[0] != 0xFF)
                //    DemoPublic.sError = CharToCString(error, 1);
                return 0;
            }
        }

        public static int ReadxzData(string EPCXZ, string Pwd, int bank, int ptr, int cnt, byte[] readdata, byte error)
        {
            int dfd = DemoPublic.UhfReadDataByXZEPC(DemoPublic.hCom, EPCXZ, Pwd, bank, ptr, cnt, readdata, DemoPublic.flag);
            if (dfd != 0)
            {
                return dfd;
            }
            else
            {
                //if (error[0] != 0xFF)
                //    ByteArrayToHexString(error);
                return 0;
            }
        }
        /// //////////////////////////////////////////////////////////////////
        /// 写数据    Write data to tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     读取数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] uii,   所要进行读取数据操作的标签UII号,数组大小:255个字节
        ///             byte[] writedata, 写入标签的数据,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static int WritexzData(string EPCXZ, string Pwd, byte bank, string ptr, byte cnt, byte[] writedata, byte[] error)
        {
            byte code = new byte();
            //int dfd = DemoPublic.UhfWriteDataByXZEPC(DemoPublic.hCom, EPCXZ, Pwd, bank, ptr, cnt, writedata, DemoPublic.flag);
            int dfd = DemoPublic.UhfWriteDataByXZEPC(DemoPublic.hCom, EPCXZ, Pwd, bank, ptr, cnt, writedata, code, DemoPublic.flag);
            if (dfd != 0)
            {
                return dfd;
            }
            else
            {
                if (error[0] != 0xFF)
                    ByteArrayToHexString(error);
                return 0;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 多字节写数据   Mutil Write data to tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     读取数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] uii,   所要进行读取数据操作的标签UII号,数组大小:255个字节
        ///             byte[] writedata, 写入标签的数据,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        ///             byte[] status
        ///             byte[] uii
        ///             byte[] writelen
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool MutilWriteData(byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] writedata, byte[] error, byte[] status, byte[] writelen, byte[] ruuii)
        {
            if (DemoPublic.UhfBlockWriteDataByEPC(DemoPublic.hCom, pwd, bank, ptr, cnt, Uii, writedata, error, status, writedata, ruuii, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取数据(不指定UII)   Read data single from tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     读取数据长度,单位为:两个字节 = 字(Word)
        /// 
        ///     Output: 
        ///             byte[] uii,      读取到数据操的标签UII号,数组大小:255个字节
        ///             byte[] uiilen,   返回的Uii数据长度,数组大小: 1个字节
        ///             byte[] readdata, 读取到的数据,数组大小:255个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ReadDataSingle(byte[] Pwd, byte bank, byte[] ptr, byte cnt, byte[] uii, byte[] uiilen, byte[] readdata, byte[] error)
        {
            if (DemoPublic.UhfReadDataFromSingleTag(DemoPublic.hCom, Pwd, bank, ptr, cnt, readdata, uii, uiilen, error, DemoPublic.flag))
            {
                DemoPublic.sTag = CharToCString(uii, (int)uiilen[0]);
                DemoPublic.sData = CharToCString(readdata, (int)cnt * 2);

                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 写数据(不指定UII)    Write data single to tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     读取数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] writedata, 写入标签的数据,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] uii,      所写入数据的标签的UII号,数组大小:255个字节
        ///             byte[] uiilen,   返回的Uii数据长度,数组大小: 1个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; true,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool WriteDataSingle(byte[] Pwd, byte bank, string ptr, byte cnt, byte[] uii, byte[] uiilen, byte[] writedata, byte[] error)
        {
            if (DemoPublic.UhfWriteDataToSingleTag(DemoPublic.hCom, Pwd, bank, ptr, cnt, writedata, uii, uiilen, error, DemoPublic.flag))
            {

                DemoPublic.sTag = CharToCString(uii, (int)uiilen[0]);

                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }
        public static int UhfWriteDataByEPCEx(string Pwd, byte bank, string ptr, byte cnt, byte[] writedata, byte[] error)
        {
            int Writa=DemoPublic.UhfWriteDataByEPCEx(DemoPublic.hCom, Pwd, bank, ptr, cnt, writedata, error, DemoPublic.flag);
            if (Writa!=0)
            {

               // DemoPublic.sTag = CharToCString(uii, (int)uiilen[0]);

                return Writa;
            }
            else
            {
                if (error[0] != 0xFF)
                    // DemoPublic.sError = CharToCString(error, 1);
                    ByteArrayToHexString(error);
                return 0;
            }
        }
        public static int JRMWriteDataByEPC(string Pwd, byte bank, string ptr, byte cnt, byte[] writedata, byte error)
        {
            int Writa = DemoPublic.UhfWriteDataByEPC(DemoPublic.hCom,Pwd, bank, ptr, cnt, writedata,  DemoPublic.flag);
            if (Writa != 0)
            {

               

                return Writa;
            }
            else
            {
                //if (error[0] != 0xFF)
                //    // DemoPublic.sError = CharToCString(error, 1);
                //    ByteArrayToHexString(error);
                return 0;
            }
        }
        public static int JRMWriteDataByxzEPC(string epcxz, string Pwd, byte bank, string ptr, byte cnt, byte[] writedata, byte error)
        {
            byte code = new byte();
            //int Writa = DemoPublic.UhfWriteDataByXZEPC(DemoPublic.hCom,epcxz, Pwd, bank, ptr, cnt, writedata, DemoPublic.flag);
            int Writa = DemoPublic.UhfWriteDataByXZEPC(DemoPublic.hCom, epcxz, Pwd, bank, ptr, cnt, writedata,code,  DemoPublic.flag);
            if (Writa != 0)
            {

                // DemoPublic.sTag = CharToCString(uii, (int)uiilen[0]);

                return Writa;
            }
            else
            {
                //if (error[0] != 0xFF)
                //    // DemoPublic.sError = CharToCString(error, 1);
                //    ByteArrayToHexString(error);
                return 0;
            }
        }
        /// //////////////////////////////////////////////////////////////////
        /// 多字节写数据(不指定UII)    Mutil Write data single to tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     写入数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] writedata, 写入标签的数据,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] uii,      所写入数据的标签的UII号,数组大小:255个字节
        ///             byte[] uiilen,   返回的Uii数据长度,数组大小: 1个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        ///             byte[] status
        ///             byte[] writelen
        /// 
        ///     return: true,操作成功; true,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool MutilWriteDataSingle(byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] writedata, byte[] Uii, byte[] uiilen, byte[] status, byte[] error, byte[] writelen)
        {
            if (DemoPublic.UhfBlockWriteDataToSingleTag(DemoPublic.hCom, pwd, bank, ptr, cnt, writedata, Uii, uiilen, status, error, writelen, DemoPublic.flag))
            {
               return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取数据-Cnt为0(指定UII)   Read data for cnt is zero from tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte[] uii,   读取到数据操的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] datalen,  返回的所读取到的数据长度,数组大小: 2个字节
        ///             byte[] readdata, 读取到的数据,数组大小:512个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ReadDataNoCnt(byte[] Pwd, byte bank, byte[] ptr, byte[] uii, byte[] datalen, byte[] readdata, byte[] error)
        {
            if (DemoPublic.UhfReadMaxDataByEPC(DemoPublic.hCom, Pwd, bank, ptr, uii, datalen, readdata, error, DemoPublic.flag))
            {
                DemoPublic.sData = CharToCString(readdata, (int)((datalen[0] << 8) + datalen[1]));

                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取数据-Cnt为0(不指定UII)   Read data single for cnt is zero from tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        /// 
        ///     Output: 
        ///             byte[] uiilen,   返回的uii数据长度，数组大小为1个字节
        ///             byte[] uii,      读取到数据操的标签UII号,数组大小:255个字节
        ///             byte[] datalen,  返回的所读取到的数据长度,数组大小: 2个字节
        ///             byte[] readdata, 读取到的数据,数组大小:512个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ReadDataSingleNoCnt(byte[] Pwd, byte bank, byte[] ptr, byte[] datalen, byte[] readdata, byte[] uii, byte[] uiilen, byte[] error)
        {
            if (DemoPublic.UhfReadMaxDataFromSingleTag(DemoPublic.hCom, Pwd, bank, ptr, datalen, readdata, uii, uiilen, error, DemoPublic.flag))
            {
                DemoPublic.sTag = CharToCString(uii, (int)uiilen[0]);
                DemoPublic.sData = CharToCString(readdata, (int)((datalen[0] << 8) + datalen[1]));

                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }
        /// //////////////////////////////////////////////////////////////////
        /// 防碰撞读取数据-(不指定UII)   Read data single for Anti Q
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        /// 
        ///     Output: 
        ///             byte[] uiilen,   返回的uii数据长度，数组大小为1个字节
        ///             byte[] uii,      读取到数据操的标签UII号,数组大小:255个字节
        ///             byte[] datalen,  返回的所读取到的数据长度,数组大小: 2个字节
        ///             byte[] readdata, 读取到的数据,数组大小:512个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ReadDataAntiSingle(byte[] status, byte[] data1_len, byte[] data1, byte[] data2_len, byte[] data2, byte[] uii, byte[] uiilen)
        {
            if (DemoPublic.UhfGetDataFromMultiTag(DemoPublic.hCom, status, data1_len, data1, data2_len, data2, uii, uiilen))
            {
                DemoPublic.sTag = CharToCString(uii, (int)uiilen[0]);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取数据线程处理函数   Read data from tag Thread
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: 无
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static void ReadDataThreadFunction()
        {
            int i;                                
            byte[] bPwd = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            byte[] Ptr = new byte[2];
            byte bCnt, bOption;
            byte[] bTag = new byte[255];
            byte[] bBank = new byte[1];
            byte[] bData = new byte[512];
            byte[] bUiiLen = new byte[1];
            byte[] Error = new byte[1];
            byte[] Data_len = new byte[2];
            byte[] Play_load = new byte[6];
            byte[] Data1_len = new byte[2];
            byte[] Data2 = new byte[512];
            byte[] Data2_len = new byte[2];
            byte[] Status = new byte[1];
            
            int iPtr,oiPtr;
            string  sBank, sPtr, osBank, osPtr;

            PublicFunction.CStringToChar(DemoPublic.sTag, bTag);

            if (DemoPublic.AccessFlg)
            {
                for (i = 0; i < (DemoPublic.sPwd.Length / 2); i++)
                    bPwd[i] = (Convert.ToByte(DemoPublic.sPwd.Substring(i * 2, 2), 16));                
            }

            iPtr = Convert.ToUInt16(DemoPublic.sAddress);

            if (iPtr > 127)
            {
                Ptr[0] = (byte)((iPtr >> 7) | 0x80);
                Ptr[1] = (byte)(iPtr & 0x7F);
            }
            else
            {
                Ptr[0] = (byte)(iPtr);
            }

            bCnt = (byte)(Convert.ToByte(DemoPublic.sCnt));
            sBank = "0x";
            sPtr = "0x";
            osBank = "0x";
            osPtr = "0x";

            bBank[0] = DemoPublic.bBank;
            sBank += CharToCString(bBank, 1);
            switch (sBank)
            {
                case "0x00":
                    sBank = "保留区";
                    break;
                case "0x01":
                    sBank = "EPC区";
                    break;
                case "0x02":
                    sBank = "TID区";
                    break;
                case "0x03":
                    sBank = "用户区";
                    break;
                default:
                    break;
            }
            if (((byte)Ptr[0] & 0x80) == 0x80)
            {
                sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) >> 12), 16);
                sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) >> 8) & 0x000F, 16);
                sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) >> 4) & 0x000F, 16);
                sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) & 0x000F), 16);
            }
            else
            {
                sPtr += Convert.ToString((byte)Ptr[0] >> 4, 16);
                sPtr += Convert.ToString((byte)Ptr[0] & 0x0F, 16);
            }

            bOption = 0;
            int ptr_ebv = 0;
            if (DemoPublic.ReadQFlg == true)
            {

                if (DemoPublic.OptionFlg == true)
                {
                    bOption = 1;
                    //bank1
                    Play_load[0] = DemoPublic.OptionBank;
                    osBank += DemoPublic.HexSingleByeToString(DemoPublic.OptionBank);
                    //ptr
                    oiPtr = Convert.ToUInt16(DemoPublic.OptionPtr);

                    if (oiPtr > 127)
                    {
                        Play_load[1] = (byte)((oiPtr >> 7) | 0x80);
                        Play_load[2] = (byte)(oiPtr & 0x7F);
                        ptr_ebv = 1;
                    }
                    else
                    {
                        Play_load[1] = (byte)(oiPtr);
                    }

                    if (((byte)Play_load[1] & 0x80) == 0x80)
                    {
                        osPtr += Convert.ToString(((((byte)Play_load[1] & 0x7F) * 127 + (byte)Play_load[2]) >> 12), 16);
                        osPtr += Convert.ToString(((((byte)Play_load[1] & 0x7F) * 127 + (byte)Play_load[2]) >> 8) & 0x000F, 16);
                        osPtr += Convert.ToString(((((byte)Play_load[1] & 0x7F) * 127 + (byte)Play_load[2]) >> 4) & 0x000F, 16);
                        osPtr += Convert.ToString(((((byte)Play_load[1] & 0x7F) * 127 + (byte)Play_load[2]) & 0x000F), 16);
                    }
                    else
                    {
                        osPtr += Convert.ToString((byte)Play_load[1] >> 4, 16);
                        osPtr += Convert.ToString((byte)Play_load[1] & 0x0F, 16);
                    }

                    //cnt1

                    Play_load[2 + ptr_ebv] = (byte)(Convert.ToByte(DemoPublic.OptionCnt));
                    Play_load[3 + ptr_ebv] = DemoPublic.OptionQ;
                    Play_load[4 + ptr_ebv] = 0x20;
                }
                else
                {
                    Play_load[0] = DemoPublic.OptionQ;
                    Play_load[1] = 0x20;
                }

                //防碰撞识别 读取标签数据
                if (!DemoPublic.UhfStartReadDataFromMultiTag(DemoPublic.hCom, bPwd, DemoPublic.bBank, Ptr, bCnt, bOption, Play_load, DemoPublic.flag))
                {
                    if (DemoPublic.EPCThread != null)
                    {
                        DemoPublic.EPCThread.Abort();
                    }

                    return;
                }
            }
            do 
            {
                System.Threading.Thread.Sleep(5);

                if (DemoPublic.ReadQFlg)//防碰撞读数据
                {
                    if (ReadDataAntiSingle(Status, Data1_len, bData, Data2_len, Data2, bTag, bUiiLen))//返回ReadDataAntiSingle所返回的数据
                    {
                        DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);
                        int data0_len;

                        if (Status[0] == 0x00)    
                        {
                            data0_len = (Data1_len[0] << 8) + Data1_len[1];
                            DemoPublic.sData = CharToCString(bData, data0_len);

                            //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString(data0_len, 10), DemoPublic.sData, "" });
                        }
                        else if (Status[0] == 0x01)
                        {
                            data0_len = (Data1_len[0] << 8) + Data1_len[1];
                            DemoPublic.sData = CharToCString(bData, data0_len);

                            //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString(data0_len, 10), DemoPublic.sData, "" });

                            data0_len = (Data2_len[0] << 8) + Data2_len[1];
                            DemoPublic.sData = CharToCString(Data2, data0_len);

                            //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", osBank, osPtr, Convert.ToString(data0_len, 10), DemoPublic.sData, "" });
                        }                                                    
                    }
                }
                else
                {
                    if (DemoPublic.NoUiiFlag)
                    {
                        if (bCnt == 0x00)
                        {
                            if (ReadDataSingleNoCnt(bPwd, DemoPublic.bBank, Ptr, Data_len, bData, bTag, bUiiLen, Error))
                            {
                                DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                                //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString(((Data_len[0] << 8) + Data_len[1]), 10), DemoPublic.sData, "" });
                            }
                            else
                            {
                                if (Error[0] != 0xFF)
                                {
                                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                                    //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", "", "1", sBank, sPtr, "0", "", DemoPublic.sError });
                                }
                            }
                        }
                        else
                        {
                            if (ReadDataSingle(bPwd, DemoPublic.bBank, Ptr, bCnt, bTag, bUiiLen, bData, Error))
                            {
                                DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                                //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), DemoPublic.sData, "" });
                            }
                            else
                            {
                                if (Error[0] != 0xFF)
                                {
                                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                                   // DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", "", "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), "", DemoPublic.sError });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (bCnt == 0)
                        {
                            if (ReadDataNoCnt(bPwd, DemoPublic.bBank, Ptr, bTag, Data_len, bData, Error))
                            {
                                DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                                //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString(((Data_len[0] << 8) + Data_len[1]), 10), DemoPublic.sData, "" });
                            }
                            else
                            {
                                if (Error[0] != 0xFF)
                                {
                                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                                    //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, "0", "", DemoPublic.sError });
                                }
                            }
                        }
                        else
                        {
                            //if (ReadData(bPwd, DemoPublic.bBank, Ptr, bCnt, bTag, bData, Error))
                            //{
                            //    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                            //    DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), DemoPublic.sData, "" });
                            //}
                            //else
                            //{
                            //    if (Error[0] != 0xFF)
                            //    {
                            //        DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                            //        DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "读取", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), "", DemoPublic.sError });
                            //    }
                            //}
                        }
                    }
                }
            }while(DemoPublic.LoopRead);

            Stop();
            
            if (DemoPublic.EPCThread != null)
            {
                DemoPublic.EPCThread.Abort();
            }
        }

         /// //////////////////////////////////////////////////////////////////
        /// 写入数据线程处理函数   Write data to tag Thread
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: 无
        /// 
        /// //////////////////////////////////////////////////////////////////
        //public static void WriteDataThreadFunction()
        //{
        //    int i;           
        //    byte[] bPwd = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
        //    byte[] Ptr = new byte[2];
        //    byte bCnt;
        //    byte[] bTag = new byte[64];
        //    byte[] bBank = new byte[1];
        //    byte[] bData = new byte[255];
        //    byte[] bUiiLen = new byte[1];
        //    byte[] Error = new byte[1];
        //    byte[] Data_len = new byte[2];
        //    byte[] bStatus = new byte[1];
        //    byte[] bWriteLen = new byte[1];
        //    byte[] bRuUii = new byte[64];
        //    int iPtr;
        //    string sBank, sPtr;

        //    PublicFunction.CStringToChar(DemoPublic.sTag, bTag);

        //    PublicFunction.CStringToChar(DemoPublic.sData, bData);

        //    if (DemoPublic.AccessFlg)
        //    {
        //        for (i = 0; i < (DemoPublic.sPwd.Length / 2); i++)
        //            bPwd[i] = (Convert.ToByte(DemoPublic.sPwd.Substring(i * 2, 2), 16));
        //    }

        //    iPtr = Convert.ToUInt16(DemoPublic.sAddress);

        //    if (iPtr > 127)
        //    {
        //        Ptr[0] = (byte)((iPtr >> 7) | 0x80);
        //        Ptr[1] = (byte)(iPtr);
        //    }
        //    else
        //    {
        //        Ptr[0] = (byte)(iPtr);
        //    }

        //    bCnt = (byte)(Convert.ToByte(DemoPublic.sCnt));
        //    sBank = "0x";
        //    sPtr = "0x";

        //    bBank[0] = DemoPublic.bBank;
        //    sBank += CharToCString(bBank, 1);
        //    switch (sBank)
        //    {
        //        case "0x00":
        //            sBank = "保留区";
        //            break;
        //        case "0x01":
        //            sBank = "EPC区";
        //            break;
        //        case "0x02":
        //            sBank = "TID区";
        //            break;
        //        case "0x03":
        //            sBank = "用户区";
        //            break;
        //        default:
        //            break;
        //    }
        //    if (((byte)Ptr[0] & 0x80) == 0x80)
        //    {
        //        sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) >> 12), 16);
        //        sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) >> 8) & 0x000F, 16);
        //        sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) >> 4) & 0x000F, 16);
        //        sPtr += Convert.ToString(((((byte)Ptr[0] & 0x7F) * 127 + (byte)Ptr[1]) & 0x000F), 16);
        //    }
        //    else
        //    {
        //        sPtr += Convert.ToString((byte)Ptr[0] >> 4, 16);
        //        sPtr += Convert.ToString((byte)Ptr[0] & 0x0F, 16);
        //    }

        //    do
        //    {
        //        if (DemoPublic.NoUiiFlag)
        //        {
        //            if (DemoPublic.MutilWriteFlg)
        //            {
        //                if (MutilWriteDataSingle(bPwd, DemoPublic.bBank, Ptr, bCnt, bData, bTag, bUiiLen, bStatus, Error, bWriteLen))
        //                {
        //                    DemoPublic.sTag = CharToCString(bTag, bUiiLen[0]);
        //                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);
                            
        //                    DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), DemoPublic.sData, "" });
        //                }
        //                else
        //                {
        //                    if (Error[0] != 0xFF)
        //                    {
        //                        DemoPublic.sTag = CharToCString(bTag, bUiiLen[0]);

        //                        DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

        //                        DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), "", DemoPublic.sError });
        //                    }
        //                }
        //            }
        //            else
        //            {
        //               // string Ptr = "";
        //                if (WriteDataSingle(bPwd, DemoPublic.bBank, Ptr, bCnt, bTag, bUiiLen, bData, Error))
        //                {
        //                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

        //                    DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), DemoPublic.sData, "" });
        //                }
        //                else
        //                {
        //                    if (Error[0] != 0xFF)
        //                    {
        //                        DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

        //                        DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", "", "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), "", DemoPublic.sError });
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (DemoPublic.MutilWriteFlg)
        //            {
        //                if (MutilWriteData(bPwd, DemoPublic.bBank, Ptr, bCnt, bTag, bData, Error, bStatus, bWriteLen, bRuUii))
        //                {
        //                    DemoPublic.sTag = CharToCString(bRuUii, ((bRuUii[0] >> 3) + 1) * 2);
        //                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

        //                    DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), DemoPublic.sData, "" });
        //                }
        //                else
        //                {
        //                    if (Error[0] != 0xFF)
        //                    {
        //                        DemoPublic.sTag = CharToCString(bRuUii, ((bRuUii[0] >> 3) + 1) * 2);

        //                        DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

        //                        //DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString(bWriteLen[0], 10), "", DemoPublic.sError });
        //                        DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), "", DemoPublic.sError });
        //                    }
        //               }
        //            }
        //            else
        //            {
        //                int adf =WriteData(bPwd, DemoPublic.bBank, Ptr,bCnt,  bData, Error);
        //                if (adf!=0)
        //                {
        //                    DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

        //                    DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), DemoPublic.sData, "" });
        //                }
        //                else
        //                {
        //                    if (Error[0] != 0xFF)
        //                    {
        //                        DemoPublic.ActingThread AcT = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

        //                        DemoPublic.PublicDM.BeginInvoke(AcT, new object[] { 0, "写入", DemoPublic.sTag, "1", sBank, sPtr, Convert.ToString((bCnt * 2), 10), "", DemoPublic.sError });
        //                    }
        //                }
        //            }
        //        }
                
        //        System.Threading.Thread.Sleep(5);

        //    } while (DemoPublic.LoopWrite);

        //    if (DemoPublic.EPCThread != null)
        //    {
        //        DemoPublic.EPCThread.Abort();
        //    }
        //}

        /// //////////////////////////////////////////////////////////////////
        /// 擦除数据    Erase data
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             byte[] ptr,   地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     擦除数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] uii,   所要进行擦除数据操作的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool EraseData(byte[] Pwd, byte bank, byte[] ptr, byte cnt, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfEraseDataByEPC(DemoPublic.hCom, Pwd, bank, ptr, cnt, uii, error, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }


        /// //////////////////////////////////////////////////////////////////
        /// 擦除数据(不指定UII)    Erase data Single
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             IntPtr[] Pwd, Access密码,默认为全0,数组大小为:4
        ///             byte bank,    存储区,取值范围:0x00/0x01/0x02/0x03
        ///                           对应值:0x00-Reserved,0x01-UII,0x02-TID,0x03-USER
        ///             IntPtr[] ptr, 地址指针,即为地址偏移量,该参数为EBV格式,数组大小为:2
        ///             byte cnt,     擦除数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] uii,   所要进行擦除数据操作的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool EraseDataSingle(byte[] Pwd, byte bank, byte[] ptr, byte cnt, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfEraseDataFromSingleTag(DemoPublic.hCom, Pwd, bank, ptr, cnt, uii, error, DemoPublic.flag))
            {
                int uiilen = ((uii[0] >> 3) + 1) * 2;
                DemoPublic.sTag = CharToCString(uii, uiilen);
                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        } 

        /// //////////////////////////////////////////////////////////////////
        /// 生成锁定码      Lock Code
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte bkill,    Kill密码锁定选项
        ///             byte baccess,  Accesss密码锁定选项
        ///             byte buii,     UII存储区锁定选项
        ///             byte btid,     Tid存储区锁定选项
        ///             byte buser,    User存储区锁定选项
        /// 
        ///     Output: 无       
        /// 
        ///     return: true
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool LockGenCode(byte bkill, byte baccess, byte buii, byte btid, byte buser)
        {
            DemoPublic.LockCode[0] = 0x00;
            DemoPublic.LockCode[1] = 0x00;
            DemoPublic.LockCode[2] = 0x00;

            if (bkill == 1)
            {
                DemoPublic.LockCode[0] |= 0x08;
                DemoPublic.LockCode[1] |= 0x02;
            }
            else if (bkill == 2)
            {
                DemoPublic.LockCode[0] |= 0x04;
                DemoPublic.LockCode[1] |= 0x01;
            }
            else
            {
                DemoPublic.LockCode[0] |= 0x08;                
            }

            if (baccess == 1)
            {
                DemoPublic.LockCode[0] |= 0x02;
                DemoPublic.LockCode[2] |= 0x80;
            }
            else if (baccess == 2)
            {
                DemoPublic.LockCode[0] |= 0x01;
                DemoPublic.LockCode[2] |= 0x40;
            }
            else
            {
                DemoPublic.LockCode[0] |= 0x02;
            }

            if (buii == 1)
            {
                DemoPublic.LockCode[1] |= 0x80;
                DemoPublic.LockCode[2] |= 0x20;
            }
            else if (buii == 2)
            {
                DemoPublic.LockCode[1] |= 0x40;
                DemoPublic.LockCode[2] |= 0x10;
            }
            else
            {
                DemoPublic.LockCode[1] |= 0x80;
            }

            if (btid == 1)
            {
                DemoPublic.LockCode[1] |= 0x20;
                DemoPublic.LockCode[2] |= 0x08;
            }
            else if (btid == 2)
            {
                DemoPublic.LockCode[1] |= 0x10;
                DemoPublic.LockCode[2] |= 0x04;
            }
            else
            {
                DemoPublic.LockCode[1] |= 0x20;
            }

            if (buser == 1)
            {
                DemoPublic.LockCode[1] |= 0x08;
                DemoPublic.LockCode[2] |= 0x02;
            }
            else if (buser == 2)
            {
                DemoPublic.LockCode[1] |= 0x04;
                DemoPublic.LockCode[2] |= 0x01;
            }
            else
            {
                DemoPublic.LockCode[1] |= 0x08;
            }

            return true;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 锁定操作      Lock operate
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             IntPtr[] Pwd,    Access密码,默认为全0,数组大小为:4
        ///             byte[] lockdata, 锁定密码,数组大小为: 6个字节
        ///             byte[] uii,      所要进行锁定操作的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static int LockMem(string epch, string Pwd, string lockdata, byte[] error)
        {
            int suod = DemoPublic.UhfLockMemByEPC(DemoPublic.hCom, epch, Pwd, lockdata, error, DemoPublic.flag);
            if (suod!=0)
            {
                return suod;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return 0;
            }
        }
        public static int unLockMem(string epch, string Pwd, string lockdata, byte[] error)
        {
           int unsuo = DemoPublic.UhfunLockMemByEPC(DemoPublic.hCom, epch, Pwd, lockdata, error, DemoPublic.flag);
            if (unsuo!=0)
            {
                return unsuo;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return 0;
            }
        }
        /// //////////////////////////////////////////////////////////////////
        /// 锁定操作(不指定UII)      Lock operate single
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             IntPtr[] Pwd,    Access密码,默认为全0,数组大小为:4
        ///             byte[] lockdata, 锁定密码,数组大小为: 6个字节
        ///             byte[] uii,      所要进行锁定操作的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool LockMemSingle(byte[] Pwd, byte[] lockdata, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfLockMemFromSingleTag(DemoPublic.hCom, Pwd, lockdata, uii, error, DemoPublic.flag))
            {
                int uiilen = ((uii[0] >> 3) + 1) * 2;
                DemoPublic.sTag = CharToCString(uii, uiilen);
                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 销毁标签      Kill operate
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             IntPtr[] Pwd,    Access密码,默认为全0,数组大小为:4
        ///             byte[] uii,      所要进行销毁操作的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool KillTag(byte[] Pwd, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfKillTagByEPC(DemoPublic.hCom, Pwd, uii, error, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }
              

        /// //////////////////////////////////////////////////////////////////
        /// 销毁标签(不指定UII)      Kill operate single
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             IntPtr[] Pwd,    Access密码,默认为全0,数组大小为:4
        ///             byte[] uii,      所要进行销毁操作的标签UII号,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool KillTagSingle(byte[] Pwd, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfKillSingleTag(DemoPublic.hCom, Pwd, uii, error, DemoPublic.flag))
            {
                int uiilen = ((uii[0] >> 3) + 1) * 2;
                DemoPublic.sTag = CharToCString(uii, uiilen);
                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取寄存器     Read register
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             int address, 所读寄存地址
        ///             int len,     所读数据长度,单位为:字节
        /// 
        ///     Output: 
        ///             byte[] status, 操作状态
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ReadReg(int address, int len)
        {
            byte[] bStatus = new byte[1];
            byte[] reg = new byte[512];

            if (DemoPublic.UhfGetRegister(DemoPublic.hCom, address, len, bStatus, reg, DemoPublic.flag))
            {
                if (len == 0)
                    len = 512 - address;

                for (int i = 0; i < len; i++)
                    DemoPublic.Register[address + i] = reg[i];

                return true;
            }
            else
            {
                if (bStatus[0] != 0xFF)
                    DemoPublic.sError = CharToCString(bStatus, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 写寄存器     Write register
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             int address, 所读寄存地址
        ///             int len,     所读数据长度,单位为:字节
        ///             byte[] reg,  数据存储数组
        /// 
        ///     Output: 
        ///             byte[] status, 操作状态
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool WriteReg(int address, int len, byte[] reg)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfSetRegister(DemoPublic.hCom, address, len, reg, bStatus, DemoPublic.flag))
            {
                for (int i = 0; i < len; i++)
                    DemoPublic.Register[address + i] = reg[i];

                return true;
            }
            else
            {
                if (bStatus[0] != 0xFF)
                    DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 恢复寄存器默认值     Reset register
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ResetReg()
        {
            if (DemoPublic.UhfResetRegister(DemoPublic.hCom, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 保存当前寄存器设置     Save register
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SaveReg()
        {
            if (DemoPublic.UhfSaveRegister(DemoPublic.hCom, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 添加Select记录     Add select record
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             SRECORD[] pSRecord, select参数结构体,数组大小为: 1
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static int AddSelect(byte[] bStatus)
        {
            bStatus = new byte[255];
            int sele=DemoPublic.UhfAddFilter(DemoPublic.hCom, 000, 000, 0x01, 0x00000020, 60, 00, bStatus,DemoPublic.flag);
            if (sele!=0)
            {
               

                return sele ;
            }
            else
            {
                if (bStatus[0] != 0xFF)
                    DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return 0;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 删除Select记录     Delete select record
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte sindex, select记录序号,取值范围: 1～15
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool DeleteSelect(byte sindex)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfDeleteFilterByIndex(DemoPublic.hCom, sindex, bStatus, DemoPublic.flag))
            {
                DemoPublic.SRerCord[(int)sindex].Slen = 0x00;
                DemoPublic.SRerCord[(int)sindex].Target = 0x00;
                DemoPublic.SRerCord[(int)sindex].Action = 0x00;
                DemoPublic.SRerCord[(int)sindex].bank = 0x00 ;
                DemoPublic.SRerCord[(int)sindex].Ptr = null;
                DemoPublic.SRerCord[(int)sindex].Len = 0x00;
                DemoPublic.SRerCord[(int)sindex].Mask = null;
                DemoPublic.SRerCord[(int)sindex].Truncate = 0x00;
                return true;
            }
            else
            {
                if (bStatus[0] != 0xFF)
                    DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 读取Select记录     Get select record
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte sindex, select记录序号,取值范围: 1～15
        ///             byte snum,   读取的select记录数量
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool GetSelect(byte sindex, byte snum)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfStartGetFilterByIndex(DemoPublic.hCom, sindex, snum, bStatus, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                if (bStatus[0] != 0xFF)
                    DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 接收Select记录     Get select record received
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             DemoPublic.SRECORD[] pSRcord, Select命令记录结构体
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool GetSelectReceived( ref DemoPublic.SRECORD pSRcord)
        {
            byte[] bStatus = new byte[1];
            bStatus[0] = 0xFF;

            if (DemoPublic.UhfReadFilterByIndex(DemoPublic.hCom, bStatus, ref pSRcord))
            {
                DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);

                DemoPublic.SRerCord[pSRcord.Sindex].Slen = pSRcord.Slen;
                DemoPublic.SRerCord[pSRcord.Sindex].Target = pSRcord.Target;
                DemoPublic.SRerCord[pSRcord.Sindex].Action = pSRcord.Action;
                DemoPublic.SRerCord[pSRcord.Sindex].bank = pSRcord.bank;
                DemoPublic.SRerCord[pSRcord.Sindex].Ptr = pSRcord.Ptr;
                DemoPublic.SRerCord[pSRcord.Sindex].Len = pSRcord.Len;
                DemoPublic.SRerCord[pSRcord.Sindex].Mask = pSRcord.Mask;
                DemoPublic.SRerCord[pSRcord.Sindex].Truncate = pSRcord.Truncate;

                return true;
            }
            else
            {
                if (bStatus[0] != 0xFF)
                    DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 接收Select记录线程处理函数     Get select record received Thread Function
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: 无
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static void GetSelectReceivedThread()
        {
            DemoPublic.sError = "00";
            DemoPublic.SRECORD Srd;     // = new DemoPublic.SRECORD[1];
            Srd.Target = 0x00;
            Srd.Action = 0x00;
            Srd.bank = 0x00;
            Srd.Len = 0x00;
            Srd.Slen = 0x00;
            Srd.Sindex = 0x00;
            Srd.Truncate = 0x00;
            Srd.Ptr = new byte[2];
            Srd.Mask = new byte[32];
            int iPtr, iLen, Clen;

            if (GetSelect((byte)DemoPublic.GetIndex, (byte)DemoPublic.GetNum))
            {    
                while (DemoPublic.sError == "00" || DemoPublic.sError == "03")
                {
                    if (GetSelectReceived(ref Srd))
                    {
                        if((DemoPublic.SRerCord[Srd.Sindex].Ptr[0] & 0x80) == 0x80)
                        {
                            iPtr = ((DemoPublic.SRerCord[Srd.Sindex].Ptr[0] & 0x7F) << 7) + DemoPublic.SRerCord[Srd.Sindex].Ptr[1];
                        }
                        else
                            iPtr = (int)DemoPublic.SRerCord[Srd.Sindex].Ptr[0];

                        iLen = (int)DemoPublic.SRerCord[Srd.Sindex].Len;

                        if(iLen == 0)
                            Clen =0;
                        else
                        {
                            Clen = iLen /8;

                            if((iLen % 8) > 0)
                                Clen = Clen + 1;
                        }

                        DemoPublic.ActingThread Act = new DemoPublic.ActingThread(DemoPublic.ShowLvTID);

                        //DemoPublic.PublicSL.BeginInvoke(Act, new object[] { 1, DemoPublic.SRerCord[Srd.Sindex].Sindex.ToString(), DemoPublic.HexSingleByeToString(DemoPublic.SRerCord[Srd.Sindex].Target),
                        //                                                    DemoPublic.HexSingleByeToString(DemoPublic.SRerCord[Srd.Sindex].Action), DemoPublic.HexSingleByeToString(DemoPublic.SRerCord[Srd.Sindex].bank),
                        //                                                    iPtr.ToString(), iLen.ToString(), CharToCString(DemoPublic.SRerCord[Srd.Sindex].Mask, Clen), DemoPublic.HexSingleByeToString(DemoPublic.SRerCord[Srd.Sindex].Truncate)});

                    }                    
                }
                
                if (DemoPublic.EPCThread != null)
                {
                    DemoPublic.EPCThread.Abort();
                }
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 选择Select     select select record
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte sindex, select记录序号,取值范围: 1～15
        ///             byte snum,   选择的select记录数量
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool ChooseSelect(byte sindex, byte snum)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfSelectFilterByIndex(DemoPublic.hCom, sindex, snum, bStatus, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                if (bStatus[0] != 0xFF)
                    DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 蜂鸣器控制     Beep Control
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             bool OpenClose, true-打开蜂鸣器，false-关闭蜂鸣器
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetBeep(bool bOpenClose)
        {
            byte[] bStatus = new byte[1];

            if (bOpenClose)
                bStatus[0] = 1;
            else
                bStatus[0] = 0;

            if (PublicFunction.WriteReg(288, 1, bStatus))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 设置Timer     Timer Control
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Time, 
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool SetTimer(byte[] Time)
        {
            if (PublicFunction.WriteReg(289, 2, Time))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 进入sleep模式     Sleep mode
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool EnterSleep()
        {
            if (DemoPublic.UhfEnterSleepMode(DemoPublic.hCom, DemoPublic.flag))
            {
                return true;
            }
            else
                return false;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 通知下位机开始升级操作   Start update operate function
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 
        ///             byte[] RN, RN值，获取下位机返回的RN32数据
        /// 
        ///     return: ture,操作成功; false,操作失败
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static bool StarUpdate(byte[] RN)
        {
            byte[] bStatus = new byte[1];
            byte[] bRN32 = new byte[4];

            if (DemoPublic.UhfUpdateInit(ref DemoPublic.hCom, DemoPublic.sPort, bStatus, bRN32, DemoPublic.flag))
            {
                for (int i = 0; i < 4; i++)
                    RN[i] = bRN32[i];

                return true;
            }
            else
            {
                DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 发送RN32值   Send RN32 function
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] RN, RN32值
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功, false,操作失败
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static bool SendRN(byte[] RN)
        {
            byte[] bStatus = new byte[1];
            byte[] bRN32 = new byte[4];
            bRN32[0] = (byte)~RN[0];
            bRN32[1] = (byte)~RN[1];
            bRN32[2] = (byte)~RN[2];
            bRN32[3] = (byte)~RN[3];

            if (DemoPublic.UhfUpdateSendRN32(DemoPublic.hCom, bRN32, bStatus, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 准备开始传送文件   Start send file function
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] FILESIZE, 文件大小
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; false,操作失败
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static bool StarTrans(byte[] FILESIZE)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfUpdateSendSize(RFID.Public.DemoPublic.hCom, bStatus, FILESIZE, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 发送数据包   Send Data Package function
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte packnum, 数据包序号，取值范围：0～0xFF
        ///                           对应值：0～0xFF
        ///             byte lastpack, 最后一个数据包标志，取值范围：0、1
        ///                            对应值：0-该数据包不是最后一个数据包；1-该数据包为最后一个数据包
        ///             int data_len, 数据包大小，取值范围：1～1024
        ///                           对应值：1～1024
        ///             byte[] data,  数据包数组
        /// 
        ///     Output: 无
        /// 
        ///     return: ture,操作成功; false,操作失败
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static bool TranPackage(byte packnum, byte lastpack, int data_len, byte[] data)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfUpdateSendData(RFID.Public.DemoPublic.hCom, bStatus, packnum, lastpack, data_len, data, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 升级完成   End the update operate function
        /// 参数说明：
        ///     
        ///     Intput: 无
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; 0xFF,操作失败
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static bool EndUpdate()
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfUpdateCommit(DemoPublic.hCom, bStatus, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                DemoPublic.sError = DemoPublic.HexSingleByeToString(bStatus[0]);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 传送整个数据文件   Send file function
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             string filepath, 文件路径字符串
        /// 
        ///     Output: 无
        /// 
        ///     return: true,操作成功; 操作,操作失败
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static void SendFile(string filepath)
        {
            DemoPublic.Progress_Report = 0;
            int i = 0;
            //byte bStatus;
            long file_byte_size = 0;
            long package_num, lastpack_len;
            byte[] package = new byte[1024];

            if (!File.Exists(filepath))
            {
                //MessageBox.Show("文件不存在");
                return;
            }
            FileInfo File_info = new FileInfo(filepath);

            file_byte_size = File_info.Length;//获取文件大小  字节
            package_num = file_byte_size / 1024;//获取完整1024byte数据包个数
            lastpack_len = file_byte_size % 1024;//获取最后一个数据包长度  byte

            DemoPublic.Progress_Size = file_byte_size + 1;

            FileStream filestream = new FileStream(filepath, FileMode.Open);
            BinaryReader BReader = new BinaryReader(filestream);

            for (i = 0; i < package_num; i++)//发送完整数据包
            {
                for (int j = 0; j < 1024; j++)
                {
                    package[j] = BReader.ReadByte();
                    DemoPublic.Progress_Report++;
                }

                if (!TranPackage((byte)i, 0x00, 1024, package))
                    return;              
            }

            for (int j = 0; j < lastpack_len; j++)
            {                
                package[j] = BReader.ReadByte();

                DemoPublic.Progress_Report++;                
            }

            if (!TranPackage((byte)i, 0x01, (int)lastpack_len, package))
                return;
            DemoPublic.Progress_Report++;  
        }

        /// //////////////////////////////////////////////////////////////////
        /// 字节数组转换成字符串   convert byte[] to string
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] byteinput：待转换的字节数组
        /// 
        ///             int len：字节数组的长度
        /// 
        ///     Output: 
        ///             string OutputStr: 转换后得到的字符串
        /// 
        ///     return: OutputStr: 转换后得到的字符串
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static string CharToCString(byte[] byteinput, int len)
        {
            try
            {
                StringBuilder OutputStr = new StringBuilder(256);

                DemoPublic.UhfCharToCString(byteinput, OutputStr, len);

                return OutputStr.ToString(0, len * 2);
            }
            catch { return "false"; }
        }
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
        /// //////////////////////////////////////////////////////////////////
        /// 字符串转换成字节数组   convert string to byte[]
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             string strinput：待转换的字符串
        /// 
        ///             byte[] byteoutput：转换后得到的字节数组
        /// 
        ///     Output: 
        ///             无
        /// 
        ///     return: 无
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static void CStringToChar(string strinput, byte[] byteoutput)
        {
            StringBuilder InputStr = new StringBuilder(strinput);

            try { DemoPublic.UhfCStringToChar(InputStr, byteoutput, InputStr.Length); }
            catch { }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 获取串口的数量   get COM count
        /// 参数说明：
        ///     
        ///     Intput: 无
        ///             
        /// 
        ///     Output: 无
        ///             
        /// 
        ///     return: 无
        ///
        /// ///////////////////////////////////////////////////////////////////
        public static int GetComNum()
        {
            int num = 0;
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");

            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();

                num = sSubKeys.Length;
            }

            return num;
        }

        /// //////////////////////////////////////////////////////////////////
        /// 写EPC(不指定UII)    Write EPC to single tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte cnt,     写入EPC数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] writedata, 写入标签的数据,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] uii,      所写入数据的标签的UII号,数组大小:255个字节
        ///             byte[] uiilen,   返回的Uii数据长度,数组大小: 1个字节
        ///             byte[] error,    错误代码,数组大小:1个字节
        ///             byte[] status
        ///             byte[] writelen
        /// 
        ///     return: true,操作成功; true,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
        public static bool WriteEPCSingle(byte[] Pwd, byte cnt, byte[] uii, byte[] uiilen, byte[] status, byte[] writedata, byte[] error, byte[] writelen)
        {
            if(DemoPublic.UhfBlockWriteEPCToSingleTag(DemoPublic.hCom, Pwd, cnt, writedata, uii, uiilen, status, error, writelen, DemoPublic.flag))
            //if (DemoPublic.UhfBlockWriteEPCToSingleTag(DemoPublic.hCom, Pwd, cnt, uii, uiilen, status, writedata, error, writelen, DemoPublic.flag))
            {
                return true;
            }
            else
            {
                if (error[0] != 0xFF)
                    DemoPublic.sError = CharToCString(error, 1);
                return false;
            }
        }

        /// //////////////////////////////////////////////////////////////////
        /// 写EPC(指定UII)    Write EPC to tag
        /// 参数说明：
        ///     
        ///     Intput: 
        ///             byte[] Pwd,   Access密码,默认为全0,数组大小为:4
        ///             byte cnt,     写入EPC数据长度,单位为:两个字节 = 字(Word)
        ///             byte[] uii,   所要进行读取数据操作的标签UII号,数组大小:255个字节
        ///             byte[] writedata, 写入标签的数据,数组大小:255个字节
        /// 
        ///     Output: 
        ///             byte[] error,    错误代码,数组大小:1个字节
        ///             byte[] status
        ///             byte[] ruuii
        ///             byte[] writelen
        /// 
        ///     return: true,操作成功; false,操作失败
        /// 
        /// //////////////////////////////////////////////////////////////////
     
    }
}
