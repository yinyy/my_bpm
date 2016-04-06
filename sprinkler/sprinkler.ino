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
#define DEFAULT_BAUD_SIM				115200
//定义PLC默认的波特率
#define DEFAULT_BAUD_CARD_READER		115200
//定义PLC默认的波特率
#define DEFAULT_BAUD_PLC				9600
//定义SIM900A的PWR_KEY
#define SIM900A_PWR_KEY_PIN				36
//设备编号
#define DEVICE_CODE "A002"

typedef struct ObjectStruct{
	String card, code;
};

typedef struct Info_Struct{
	ObjectStruct driver, trunk;
	long time;
};

CardReaderHelperClass crh;
PlcHelperClass plch;
SimHelperClass simh;

Info_Struct current, last;

void openIP() {
	//打开IP应用
	int count = 0;
	do {
		//Serial.println("SIM900A_CONNECTING_NETWORK");

		int cs = simh.checkContextStatus();
		if (cs == Context_Status_Connected) {
			break;
		}
		else if (cs == Context_Status_Closed) {
			simh.openContext();
		}
		else {
			;//Context_Status_Closing或者Context_Status_Connecting时，等待即可。
		}

		delay(1000);

		if (count++ > 5) {
			break;
		}
	} while (1);
}

void setup()
{
	delay(20000);

	current.driver.card = "";
	current.trunk.card = "";
	current.time = 0;

	last.driver.card = "";
	last.trunk.card = "";
	last.time = 0;

	//Arduino准备
	//Serial.begin(9600);
	Serial1.begin(DEFAULT_BAUD_SIM);//SIM900A
	Serial2.begin(DEFAULT_BAUD_PLC);//PLC
	Serial3.begin(DEFAULT_BAUD_CARD_READER);//JR5815

	while (!Serial1);
	while (!Serial2);
	while (!Serial3); 
	//while (!Serial);

	simh.init();

	int count = 0;
	//判断SIM900A是否准备就绪
	while (!simh.isReady()){
		//Serial.println("SIM900A_ERROR");

		//SIM900A模块无法启动，程序停止
		delay(1000);

		if (count++ > 20){
			break;
		}
	}
	//Serial.println("SIM900A AT Ready.");

	//检查信号状态
	uint8_t sq = simh.checkSignal();
	//Serial.println("SIM900A Singal Regular:" + String(sq));
	//如果信号质量小于n，即判断无法获得信号

	count = 0;
	while (sq < 5 || sq == 99){
		//Serial.println("Singal Low!");
		
		//SIM900A信号太弱，无法连接网络
		delay(1000);

		sq = simh.checkSignal();
		//Serial.println("SIM900A Singal Regular:" + String(sq));

		if (count++ > 5){
			break;
		}
	}
	
	openIP();
	 
	//SIM900A进入工作状态
	//Serial.println("SIM900A IP Context 1 Ready.");
}

/*
	完整流程：
	1、读车卡。读不到，返回步骤1。
	2、读人卡。读不到，返回步骤1。
	3、询问PLC是否准备好——接收PLC回复的数据。
	（1）如果接收数据错误（收到PLC返回的错误提示信息），则返回步骤3。
	（2）如果接受数据正确，则：
	——如果忙，则返回步骤3。
	——如果PLC空闲，向发送车牌号、人员编号、车辆体积*10。
	4、等待接收PLC完成加水后结束标志——接收到结束标志后通过SIM900A将设备编号、车牌号、人员编号、加水体积、浓度、类型发送到服务器。
	5、返回步骤1
	*/

