using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;


namespace RFID.Public
{        
    class DemoPublic
    {
        public struct SRECORD
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Sindex;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Slen;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Target;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Action;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte bank;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Ptr;

            
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Len;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Mask;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Truncate;
        }
        
        public static bool Enabel_flg;              //使能标志，确定COM口与JRM连接成功或断开
        public static string JRM_version = "";      //JRM的版本
        public static string sPort, Star_Path;      //COM口字符串信息; 程序与运行根目录
        public const string API_Path = "\\UhfReader_API.dll";  //API.DLL文件放到执行文件相同目录下
        public static IntPtr hCom;                  //COM设备参数
        /**
         * Debug
         */
        public static int comNum;                   //串口数量
        public static int Power;                    //输出功率
        public static long Baud;                    //通信波特率
        public static byte flag;               //CRC、通信波特率标志; 
        public static long Progress_Report;         //升级进度报告
        public static long Progress_Size;           //升级进度条最大值
        public static byte FreMode, FreBase, FreChannel, FreSpc, FreHop;
                                                    /*频率工作模式
                                                      频率基数
                                                      频道数
                                                      频带带宽积数
                                                      频率跳换方式*/
        public static int sFreI, sFreD, eFreI, eFreD;
                                                    /*起始频率整数部分
                                                      起始频率小数部分
                                                      最终频率整数部分
                                                      最终频率小数部分
                                                     */
        public static byte[] Serial = new byte[6];  //硬件版本号信息
        public static byte[] Version = new byte[3]; //固件版本号
        public static byte[] Uid    = new byte[12]; //产品UID

        //public static Form1 PublicDM;           //主窗体句柄，可选择保留使用
        //public static Form1 PublicSL;
        //public static FmTabPowerFre PublicPowerFre;
        //public static ListViewItem LvItem;          //用于识别操作的ListView控件项

        public static bool LoopEnable;           //标志是否进行循环操作

        public static int TagNum;                 //当次操作识别到的标签数量
        public static int BufTagNum;              //从标签缓存区中读到的标签数量
        public static byte[] TagArray = new byte[255];//标签信息
        public static bool OldInventoryFlg = false;   //记录上次识别标签是否是单步识别，据此判断是否清空数据窗口
        
        public static Thread EPCThread;//识别标签线程

        public static bool NoUiiFlag, LoopRead, LoopWrite, AccessFlg;

        public static string sTag, sPwd, sAddress, sCnt, sData, sError;

        public static byte bBank;

        public const byte Kill_UnLock = 0;
        public const byte Kill_AccessLock = 1;
        public const byte Kill_PerLock = 2;

        public const byte Access_Unlock = 0;
        public const byte Access_AccessLock = 1;
        public const byte Access_PerLock = 2;

        public const byte Uii_UnLock = 0;
        public const byte Uii_AccessLock = 1;
        public const byte UII_PerLock = 2;

        public const byte Tid_Unlock = 0;
        public const byte Tid_AccessLock = 1;
        public const byte Tid_PerLock = 2;

        public const byte User_Unlock = 0;
        public const byte User_AccessLock = 1;
        public const byte User_PerLock = 2;

        public static byte[] LockCode = new byte[3];

        public static byte[] Register = new byte[512];

        public static SRECORD[] SRerCord = new SRECORD[16];

        public static int GetIndex, GetNum;

        public static byte OptionBank, OptionQ;
        public static string OptionCnt, OptionPtr;
        public static bool OptionFlg = false;
        public static bool ReadQFlg = false;//防碰撞读数据标志
        public static bool MutilWriteFlg = false;

        public static byte[] ByteOutput = new byte[512];

        public static bool selected = true;//连接后可能弹出“频率高级设置”窗口，不太友好，暂时没有好方法，有待改善
       

        //代理
        public delegate void ActingThread(int itype, string p0, string p1, string p2, string p3, string p4, string p5, string p6, string p7);
        

