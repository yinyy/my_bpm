// 
// 
// 

#include "ScreenHelper.h"

void ScreenHelperClass::changeScreen(int index){
	Serial1.write("SPG(");
	Serial1.write((char)(0x30 + index));
	Serial1.write("\r\n");
	currentScreen = index;
	
	//µÈ´ý´¥ÃþÆÁ·µ»ØOK
	while (waitForOk() != "OK");
}

void ScreenHelperClass::enableTouchScreen() {
	Serial1.write("TPN(2);\r\n");
	while (waitForOk() != "OK");
}

String ScreenHelperClass::waitForDriverCode() {
	while (1) {
		if (Serial1.available() > 0) {
			String msg = Serial1.readString();
			msg.trim();

			//Serial.println("[===" + msg + "===]");
			
			if (msg.indexOf('{') >= 0) {
				return  msg.substring(msg.indexOf('{') + 1, msg.indexOf("}"));
				break;
			}
		}
	}
}

void ScreenHelperClass::drawString(char* cmd){
	Serial1.write(cmd);
}

void ScreenHelperClass::end(){
	Serial1.write("\r\n");
	while (waitForOk() != "OK");
}

void ScreenHelperClass::showTime(String time){
	if (time == "00:00:00"){
		time = "DS16(10,15,'" + time + "',2);PIC(300,7,5);";
	}
	else{
		time = "DS16(10,15,'" + time + "',2);PIC(300,7,2);";
	}
	char cs[50];
	time.toCharArray(cs, 50);

	drawString(cs);
	end();
}

String ScreenHelperClass::readCommand() {
	String msg;
	if (Serial1.available() > 0) {
		msg = Serial1.readString();
		msg.trim();
	}

	return msg;
}

User_Command ScreenHelperClass::commandIntent(String command){
	if (command.lastIndexOf("[BN:1]") >= 0) {
		return User_Command_Fat_Pipe;
	}
	else if (command.lastIndexOf("[BN:2]") >= 0) {
		return User_Command_Slim_Pipe;
	}
	else if (command.lastIndexOf("[BN:11]") >= 0) {
		return User_Command_Ok;
	}
	else if (command.lastIndexOf("[BN:12]") >= 0) {
		return User_Command_Cancel;
	}

	return User_Command_Unknow;
}

String ScreenHelperClass::waitForOk(){
	String line;

	while (Serial1.available() <= 0);
	if (Serial1.available() > 0){
		line = Serial1.readStringUntil('\n');
		line.trim();
	}

	return line;
}

ScreenHelperClass ScreenHelper;