////新读卡器
//void loop()
//{
//	delay(100);
//	if (crh.getCard(&current.trunk.card, &current.driver.card)){
//		current.time = millis();
//
//		//车卡和人卡都读出来了，把车牌号和人员编号发送给PLC
//		current.trunk.code = crh.getPlate(current.trunk.card);
//		current.driver.code = crh.getCode(current.driver.card);
//		float volumn = crh.getVolumn(current.trunk.card);
//
//		//只有满足一下条件才能继续操作
//		//1、读到的卡不一样
//		//2、读到是特殊卡
//		//3、同一张卡，超过了5分钟
//		if (
//			(current.driver.code + current.trunk.code == "0000E00000") ||
//			(current.driver.code + current.trunk.code != last.driver.code + last.trunk.code) ||
//			(current.time - last.time >= 300000)){
//
//			while (!plch.isReady()){
//				delay(1000);//在PLC没有准备好的情况下，每1秒询问一次
//				//Serial.println("PLC ready?");
//			}
//
//			//Serial.println("PLC Ready");
//			//PLC就绪
//			plch.send(current.driver.code, current.trunk.code, volumn);
//
//			String cmd;
//			do{
//				delay(1000);
//				cmd = plch.readCommand();
//			} while (!cmd.startsWith("010205"));//如果读到的PLC的指令不是以“010205”开头的，则一直等待
//
//			//加水完成
//			int kind;
//			int potency;
//			if (plch.getVolumnPotencyKind(cmd, &volumn, &potency, &kind)){
//				//把相关的信息存储到服务器
//				if (!simh.save(DEVICE_CODE, current.driver.code, current.trunk.code, volumn, potency, kind)){
//					plch.saveError();
//					//Serial.println("Save Error");
//				}
//				else{
//					plch.saveSuccess();
//					//Serial.println("Save Success");
//				}
//			}
//
//			//完成一次加水过程，等到30秒
//			delay(30000);
//
//			last.driver.code = current.driver.code;
//			last.trunk.code = current.trunk.code;
//			last.time = current.time;
//		}
//	}
//}

//老读卡器（玖锐）
void loop()
{
	/*
	完整流程：
	1、读车卡。读不到，返回步骤1。
	2、读人卡。读不到，返回步骤1。
	3、询问PLC是否准备好——接收PLC回复的数据。
	（1）如果接收数据错误（收到PLC返回的错误提示信息），则返回步骤3。
	（2）如果接受数据正确，则：
	——如果忙，则返回步骤3。
	——如果PLC空闲，向发送车牌号、人员编号、车辆体积*10。
	4、等待接收PLC完成加水后结束标志——接收到结束标志后通过SIM900A将设备编号、车牌号、人员编号、加水体积、浓度、类型发送到服务器。
	5、返回步骤1
	*/

	//delay(20);
	current.trunk.card = crh.readTrunkCard();//"544B000A4538323736320058";
	if (current.trunk.card != ""){
		int read_count = 0;

		do{
			//delay(20);

			current.driver.card = crh.readDriverCard();//"5050000830303031";// 
			read_count++;
		} while (read_count < 10 && current.driver.card == "");

		if (current.driver.card != ""){
			current.time = millis();

			//车卡和人卡都读出来了，把车牌号和人员编号发送给PLC
			current.trunk.code = crh.getPlate(current.trunk.card);
			current.driver.code = crh.getCode(current.driver.card);
			float volumn = crh.getVolumn(current.trunk.card);

			//只有满足一下条件才能继续操作
			//1、读到的卡不一样
			//2、读到是特殊卡
			//3、同一张卡，超过了5分钟
			if (
				(current.driver.code + current.trunk.code == "0000E00000") ||
				(current.driver.code + current.trunk.code != last.driver.code + last.trunk.code) ||
				(current.time - last.time >= 300000)){

				while (!plch.isReady()){
					delay(1000);//在PLC没有准备好的情况下，每1秒询问一次
					//Serial.println("PLC ready?");
				}

				//Serial.println("PLC Ready");
				//PLC就绪
				plch.send(current.driver.code, current.trunk.code, volumn);

				String cmd;// = "0102050051000000";
				do{
					delay(1000);
					cmd = plch.readCommand();
				} while (!cmd.startsWith("010205"));//如果读到的PLC的指令不是以“010205”开头的，则一直等待

				//加水完成
				int kind;
				int potency;
				if (plch.getVolumnPotencyKind(cmd, &volumn, &potency, &kind)){
					/*Serial.print("plate = ");
					Serial.println(current.trunk.code);
					Serial.print("code = ");
					Serial.println(current.driver.code);
					Serial.print("volumn = ");
					Serial.println(volumn);
					Serial.print("potency = ");
					Serial.println(potency);
					Serial.print("kind = ");
					Serial.println(kind);*/

					int saveCount = 0;
					boolean saved = false;

					openIP();

					do{
						//把相关的信息存储到服务器，尝试提交5次
						saved = simh.save(DEVICE_CODE, current.driver.code, current.trunk.code, volumn, potency, kind);
						//Serial.print("Saved ");
						//Serial.println(saveCount);

						if (saved){
							break;
						}
					} while (saveCount++ < 5);

					if (saved){
						plch.saveSuccess();
						//Serial.println("Save Success");
					}
					else{
						plch.saveError();
						//Serial.println("Save Error");
					}
				}

				//完成一次加水过程，等到10秒
				delay(10000);
			
				last.driver.code = current.driver.code;
				last.trunk.code = current.trunk.code;
				last.time = current.time;
			}
		}
	}
}