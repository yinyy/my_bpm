/*波特率暂时使用19200，SIM900A是自适应的， 所以暂时不设置它
Arduino		TX1		RX1		TX3		RX3		TX2		RX2
SIM900A		SRXD	STXD
PLC							m		n					
5815
*/

/*定义系统常量*/
//SIM900A发生错误
#include <JR5815.h>
#include <CardHelper.h>
#include <SIM900A.h>
#include <Led.h>

//SIM900A错误
#define SIM900A_ERROR					"info_SIM900A_Error"
//SIM900AIP设置错误	
#define SIM900A_IP_ERROR				"info_SIM900A_IP_Error"
//SIM900A连网网络
#define SIM900A_CONNECTING_NETWORK		"info_SIM900A_Connecting_Network"
//SIM900A信号不好
#define SIM900A_SINGAL_QUALITY_WEAK		"info_SIM900A_SINGAL_WEAK"
//SIM900A装备就绪
#define SIM900A_READY					"info_SIM900A_Ready"
//定义任务日期错误
#define ERROR_DATE						"error_date"
//定义任务编号错误
#define ERROR_DISPATCH					"error_dispatch"
//定义其它错误
#define ERROR_OTHER						"error_other"
//定义车辆错误
#define ERROR_TRUNK						"error_trunk"
//定义人员错误
#define ERROR_DRIVER					"error_driver"
//定义保存操作成功
#define SUCCESS_SAVE					"success_save"



//信号质量的最小值
#define SINGAL_QUALITY					5
//定义PLC使用的串口
#define PLC								Serial3
//定义调试使用的端口
#define CONSOLE							Serial
//定义默认的波特率
#define DEFAULT_BAUD					115200
//定义循环读卡的时间间隔，默认5秒读一次卡
#define DEFAULT_READ_INTERVAL			5000


//定义PLC的是否可以接收数据的管脚
#define PLC_ALLOW_DATA_PIN				35


//记录上一次读卡的时间
long lastReadMillis;
//车辆卡号和上一次的车辆卡号
String trunkCard, lastTrunkCard;
//人员卡号
String driverCard;
//玖锐5815
JR5815Class reader;
//SIM900A
SIM900AClass sim;
//设备编号
String DEVICE_CODE = "A002";
//服务器地址
String SERVER_URL = "http://221.2.232.82:8766/Sanitation/ashx/SanitationHandler.ashx?";
//是否允许读卡，低电平有效
int bAllowReadCard;

//定义CardHelper对象
CardHelperClass cardHelper;

void setup()
{
	/*先执行高电平的操作，使得SIM900A模块启动，现在不能保证模块成功启动*/
	//延迟
	delay(0);

	//Arduino准备
	CONSOLE.begin(DEFAULT_BAUD);
	PLC.begin(DEFAULT_BAUD);
	Serial1.begin(DEFAULT_BAUD);//SIM900A
	Serial2.begin(DEFAULT_BAUD);//JR5815
	
	while (!Serial1);
	while (!Serial2);
	while (!CONSOLE);
	while (!PLC);
	
	//用Serial1控制SIM900A
	sim.init(&Serial1, true);
	//用Serial2控制玖锐5815
	reader.init(&Serial2, true);

	//判断SIM900A是否准备就绪
	if (!sim.isReady()){
		PLC.println(SIM900A_ERROR);
		CONSOLE.println(SIM900A_ERROR);

		//SIM900A模块无法启动，程序停止
		while (true);
	}
	CONSOLE.println("SIM900A AT Ready.");

	//检查信号状态
	uint8_t sq = sim.checkSignal();
	//如果信号质量小于n，即判断无法获得信号
	if (sq < 5 || sq == 99){
		PLC.println(SIM900A_SINGAL_QUALITY_WEAK);
		CONSOLE.println("SIM900A Singal Quality:" + String(sq));

		//SIM900A信号太弱，无法连接网络
		while (true);
	}
	CONSOLE.println("SIM900A Singal Regular:" + String(sq));
	
	//打开IP应用
	do{
		PLC.println(SIM900A_CONNECTING_NETWORK);
		int cs = sim.checkContextStatus(1);
		if (cs == Context_Status_Connected){
			break;
		}
		else if (cs == Context_Status_Closed){
			sim.openContext(1);
		}
		else{
			//Context_Status_Closing或者Context_Status_Connecting时，等待即可。
			;
		}

		delay(1000);
	} while (1);
	 
	//SIM900A进入工作状态
	PLC.println(SIM900A_READY);
	CONSOLE.println("SIM900A IP Context 1 Ready.");

	pinMode(PLC_ALLOW_DATA_PIN, INPUT);
	
	//初始化一些变量
	lastReadMillis = millis();
	lastTrunkCard = trunkCard;
}