        //重定义API函数

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///     玖锐技术RFID超高频模块系列 API 函数
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="hCom"></param>
        /// <param name="cPort"></param>
        /// <param name="flagCrc"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //open and connect  
        public static extern bool UhfReaderConnect(ref IntPtr hCom, string cPort, byte flagCrc);   

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //close and disconnect
        public static extern bool UhfReaderDisconnect(ref IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get status
        public static extern bool UhfGetPaStatus(IntPtr hCom, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get power
        public static extern bool UhfGetPower(IntPtr hCom, byte[] power, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //set power
        public static extern bool UhfSetPower(IntPtr hCom, byte option, byte power, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get fre
        public static extern bool UhfGetFrequency(IntPtr hCom, byte[] fremode, byte[] frebase, byte[] basefre, byte[] channnum, byte[] channspc, byte[] frehop, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //set fre
        public static extern bool UhfSetFrequency(IntPtr hCom, byte fremode, byte frebase, byte[] basefre, byte channnum, byte channspc, byte frehop, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get version
        public static extern bool UhfGetVersion(IntPtr hCom, byte[] serial, byte[] version, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get uid
        public static extern bool UhfGetReaderUID(IntPtr hCom, byte[] uid, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get Tid
        public static extern int UhfReadDataByTID(IntPtr hCom, int sa, int dl, byte[] uDataReturn, byte flagCrc);/*uDataReturn是返回的数据*/
       
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //inventory
        public static extern bool UhfStartInventory(IntPtr hCom, byte flagAnti, byte initQ, byte flagCrc);

        //[System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               // Get Received
        //public static extern bool UhfReadInventory(IntPtr hCom, byte[] ulen, byte[] Uii);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //stop
        public static extern int UhfStopOperation(IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //inventory single
        public static extern int UhfInventorySingleTag(IntPtr hCom, byte[] ulen, byte[] Uii, byte flagCrc);
        
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data
        public static extern int UhfReadDataByEPC(IntPtr hCom, string pwd, int bank, int ptr, int cnt,  byte[] readdata,  byte flagCrc);
        
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]   //手工选择一个EPC号码，然后再读此标签的各个存储区数据                            //read data
        public static extern int UhfReadDataByXZEPC(IntPtr hCom,string epcxz, string pwd, int bank, int ptr, int cnt, byte[] readdata, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //write data
        public static extern int UhfWriteDataByEPC(IntPtr hCom, string pwd, byte bank, string ptr, byte cnt, byte[] writedata, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single
        public static extern bool UhfReadDataFromSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] readdata, byte[] Uii, byte[] uiiLen, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //write data single
        public static extern bool UhfWriteDataToSingleTag(IntPtr hCom, byte[] pwd, byte bank, string ptr, byte cnt, byte[] writedata, byte[] Uii, byte[] uiiLen, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //erase data
        public static extern bool UhfEraseDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //lock
        public static extern int UhfLockMemByEPC(IntPtr hCom, string epch, string pwd, string lockdata, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //unlock
        public static extern int UhfunLockMemByEPC(IntPtr hCom, string epch, string pwd, string lockdata, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //kill
        public static extern bool UhfKillTagByEPC(IntPtr hCom, byte[] pwd, byte[] Uii, byte[] error, byte flagCrc);


        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 玖锐技术RFID超高频系列 API 函数重定义完成
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 玖锐技术RFID超高频系列 API 函数重定义
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //mutil write data
        public static extern bool UhfBlockWriteDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] writedata, byte[] error, byte[] status, byte[] writelen, byte[] ruuii, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data for Cnt is zero
        public static extern bool UhfReadMaxDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte[] Uii, byte[] datalen, byte[] readdata, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single for Cnt is zero
        public static extern bool UhfReadMaxDataFromSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte[] datalen, byte[] readdata, byte[] Uii, byte[] uiiLen, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single for Anti Q
        public static extern bool UhfStartReadDataFromMultiTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte option, byte[] playload, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single for Anti Q
        public static extern bool UhfGetDataFromMultiTag(IntPtr hCom, byte[] status, byte[] ufData_len, byte[] ufReadData, byte[] usData_len, byte[] usReadData, byte[] uii, byte[] uiilen);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //erase data single
        public static extern bool UhfEraseDataFromSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //lock single
        public static extern bool UhfLockMemFromSingleTag(IntPtr hCom, byte[] pwd, byte[] lockdata, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //kill single
        public static extern bool UhfKillSingleTag(IntPtr hCom, byte[] pwd, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //mutil write data single
        public static extern bool UhfBlockWriteDataToSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] writedata, byte[] Uii, byte[] uiilen, byte[] status, byte[] error, byte[] writelen, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read register
        public static extern bool UhfGetRegister(IntPtr hCom, int radd, int rlen, byte[] status, byte[] reg, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //write register
        public static extern bool UhfSetRegister(IntPtr hCom, int radd, int rlen, byte[] reg, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //reset register
        public static extern bool UhfResetRegister(IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //save register
        public static extern bool UhfSaveRegister(IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //add select
        public static extern int UhfAddFilter(IntPtr hCom,  int intSelTarget, int intAction, int intSelMemBank,int intSelPointer, int intMaskLen, int intTruncate,byte[] txtSelMask, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //delete select
        public static extern bool UhfDeleteFilterByIndex(IntPtr hCom, byte sindex, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get select
        public static extern bool UhfStartGetFilterByIndex(IntPtr hCom, byte sindex, byte snum, byte[] status, byte flagCrc);
        
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get select received
        public static extern bool UhfReadFilterByIndex(IntPtr hCom, byte[] status, ref SRECORD pSRecord);
        
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //choose select
        public static extern bool UhfSelectFilterByIndex(IntPtr hCom, byte sindex, byte snum, byte[] status, byte flagCrc);
        
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //sleep mode
        public static extern bool UhfEnterSleepMode(IntPtr hCom, byte flagCrc);
        
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //start update
        public static extern bool UhfUpdateInit(ref IntPtr hCom, string cPort, byte[] status, byte[] RN32, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //send inverse
        public static extern bool UhfUpdateSendRN32(IntPtr hCom, byte[] RN32, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //start trans
        public static extern bool UhfUpdateSendSize(IntPtr hCom, byte[] status, byte[] filesize, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //tran package
        public static extern bool UhfUpdateSendData(IntPtr hCom, byte[] status, byte packnum, byte lastpack, int data_len, byte[] trandata, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //end update
        public static extern bool UhfUpdateCommit(IntPtr hCom, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern void UhfCharToCString(byte[] byteinput, StringBuilder stroutput, int len);    //convert byte[] to string 

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern void UhfCStringToChar(StringBuilder strinput, byte[] byteoutput, int len);    //convert string to byte[]

        /**
         * Debug
         */
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfBlockWriteEPCToSingleTag(IntPtr hCom, byte[] uAccessPwd, byte uCnt, byte[] uWriteData, byte[] uUii, byte[] uLenUii, byte[] uStatus, byte[] uErrorCode, byte[] uWritedLen, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfBlockWriteEPCByEPC(IntPtr hCom, byte[] uAccessPwd, byte uCnt, byte[] uUii, byte[] uWriteData, byte[] uErrorCode, byte[] uStatus, byte[] uWritedLen, byte[] RuUii, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfSetMode(IntPtr hCom, byte mode, byte flagCrc);//设置读卡模式/*0高速识别，1防碰撞群读识别*/

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfSaveConfig(IntPtr hCom, byte flagCrc);//保存设置的配置/

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfSleep(IntPtr hCom, byte flagCrc);//设备休眠/

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfAutoFrequeC(IntPtr hCom,int mode,  byte flagCrc);//自动调频，mode为0取消自动调频，是1设置为自动调频/

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfSelectQ(IntPtr hCom, int mode, byte flagCrc); //选择Q值 mode从1-8整形

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //随机写EPC号码
        public static extern int UhfWriteDataByEPCEx(IntPtr hCom, string pwd, byte uBank, string ptr, byte uCnt, byte[] uWriteData, byte[] error, byte flagCrc);

         [System.Runtime.InteropServices.DllImportAttribute(API_Path)]     
        public static extern int UhfReadInventory(IntPtr hCom, byte[]Uiilen, byte[] uUii);

         [System.Runtime.InteropServices.DllImportAttribute(API_Path)]     //UhfRecvData用于用户直接循环读取数据
         public static extern int UhfRecvData(IntPtr hCom,byte[] uUii);//uLenUii/*返回的数据长度*//*返回的数据缓冲*/
       //  public static extern int UhfRecvData(IntPtr hCom, byte[] Uiilen,byte[] uUii);//uLenUii/*返回的数据长度*//*返回的数据缓冲*/
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 玖锐技术RFID超高频系列 API 函数重定义结束
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        //下面这些函数可能与之前的定义不一致
        //这个函数与UhfWriteDataByEPC函数的区别是写数据前，内部会自动做选择标签操作，同时固定写入USER区
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
         public static extern int UhfWriteDataByUSER(IntPtr hCom,string pwd, string ptr, byte cnt, byte[] data, byte error, byte flag);

        //这个函数与UhfWriteDataByEPC函数的区别是写数据前，内部会自动做选择用户指定标签操作
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]       
        public static extern int UhfWriteDataByXZEPC(IntPtr hCom, string epcxz, string pwd, byte bank, string ptr, byte cnt, byte[] writedata, byte errorcode, byte flagCrc);


 



        public static bool CheckVersion()
        {
            if (DemoPublic.JRM_version == null)
            {
                //MessageBox.Show("请选择JRM的版本");
                return true;
            }
            else
                return false;
        }

        public static bool CheckCOM()
        {
            if (DemoPublic.sPort == null)
            {
                //MessageBox.Show("请选择COM口");
                return true;
            }
            else
                return false;
        }

        public static bool CheckBaud()
        {
            if (DemoPublic.Baud == 0)
            {
                //MessageBox.Show("请先选择通信波特率");
                return true;
            }
            else
                return false;
        }

        public static bool CheckDigit(string sString)//判断字符串是否为十六进制数
        {
            if (sString == "")
            {
                //MessageBox.Show("传入字符串参数为空");
                return false;
            }

            int i;

            bool Res = false;

            char[] c = sString.ToUpper().ToCharArray();
            for (i = 0; i < c.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch((c[i]).ToString(), "[0-9]") || System.Text.RegularExpressions.Regex.IsMatch((c[i]).ToString(), "[A-F]"))
                    Res = true;
                else
                {
                    Res = false;
                    break;
                }
            }
            return Res;
        }

        public static bool CheckDecimal(string sString)//判断字符串是否为十进制数
        {
            if (sString == "")
            {
                //MessageBox.Show("传入字符串参数为空");
                return false;
            }

            int i;

            bool Res = false;

            char[] c = sString.ToUpper().ToCharArray();
            for (i = 0; i < c.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch((c[i]).ToString(), "[0-9]"))
                    Res = true;
                else
                {
                    Res = false;
                    break;
                }
            }
            return Res;
        }

        public static string StringAddPlace(string InputStr)
        {
            if (InputStr.Length == 0)
                return "";

            string P_str = "";

            for (int i = 0; i < InputStr.Length/2; i++)
            {
                P_str += InputStr.Substring(2 * i, 2) + " ";
            }

            return P_str;
        }
       
        public static void StringToHexByte(string InputStr, byte[] OutPutByte)
        {
            if (InputStr.Length == 0)
                return;

            for (int strLen = 0; strLen < InputStr.Length / 2; strLen++)
                OutPutByte[strLen] = Convert.ToByte(InputStr.Substring(strLen*2,2), 16);
        }

        public static byte StringToSingleHexByte(string InputStr)
        {
            if (InputStr.Length != 2)
                return 0x00;
            
            return Convert.ToByte(InputStr, 16);
        }
        /*
        public static string HexByteToString(byte[] InPutByte, int ConvertLen)
        {
            string OutPutStr = "";
            try
            {
                for (int i = 0; i < ConvertLen; i++)
                {
                    OutPutStr += Convert.ToString((InPutByte[i] >> 4), 16);
                    OutPutStr += Convert.ToString((InPutByte[i] & 0x0F), 16);
                }
                return OutPutStr;
            }
            catch (Exception)
            {
                return "";
            }
        }
        */
        
        public static string HexSingleByeToString(byte InputByte)
        {
            string OutPutStr = "";
            
            try
            {
                OutPutStr += Convert.ToString((InputByte >> 4), 16);
                OutPutStr += Convert.ToString((InputByte & 0x0F), 16);
                return OutPutStr;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool CheckConnect()
        {
            if (DemoPublic.Enabel_flg)
                return false;
            else
            {
                //MessageBox.Show("设备未连接，请先进行【连接】操作");
                return true;
            }
        }

        //// 主窗体信息显示处理函数，可选择保留使用
        //public static void ShowInfo(Form1 objFrom, string info)   // 显示操作信息
        //{
        //        objFrom.SetInfoText = info + "\r\n" + objFrom.GetInfoText;
        //}

        public static void ShowTagNum(bool tagtype)
        {
            if (tagtype)
            {
                TagNum++;
                //DemoPublic.PublicDM.SetTagNumText = TagNum.ToString();
            }
            else
            {
                BufTagNum++;
                //DemoPublic.PublicDM.SetTagNumText = BufTagNum.ToString();
            }
            
        }
        public static void ShowLvTID(int operate_type, string p0, string p1, string p2, string p3, string p4, string p5, string p6,string p7)
        {
            try
            {
               
                if (p2 != "")
                {
                    p1 = p2.Substring(0, 4);
                    p2 = p2.Substring(4, p2.Length-4);
                   
                }
                int j;
                bool flag = true;
                bool newtag = true;

   
                //在主窗口中显示数据
                //DemoPublic.PublicDM.LvControl.BeginUpdate();

                //if (operate_type == 0)//显示数据窗口信息
                //{
                //    for (j = 0; j < DemoPublic.PublicDM.LvControl.Items.Count; j++)
                //    {
                //        if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[2].Text.ToString() == p2)
                //        {
                //            if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[6].Text.ToString() == p6 || p6 == "")
                //                newtag = false;
                //        }

                //        if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[0].Text.ToString() == p0 && DemoPublic.PublicDM.LvControl.Items[j].SubItems[1].Text.ToString() == p1
                          
                //            && DemoPublic.PublicDM.LvControl.Items[j].SubItems[6].Text.ToString() == p5)
                //        {
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[4].Text = Convert.ToString((Convert.ToUInt16(DemoPublic.PublicDM.LvControl.Items[j].SubItems[4].Text) + 1));


                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[3].Text = "";
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[8].Text = p1 + p2;
                //            flag = false;
                //            newtag = false;

                //            break;
                //        }
                //    }

                //    if (flag)
                //    {

                //        DemoPublic.LvItem = new ListViewItem(new string[] { p0, p1, p2, p3, p4, "TID", p5, p7, p1 + p2 });

                //        DemoPublic.PublicDM.LvControl.Items.Add(DemoPublic.LvItem);
                //    }

                //    if (newtag)
                //        DemoPublic.ShowTagNum(true);
                //}
                //else//(operate_type == 1) //显示Select操作信息
                //{
                //    // p7 = p1 + p2;
                //    DemoPublic.LvItem = new ListViewItem(new string[] { p0, p1, p2, p3, p4, p5, p6, p1 + p2 });

                //    DemoPublic.PublicSL.LvSelect.Items.Add(DemoPublic.LvItem);
                //}

                //DemoPublic.PublicDM.LvControl.EndUpdate();
            }
            catch { }
        }
        public static void ShowLvks(int operate_type, string p0, string p1, string p2, string p3, string p4, string p5, string p6, string p7)
        {
            //增加一个显示完全列
            try
            {
                string addCol = p2;
                string prssi = "";
                int RSSIp3;

                if (p2 != "")
                {
                    p1 = addCol.Substring(2, 4);
                    p3 = addCol.Substring(0, 2);
                    RSSIp3 = Convert.ToInt16(p3, 16);

                    if (RSSIp3 > 127)
                    {
                        RSSIp3 = -((-RSSIp3) & 0xFF);
                    }
                    prssi = RSSIp3.ToString() + "dBm";
                    if (DemoPublic.TagNum == 0)
                        p3 = prssi;

                }
                else return;
                int j;
                bool flag = true;
                bool newtag = true;

                //根据起始地址和读取字数对EPC号显示作处理
                int addr = Convert.ToInt32(PublicFunction.addr);
                int len = Convert.ToInt32(PublicFunction.len);

                if (string.IsNullOrEmpty(p2))
                {
                    ///////
                }
                else
                {
                    try { p2 = addCol.Substring(6, len * 4); }
                    catch { p2 = addCol.Substring(6, addCol.Length - 6); }
                }

                //在主窗口中显示数据
                //DemoPublic.PublicDM.LvControl.BeginUpdate();

                //if (operate_type == 0)//显示数据窗口信息
                //{
                //    for (j = 0; j < DemoPublic.PublicDM.LvControl.Items.Count; j++)
                //    {
                //        if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[2].Text.ToString() == p2)
                //        {
                //            if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[6].Text.ToString() == p6 || p6 == "")
                //                newtag = false;
                //        }
                //        if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[0].Text.ToString() == p0 && DemoPublic.PublicDM.LvControl.Items[j].SubItems[2].Text.ToString() == p2)
                //        {
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[4].Text = Convert.ToString((Convert.ToUInt16(DemoPublic.PublicDM.LvControl.Items[j].SubItems[4].Text) + 1));
                //            //  DemoPublic.PublicDM.LvControl.Items[j].SubItems[2].Text = Convert.ToString(addCol.Substring(addr*4, len*4));
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[3].Text = Convert.ToString(prssi);
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[8].Text = addCol.Substring(2, addCol.Length - 2); ;
                //            flag = false;
                //            newtag = false;

                //            break;
                //        }
                //    }

                //    if (flag)
                //    {

                //        DemoPublic.LvItem = new ListViewItem(new string[] { p0, p1, p2, prssi, p4, p5, p6, p7, addCol.Substring(2, addCol.Length - 2) });

                //        DemoPublic.PublicDM.LvControl.Items.Add(DemoPublic.LvItem);
                //    }

                //    if (newtag)
                //        DemoPublic.ShowTagNum(true);
                //}
                //else//(operate_type == 1) //显示Select操作信息
                //{
                //    // p7 = p1 + p2;
                //    DemoPublic.LvItem = new ListViewItem(new string[] { p0, p1, p2, p3, p4, p5, p6, addCol.Substring(2, addCol.Length - 2) });

                //    DemoPublic.PublicSL.LvSelect.Items.Add(DemoPublic.LvItem);
                //}

                //DemoPublic.PublicDM.LvControl.EndUpdate();
            }
            catch { }
        }
        /// //////////////////////////////////////////////////////////////////
        /// 数据窗口信息显示    Show data on ListView contorl
        
        public static void ShowLvbydata(int operate_type, string p0, string p1, string p2, string p3, string p4, string p5, string p6, string p7)
        {
            //增加一个显示完全列
            try
            {
                string addCol = p5;
                string prssi = "";
               // int RSSIp3;

                if (p5 != "")
                {
                   // p1 = addCol.Substring(0, 4);
                    //p3 = addCol.Substring(addCol.Length - 2, 2);
                    //RSSIp3 = Convert.ToInt16(p3, 16);

                    //if (RSSIp3 > 127)
                    //{
                    //    RSSIp3 = -((-RSSIp3) & 0xFF);
                    //}
                    //prssi = RSSIp3.ToString() + "dBm";
                    //if (DemoPublic.TagNum == 0)
                    //    p3 = prssi;

                }
                else return;
                int j;
                bool flag = true;
                bool newtag = true;

                //根据起始地址和读取字数对EPC号显示作处理
                int addr = Convert.ToInt32(PublicFunction.addr);
                int len = Convert.ToInt32(PublicFunction.len);

                if (string.IsNullOrEmpty(p5))
                {
                    ///////
                }
                else
                {
                   // try { p5 = addCol.Substring(addr, len * 4); }
                  //  catch { p5 = addCol.Substring(4, addCol.Length - 6); }
                }

                //在主窗口中显示数据
                //DemoPublic.PublicDM.LvControl.BeginUpdate();

                //if (operate_type == 0)//显示数据窗口信息
                //{
                //    for (j = 0; j < DemoPublic.PublicDM.LvControl.Items.Count; j++)
                //    {
                //        if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[2].Text.ToString() == p2)
                //        {
                //            if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[6].Text.ToString() == p6 || p6 == "")
                //                newtag = false;
                //        }
                //        if (DemoPublic.PublicDM.LvControl.Items[j].SubItems[0].Text.ToString() == p0 && DemoPublic.PublicDM.LvControl.Items[j].SubItems[2].Text.ToString() == p2)
                //        {
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[4].Text = Convert.ToString((Convert.ToUInt16(DemoPublic.PublicDM.LvControl.Items[j].SubItems[4].Text) + 1));
                //            //  DemoPublic.PublicDM.LvControl.Items[j].SubItems[2].Text = Convert.ToString(addCol.Substring(addr*4, len*4));
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[3].Text = Convert.ToString(prssi);
                //            DemoPublic.PublicDM.LvControl.Items[j].SubItems[8].Text = addCol.Substring(0, addCol.Length-2); ;
                //            flag = false;
                //            newtag = false;

                //            break;
                //        }
                //    }

                //    if (flag)
                //    {

                //        DemoPublic.LvItem = new ListViewItem(new string[] { p0, p1, p2, prssi, p6,p4, p5, p7, addCol.Substring(0, addCol.Length-2) });

                //        DemoPublic.PublicDM.LvControl.Items.Add(DemoPublic.LvItem);
                //    }

                //    if (newtag)
                //        DemoPublic.ShowTagNum(true);
                //}
                //else//(operate_type == 1) //显示Select操作信息
                //{
                //    // p7 = p1 + p2;
                //    DemoPublic.LvItem = new ListViewItem(new string[] { p0, p1, p2, p3, p4, p5, p6, addCol.Substring(0, addCol.Length - 2) });

                //    DemoPublic.PublicSL.LvSelect.Items.Add(DemoPublic.LvItem);
                //}

                //DemoPublic.PublicDM.LvControl.EndUpdate();
            }
            catch { }
        }
        public static string CRC16(string InputStr)
        {
            if ((InputStr.Length % 2) != 0 || InputStr.Length == 0)
            {
                //MessageBox.Show("输入字符串长度必须为2的倍数");
                return "";
            }

            if (!CheckDigit(InputStr))
            {
                //MessageBox.Show("输入字符串必须为十六进制数据");
                return "";
            }            

            int i, j;
            UInt16 XorVal;
            byte crcHigh, crcLow;
            UInt16 CrcResult = 0xFFFF;
            int ByteNum = InputStr.Length / 2;
            byte CrcByte;
            string OutPutStr;

            for (i = 0; i < ByteNum; i++)
            {
                CrcByte = StringToSingleHexByte(InputStr.Substring(i*2,2));

                for (j = 0; j < 8; j++)
                {
                    XorVal = (UInt16)(((CrcResult >> 8) ^ (CrcByte << j)) & 0x0080);
                    CrcResult = (UInt16)((CrcResult << 1) & 0xfffe);
                    if (XorVal > 0)
                        CrcResult ^= 0x1021;
                }
            }

            CrcResult = (UInt16)(~CrcResult);

            crcHigh = (byte)(CrcResult >> 8);
            CrcResult <<= 8;
            crcLow = (byte)(CrcResult >> 8);

            OutPutStr = HexSingleByeToString(crcHigh);
            OutPutStr += HexSingleByeToString(crcLow);
            return OutPutStr;
        }
    }
}
