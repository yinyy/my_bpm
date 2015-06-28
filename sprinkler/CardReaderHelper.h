// CardReaderHelper.h

#ifndef _CARDREADERHELPER_h
#define _CARDREADERHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class CardReaderHelperClass
{
 protected:


 public:
	 String readTrunkCard();
	 String readDriverCard();
	 String getPlate(String cno);
	 String getCode(String cno);
	 float getVolumn(String cno);
	 boolean getCard(String* tc, String* pc);
};

extern CardReaderHelperClass CardReaderHelper;

#endif

