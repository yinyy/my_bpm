// 
// 
// 

#include "PlcHelper.h"

//低位在前，高位在后
word crc(byte* bs, uint8_t len){
	word value = 0xffff;

	for (int j = 0; j < len; j++){
		value = value^ bs[j];

		for (int i = 0; i < 8; i++){
			if (value & 0x0001 == 0x0001){
				value = value >> 1;
				value = value ^ 0xa001;
			}
			else{
				value = value >> 1;
			}
		}
	}

	return value;
	/*Serial.println(crc & 0xff, HEX);
	Serial.println((crc & 0xff00) >> 8, HEX);*/
}

void write(byte* bs, int len){
	word value = crc(bs, len);
	byte crc1 = (byte)(value & 0xff);
	byte crc2 = (byte)(value >> 8);

	/*for (int i = 0; i < len; i++){
		Serial.print(bs[i], 16);
		Serial.print(" ");
	}
*/
	/*Serial.print(crc1, 16);
	Serial.print(" ");
	Serial.println(crc2, 16);*/

	Serial2.write(bs, len);
	Serial2.write(crc1);
	Serial2.write(crc2);
}


void PlcHelperClass::send(String code, String plate, float volumn){
	/*Serial.print(code);
	Serial.print(",");
	Serial.print(plate);
	Serial.print(",");
	Serial.println(volumn);*/

	byte bs[15] = { 0x02, 0x02, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
	for (int i = 0; i < code.length(); i++){
		char c = code.charAt(i);
		bs[3 + i] = (byte)c;
	}

	for (int i = 0; i < plate.length(); i++){
		char c = plate.charAt(i);
		bs[7 + i] = (byte)c;
	}

	bs[13] = (((int)(volumn * 10)) >> 8) & 0xff;
	bs[14] = ((int)(volumn * 10)) & 0xff;

	write(bs, 15);
}

String PlcHelperClass::readCommand(){
	String msg;

	long last_time;
	int x;
	String t;

	//如果1秒钟内没有读到数据，则结束返回
	last_time = millis();
	do{
		x = Serial2.read();

		if (x >= 0){
			t = String(x, HEX);

			if (t.length() == 1){
				msg.concat("0");
			}
			msg.concat(t);

			last_time = millis();
		}
	} while (millis() - last_time <= 1000);

	msg.trim();
	msg.toUpperCase();
	return msg;
}

boolean PlcHelperClass::getVolumnPotencyKind(String command, float* volumn, int* potency, int* kind){
	if (command.startsWith("010205")){
		char ch[4];
		ch[0] = command.charAt(6);
		ch[1] = command.charAt(7);
		ch[2] = command.charAt(8);
		ch[3] = command.charAt(9);
		*volumn = strtol(ch, NULL, 16) / 10.0f;
		
		ch[0] = command.charAt(10);
		ch[1] = command.charAt(11);
		ch[2] = command.charAt(12);
		ch[3] = command.charAt(13);
		*potency = (int)strtol(ch, NULL, 16);

		char ch2[2];
		ch2[0] = command.charAt(14);
		ch2[1] = command.charAt(15);
		*kind = (int)strtol(ch2, NULL, 16);

		return true;
	}
	else{
		return false;
	}
}

void PlcHelperClass::saveError(){
	byte bs[4] = { 0x02, 0x03, 0x01, 0x01 };
	write(bs, 4);
}

void PlcHelperClass::saveSuccess(){
	byte bs[4] = { 0x02, 0x03, 0x01, 0x00 };
	write(bs, 4);
}

boolean PlcHelperClass::isReady(){
	byte bs[4] = { 0x02, 0x01, 0x01, 0xFF }; 
	write(bs, 4);
	
	delay(1000);

	String msg = readCommand();

	//Serial.print("PLC readed");
	//Serial.println(msg);

	if (msg.startsWith("010101FF")){
		return true;
	}
	else{
		return false;
	}
}

PlcHelperClass PlcHelper;

