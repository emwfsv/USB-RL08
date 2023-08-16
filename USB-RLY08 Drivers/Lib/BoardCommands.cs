using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USB_RLY08_Drivers.Lib
{
    public class BoardCommands
    {
        public enum Functions
        {
            AllRelaysOn,
            AllRelaysOff,
            Relay_1_On,
            Relay_2_On,
            Relay_3_On,
            Relay_4_On,
            Relay_5_On,
            Relay_6_On,
            Relay_7_On,
            Relay_8_On,
            Relay_1_Off,
            Relay_2_Off,
            Relay_3_Off,
            Relay_4_Off,
            Relay_5_Off,
            Relay_6_Off,
            Relay_7_Off,
            Relay_8_Off,
            SetRelayState,
            ReadRelayState,
            ReadSerialNumber,
            ReadSoftwareVersion,
        }
        public Dictionary<Functions, byte[]> AvailableCommands()
        {
            var returnDictionary = new Dictionary<Functions, byte[]>
            {
                { Functions.AllRelaysOn, new byte[1] {0x64 } },
                { Functions.AllRelaysOff, new byte[1] {0x6E } },
                { Functions.Relay_1_On, new byte[1] {0x65 } },
                { Functions.Relay_1_Off, new byte[1] {0x6F } },
                { Functions.Relay_2_On, new byte[1] {0x66 } },
                { Functions.Relay_2_Off, new byte[1] {0x70 } },
                { Functions.Relay_3_On, new byte[1] {0x67 } },
                { Functions.Relay_3_Off, new byte[1] {0x71 } },
                { Functions.Relay_4_On, new byte[1] {0x68 } },
                { Functions.Relay_4_Off, new byte[1] {0x72 } },
                { Functions.Relay_5_On, new byte[1] {0x69 } },
                { Functions.Relay_5_Off, new byte[1] {0x73 } },
                { Functions.Relay_6_On, new byte[1] {0x6A } },
                { Functions.Relay_6_Off, new byte[1] {0x74 } },
                { Functions.Relay_7_On, new byte[1] {0x6B } },
                { Functions.Relay_7_Off, new byte[1] {0x75 } },
                { Functions.Relay_8_On, new byte[1] {0x6C } },
                { Functions.Relay_8_Off, new byte[1] {0x76 } },
                { Functions.ReadRelayState, new byte[1] {0x5B} },
                { Functions.ReadSerialNumber, new byte[1] {0x38} },
                { Functions.ReadSoftwareVersion, new byte[1] {0x5A} },
                { Functions.SetRelayState, new byte[2] {0x5C, 0x00} },
            };

            return returnDictionary;
        }
    }
}
