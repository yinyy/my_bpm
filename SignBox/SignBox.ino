#include <LGPRS.h>
#include <LGPRSClient.h>
#include <LGPS.h>

#include "GprsHelper.h"
#include "ScreenHelper.h"
#include "GpsHelper.h"


#define PLATE						"E82705"
#define SIGN_BUTTON_LEFT			"BTN(1,50,90,150,190,2);"
#define SIGN_BUTTON_RIGHT			"BTN(2,250,90,350,190,2);"
//"DS16(68,132,'粗管签到',2);"
char SIGN_BUTTON_LEFT_TITLE[] = { 0x44, 0x53, 0x31, 0x36, 0x28, 0x36, 0x38, 0x2C, 0x31, 0x33, 0x32, 0x2C, 0x27, 0xB4, 0xD6, 0xB9, 0xDC, 0xC7, 0xA9, 0xB5, 0xBD, 0x27, 0x2C, 0x32, 0x29, 0x3B };
//"DS16(268,132,'细管签到',2);"
char SIGN_BUTTON_RIGHT_TITLE[] = { 0x44, 0x53, 0x31, 0x36, 0x28, 0x32, 0x36, 0x38, 0x2C, 0x31, 0x33, 0x32, 0x2C, 0x27, 0xCF, 0xB8, 0xB9, 0xDC, 0xC7, 0xA9, 0xB5, 0xBD, 0x27, 0x2C, 0x32, 0x29, 0x3B };
//"DS12(56,116,'暂时没有需要签到的项目！',1);"
char NO_TASK[] = { 0x44, 0x53, 0x32, 0x34, 0x28, 0x35, 0x36, 0x2C, 0x31, 0x31, 0x36, 0x2C, 0x27, 0xD4, 0xDD, 0xCA, 0xB1, 0xC3, 0xBB, 0xD3, 0xD0, 0xD0, 0xE8, 0xD2, 0xAA, 0xC7, 0xA9, 0xB5, 0xBD, 0xB5, 0xC4, 0xCF, 0xEE, 0xC4, 0xBF, 0xA3, 0xA1, 0x27, 0x2C, 0x31, 0x29, 0x3B };


GprsHelperClass gprsHelper;
GpsHelperClass gpsHelper;
ScreenHelperClass screenHelper;
GPSDataStruct gps;

String code = "";//0001
boolean useFatPipe = false;

long lastTime[3];//0:GPS,1:GPRS,2:show time




void initScreen(){
	screenHelper.changeScreen(4);
	screenHelper.drawString(NO_TASK);
	screenHelper.end();
}

void executeCommand(String command) {
	int intent = screenHelper.commandIntent(command);

	switch (intent)
	{
	case User_Command_Fat_Pipe:
		useFatPipe = true;
	case User_Command_Slim_Pipe:
		useFatPipe = false;
		screenHelper.changeScreen(3);
		break;
	case User_Command_Cancel:
		initScreen();
		break;
	case User_Command_Ok:
		screenHelper.changeScreen(6);
		gprsHelper.sendCommand(code, PLATE, gps.longitude, gps.latitude, useFatPipe ? 0 : 1);
		break;
	default:
		break;
	}
}

void drawScreenByCommand(String command) {
	if (command.startsWith("current:") && (screenHelper.currentScreen == 4)) {
		int count = command.substring(8).toInt();
		if (count > 0) {
			screenHelper.changeScreen(4);
			screenHelper.drawString(SIGN_BUTTON_LEFT);
			screenHelper.drawString(SIGN_BUTTON_LEFT_TITLE);
			screenHelper.drawString(SIGN_BUTTON_RIGHT);
			screenHelper.drawString(SIGN_BUTTON_RIGHT_TITLE);
			screenHelper.enableTouchScreen();
			delay(100);
		}
		else {
			initScreen();
		}
	}
	else if (command.startsWith("signed:")){
		initScreen();
	}
}

void setup() {
	Serial1.begin(115200);
	while (!Serial1);

	Serial.begin(115200);

	screenHelper.changeScreen(1);
	delay(5000);
	screenHelper.changeScreen(5);

	code = screenHelper.waitForDriverCode();
	initScreen();

	gpsHelper.init();
	gprsHelper.init();

	lastTime[0] = millis();
	lastTime[1] = millis();
	lastTime[2] = millis();
}

void loop() {
	if ((millis() - lastTime[2] > 1000) && (screenHelper.currentScreen == 4)) {
		if ((gps.statusOfGPS == 1) || (gps.statusOfGPS == 2)) {
			screenHelper.showTime(gps.time);
		}
		else {
			screenHelper.showTime("00:00:00");
		}
	}

	//check GPS 1s, the reason is I want to show the gps time in the touch screen.
	if (millis() - lastTime[0] > 1000) {
		gpsHelper.getGpsData(&gps);
		lastTime[0] = millis();
	}

	//check works per 30s.
	if ((screenHelper.currentScreen == 4) && (millis() - lastTime[1] > 30000)) {
		gprsHelper.sendCommand(code, PLATE);
		lastTime[1] = millis();
	}

	String command = gprsHelper.readCommand();
	if (command.length() > 0) {
		drawScreenByCommand(command);
	}

	command = screenHelper.readCommand();
	//Serial.println(command);
	if (command.length() > 0) {
		executeCommand(command);
	}
}

