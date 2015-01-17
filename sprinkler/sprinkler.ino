/*波特率暂时使用19200，SIM900A是自适应的， 所以暂时不设置它
Arduino		TX1		RX1		TX3		RX3		TX2		RX2
SIM900A		SRXD	STXD
PLC							m		n					
5815
*/

/*定义系统常量*/
//SIM900A发生错误
#include "Plc.h"
#include <JR5815.h>
#include <CardHelper.h>
#include <SIM900A.h>
#include <Led.h>

//信号质量的最小值
#define SINGAL_QUALITY					5
//定义调试使用的端口
#define CONSOLE							Serial
//定义默认的波特率
#define DEFAULT_BAUD					115200
//定义循环读卡的时间间隔，默认5秒读一次卡
#define DEFAULT_READ_INTERVAL			5000
//定义PLC的是否可以接收数据的管脚
#define PLC_ALLOW_DATA_PIN				35
//定义SIM900A的PWR_KEY
#define SIM900A_PWR_KEY_PIN				36


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
//定义PLC设备
PlcClass plc;
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
	//等待SIM900A加电
	pinMode(SIM900A_PWR_KEY_PIN, OUTPUT);
	digitalWrite(SIM900A_PWR_KEY_PIN, HIGH);
	delay(500);

	//延迟5秒，模拟按键，启动SIM900A模块
	digitalWrite(SIM900A_PWR_KEY_PIN, LOW);
	delay(5000);

	//Arduino准备
	CONSOLE.begin(DEFAULT_BAUD);
	Serial1.begin(DEFAULT_BAUD);//SIM900A
	Serial2.begin(DEFAULT_BAUD);//JR5815
	Serial3.begin(DEFAULT_BAUD);//PLC

	while (!Serial1);
	while (!Serial2);
	while (!Serial3); 
	while (!CONSOLE);
	
	//用Serial1控制SIM900A
	sim.init(&Serial1, true);
	//用Serial2控制玖锐5815
	reader.init(&Serial2, true);
	//用Serial3控制PLC
	plc.init(&Serial3);

	//判断SIM900A是否准备就绪
	if (!sim.isReady()){
		plc.send(PlcInfoType_error);
		CONSOLE.println("SIM900A_ERROR");

		//SIM900A模块无法启动，程序停止
		while (true);
	}
	CONSOLE.println("SIM900A AT Ready.");

	//检查信号状态
	uint8_t sq = sim.checkSignal();
	CONSOLE.println("SIM900A Singal Regular:" + String(sq));
	//如果信号质量小于n，即判断无法获得信号
	if (sq < 5 || sq == 99){
		plc.send(PlcInfoType_date);
		
		//SIM900A信号太弱，无法连接网络
		while (true);
	}
	
	//打开IP应用
	do{
		CONSOLE.println("SIM900A_CONNECTING_NETWORK");

		int cs = sim.checkContextStatus(1);
		if (cs == Context_Status_Connected){
			break;
		}
		else if (cs == Context_Status_Closed){
			sim.openContext(1);
		}
		else{
			;//Context_Status_Closing或者Context_Status_Connecting时，等待即可。
		}

		delay(1000);
	} while (1);
	 
	//SIM900A进入工作状态
	plc.send(PlcInfoType_ready);
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
			trunkPlate = trunkPlate.substring(1);


			String url = SERVER_URL + "action=get&timespan=" + String(millis(), HEX) + "&dispatchId=" + dispatchId;//加入时间戳，避免缓存
			CONSOLE.println("Accessing " + url);

			String value = sim.sendHttpRequest(1, url);
			CONSOLE.println(value);
			if (value.startsWith("success_")){
				//获取足够的信息进行验证
				//"success_{0},{1},{2},{3},{4},{5},{6}", dispatchModel.DriverId, dispatchModel.TrunkId, trunkModel.Volumn, dispatchModel.Workload, finished, potenct, kind
				value = value.substring(value.indexOf('_') + 1);
				value.trim();

				int index;
				String sDriverId = value.substring(0, (index = value.indexOf(',')));
				String sTrunkId = value.substring(index + 1, (index = value.indexOf(',', index + 1)));
				String sVolumn = value.substring(index + 1, (index = value.indexOf(',', index + 1)));
				String sWorkload = value.substring(index + 1, (index = value.indexOf(',', index + 1)));
				String sFinished = value.substring(index + 1, (index = value.indexOf(',', index + 1)));
				String sPotency = value.substring(index + 1, (index = value.indexOf(',', index + 1)));
				String sKind = value.substring(index + 1);

				if (String(driverId) != sDriverId){
					//任务定义的人员与读卡得到的人员不一致
					plc.send(PlcInfoType_driver);
				}
				else if (String(trunkId) != sTrunkId){
					//任务定义的车辆与读卡得到的车辆不一致
					plc.send(PlcInfoType_trunk);
				}
				else{
					//通过了验证，将必要的信息发送给PLC
					plc.send(dispatchId, driverCode, trunkPlate, int(sVolumn.toFloat() * 10), sWorkload.toInt(), sFinished.toInt(), sPotency.toFloat(), sKind);
				}
			}
			else if (value == "error_date"){
				//不是当前日期
				plc.send(PlcInfoType_date);
			}
			else if (value == "error_dispatch"){
				//没有相应的调度任务
				plc.send(PlcInfoType_dispatch);
			}
			else{
				//其它错误
				plc.send(PlcInfoType_unknow);
			}
		}
	}

	//处理PLC返回的数据
	String msg = plc.read();
	if (msg != "" && msg.startsWith("010306")){
		//完成操作
		char cs[9];
		msg.substring(6, 14).toCharArray(cs, 9, 0);
		String dispatchId = String(strtol(cs, NULL, 16), DEC);
		
		msg.substring(14, 18).toCharArray(cs, 5, 0);
		uint16_t v = (uint16_t)strtol(cs, NULL, 16);
		String volumn = String(v*1.0 / 10);

		String url = SERVER_URL + "action=save&timespan=" + String(millis(), HEX) + "&dispatchId=" + dispatchId + "&address=" + DEVICE_CODE + "&volumn=" + volumn;
		CONSOLE.println("Accessing " + url);

		String value = sim.sendHttpRequest(1, url);
		CONSOLE.println(value);
		if (value.startsWith("success_save")){
			//成功保存工作量，通知PLC
			plc.send(PlcInfoType_uploaded);
			CONSOLE.println("SUCCESS_SAVE");
		}
		else{
			//其它错误
			plc.send(PlcInfoType_unknow);
			CONSOLE.println("ERROR_OTHER");
		}
	}
}