/*��������ʱʹ��19200��SIM900A������Ӧ�ģ� ������ʱ��������
Arduino		TX1		RX1		TX3		RX3		TX2		RX2
SIM900A		SRXD	STXD
PLC							m		n					
5815
*/

#include "SimHelper.h"
#include "PlcHelper.h"
#include "CardReaderHelper.h"

//�ź���������Сֵ
#define SINGAL_QUALITY					5
//����Ĭ�ϵĲ�����
#define DEFAULT_BAUD					115200
//����PLCĬ�ϵĲ�����
#define DEFAULT_BAUD_PLC				9600
//����SIM900A��PWR_KEY
#define SIM900A_PWR_KEY_PIN				36
//�豸���
#define DEVICE_CODE "A002"


//��¼��һ�ζ�����ʱ��
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
	//�ȴ�SIM900A�ӵ�
	pinMode(SIM900A_PWR_KEY_PIN, OUTPUT);
	digitalWrite(SIM900A_PWR_KEY_PIN, HIGH);
	delay(500);

	//�ӳ�5�룬ģ�ⰴ��������SIM900Aģ��
	digitalWrite(SIM900A_PWR_KEY_PIN, LOW);
	delay(5000);

	//Arduino׼��
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
	
	//�ж�SIM900A�Ƿ�׼������
	if (!simh.isReady()){
		Serial.println("SIM900A_ERROR");

		//SIM900Aģ���޷�����������ֹͣ
		while (true);
	}
	Serial.println("SIM900A AT Ready.");

	//����ź�״̬
	uint8_t sq = simh.checkSignal();
	Serial.println("SIM900A Singal Regular:" + String(sq));
	//����ź�����С��n�����ж��޷�����ź�
	if (sq < 5 || sq == 99){
		Serial.println("Singal Low!");
		
		//SIM900A�ź�̫�����޷���������
		while (true);
	}
	
	//��IPӦ��
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
			;//Context_Status_Closing����Context_Status_Connectingʱ���ȴ����ɡ�
		}

		delay(1000);
	} while (1);
	 
	//SIM900A���빤��״̬
	Serial.println("SIM900A IP Context 1 Ready.");
}

void loop()
{
	String plcCommand = plch.readCommand();
	if (plcCommand.startsWith("02010101")){
		//�Ƿ���Զ���
		canReadCard = true;
	}
	else if (plcCommand.startsWith("02010100")){
		canReadCard = false;
	}
	else if (plcCommand.startsWith("020303")){
		//��ˮ��ɣ�������Ϣ
		float volumn;
		int kind;

		if (plch.getVolumnAndKind(plcCommand, &volumn, &kind)){
			//����ص���Ϣ�洢��������
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
				//�������˿����������ˣ��ѳ��ƺź���Ա��ŷ��͸�PLC
				trunkPlate = crh.getPlate(trunkCard);
				driverCode = crh.getCode(driverCard);
				float volumn = crh.getVolumn(trunkCard);

				plch.send(driverCode, trunkPlate, volumn);

				canReadCard = false;
			}
		}
	}
}