// GprsHelper.h

#ifndef _GPRSHELPER_h
#define _GPRSHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <LGPRS\LGPRS.h>
#include <LGPRS\LGPRSClient.h>

#define SERVER_URL		"221.2.232.82"
#define SERVER_PORT		8766

class GprsHelperClass
{
protected:

private:
	LGPRSClient gprs;
	String readGprsData();
	int timedRead();

public:
	void init();
	String readCommand();
	boolean sendCommand(String code, String plate);
	boolean sendCommand(String code, String plate, String lng, String lat, int pipe);
};

extern GprsHelperClass GprsHelper;

#endif

