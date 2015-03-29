/*波特率暂时使用19200，SIM900A是自适应的， 所以暂时不设置它
Arduino		TX1		RX1		TX3		RX3		TX2		RX2
SIM900A		SRXD	STXD
PLC							m		n					
5815
*/

#include "SimHelper.h"
#include "PlcHelper.h"
#include "CardReaderHelper.h"

//信号质量的最小值
#define SINGAL_QUALITY					5
//定义默认的波特率
#define DEFAULT_BAUD					115200
//定义PLC默认的波特率
#define DEFAULT_BAUD_PLC				9600
//定义SIM900A的PWR_KEY
#define SIM900A_PWR_KEY_PIN				36
//设备编号
#define DEVICE_CODE "A002"


//记录上一次读卡的时间
long lastReadMillis;

CardReaderHelperClass crh;
PlcHelperClass plch;
SimHelperClass simh;

String driverCard;
String trunkCard;
String driverCode;
String trunkPlate;
boolean canReadCard = true;


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
	Serial.begin(DEFAULT_BAUD);
	Serial1.begin(DEFAULT_BAUD);//SIM900A
	Serial2.begin(DEFAULT_BAUD_PLC);//PLC
	Serial3.begin(DEFAULT_BAUD);//JR5815

	while (!Serial1);
	while (!Serial2);
	while (!Serial3); 
	while (!Serial);

	simh.init();
	delay(1000);
	
	//判断SIM900A是否准备就绪
	if (!simh.isReady()){
		Serial.println("SIM900A_ERROR");

		//SIM900A模块无法启动，程序停止
		while (true);
	}
	Serial.println("SIM900A AT Ready.");

	//检查信号状态
	uint8_t sq = simh.checkSignal();
	Serial.println("SIM900A Singal Regular:" + String(sq));
	//如果信号质量小于n，即判断无法获得信号
	if (sq < 5 || sq == 99){
		Serial.println("Singal Low!");
		
		//SIM900A信号太弱，无法连接网络
		while (true);
	}
	
	//打开IP应用
	do{
		Serial.println("SIM900A_CONNECTING_NETWORK");

		int cs = simh.checkContextStatus();
		if (cs == Context_Status_Connected){
			break;
		}
		else if (cs == Context_Status_Closed){
			simh.openContext();
		}
		else{
			;//Context_Status_Closing或者Context_Status_Connecting时，等待即可。
		}

		delay(1000);
	} while (1);
	 
	//SIM900A进入工作状态
	Serial.println("SIM900A IP Context 1 Ready.");
}

void loop()
{
	String plcCommand = plch.readCommand();
	if (plcCommand.startsWith("02010101")){
		//是否可以读卡
		canReadCard = true;
	}
	else if (plcCommand.startsWith("02010100")){
		canReadCard = false;
	}
	else if (plcCommand.startsWith("020303")){
		//加水完成，保存信息
		float volumn;
		int kind;

		if (plch.getVolumnAndKind(plcCommand, &volumn, &kind)){
			//把相关的信息存储到服务器
			if (!simh.save(DEVICE_CODE, driverCode, trunkPlate, volumn, kind)){
				plch.saveError();
			}
			else{
				plch.saveSuccess();
			}
		}
	}

	if (canReadCard){
		trunkCard = crh.readTrunkCard();
		if (trunkCard != ""){
			driverCard = crh.readDriverCard();
			if (driverCard != ""){
				//车卡和人卡都读出来了，把车牌号和人员编号发送给PLC
				trunkPlate = crh.getPlate(trunkCard);
				driverCode = crh.getCode(driverCard);
				float volumn = crh.getVolumn(trunkCard);

				plch.send(driverCode, trunkPlate, volumn);

				canReadCard = false;
			}
		}
	}
}