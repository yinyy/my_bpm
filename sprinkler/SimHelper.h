// SimHelper.h


#ifndef _SIMHELPER_h
#define _SIMHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

enum Context_Status
{
	Context_Status_Connecting = 0,
	Context_Status_Connected = 1,
	Context_Status_Closing = 2,
	Context_Status_Closed = 3,
	Context_Status_Error = 4
};

class SimHelperClass
{
private:
	String SERVER_URL = "http://221.2.232.82:8766/Sanitation/ashx/SanitationHandler.ashx?";

protected:


public:
	void init();
	boolean isReady();
	uint8_t checkSignal();
	Context_Status checkContextStatus(uint8_t contextId = 1);
	boolean openContext(uint8_t contextId = 1);
	boolean save(String drivceCode, String driverCode, String trunkPlate, float volumn, int potency, int kind, int contextId=1);
};

extern SimHelperClass SimHelper;

#endif

