using System.IO.Ports;

namespace USB_RLY08_Drivers.Lib
{
    public class BoardHandler
    {
        public string ComPort { get; set; }
        public bool Connected { get; set; }
        private SerialPort SerialPort { get; set; }
        private BoardCommands BoardCommands { get; set; }

        public BoardHandler(string comPort) 
        {
            ComPort = comPort;
            BoardCommands = new BoardCommands();

            // Create the serial port with basic settings
            SerialPort = new SerialPort(ComPort, 19200, Parity.None, 8, StopBits.One);
            //ialPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        public bool InitiateBoard(out string softwareVersion)
        {
            softwareVersion = "";            
            
            try
            {
                //Open Serial port
                SerialPort.Open();

                //Write commands that will set all relays off and read ouut the board serialnumber
                WriteSerialportCommands(BoardCommands.AvailableCommands().FirstOrDefault(x => x.Key == BoardCommands.Functions.AllRelaysOff).Value);  //new byte[1] { 0x6E }); // ;

                //Read the serial number after initiating port
                var resp = WriteReadSerialportCommands(BoardCommands.AvailableCommands().FirstOrDefault(x => x.Key == BoardCommands.Functions.ReadSoftwareVersion).Value); // new byte[1] { 0x5A });   //SerialPort.Read(buff, 0, SerialPort.ReadBufferSize).ToString();

                if(resp.Length >= 2)
                {
                    softwareVersion = resp[1].ToString();
                }

                return !string.IsNullOrEmpty(softwareVersion); // true;
            }
            catch { return false; }
        }

        public void DisconnectBoard()
        {
            Connected = false;
            ComPort = "";
            SerialPort.Close();
            SerialPort.Dispose();
        }

        public void SetRelay_Close(int relayToClose)
        {
            var h = new byte[0];
            switch(relayToClose)
            {
                case 1:
                    h = BoardCommands.AvailableCommands().FirstOrDefault(x => x.Key == BoardCommands.Functions.Relay_1_On).Value;
                    break;
                default: break;
            }

            WriteSerialportCommands(h);

        }

        public void SetRelay_Open(int relayToOpen)
        {
            var h = new byte[0];
            switch (relayToOpen)
            {
                case 1:
                    h = BoardCommands.AvailableCommands().FirstOrDefault(x => x.Key == BoardCommands.Functions.Relay_1_Off).Value;
                    break;
                default: break;
            }

            WriteSerialportCommands(h);
        }

        public void SetRelay(int relayData)
        {
            var command = BoardCommands.AvailableCommands().FirstOrDefault(x => x.Key == BoardCommands.Functions.SetRelayState).Value;
            command[1] = byte.Parse(relayData.ToString());

            WriteSerialportCommands(command);
        }

        public void SetAllRelays_Off()
        {
            WriteSerialportCommands(BoardCommands.AvailableCommands().FirstOrDefault(x => x.Key == BoardCommands.Functions.AllRelaysOff).Value);
        }

        public void SetAllRelays_On()
        {
            WriteSerialportCommands(BoardCommands.AvailableCommands().FirstOrDefault(x => x.Key == BoardCommands.Functions.AllRelaysOn).Value);
        }

        //========================================================
        //                  Private Methods
        //========================================================

        private void WriteSerialportCommands(byte[] byteArray)
        {
            //Write commands
            SerialPort.Write(byteArray, 0, byteArray.Length);

            //Wait for the relays to be set
            Thread.Sleep(50);
        }

        private byte[] WriteReadSerialportCommands(byte[] byteArray) 
        {
            //Write serial command
            WriteSerialportCommands(byteArray);

            //Read serial data
            byte[] buff = new byte[SerialPort.ReadBufferSize];
            var resp = SerialPort.Read(buff, 0, SerialPort.ReadBufferSize).ToString();

            if(int.TryParse(resp, out var bytesInRespones))
            {
                return buff.Take(bytesInRespones).ToArray();
            }
            else
            {
                return new byte[0];
            }
        }

        //private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    // Show all the incoming data in the port's buffer
        //    var receivedData = SerialPort.ReadExisting();
        //}

    }
}