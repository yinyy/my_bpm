// 
// 
// 

#include "SimHelper.h"

//清空900A的数据
void clear(){
	while (Serial1.available() > 0){
		Serial1.read();
	}
}

//读取900A的数据
String read(){
	String msg = "";
	long last_time;
	int x;

	//如果1秒钟内没有读到数据，则结束返回
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

Http_Save_Status SimHelperClass::save(String deviceCode, String driverCode, String trunkPlate, float volumn, int potency, int kind, int contextId){
	String url = SERVER_URL + "action=save&timespan=" + String(millis(), HEX) + "&address=" + deviceCode + "&volumn=" + volumn + "&kind=" + String(kind) + "&driver=" + driverCode + "&trunk=" + trunkPlate + "&potency="+String(potency);
	//Serial.println(url);

	//发送HTTP请求
	clear();

	String cid = String(contextId);

	//先结束HTTP服务
	Serial1.print("AT+HTTPTERM\r");
	Serial1.flush();

	String msg = read();
	//不判断HTTPTERM操作是否成功。
	delay(500);

	//初始化HTTP
	Serial1.print("AT+HTTPINIT\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return Http_Save_Status_INIT;
	}

	//设置HTTP请求参数CID
	Serial1.print("AT+HTTPPARA=\"CID\", \"" + cid + "\"\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return Http_Save_Status_HTTPPARA_CID;
	}

	//设置HTTP请求参数URL
	Serial1.print("AT+HTTPPARA=\"URL\", \"" + url + "\"\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return Http_Save_Status_HTTPPARA_URL;
	}

	//激活HTTP请求
	Serial1.print("AT+HTTPACTION=0\r");
	Serial1.flush();

	msg = read();
	if (msg != "OK"){
		return Http_Save_Status_HTTPACTION;
	}

	//Serial.println("Maybe error");
	//等到HTTP返回的结果，最多延时60秒钟
	long last_time = millis();
	while (!(msg = read()).startsWith("+HTTPACTION") && millis() - last_time < 60000){
		//Serial.println("READED:"+msg);
		delay(1000);
	};

	//Serial.println("5:" + msg);
	if (!msg.startsWith("+HTTPACTION")){
		return Http_Save_Status_HTTPACTION2;
	}

	String sc = msg.substring(msg.indexOf(',') + 1, msg.lastIndexOf(','));
	//Serial.println("6:" + sc);
	if (sc != "200"){
		return Http_Save_Status_HTTPACTION_200;
	}

	//可以读取数据了
	int len = msg.substring(msg.lastIndexOf(',') + 1).toInt();
	Serial1.print("AT+HTTPREAD=0," + String(len) + '\r');
	Serial1.flush();
	 
	//等到返回的结果，最多延时20秒钟
	last_time = millis();
	while (!(msg = read()).startsWith("+HTTPREAD") && (millis() - last_time < 60000)){
		//Serial.println("READED:" + msg);
		delay(1000);
	}

	if (msg.startsWith("+HTTPREAD") && msg.endsWith("OK")){
		//Serial.println(msg);

		//去掉开头和结尾的字符
		String ans = msg.substring(msg.indexOf('\n') + 1, msg.lastIndexOf('\r'));
		ans.trim();
	}

	return Http_Save_Status_Success;
}

void SimHelperClass::sendShortMessage(String message, String phone) {
	clear();

	//设置短消息为文本模式
	Serial1.print("AT+CMGF=1\r");
	Serial1.flush();
	delay(500);
	clear();

	//设置字符集
	Serial1.print("AT+CSCS=\"GSM\"\r");
	Serial1.flush();
	delay(500);
	clear();

	//发送消息
	Serial1.print("AT+CMGS=\"" + phone + "\"\r");
	Serial1.flush();
	delay(500);//等到出现">"

	Serial1.print(message);
	Serial1.flush();
	delay(500);
	Serial1.write(0x1A);//结束信息并发送
	//Serial1.write(0x1B);//结束信息并取消
	Serial1.flush();
	delay(500);

	clear();
}

String SimHelperClass::status() {
	String message = "";

	clear();
	switch (checkContextStatus()) {
	case Context_Status_Closed:
		message = "closed,";
		break;
	case Context_Status_Closing:
		message = "closing";
		break;
	case Context_Status_Connected:
		message = "connected";
		break;
	case Context_Status_Connecting:
		message = "connecting";
		break;
	case Context_Status_Error:
		message = "error";
		break;
	default:
		message = "unknown";
		break;
	}


	return message;
}

SimHelperClass SimHelper;

