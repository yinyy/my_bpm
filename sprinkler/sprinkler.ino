/*��������ʱʹ��19200��SIM900A������Ӧ�ģ� ������ʱ��������
Arduino		TX1		RX1		TX3		RX3		TX2		RX2
SIM900A		SRXD	STXD
PLC							m		n					
5815
*/

/*����ϵͳ����*/
//SIM900A��������
#include <JR5815.h>
#include <CardHelper.h>
#include <SIM900A.h>
#include <Led.h>

//SIM900A����
#define SIM900A_ERROR					"info_SIM900A_Error"
//SIM900AIP���ô���	
#define SIM900A_IP_ERROR				"info_SIM900A_IP_Error"
//SIM900A��������
#define SIM900A_CONNECTING_NETWORK		"info_SIM900A_Connecting_Network"
//SIM900A�źŲ���
#define SIM900A_SINGAL_QUALITY_WEAK		"info_SIM900A_SINGAL_WEAK"
//SIM900Aװ������
#define SIM900A_READY					"info_SIM900A_Ready"
//�����������ڴ���
#define ERROR_DATE						"error_date"
//���������Ŵ���
#define ERROR_DISPATCH					"error_dispatch"
//������������
#define ERROR_OTHER						"error_other"
//���峵������
#define ERROR_TRUNK						"error_trunk"
//������Ա����
#define ERROR_DRIVER					"error_driver"
//���屣������ɹ�
#define SUCCESS_SAVE					"success_save"



//�ź���������Сֵ
#define SINGAL_QUALITY					5
//����PLCʹ�õĴ���
#define PLC								Serial3
//�������ʹ�õĶ˿�
#define CONSOLE							Serial
//����Ĭ�ϵĲ�����
#define DEFAULT_BAUD					115200
//����ѭ��������ʱ������Ĭ��5���һ�ο�
#define DEFAULT_READ_INTERVAL			5000


//����PLC���Ƿ���Խ������ݵĹܽ�
#define PLC_ALLOW_DATA_PIN				35


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
	/*��ִ�иߵ�ƽ�Ĳ�����ʹ��SIM900Aģ�����������ڲ��ܱ�֤ģ��ɹ�����*/
	//�ӳ�
	delay(0);

	//Arduino׼��
	CONSOLE.begin(DEFAULT_BAUD);
	PLC.begin(DEFAULT_BAUD);
	Serial1.begin(DEFAULT_BAUD);//SIM900A
	Serial2.begin(DEFAULT_BAUD);//JR5815
	
	while (!Serial1);
	while (!Serial2);
	while (!CONSOLE);
	while (!PLC);
	
	//��Serial1����SIM900A
	sim.init(&Serial1, true);
	//��Serial2���ƾ���5815
	reader.init(&Serial2, true);

	//�ж�SIM900A�Ƿ�׼������
	if (!sim.isReady()){
		PLC.println(SIM900A_ERROR);
		CONSOLE.println(SIM900A_ERROR);

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

		//SIM900A�ź�̫�����޷���������
		while (true);
	}
	CONSOLE.println("SIM900A Singal Regular:" + String(sq));
	
	//��IPӦ��
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
			//Context_Status_Closing����Context_Status_Connectingʱ���ȴ����ɡ�
			;
		}

		delay(1000);
	} while (1);
	 
	//SIM900A���빤��״̬
	PLC.println(SIM900A_READY);
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

			String url = SERVER_URL + "action=get&dispatchId=" + dispatchId;
			CONSOLE.println("Accessing " + url);

			String value = sim.sendHttpRequest(1, url);
			CONSOLE.println(value);
			if (value.startsWith("success_")){
				//��ȡ�㹻����Ϣ������֤
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
					//���������Ա������õ�����Ա��һ��
					PLC.println(ERROR_DRIVER);
				}
				else if (String(trunkId) != sTrunkId){
					//������ĳ���������õ��ĳ�����һ��
					PLC.println(ERROR_TRUNK);
				}
				else{
					//ͨ������֤������Ҫ����Ϣ���͸�PLC
					String msg = "work_" + sDriverId + "," + driverCode + "," + sTrunkId + "," + trunkPlate + "," + sVolumn + "," + sWorkload + "," + sFinished;
					PLC.println(msg);
				}
			}
			else if (value == "error_date"){
				//���ǵ�ǰ����
				PLC.println(ERROR_DATE);
			}
			else if (value == "error_dispatch"){
				//û����Ӧ�ĵ�������
				PLC.println(ERROR_DISPATCH);
			}
			else{
				//��������
				PLC.println(ERROR_OTHER);
			}
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

			String url = SERVER_URL + "action=save&dispatchId=" + dispatchId + "&driverId=" + driverId + "&trunkId=" + trunkId + "&address=" + DEVICE_CODE + "&volumn=" + volumn;
			CONSOLE.println("Accessing " + url);

			String value = sim.sendHttpRequest(1, url);
			CONSOLE.println(value);
			if (value.startsWith("success_save")){
				//�ɹ����湤������֪ͨPLC
				PLC.println(SUCCESS_SAVE);
				CONSOLE.println(SUCCESS_SAVE);
			}
			else{
				//��������
				PLC.println(ERROR_OTHER);
				CONSOLE.println(ERROR_OTHER);
			}
		}
	}
}