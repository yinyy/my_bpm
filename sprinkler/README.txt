Arduino发送给PLC的数据：
（1）SYSTEM_ERROR：当Arduino发生错误，无法工作的情况下发送。例如：SIM900A模块没有准备好，或者串口没有准备好等。只发送一次。
（2）SYSTEM_READY：当Arduino准备就绪，通知PLC。只发送一次。
（3）error_date：应提示操作员“日期错误”，也就是拿着昨天的卡来加今天的水。
（4）error_dispatch：应提示操作员“数据错误，请重新写卡”。
（5）error_other：应提示操作员“未知错误”。
（6）error_trunk：应提示操作员“车辆卡错误”。
（7）error_driver：应提示操作员“人员卡错误”。
（8）success_save：应提示操作员“数据上传成功”。
（9）work_xxx：应根据后续的数据进行操作。
	数据的格式：work_sDriverId,driverCode,sTrunkId,trunkPlate,sVolumn,sWorkload,sFinished
					 人员ID	   人员编号   车辆ID   车牌号     容积    计划车次  已完成


PLC使用一个脚置高电平或低电平控制Arduino是否读卡。
当该脚为低电平时，允许Arduino读卡。
当该脚为高电平时，不允许Arduino读卡。

何时不允许Arduino读卡？
1、PLC收到SYSTEM_READY后，可以控制Arduino是否读卡，在这之前，控制无效。
2、PLC在提示操作员各种信息的情况下，不允许Arduino读卡。
3、PLC在加水过程中，不允许Arduino读卡。

何时允许Arduino读卡？
1、PLC收到SYSTEM_READY后，可以控制Arduino读卡。
2、当操作员完成各项操作化，可以控制Arduino读卡。
