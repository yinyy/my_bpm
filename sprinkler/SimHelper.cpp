// 
// 
// 

#include "SimHelper.h"

//���900A������
void clear(){
	while (Serial1.available() > 0){
		Serial1.read();
	}
}

//��ȡ900A������
String read(){
	String msg = "";
	long last_time;
	int x;

	//���1������û�ж������ݣ����������
	last_time = millis();
	do{
		x = Serial1.read();

		if (x > 0){
			msg.concat((char)x);
			last_time = millis();
		}
	} while (millis() - last_time <= 1000);

	msg.trim();
	return msg;
}

void SimHelperClass::init(){
	Serial1.print("ATE0\r");
	Serial1.flush();
}

boolean SimHelperClass::isReady()
{
	clear();

	Serial1.print("AT\r");
	Serial1.flush();

	String msg = read();
	if (msg == "OK"){
		return true;
	}
	else{
		return false;
	}
}

uint8_t SimHelperClass::checkSignal(){
	clear();

	Serial1.print("AT+CSQ\r");
	Serial1.flush();

	uint8_t sq = 99;
	String msg = read();
	if (msg.endsWith("OK")){
		msg = msg.substring(msg.indexOf(':') + 1, msg.indexOf(','));
		msg.trim();

		sq = msg.toInt();
	}

	return sq;
}

Context_Status SimHelperClass::checkContextStatus(uint8_t contextId){
	clear();

	String cid = String(contextId);
	Serial1.print("AT+SAPBR=2," + cid + "\r");
	Serial1.flush();

	String msg = read();
	if (msg.endsWith("OK")){
		msg = msg.substring(msg.indexOf(',') + 1, msg.indexOf(',', msg.indexOf(',') + 1));
	}

	if (msg == "0"){
		return Context_Status_Connecting;
	}
	else if (msg == "1"){
		return Context_Status_Connected;
	}
	else if (msg == "2"){
		return Context_Status_Closing;
	}
	else if (msg == "3"){
		return Context_Status_Closed;
	}
	else {
		return Context_Status_Error;
	}
}

boolean SimHelperClass::openContext(uint8_t contextId){
	clear();

	String cid = String(contextId);

	Serial1.print("AT+SAPBR=1," + cid + "\r");
	Serial1.flush();

	String msg = read();
	if (msg == "OK"){
		return true;
	}
	else {
		return false;
	}
}

boolean SimHelperClass::save(String deviceCode, String driverCode, String trunkPlate, float volumn, int potency, int kind, int contextId){
	String url = SERVER_URL + "action=save&timespan=" + String(millis(), HEX) + "&address=" + deviceCode + "&volumn=" + volumn + "&kind=" + String(kind) + "&driver=" + driverCode + "&trunk=" + trunkPlate + "&potency="+String(potency);
	//Serial.println(url);

	//����HTTP����
	clear();

	String cid = String(contextId);

	//�Ƚ���HTTP����
	Serial1.print("AT+HTTPTERM\r");
	Serial1.flush();

	String msg = read();
	//���ж�HTTPTERM�����Ƿ�ɹ���
	delay(500);

	//��ʼ��HTTP
	Serial1.print("AT+HTTPINIT\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return false;
	}

	//����HTTP�������CID
	Serial1.print("AT+HTTPPARA=\"CID\", \"" + cid + "\"\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return false;
	}

	//����HTTP�������URL
	Serial1.print("AT+HTTPPARA=\"URL\", \"" + url + "\"\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return false;
	}

	//����HTTP����
	Serial1.print("AT+HTTPACTION=0\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return false;
	}

	//Serial.println("Maybe error");
	//�ȵ�HTTP���صĽ���������ʱ60����
	long last_time = millis();
	while (!(msg = read()).startsWith("+HTTPACTION") && millis() - last_time < 60000){
		//Serial.println("READED:"+msg);
		delay(1000);
	};

	//Serial.println("5:" + msg);
	if (!msg.startsWith("+HTTPACTION")){
		return false;
	}

	String sc = msg.substring(msg.indexOf(',') + 1, msg.lastIndexOf(','));
	//Serial.println("6:" + sc);
	if (sc != "200"){
		return false;
	}

	//���Զ�ȡ������
	int len = msg.substring(msg.lastIndexOf(',') + 1).toInt();
	Serial1.print("AT+HTTPREAD=0," + String(len) + '\r');
	Serial1.flush();
	 
	//�ȵ����صĽ���������ʱ20����
	last_time = millis();
	while (!(msg = read()).startsWith("+HTTPREAD") && (millis() - last_time < 60000)){
		//Serial.println("READED:" + msg);
		delay(1000);
	}

	if (msg.startsWith("+HTTPREAD") && msg.endsWith("OK")){
		//Serial.println(msg);

		//ȥ����ͷ�ͽ�β���ַ�
		String ans = msg.substring(msg.indexOf('\n') + 1, msg.lastIndexOf('\r'));
		ans.trim();
	}

	return true;
}

void SimHelperClass::sendShortMessage(String message, String phone) {

}

SimHelperClass SimHelper;

