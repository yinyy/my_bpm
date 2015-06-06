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
#define DEFAULT_BAUD_SIM				115200
//����PLCĬ�ϵĲ�����
#define DEFAULT_BAUD_CARD_READER		115200
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

CardReaderHelperClass crh;
PlcHelperClass plch;
SimHelperClass simh;

Info_Struct current, last;

void setup()
{
	current.driver.card = "";
	current.driver.code = "";
	current.time = 0;

	last.driver.card = "";
	last.driver.code = "";
	last.time = 0;

	//Arduino׼��
	//Serial.begin(9600);
	Serial1.begin(DEFAULT_BAUD_SIM);//SIM900A
	Serial2.begin(DEFAULT_BAUD_PLC);//PLC
	Serial3.begin(DEFAULT_BAUD_CARD_READER);//JR5815

	while (!Serial1);
	while (!Serial2);
	while (!Serial3); 
	//while (!Serial);

	simh.init();
	delay(1000);
	
	int count = 0;

	//�ж�SIM900A�Ƿ�׼������
	while (!simh.isReady()){
		//Serial.println("SIM900A_ERROR");

		//SIM900Aģ���޷�����������ֹͣ
		delay(1000);

		if (count++ > 5){
			break;
		}
	}
	//Serial.println("SIM900A AT Ready.");

	//����ź�״̬
	uint8_t sq = simh.checkSignal();
	//Serial.println("SIM900A Singal Regular:" + String(sq));
	//����ź�����С��n�����ж��޷�����ź�

	count = 0;
	while (sq < 5 || sq == 99){
		//Serial.println("Singal Low!");
		
		//SIM900A�ź�̫�����޷���������
		delay(1000);

		sq = simh.checkSignal();
		//Serial.println("SIM900A Singal Regular:" + String(sq));

		if (count++ > 5){
			break;
		}
	}
	
	//��IPӦ��
	count = 0;
	do{
		//Serial.println("SIM900A_CONNECTING_NETWORK");

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

		if (count++ > 5){
			break;
		}
	} while (1);
	 
	//SIM900A���빤��״̬
	//Serial.println("SIM900A IP Context 1 Ready.");

	//current.driver.code = "0002";
	//current.trunk.code = "E82762";
}

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

	delay(1000);
	current.trunk.card = crh.readTrunkCard();
	if (current.trunk.card != ""){
		current.driver.card = crh.readDriverCard();
		if (current.driver.card != ""){
			current.time = millis();

			//�������˿����������ˣ��ѳ��ƺź���Ա��ŷ��͸�PLC
			current.trunk.code = crh.getPlate(current.trunk.card);
			current.driver.code = crh.getCode(current.driver.card);
			float volumn = crh.getVolumn(current.trunk.card);

			//ֻ������һ���������ܼ�������
			//1�������Ŀ���һ��
			//2�����������⿨
			//3��ͬһ�ſ���������5����
			if (
				(current.driver.code + current.trunk.code == "0000E00000") ||
				(current.driver.code + current.trunk.code != last.driver.code + last.trunk.code) ||
				(current.time - last.time >= 300000)){

				while (!plch.isReady()){
					delay(1000);//��PLCû��׼���õ�����£�ÿ1��ѯ��һ��
					//Serial.println("PLC ready?");
				}

				//Serial.println("PLC Ready");
				//PLC����
				plch.send(current.driver.code, current.trunk.code, volumn);

				String cmd;
				do{
					delay(1000);
					cmd = plch.readCommand();
				} while (!cmd.startsWith("010205"));//���������PLC��ָ����ԡ�010205����ͷ�ģ���һֱ�ȴ�

				//��ˮ���
				int kind;
				int potency;
				if (plch.getVolumnPotencyKind(cmd, &volumn, &potency, &kind)){
					//����ص���Ϣ�洢��������
					if (!simh.save(DEVICE_CODE, current.driver.code, current.trunk.code, volumn, potency, kind)){
						plch.saveError();
						//Serial.println("Save Error");
					}
					else{
						plch.saveSuccess();
						//Serial.println("Save Success");
					}
				}

				//���һ�μ�ˮ���̣��ȵ�30��
				delay(30000);

				last.driver.code = current.driver.code;
				last.trunk.code = current.trunk.code;
				last.time = current.time;
			}
		}
	}
}