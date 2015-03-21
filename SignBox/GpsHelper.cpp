// 
// 
// 

#include "GpsHelper.h"
#include <LGPS.h>

void GpsHelperClass::init(){
	//open GPS
	LGPS.powerOn();
}

void GpsHelperClass::getGpsData(GPSDataStruct* gps) {
	LGPS.getData(&info);
	if (info.GPGGA != NULL) {
		char* datas[15];
		char** p = datas;

		//$GPGGA,121618.083,3618.8294,N,12023.9213,E,0,0,,145.7,M,4.3,M,,*75
		char *data = strtok((char*)info.GPGGA, ",");
		while (data != NULL) {
			*p++ = data;
			data = strtok(NULL, ",");
		}

		String t = datas[1];
		int hour = t.substring(0, 2).toInt();
		hour = (hour + 8) % 24;
		gps->time = ((hour < 10) ? "0" + String(hour) : String(hour)) + ":" + t.substring(2, 4) + ":" + t.substring(4, 6);
		
		gps->latitude = datas[2];
		gps->ns = datas[3][0];
		gps->longitude = datas[4];
		gps->ew = datas[5][0];
		gps->statusOfGPS = atoi(datas[6]);
		gps->countOfSatellites = atoi(datas[7]);
	}
}

GpsHelperClass GpsHelper;

