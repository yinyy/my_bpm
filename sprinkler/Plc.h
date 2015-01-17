// Plc.h

#ifndef _PLC_h
#define _PLC_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif

enum PlcInfoType
{
	PlcInfoType_data,
	PlcInfoType_date,
	PlcInfoType_dispatch,
	PlcInfoType_driver,
	PlcInfoType_error,
	PlcInfoType_ready,
	PlcInfoType_trunk,
	PlcInfoType_unknow,
	PlcInfoType_uploaded,
};



class PlcClass
{
protected:

private:
	void write(byte* bs, uint8_t len);
	word crc(byte* bs, uint8_t len);
	Stream* xStream;
	bool checkCrc(char* bs, int len);
	bool checkLength(char* bs, int len);

public:
	void init(Stream* xStream);
	void send(PlcInfoType type);
	void send(unsigned long keyid, String code, String plate, uint16_t volumn, uint16_t planed, uint16_t finished, uint16_t potency, String kind);
	String read();
	bool isValidData(char* bs, int len);
};

extern PlcClass Plc;

#endif

