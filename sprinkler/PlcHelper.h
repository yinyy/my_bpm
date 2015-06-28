// PlcHelper.h

#ifndef _PLCHELPER_h
#define _PLCHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class PlcHelperClass
{
 protected:


 public:
	 boolean isReady();
	 String readCommand();
	 void send(String code, String plate, float volumn);
	 boolean getVolumnPotencyKind(String command, float* volumn, int* potency, int* kind);
	 void saveError();
	 void saveSuccess();
};

extern PlcHelperClass PlcHelper;

#endif

