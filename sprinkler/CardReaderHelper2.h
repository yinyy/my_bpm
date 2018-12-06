// CardReaderHelper2.h

#ifndef _CARDREADERHELPER2_h
#define _CARDREADERHELPER2_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class CardReaderHelper2Class
{
public:
	String readCards();
	String getPlate(String cno);
	String getCode(String cno);
	float getVolumn(String cno);
};

extern CardReaderHelper2Class CardReaderHelper2;

#endif

