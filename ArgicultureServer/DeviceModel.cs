using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgicultureServer
{
    public class DeviceModel
    {
        public DeviceModel(string mac, string kind)
        {
            Mac = mac;
            Kind = kind;
        }

        public string Mac { get; set; }
        public string Kind { get; set; }
    }

    public class TemperatureHumidityDeviceModel:DeviceModel
    {
        public TemperatureHumidityDeviceModel(string mac):base(mac,"03")
        {
        }

        public float Temperature { get; set; }
        public int Humidity { get; set; }
    }

    public class GroundDeviceModel : DeviceModel
    {
        public GroundDeviceModel(string mac) : base(mac,"04")
        {
        }

        public int Humidity { get; set; }
    }

    public class FineParticulateMatterDeviceModel : DeviceModel
    {
        public FineParticulateMatterDeviceModel(string mac):base(mac, "05")
        {

        }

        public int Value { get; set; }
    }

    public class PwmDeviceModel : DeviceModel
    {
        public PwmDeviceModel(string mac):base(mac, "06")
        {

        }

        public int[] Values { get; set; }
    }

    public class SwitcherDeviceModel : DeviceModel
    {
        public SwitcherDeviceModel(string mac):base(mac, "02")
        {

        }

        public int[] Status { get; set; }
    }
}
