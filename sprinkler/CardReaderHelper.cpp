// 
// 
// 

#include "CardReaderHelper.h"

void send(unsigned char* command, uint8_t len){
	while (Serial3.available() > 0){
		Serial3.read();
	}

	//发送读取EPC的指令
	for (int i = 0; i < len; i++){
		Serial3.write(command[i]);
	}
	Serial3.flush();
}

String readData(){
	String msg = "";
	long _lastTime = millis();

	//等到指令的结果，20毫秒没有收到即退出
	while ((Serial3.available() == 0) && (millis() - _lastTime < 20));
	while (Serial3.available() > 0){
		String d = String(Serial3.read(), HEX);

		if (d.length() == 1){
			msg.concat("0");
		}

		msg.concat(d);

		//delay(5);
	}
	msg.toUpperCase();

	return msg;
}

//识别数据中的EPC号
String getEpcs(String data, bool is485){
	//参考数据：F011EE010106544B000745313233343500001F
	//index:    01234567890123456789012345678901234567
	//                    1         2         3
	String epcs;
	String tmp = data;
	char ch[2];

	ch[0] = tmp.charAt(2);
	ch[1] = tmp.charAt(3);

	//有效数据的总长度
	uint8_t len = (uint8_t)strtol(ch, NULL, 16);

	if (is485){
		//RS485设备的地址
		String rs485 = tmp.substring(6, 8);
		tmp = tmp.substring(8);
	}
	else{
		tmp = tmp.substring(6);
	}

	ch[0] = tmp.charAt(0);
	ch[1] = tmp.charAt(1);

	//读到的EPC数据的数量
	uint8_t count = (uint8_t)strtol(ch, NULL, 16);
	
	String epc = "";
	int index = 2;
	uint8_t epclen;

	//依次读出所有的EPC
	for (int i = 0; i < count; i++){
		ch[0] = tmp.charAt(index++);
		ch[1] = tmp.charAt(index++);

		epclen = (uint8_t)(strtol(ch, NULL, 16) * 4);
		epc = tmp.substring(index, index + epclen);

		index += epclen;

		epcs.concat(epc);
		epcs.concat(',');
	}

	return epcs;
}

String readCard(unsigned char* command, unsigned char len){
	send(command, len);

	String data = readData();
	//正确的数据应该是：F011EE010106544B000745313233343500001F
	if (data.startsWith("F0")){
		return getEpcs(data, true);
	}
	//错误的数据应该是：F404EE010217
	else if (data.startsWith("F4")){
		return "";
	}
	else{
		return "";
	}
}

String readCard2(unsigned char* command, unsigned char len){
	send(command, len);

	String data = readData();
	data = data.substring(2);
	//正确的数据应该是：xx0201xx02
	if (data.startsWith("0201")){//这是02设备对01命令的相应
		data = data.substring(6);
		
		char cs[2];
		data.substring(0, 2).toCharArray(cs, 2);
		long count = strtol(cs, NULL, 16);

		data = data.substring(2);
		String epcs = "";
		for (int i = 0; i < count; i++){
			data.substring(0, 2).toCharArray(cs, 2);
			long len = strtol(cs, NULL, 16);

			epcs.concat(data.substring(2, 2 + len * 2));
			epcs.concat(",");

			data = data.substring(2 + len * 2);
		}
		
		return epcs;
	}
	else{
		return "";
	}
}

//识别人员的编号
String CardReaderHelperClass::getCode(String card){
	String code = "";

	char cs[2];
	cs[0] = card.charAt(8);
	cs[1] = card.charAt(9);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(10);
	cs[1] = card.charAt(11);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(12);
	cs[1] = card.charAt(13);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(14);
	cs[1] = card.charAt(15);
	code.concat((char)strtol(cs, NULL, 16));

	return code;
}

//识别车辆的车牌号
String CardReaderHelperClass::getPlate(String card){
	String code = "";

	char cs[2];
	cs[0] = card.charAt(8);
	cs[1] = card.charAt(9);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(10);
	cs[1] = card.charAt(11);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(12);
	cs[1] = card.charAt(13);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(14);
	cs[1] = card.charAt(15);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(16);
	cs[1] = card.charAt(17);
	code.concat((char)strtol(cs, NULL, 16));

	cs[0] = card.charAt(18);
	cs[1] = card.charAt(19);
	code.concat((char)strtol(cs, NULL, 16));

	return code;
}

//识别车辆容积
float CardReaderHelperClass::getVolumn(String card){
	char cs[4];
	cs[0] = card.charAt(20);
	cs[1] = card.charAt(21);
	cs[2] = card.charAt(22);
	cs[3] = card.charAt(23);
	return strtol(cs, NULL, 16) / 10.0f;
}

//读车辆卡
String CardReaderHelperClass::readTrunkCard(){
	//							Head  LEN   CMD   ADDR  DATA                                CHK
	//													MEM	  ADDR  ADDR  LEN   MASK
	unsigned char command[] = { 0xAA, 0x09, 0xEE, 0x01, 0x01, 0x00, 0x00, 0x10, 0x54, 0x4B, 0xAE };
	unsigned char len = 11;

	String epcs = readCard(command, len);
	return epcs.substring(0, epcs.indexOf(','));//对于多辆车，只取识别到的第一辆
}

//读人员卡
String CardReaderHelperClass::readDriverCard(){
	unsigned char command[] = { 0xAA, 0x09, 0xEE, 0x01, 0x01, 0x00, 0x00, 0x10, 0x50, 0x50, 0xAD };
	unsigned char len = 11;

	String epcs = readCard(command, len);
	return epcs.substring(0, epcs.indexOf(','));//对于多个人，只取识别到的第一个人
	//F0 11 EE 01 01 06 50 50 00 00 00 00 00 00 00 00 00 00 00 0D
}

boolean CardReaderHelperClass::getCard(String* tc, String* pc){
	*tc = "";
	*pc = "";

	unsigned char command[] = { 0x04, 0x02, 0x01, 0x6B, 0x78 };
	unsigned char len = 5;

	String epcs = readCard2(command, len);
	int idx1 = epcs.indexOf(',');
	if (idx1 == -1){
		return false;
	}

	int idx2 = epcs.indexOf(',', idx1);
	if (idx2 == -1){
		return false;
	}

	String t = epcs.substring(0, idx1);
	if (t.startsWith("5050")){
		*pc = t;
	}
	else if (t.startsWith("544B")){
		*tc = t;
	}

	t = epcs.substring(idx1 + 1, idx2);
	if (t.startsWith("5050")){
		*pc = t;
	}
	else if (t.startsWith("544B")){
		*tc = t;
	}

	if (*tc == "" || *pc == ""){
		return false;
	}

	return true;
}

CardReaderHelperClass CardReaderHelper;