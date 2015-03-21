// GpsHelper.h

#ifndef _GPSHELPER_h
#define _GPSHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <LGPS\LGPS.h>

typedef struct {
	String time;
	String latitude;
	char ns;
	String longitude;
	char ew;
	unsigned char statusOfGPS;
	unsigned char countOfSatellites;
} GPSDataStruct;

class GpsHelperClass
{
 protected:

private:
	gpsSentenceInfoStruct info;


 public:
	 void init();
	 void getGpsData(GPSDataStruct* gps);
};

extern GpsHelperClass GpsHelper;

#endif

