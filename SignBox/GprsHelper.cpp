// 
// 
// 

#include "GprsHelper.h"
#include <LGPRS\LGPRS.h>
#include <LGPRS\LGPRSClient.h>

long _timeout = 1000;
long _startMillis;

void GprsHelperClass::init(){
	//open gprs
	while (!LGPRS.attachGPRS("CMNET", NULL, NULL)) {
		Serial.println("attach GPRS");
		delay(500);
	}
}

int GprsHelperClass::timedRead()
{
	int c;
	_startMillis = millis();
	do {
		c = gprs.read();
		if (c >= 0) return c;
	} while (millis() - _startMillis < _timeout);
	return -1;     // -1 indicates timeout
}

String GprsHelperClass::readGprsData() {
	String ret;

	if (gprs.available()>0){
		ret = gprs.readString();
	}

	/*int c = timedRead();
	while (c >= 0)
	{
		ret += (char)c;
		c = timedRead();
	}*/
	return ret;
}

String GprsHelperClass::readCommand() {
	String command;
	String data = readGprsData();
	
	//first:find Content-Length
	int index = data.indexOf("Content-Length:");
	if (index < 0) {
		return command;
	}

	command = data.substring(index + 16);

	//long readed = long(cnt.substring(0, cnt.indexOf("\r\n")));

	command = command.substring(command.indexOf("\r\n") + 4);
	command.trim();

	return command;
}

boolean GprsHelperClass::sendCommand(String code, String plate){
	if (gprs.connect(SERVER_URL, SERVER_PORT)) {
		gprs.println("GET /Sanitation/ashx/SanitationHandler.ashx?action=current&code=" + code + "&plate=" + plate + " HTTP/1.1");
		gprs.println("Host: " SERVER_URL ":" + String(SERVER_PORT));
		gprs.println("Connection: close");
		gprs.println();

		return true;
	}

	return false;
}

boolean GprsHelperClass::sendCommand(String code, String plate, String lng, String lat, int pipe){
	if (gprs.connect(SERVER_URL, SERVER_PORT)) {
		gprs.println("GET /Sanitation/ashx/SanitationHandler.ashx?action=sign&code=" + code + "&plate=" + plate + "&lng=" + lng + "&lat=" + lat + "&pipe=" + String(pipe) + " HTTP/1.1");
		gprs.println("Host: " SERVER_URL ":" + String(SERVER_PORT));
		gprs.println("Connection: close");
		gprs.println();

		return true;
	}

	return false;
}

GprsHelperClass GprsHelper;

