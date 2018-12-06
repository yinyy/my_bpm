/*��������ʱʹ��19200��SIM900A������Ӧ�ģ� ������ʱ��������
Arduino		TX1		RX1		TX3		RX3		TX2		RX2
SIM900A		SRXD	STXD
PLC							m		n					
5815
*/

#include "CardReaderHelper2.h"
#include "SimHelper.h"
#include "PlcHelper.h"

//�ź���������Сֵ
#define SINGAL_QUALITY					5
//����Ĭ�ϵĲ�����
#define DEFAULT_BAUD_SIM				115200
//����PLCĬ�ϵĲ�����
#define DEFAULT_BAUD_CARD_READER		57600
//����PLCĬ�ϵĲ�����
#define DEFAULT_BAUD_PLC				9600
//����SIM900A��PWR_KEY
#define SIM900A_PWR_KEY_PIN				36
//�豸���
#define DEVICE_CODE "A002"

typedef struct ObjectStruct{
	String card, code;
};

typedef struct Info_Struct{
	ObjectStruct driver, trunk;
	long time;
};

CardReaderHelper2Class crh;
PlcHelperClass plch;
SimHelperClass simh;

Info_Struct current, last;
String shortMessage = "";
String cards;

void openIP() {
	//��IPӦ��
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
			;//Context_Status_Closing����Context_Status_Connectingʱ���ȴ����ɡ�
		}

		delay(1000);

		if (count++ > 5) {
			break;
		}
	} while (1);
}

void closeIP() {
	//��IPӦ��
	int count = 0;
	do {
		//Serial.println("SIM900A_CONNECTING_NETWORK");

		int cs = simh.checkContextStatus();
		if (cs == Context_Status_Connected) {
			simh.closeContext();
		}
		else if (cs == Context_Status_Closed) {
			break;
		}
		else {
			
		}

		delay(1000);

		if (count++ > 5) {
			break;
		}
	} while (1);
}

void setup()
{
	delay(10000);

	current.driver.card = "";
	current.trunk.card = "";
	current.time = 0;

	last.driver.card = "";
	last.trunk.card = "";
	last.time = 0;
	
	//Arduino׼��
	//Serial.begin(9600);
	//Serial1.begin(DEFAULT_BAUD_SIM);//SIM900A
	Serial2.begin(DEFAULT_BAUD_PLC);//PLC
	Serial3.begin(DEFAULT_BAUD_CARD_READER);//JR5815

	//while (!Serial1);
	while (!Serial2);
	while (!Serial3); 
	//while (!Serial);

	//simh.init();

	//int count = 0;
	////�ж�SIM900A�Ƿ�׼������
	//while (!simh.isReady()){
	//	//Serial.println("SIM900A_ERROR");

	//	//SIM900Aģ���޷�����������ֹͣ
	//	delay(1000);

	//	if (count++ > 20){
	//		break;
	//	}
	//}
	//Serial.println("SIM900A AT Ready.");

	////����ź�״̬
	//uint8_t sq = simh.checkSignal();
	////Serial.println("SIM900A Singal Regular:" + String(sq));
	////����ź�����С��n�����ж��޷�����ź�

	//count = 0;
	//while (sq < 5 || sq == 99){
	//	//Serial.println("Singal Low!");
	//	
	//	//SIM900A�ź�̫�����޷���������
	//	delay(1000);

	//	sq = simh.checkSignal();
	//	//Serial.println("SIM900A Singal Regular:" + String(sq));

	//	if (count++ > 5){
	//		break;
	//	}
	//}
	
	//openIP();
	 
	//SIM900A���빤��״̬
	//Serial.println("SIM900A IP Context 1 Ready.");
}


