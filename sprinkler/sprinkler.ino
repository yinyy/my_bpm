/*
Arduino		TX1		RX1		TX3		RX3
SIM900A		SRXD	STXD
PLC							m		n						
*/

/*定义系统常量*/
//SIM900A发生错误
#include <SIM900A.h>
#include <Led.h>

#define SIM900A_ERROR					"Error001"
//SIM900AIP设置错误	
#define SIM900A_ERROR_IP				"Error002"
//SIM900A信号不好
#define SIM900A_SINGAL_QUALITY_WEAK		"SINGAL_WEAK"
//信号质量的最小值
#define SINGAL_QUALITY					5
//定义读卡器使用的串口
#define READER							Serial1
//定义PLC使用的串口
#define PLC								Serial
//定义调试使用的端口
#define CONSOLE							Serial

SIM900AClass sim;
//设备编号
String DEVICE_CODE = "A002";
//服务器地址
String SERVER_URL = "http://221.2.232.82:8766/Sanitation/ashx/SanitationHandler.ashx?";

void setup()
{
	/*先执行高电平的操作，使得SIM900A模块启动，现在不能保证模块成功启动*/
	//延迟
	delay(0);

	//Arduino准备
	CONSOLE.begin(115200);
	Serial1.begin(115200);
	PLC.begin(115200);

	while (!Serial1);
	while (!CONSOLE);
	while (!PLC);
	
	//用Serial1控制SIM900A
	sim.init(&Serial1, true);
	
	//判断SIM900A是否准备就绪
	if (!sim.isReady()){
		PLC.println(SIM900A_ERROR);
		CONSOLE.println("SIM900A Error");

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

		while (true);
	}
	CONSOLE.println("SIM900A Singal Regular:" + String(sq));
	
	//打开IP应用
	int cs = sim.checkContextStatus(1);
	while (cs != Context_Status_Connected){
		if (cs == Context_Status_Closed){
			sim.openContext(1);
		}

		delay(1000);

		cs = sim.checkContextStatus(1);
	}
	CONSOLE.println("SIM900A IP Context 1 Ready.");
}

void loop()
{
	//2014-12-12,DispatchID,DriverId[DriverCode],TrunkId[TrunkPlate],Workload
	//2014-12-25,25,4[D003],9[E0K123],5
	if (READER.available() > 0){
		String msg = READER.readString();
		msg.trim();

		int index = msg.indexOf(',');
		String time = msg.substring(0, index);
		String dispatchId = msg.substring(index + 1, (index = msg.indexOf(',', index + 1)));
		String driver = msg.substring(index + 1, (index = msg.indexOf(',', index + 1)));
		String driverId = driver.substring(driver.indexOf('['));
		String driverCode = driver.substring(driver.indexOf('[') + 1, driver.indexOf(']'));
		String trunk = msg.substring(index + 1, (index = msg.indexOf(',', index + 1)));
		String trunkId = trunk.substring(trunk.indexOf('['));
		String trunkPlate = trunk.substring(trunk.indexOf('[') + 1, trunk.indexOf(']'));
		String workload = msg.substring(index + 1);

		String url = SERVER_URL +"action=get&time=" + time + "&dispatchId=" + dispatchId;
		CONSOLE.println("Prepare Access " + url);

		String value = sim.sendHttpRequest(1, url);	
		CONSOLE.println(value);
		if (value.startsWith("success_")){
			//成功获取了今天已经完成的工作量，将所有信息发给PLC
			String worked = value.substring(value.indexOf('_') + 1);
			PLC.println(msg + "," + worked);
		}
		else if (value == "error_date"){
			//不是当前日期
			PLC.println("error_date");
		}
		else if (value == "error_dispatch"){
			//没有相应的调度任务
			PLC.println("error_dispatch");
		}
		else{
			//其它错误
			PLC.println("error_others");
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

			String url = SERVER_URL + "action=save&dispatchId=" + dispatchId + "&driverId="+driverId +"&trunkId=" + trunkId + "&address=" + DEVICE_CODE + "&volumn=" + volumn;
			CONSOLE.println("Prepare Access " + url);

			String value = sim.sendHttpRequest(1, url);
			CONSOLE.println(value);
			if (value.startsWith("success_saved")){
				//成功保存工作量，通知PLC
				PLC.println("success_saved");
			}
			else{
				//其它错误
				PLC.println("error_others");
			}
		}
	}
	
	//if (Serial1.available() > 0){
	//	int x = Serial1.read();
	//	Serial.write(x);
	//	/*String m = Serial1.readString();
	//	Serial.print(m);
	//	Serial.print('[');
	//	Serial.print(m.length());
	//	Serial.print(']');*/
	//	//Serial.println(Serial1.readString());
	//}
}