bool allowReadCard(){
	bAllowReadCard = digitalRead(PLC_ALLOW_DATA_PIN);
	if ((bAllowReadCard == 0) && (millis() - lastReadMillis > DEFAULT_READ_INTERVAL)){
		lastReadMillis = millis();
		return true;
	}

	return false;
}

void loop()
{
	if (allowReadCard() && cardHelper.isTrunkCard(trunkCard = reader.readTrunkCard())){
		//读到了和上一次不一样的车辆卡
		driverCard = reader.readDriverCard();

		if (cardHelper.isDriverCard(driverCard)){
			//读到了车辆卡和人员卡
			unsigned int trunkId = cardHelper.parseTrunkId(trunkCard);
			unsigned int driverId = cardHelper.parseDriverId(driverCard);
			unsigned long dispatchId = cardHelper.parseDispatchId(driverCard);
			String driverCode = cardHelper.parseDriverCode(driverCard);
			String trunkPlate = cardHelper.parseTrunkPlate(trunkCard);

			String url = SERVER_URL + "action=get&dispatchId=" + dispatchId;
			CONSOLE.println("Accessing " + url);

			String value = sim.sendHttpRequest(1, url);
			CONSOLE.println(value);
			if (value.startsWith("success_")){
				//获取足够的信息进行验证
				//"success_{0},{1},{2},{3},{4}", dispatchModel.DriverId, dispatchModel.TrunkId, trunkModel.Volumn, dispatchModel.Workload, finished
				value = value.substring(value.indexOf('_') + 1);
				value.trim();

				int index;
				String sDriverId= value.substring(0, (index = value.indexOf(',')));
				String sTrunkId = value.substring(index + 1, (index = value.indexOf(',', index+1)));
				String sVolumn = value.substring(index + 1, (index = value.indexOf(',', index + 1)));
				String sWorkload = value.substring(index + 1, (index = value.indexOf(',', index + 1)));
				String sFinished = value.substring(index + 1);

				if (String(driverId) != sDriverId){
					//任务定义的人员与读卡得到的人员不一致
					PLC.println(ERROR_DRIVER);
				}
				else if (String(trunkId) != sTrunkId){
					//任务定义的车辆与读卡得到的车辆不一致
					PLC.println(ERROR_TRUNK);
				}
				else{
					//通过了验证，将必要的信息发送给PLC
					String msg = "work_" + sDriverId + "," + driverCode + "," + sTrunkId + "," + trunkPlate + "," + sVolumn + "," + sWorkload + "," + sFinished;
					PLC.println(msg);
				}
			}
			else if (value == "error_date"){
				//不是当前日期
				PLC.println(ERROR_DATE);
			}
			else if (value == "error_dispatch"){
				//没有相应的调度任务
				PLC.println(ERROR_DISPATCH);
			}
			else{
				//其它错误
				PLC.println(ERROR_OTHER);
			}
		}
	}
	else if (PLC.available() > 0){
		//返回的数据格式应用为：
		//save,dispatchId,driverId,trunkId,volumn
		//save,25,4,9,13.8
		String msg = PLC.readString();
		msg.trim();

		if (msg.startsWith("save,")){
			msg = msg.substring(5);

			int index;
			String dispatchId = msg.substring(0, (index = msg.indexOf(',')));
			String driverId = msg.substring(index + 1, (index = msg.indexOf(',', index + 1)));
			String trunkId = msg.substring(index + 1, (index = msg.indexOf(',', index + 1)));
			String volumn = msg.substring(index + 1);

			String url = SERVER_URL + "action=save&dispatchId=" + dispatchId + "&driverId=" + driverId + "&trunkId=" + trunkId + "&address=" + DEVICE_CODE + "&volumn=" + volumn;
			CONSOLE.println("Accessing " + url);

			String value = sim.sendHttpRequest(1, url);
			CONSOLE.println(value);
			if (value.startsWith("success_save")){
				//成功保存工作量，通知PLC
				PLC.println(SUCCESS_SAVE);
				CONSOLE.println(SUCCESS_SAVE);
			}
			else{
				//其它错误
				PLC.println(ERROR_OTHER);
				CONSOLE.println(ERROR_OTHER);
			}
		}
	}
}