////�϶�����������
//void loop()
//{
//	/*
//	�������̣�
//	1���������������������ز���1��
//	2�����˿��������������ز���1��
//	3��ѯ��PLC�Ƿ�׼���á�������PLC�ظ������ݡ�
//	��1������������ݴ����յ�PLC���صĴ�����ʾ��Ϣ�����򷵻ز���3��
//	��2���������������ȷ����
//	�������æ���򷵻ز���3��
//	�������PLC���У����ͳ��ƺš���Ա��š��������*10��
//	4���ȴ�����PLC��ɼ�ˮ�������־�������յ�������־��ͨ��SIM900A���豸��š����ƺš���Ա��š���ˮ�����Ũ�ȡ����ͷ��͵���������
//	5�����ز���1
//	*/
//
//	shortMessage = "";
//
//	//delay(20);
//	current.trunk.card = crh.readTrunkCard();//"544B000A4538323736320058";
//	if (current.trunk.card != ""){
//		int read_count = 0;
//
//		do{
//			//delay(20);
//
//			current.driver.card = crh.readDriverCard();//"5050000830303031";// 
//			read_count++;
//		} while (read_count < 10 && current.driver.card == "");
//
//		if (current.driver.card != ""){
//			current.time = millis();
//
//			//�������˿����������ˣ��ѳ��ƺź���Ա��ŷ��͸�PLC
//			current.trunk.code = crh.getPlate(current.trunk.card);
//			current.driver.code = crh.getCode(current.driver.card);
//			float volumn = crh.getVolumn(current.trunk.card);
//
//			//ֻ������һ���������ܼ�������
//			//1�������Ŀ���һ��
//			//2�����������⿨
//			//3��ͬһ�ſ���������5����
//			if (
//				(current.driver.code + current.trunk.code == "0000E00000") ||
//				(current.driver.code + current.trunk.code != last.driver.code + last.trunk.code) ||
//				(current.time - last.time >= 300000)){
//
//				while (!plch.isReady()){
//					delay(1000);//��PLCû��׼���õ�����£�ÿ1��ѯ��һ��
//					//Serial.println("PLC ready?");
//				}
//
//				//Serial.println("PLC Ready");
//				//PLC����
//				plch.send(current.driver.code, current.trunk.code, volumn);
//
//				String cmd;// = "0102050051000000";
//				do{
//					delay(1000);
//					cmd = plch.readCommand();
//				} while (!cmd.startsWith("010205"));//���������PLC��ָ����ԡ�010205����ͷ�ģ���һֱ�ȴ�
//
//				//��ˮ���
//				int kind;
//				int potency;
//				if (plch.getVolumnPotencyKind(cmd, &volumn, &potency, &kind)){
//					/*Serial.print("plate = ");
//					Serial.println(current.trunk.code);
//					Serial.print("code = ");
//					Serial.println(current.driver.code);
//					Serial.print("volumn = ");
//					Serial.println(volumn);
//					Serial.print("potency = ");
//					Serial.println(potency);
//					Serial.print("kind = ");
//					Serial.println(kind);*/
//
//					int saveCount = 0;
//					int saved = 0;
//
//					do{
//						openIP();
//
//						//����ص���Ϣ�洢���������������ύ5��
//						saved = simh.save(DEVICE_CODE, current.driver.code, current.trunk.code, volumn, potency, kind);
//						//Serial.print("Saved ");
//						//Serial.println(saveCount);
//						
//						closeIP();
//
//						if (saved==Http_Save_Status_Success){
//							break;
//						}
//					} while (saveCount++ < 5);
//
//					if (saved==Http_Save_Status_Success){
//						plch.saveSuccess();
//						//Serial.println("Save Success");
//					}
//					else{
//						plch.saveError();
//
//						simh.sendShortMessage("NotSaved.Info:" + current.driver.code + "," + current.trunk.code + ".SaveStatus:" + saved + ".");
//						//Serial.println("Save Error");
//					}
//				}
//
//				//���һ�μ�ˮ���̣��ȵ�10��
//				delay(10000);
//			
//				last.driver.code = current.driver.code;
//				last.trunk.code = current.trunk.code;
//				last.time = current.time;
//			}
//		}
//	}
//}

//�¶��������Ͼ���
void loop()
{
	/*
	�������̣�
	1���������������������ز���1��
	2�����˿��������������ز���1��
	3��ѯ��PLC�Ƿ�׼���á�������PLC�ظ������ݡ�
	��1������������ݴ����յ�PLC���صĴ�����ʾ��Ϣ�����򷵻ز���3��
	��2���������������ȷ����
	�������æ���򷵻ز���3��
	�������PLC���У����ͳ��ƺš���Ա��š��������*10��
	4���ȴ�����PLC��ɼ�ˮ�������־�������յ�������־��ͨ��SIM900A���豸��š����ƺš���Ա��š���ˮ�����Ũ�ȡ����ͷ��͵���������
	5�����ز���1
	*/

	cards = crh.readCards();
	//delay(3000);
	//Serial.println("read card");
	//Serial.println(cards);
	if (cards != "") {
		current.trunk.card = cards.substring(0, cards.indexOf(","));//"544B000A4538323736320058";
		current.driver.card = cards.substring(cards.indexOf(",") + 1);//"5050000830303031";

		//Serial.println(current.trunk.card);
		//Serial.println(current.driver.card);

		//�������˿����������ˣ��ѳ��ƺź���Ա��ŷ��͸�PLC
		current.trunk.code = crh.getPlate(current.trunk.card);
		current.driver.code = crh.getCode(current.driver.card);
		float volumn = crh.getVolumn(current.trunk.card);

		//ֻ������һ���������ܼ�������
		//1�������Ŀ���һ��
		//2�����������⿨
		//3��ͬһ�ſ���������5����
		current.time = millis();
		if (
			(current.driver.code + current.trunk.code == "0000E00000") ||
			(current.driver.code + current.trunk.code != last.driver.code + last.trunk.code) ||
			(current.time - last.time >= 300000)) {
			
			//��PLCû��׼���õ�����£�ÿ1��ѯ��һ��
			while (!plch.isReady()) {
				delay(1000);
			}

			//PLC����
			plch.send(current.driver.code, current.trunk.code, volumn);

			String cmd;// = "0102050051000000";
			do {
				delay(1000);
				cmd = plch.readCommand();
			} while (!cmd.startsWith("010205"));//���������PLC��ָ����ԡ�010205����ͷ�ģ���һֱ�ȴ�

			//��ˮ���
			int kind;
			int potency;
			if (plch.getVolumnPotencyKind(cmd, &volumn, &potency, &kind)) {
				//int saveCount = 0;
				//int saved = 0;

				//do {
				//	openIP();

				//	//����ص���Ϣ�洢���������������ύ5��
				//	saved = simh.save(DEVICE_CODE, current.driver.code, current.trunk.code, volumn, potency, kind);

				//	closeIP();

				//	if (saved == Http_Save_Status_Success) {
				//		break;
				//	}
				//} while (saveCount++ < 5);

				//if (saved == Http_Save_Status_Success) {
				//	plch.saveSuccess();
				//}
				//else {
				//	plch.saveError();

				//	simh.sendShortMessage("NotSaved.Info:" + current.driver.code + "," + current.trunk.code + ".SaveStatus:" + saved + ".");
				//}
			}

			//���һ�μ�ˮ���̣��ȵ�10��
			delay(10000);

			last.driver.code = current.driver.code;
			last.trunk.code = current.trunk.code;
			last.time = current.time;
		}
	}
}