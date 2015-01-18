// 
// 
// 

#include "Plc.h"

void PlcClass::init(Stream* xStream)
{
	this->xStream = xStream;
}

void PlcClass::send(PlcInfoType type){
	byte bs[5]={ 0x02, 0x06, 0x02, 0x00, 0x00 };

	if (type == PlcInfoType_error){
		bs[4] = 0x00;
	}
	else if (type == PlcInfoType_ready){
		bs[4] = 0x01;
	}
	else if (type == PlcInfoType_date){
		bs[4] = 0x02;
	}
	else if (type == PlcInfoType_data){
		bs[4] = 0x03;
	}
	else if (type == PlcInfoType_unknow){
		bs[4] = 0x04;
	}
	else if (type == PlcInfoType_trunk){
		bs[4] = 0x05;
	}
	else if (type == PlcInfoType_driver){
		bs[4] = 0x06;
	}
	else if (type == PlcInfoType_uploaded){
		bs[4] = 0x07;
	}
	else if (type == PlcInfoType_dispatch){
		bs[4] = 0x08;
	}

	this->write(bs, 5);
}

void PlcClass::send(unsigned long keyid, String code, String plate, uint16_t volumn, uint16_t planed, uint16_t finished, uint16_t potency, String kind){
	//				4字节				 4字节		  5字节			2字节			 2字节			  2字节				 2字节			   1字节
	byte bs[25] = {0x02, 0x08, 0x16};

	//任务编号keyid
	bs[3] = (keyid & 0xff000000)>>24;
	bs[4] = (keyid & 0xff0000) >> 16;
	bs[5] = (keyid & 0xff00) >> 8;
	bs[6] = keyid & 0xff;

	//人员编号code
	bs[7] = code.charAt(0);
	bs[8] = code.charAt(1);
	bs[9] = code.charAt(2);
	bs[10] = code.charAt(3);

	//车牌号plate
	bs[11] = plate.charAt(0);
	bs[12] = plate.charAt(1);
	bs[13] = plate.charAt(2);
	bs[14] = plate.charAt(3);
	bs[15] = plate.charAt(4);

	//车辆体积volumn
	bs[16] = (volumn & 0xff00) >> 8;
	bs[17] = volumn & 0xff;

	//计划人数planed
	bs[18] = (planed & 0xff00) >> 8;
	bs[19] = planed & 0xff;

	//当天已完成的任务finished
	bs[20] = (finished & 0xff00) >> 8;
	bs[21] = finished & 0xff;

	//加注的浓度potency，千分之几
	bs[22] = (potency & 0xff00) >> 8;
	bs[23] = potency & 0xff;

	//车载类型
	if (kind == "lhs"){
		bs[24] = 0x01;
	}
	else if(kind=="nys"){
		bs[24] = 0x02;
	}
	else if (kind == "fls"){
		bs[24] = 0x03;
	}
	else{
		bs[24] = 0x01;
	}

	this->write(bs, 25);
}

void PlcClass::write(byte* bs, uint8_t len){
	word value = this->crc(bs, len);
	char crc1 = value & 0xff;
	char crc2 = (value & 0xff00) >> 8;

	xStream->write(bs, len);
	xStream->write(crc1);
	xStream->write(crc2);
}

//低位在前，高位在后
word PlcClass::crc(byte* bs, uint8_t len){
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

String PlcClass::read(){
	if (xStream->available() > 0){
		char bs[1024];
		int len = xStream->readBytes(bs, 1024);
		if (this->isValidData(bs, len)){
			String data = "";

			for (int i = 0; i < len; i++){
				String value = String(bs[i], HEX);
				if (value.length() == 1){
					data.concat('0');
				}

				data.concat(value);
			}

			data.toUpperCase();

			return data;
		}
	}

	return "";
}

bool PlcClass::isValidData(char* bs, int len){
	if (checkCrc(bs, len) && checkLength(bs, len)){
		return true;
	}

	return false;
}

bool PlcClass::checkCrc(char* bs, int len){
	word value = 0xffff;

	for (int i = 0; i < len - 2; i++){
		char c = bs[i];

		value = value^ c;

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

	char crc1 = value & 0xff;
	char crc2 = (value & 0xff00) >> 8;

	if ((crc1 == bs[len - 2]) && (crc2 == bs[len - 1])){
		return true;
	}

	return false;
}

bool PlcClass::checkLength(char* bs, int len){
	int l = bs[2];
	if (len == l + 5){//2个字节的CRC校验，1个字节的目标地址，1个字节的命令码，1个字节的数据长度
		return true;
	}

	return false;
}

PlcClass Plc;

