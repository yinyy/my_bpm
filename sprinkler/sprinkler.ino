/*
Arduino		TX1		RX1		TX3		RX3
SIM900A		SRXD	STXD
PLC							m		n						
*/

/*����ϵͳ����*/
//SIM900A��������
#include <SIM900A.h>
#include <Led.h>

#define SIM900A_ERROR					"Error001"
//SIM900AIP���ô���	
#define SIM900A_ERROR_IP				"Error002"
//SIM900A�źŲ���
#define SIM900A_SINGAL_QUALITY_WEAK		"SINGAL_WEAK"
//�ź���������Сֵ
#define SINGAL_QUALITY					5
//���������ʹ�õĴ���
#define READER							Serial1
//����PLCʹ�õĴ���
#define PLC								Serial
//�������ʹ�õĶ˿�
#define CONSOLE							Serial

SIM900AClass sim;
//�豸���
String DEVICE_CODE = "A002";
//��������ַ
String SERVER_URL = "http://221.2.232.82:8766/Sanitation/ashx/SanitationHandler.ashx?";

void setup()
{
	/*��ִ�иߵ�ƽ�Ĳ�����ʹ��SIM900Aģ�����������ڲ��ܱ�֤ģ��ɹ�����*/
	//�ӳ�
	delay(0);

	//Arduino׼��
	CONSOLE.begin(115200);
	Serial1.begin(115200);
	PLC.begin(115200);

	while (!Serial1);
	while (!CONSOLE);
	while (!PLC);
	
	//��Serial1����SIM900A
	sim.init(&Serial1, true);
	
	//�ж�SIM900A�Ƿ�׼������
	if (!sim.isReady()){
		PLC.println(SIM900A_ERROR);
		CONSOLE.println("SIM900A Error");

		//SIM900Aģ���޷�����������ֹͣ
		while (true);
	}
	CONSOLE.println("SIM900A AT Ready.");

	//����ź�״̬
	uint8_t sq = sim.checkSignal();
	//����ź�����С��n�����ж��޷�����ź�
	if (sq < 5 || sq == 99){
		PLC.println(SIM900A_SINGAL_QUALITY_WEAK);
		CONSOLE.println("SIM900A Singal Quality:" + String(sq));

		while (true);
	}
	CONSOLE.println("SIM900A Singal Regular:" + String(sq));
	
	//��IPӦ��
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
			//�ɹ���ȡ�˽����Ѿ���ɵĹ���������������Ϣ����PLC
			String worked = value.substring(value.indexOf('_') + 1);
			PLC.println(msg + "," + worked);
		}
		else if (value == "error_date"){
			//���ǵ�ǰ����
			PLC.println("error_date");
		}
		else if (value == "error_dispatch"){
			//û����Ӧ�ĵ�������
			PLC.println("error_dispatch");
		}
		else{
			//��������
			PLC.println("error_others");
		}
	}
	else if (PLC.available() > 0){
		//���ص����ݸ�ʽӦ��Ϊ��
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
				//�ɹ����湤������֪ͨPLC
				PLC.println("success_saved");
			}
			else{
				//��������
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