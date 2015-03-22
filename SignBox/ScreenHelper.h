// ScreenHelper.h

#ifndef _SCREENHELPER_h
#define _SCREENHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

enum User_Command
{
	User_Command_Unknow,
	User_Command_Fat_Pipe,
	User_Command_Slim_Pipe,
	User_Command_Ok,
	User_Command_Cancel
};


class ScreenHelperClass
{
 protected:
private:
	String waitForOk();
 public:
	 int currentScreen;
	 void changeScreen(int index);
	 void enableTouchScreen();
	 String waitForDriverCode();
	 void drawString(char* cmd);
	 void end();
	 void showTime(String time);
	 String readCommand();
	 User_Command commandIntent(String command);
};

extern ScreenHelperClass ScreenHelper;

#endif

