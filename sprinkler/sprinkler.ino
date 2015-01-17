/*��������ʱʹ��19200��SIM900A������Ӧ�ģ� ������ʱ��������
Arduino		TX1		RX1		TX3		RX3		TX2		RX2
SIM900A		SRXD	STXD
PLC							m		n					
5815
*/

/*����ϵͳ����*/
//SIM900A��������
#include "Plc.h"
#include <JR5815.h>
#include <CardHelper.h>
#include <SIM900A.h>
#include <Led.h>

//�ź���������Сֵ
#define SINGAL_QUALITY					5
//�������ʹ�õĶ˿�
#define CONSOLE							Serial
//����Ĭ�ϵĲ�����
#define DEFAULT_BAUD					115200
//����ѭ��������ʱ������Ĭ��5���һ�ο�
#define DEFAULT_READ_INTERVAL			5000
//����PLC���Ƿ���Խ������ݵĹܽ�
#define PLC_ALLOW_DATA_PIN				35
//����SIM900A��PWR_KEY
#define SIM900A_PWR_KEY_PIN				36


//��¼��һ�ζ�����ʱ��
long lastReadMillis;
//�������ź���һ�εĳ�������
String trunkCard, lastTrunkCard;
//��Ա����
String driverCard;
//����5815
JR5815Class reader;
//SIM900A
SIM900AClass sim;
//����PLC�豸
PlcClass plc;
//�豸���
String DEVICE_CODE = "A002";
//��������ַ
String SERVER_URL = "http://221.2.232.82:8766/Sanitation/ashx/SanitationHandler.ashx?";
//�Ƿ�����������͵�ƽ��Ч
int bAllowReadCard;
//����CardHelper����
CardHelperClass cardHelper;


void setup()
{
	//�ȴ�SIM900A�ӵ�
	pinMode(SIM900A_PWR_KEY_PIN, OUTPUT);
	digitalWrite(SIM900A_PWR_KEY_PIN, HIGH);
	delay(500);

	//�ӳ�5�룬ģ�ⰴ��������SIM900Aģ��
	digitalWrite(SIM900A_PWR_KEY_PIN, LOW);
	delay(5000);

	//Arduino׼��
	CONSOLE.begin(DEFAULT_BAUD);
	Serial1.begin(DEFAULT_BAUD);//SIM900A
	Serial2.begin(DEFAULT_BAUD);//JR5815
	Serial3.begin(DEFAULT_BAUD);//PLC

	while (!Serial1);
	while (!Serial2);
	while (!Serial3); 
	while (!CONSOLE);
	
	//��Serial1����SIM900A
	sim.init(&Serial1, true);
	//��Serial2���ƾ���5815
	reader.init(&Serial2, true);
	//��Serial3����PLC
	plc.init(&Serial3);

	//�ж�SIM900A�Ƿ�׼������
	if (!sim.isReady()){
		plc.send(PlcInfoType_error);
		CONSOLE.println("SIM900A_ERROR");

		//SIM900Aģ���޷�����������ֹͣ
		while (true);
	}
	CONSOLE.println("SIM900A AT Ready.");

	//����ź�״̬
	uint8_t sq = sim.checkSignal();
	CONSOLE.println("SIM900A Singal Regular:" + String(sq));
	//����ź�����С��n�����ж��޷�����ź�
	if (sq < 5 || sq == 99){
		plc.send(PlcInfoType_date);
		
		//SIM900A�ź�̫�����޷���������
		while (true);
	}
	
	//��IPӦ��
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
			;//Context_Status_Closing����Context_Status_Connectingʱ���ȴ����ɡ�
		}

		delay(1000);
	} while (1);
	 
	//SIM900A���빤��״̬
	plc.send(PlcInfoType_ready);
	CONSOLE.println("SIM900A IP Context 1 Ready.");

	pinMode(PLC_ALLOW_DATA_PIN, INPUT);
	
	//��ʼ��һЩ����
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
		//�����˺���һ�β�һ���ĳ�����
		driverCard = reader.readDriverCard();

		if (cardHelper.isDriverCard(driverCard)){
			//�����˳���������Ա��
			unsigned int trunkId = cardHelper.parseTrunkId(trunkCard);
			unsigned int driverId = cardHelper.parseDriverId(driverCard);
			unsigned long dispatchId = cardHelper.parseDispatchId(driverCard);
			String driverCode = cardHelper.parseDriverCode(driverCard);
			String trunkPlate = cardHelper.parseTrunkPlate(trunkCard);
			trunkPlate = trunkPlate.substring(1);


			String url = SERVER_URL + "action=get&timespan=" + String(millis(), HEX) + "&dispatchId=" + dispatchId;//����ʱ��������⻺��
			CONSOLE.println("Accessing " + url);

			String value = sim.sendHttpRequest(1, url);
			CONSOLE.println(value);
			if (value.startsWith("success_")){
				//��ȡ�㹻����Ϣ������֤
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
					//���������Ա������õ�����Ա��һ��
					plc.send(PlcInfoType_driver);
				}
				else if (String(trunkId) != sTrunkId){
					//������ĳ���������õ��ĳ�����һ��
					plc.send(PlcInfoType_trunk);
				}
				else{
					//ͨ������֤������Ҫ����Ϣ���͸�PLC
					plc.send(dispatchId, driverCode, trunkPlate, int(sVolumn.toFloat() * 10), sWorkload.toInt(), sFinished.toInt(), sPotency.toFloat(), sKind);
				}
			}
			else if (value == "error_date"){
				//���ǵ�ǰ����
				plc.send(PlcInfoType_date);
			}
			else if (value == "error_dispatch"){
				//û����Ӧ�ĵ�������
				plc.send(PlcInfoType_dispatch);
			}
			else{
				//��������
				plc.send(PlcInfoType_unknow);
			}
		}
	}

	//����PLC���ص�����
	String msg = plc.read();
	if (msg != "" && msg.startsWith("010306")){
		//��ɲ���
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
			//�ɹ����湤������֪ͨPLC
			plc.send(PlcInfoType_uploaded);
			CONSOLE.println("SUCCESS_SAVE");
		}
		else{
			//��������
			plc.send(PlcInfoType_unknow);
			CONSOLE.println("ERROR_OTHER");
		}
	}
}