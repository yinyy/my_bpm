#include <LBattery.h>
#include <PubSubClient.h>
#include <LGPRSClient.h>
#include <LGPRS.h>

typedef struct _BatteryInfo
{
	int level;
	boolean charging;
} BatteryInfo;


LGPRSClient gprsClient;
PubSubClient client(gprsClient);
uint8_t receivedBuffer[512];
uint16_t len = sizeof(receivedBuffer) / sizeof(uint8_t);
uint8_t ch;
BatteryInfo battery;

uint8_t CalcFCS(uint8_t* buffer, uint8_t len)
{
	uint8_t xorResult = 0;
	for (uint8_t x = 0; x < len; x++, buffer++) {
		xorResult = xorResult ^ *buffer;
	}

	return (xorResult);
}

void reconnect() {
	digitalWrite(13, LOW);
	
	// Loop until we're reconnected
	while (!client.connected()) {
		//Serial.println("Attempting MQTT connection...");
		// Attempt to connect
		if (client.connect("AgricultureArduinoClient")) {
			client.subscribe("/a/l/in");
			//Serial.println("connected");
			digitalWrite(13, HIGH);
		}
		else {
			//Serial.print("failed, rc=");
			//Serial.println(client.state());
			//Serial.println(" try again in 5 seconds");
			// Wait 5 seconds before retrying
			delay(5000);
		}
	}
}

void callback(char* topic, byte* payload, unsigned int length) {
	//订阅消息的主题是/a/l/in
	if ((topic[0] == '/') && (topic[1] == 'a') && (topic[2] == '/') && (topic[3] == 'l') && 
		(topic[4] == '/') && (topic[5] == 'i') && (topic[6] == 'n')) {
		Serial.println("Entered");
		if ((payload[0] == 0x00) && (payload[1] == 0x00) && (payload[2] == 0x00) && (payload[3] == 0x00) && 
			(payload[4] == 0x00) && (payload[5] == 0x00) && (payload[6] == 0x00) && (payload[7] == 0x00)) {//特殊设备
			if ((payload[8] == 0x00) && (payload[9] == 0x01)) {//查询所有设备
				receivedBuffer[0] = 0xfe;
				receivedBuffer[1] = 0x00;
				receivedBuffer[2] = 0x00;
				receivedBuffer[3] = 0x01;
				receivedBuffer[4] = CalcFCS(receivedBuffer, 4);

				Serial1.write(receivedBuffer, 5);
			}
			else if ((payload[8] == 0x00) && (payload[9] == 0x03)) {//查询电池状态
				battery.charging = LBattery.isCharging();
				battery.level = LBattery.level();

				receivedBuffer[0] = 0x00;
				receivedBuffer[1] = 0x00;
				receivedBuffer[2] = 0x00;
				receivedBuffer[3] = 0x00;
				receivedBuffer[4] = 0x00;
				receivedBuffer[5] = 0x00;
				receivedBuffer[6] = 0x00;
				receivedBuffer[7] = 0x00;
				receivedBuffer[8] = 0x10;//设备类型
				receivedBuffer[9] = battery.charging ? 0x01 : 0x00;
				receivedBuffer[10] = (uint8_t)battery.level;

				client.publish("/a/l/out", receivedBuffer, 11);

				Serial.write(receivedBuffer, 11);
			}
		}
		else {//一般设备
			if ((payload[8] == 0x00) && (payload[9] == 0x01)) {//查询指定设备的状态
				receivedBuffer[0] = 0xfe;
				receivedBuffer[1] = length - 2;
				receivedBuffer[2] = 0x00;
				receivedBuffer[3] = 0x01;

				for (int i = 0;i < 8;i++) {
					receivedBuffer[4 + i] = payload[i];
				}

				receivedBuffer[12] = CalcFCS(receivedBuffer, 12);

				Serial.write(receivedBuffer, 13);
				Serial1.write(receivedBuffer, 13);
			}
			else if ((payload[8] == 0x00) && (payload[9] == 0x02)) {//更新设备状态
				receivedBuffer[0] = 0xfe;
				receivedBuffer[1] = length - 2;
				receivedBuffer[2] = 0x00;
				receivedBuffer[3] = 0x02;

				for (int i = 0;i < 8;i++) {
					receivedBuffer[4 + i] = payload[i];
				}

				for (int i = 10;i < length;i++) {
					receivedBuffer[i + 2] = payload[i];
				}

				receivedBuffer[length + 2] = CalcFCS(receivedBuffer, length + 2);

				Serial.write(receivedBuffer, length + 3);
				Serial1.write(receivedBuffer, length + 3);
			}
		}
	}
}

void setup()
{
	battery.level = 0;
	battery.charging = false;

	pinMode(13, OUTPUT);
	digitalWrite(13, LOW);

	Serial.begin(115200);
	Serial1.begin(115200);

	while (!LGPRS.attachGPRS())
	{
		delay(500);
		//Serial.println("connecting");
	}

	client.setServer("139.129.43.203", 1883);
	client.setCallback(callback);

	delay(3000);
	//Serial.println("Enter");
}

void loop()
{
	if (!client.connected()) {
		reconnect();
	}
	client.loop();


	//协调发发送过来数据
	while ((Serial1.available() > 0) && ((ch = Serial1.read()) != 0xfe));
	//等到命令开始的字符
	if (ch == 0xfe) {//一条命令
		receivedBuffer[0] = 0xfe;

		while (Serial1.available() == 0);
		receivedBuffer[1] = Serial1.read();//长度

		Serial1.readBytes(&receivedBuffer[2], receivedBuffer[1] + 3);

		ch = CalcFCS(&receivedBuffer[1], receivedBuffer[1] + 3);
		Serial.write(ch);
		if (ch == receivedBuffer[receivedBuffer[1] + 4]) {
			//通过MQTT发出信息
			Serial.write(&receivedBuffer[4], receivedBuffer[1]);
			client.publish("/a/l/out", &receivedBuffer[4], receivedBuffer[1]);
		}
	}

	//获取电池信息
	if ((LBattery.isCharging() != battery.charging)||(LBattery.level()!=battery.level)) {
		battery.charging = LBattery.isCharging();
		battery.level = LBattery.level();

		receivedBuffer[0] = 0x00;
		receivedBuffer[1] = 0x00;
		receivedBuffer[2] = 0x00;
		receivedBuffer[3] = 0x00;
		receivedBuffer[4] = 0x00;
		receivedBuffer[5] = 0x00;
		receivedBuffer[6] = 0x00;
		receivedBuffer[7] = 0x00;
		receivedBuffer[8] = 0x10;//设备类型
		receivedBuffer[9] = battery.charging ? 0x01 : 0x00;
		receivedBuffer[10] = (uint8_t)battery.level;

		client.publish("/a/l/out", receivedBuffer, 11);

		Serial.write(receivedBuffer, 11);
	}
}