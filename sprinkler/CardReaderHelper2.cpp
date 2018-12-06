// 
// 
// 

#include "CardReaderHelper2.h"

//识别人员的编号
String CardReaderHelper2Class::getCode(String card) {
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
String CardReaderHelper2Class::getPlate(String card) {
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
float CardReaderHelper2Class::getVolumn(String card) {
	char cs[4];
	cs[0] = card.charAt(20);
	cs[1] = card.charAt(21);
	cs[2] = card.charAt(22);
	cs[3] = card.charAt(23);
	return strtol(cs, NULL, 16) / 10.0f;
}

//识别卡
String CardReaderHelper2Class::readCards() {
	unsigned char command[] = { 0x04, 0x02, 0x01, 0x6b, 0x78 };
	int len;

	while (Serial3.available() > 0) {
		Serial3.read();
	}

	//发送指令
	for (int i = 0; i < 5; i++) {
		Serial3.write(command[i]);
	}
	Serial3.flush();

	String cards = "";
	byte buffer[1024];
	//long _lastTime = millis();

	delay(300);

	//延时等待指令结束，与设备设置有关，默认查询指令的时间阈值是1s
	//while (((len = Serial3.available()) == 0) && (millis() - _lastTime < 1000));
	if (Serial3.available() > 0) {
		Serial3.readBytes(buffer, Serial3.available());
	}

	/*for (int i = 0;i < buffer[0];i++) {
		Serial.write(buffer[i]);
	}*/

	//		地址01					命令01					状态01				数据02
	if ((buffer[1] == 0x02) && (buffer[2] == 0x01) && (buffer[3] == 0x01) && (buffer[4] == 0x02)) {
		String t;

		int len1 = buffer[5];
		String card1 = "";
		for (int i = 6;i < 6 + len1;i++) {
			t = String(buffer[i], HEX);
			if (t.length() == 1) {
				card1.concat("0");
			}
			card1.concat(t);
		}
		card1.toUpperCase();

		int len2 = buffer[6 + len1];
		String card2 = "";
		for (int i = 6 + len1 + 1;i < 6 + len1 + 1 + len2;i++) {
			t = String(buffer[i], HEX);
			if (t.length() == 1) {
				card2.concat("0");
			}
			card2.concat(t);
		}
		card2.toUpperCase();

		if (card1.startsWith("544B")&&card2.startsWith("5050")) {
			cards.concat(card1);
			cards.concat(",");
			cards.concat(card2);
		}
		else if (card2.startsWith("544B") && card1.startsWith("5050")) {
			cards.concat(card2);
			cards.concat(",");
			cards.concat(card1);
		}
	}

	return cards;
}

CardReaderHelper2Class CardReaderHelper2;

