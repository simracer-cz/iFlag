using System;
using System.IO.Ports;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using iFlag.Properties;

namespace iFlag
{
    public partial class mainForm : Form
    {
        static SerialPort SP;                     // Serial port IO instance
        String[] ports;                           // List of known ports to scan through for device
        String port;                              // Currently open serial port name
        byte tryPortIndex;                        // Index in list of `ports`

        int[] bauds = new int[2] { 9600, 38400 }; // List of supported serial port baudrates
        int baud;                                 // Currently used baudrate
        byte tryBaudIndex;                        // Index in list of `bauds`

        bool deviceConnected;                     // Well, whether device is connected or not
        byte firmwareVersionMajor;                // Major firmware version of the device connected
        byte firmwareVersionMinor;                // Minor version of the same
        int serialTimeout = 1000;                 // Miliseconds of silence before dropping the comm port
        DateTime lastPingTime;                    // Timestamp of last received ping beacon

                                                  // Serial commands, which match the firmware
                                                  // set of instructions.
        byte[] COMMAND_RESET        = new byte[8] { 0xFF, 0xFF, 0xA9, 0x00, 0x00, 0x00, 0x00, 0x00 };
        byte[] COMMAND_DRAW         = new byte[8] { 0xFF, 0xFF, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00 };
        byte[] COMMAND_BLINK_FAST   = new byte[8] { 0xFF, 0xFF, 0xA1, 0x04, 0x00, 0x00, 0x00, 0x00 };
        byte[] COMMAND_BLINK_SLOW   = new byte[8] { 0xFF, 0xFF, 0xA1, 0x02, 0x00, 0x00, 0x00, 0x00 };
        byte[] COMMAND_NOBLINK      = new byte[8] { 0xFF, 0xFF, 0xA1, 0x00, 0x00, 0x00, 0x00, 0x00 };
        byte[] COMMAND_LUMA         = new byte[8] { 0xFF, 0xFF, 0xA2, 0x64, 0x00, 0x00, 0x00, 0x00 };

                                                  // Builds a list of serial ports (`ports`)
                                                  // for the port probe to cycle over
                                                  // when searching for compatible device
        private void startCommunication()
        {
            string[] portsList = SerialPort.GetPortNames();
            if (Settings.Default.SerialPort != "")
            {
                                                  // If we know the port from last time,
                                                  // put it first on the list. This makes second
                                                  // and subsequent connections faster
                                                  // than the very first one.
                ports = new string[portsList.Length + 1];
                ports[0] = Settings.Default.SerialPort;
                Array.Copy(portsList, 0, ports, 1, portsList.Length);
            }
            else
            {
                ports = portsList;
            }

                                                  // Make first attempt now, the rest are timer-launched
            attemptConnection();
        }

                                                  // Stores the currently used port into settings for next time
        private void storeCommunication()
        {
            Settings.Default.SerialPort = port;
        }

        private void restoreCommunication()
        {
        }

                                                  // Probes a serial port for a communicative USB device
                                                  // to eventually estzablish a working connection.
        private void attemptConnection()
        {
            if (!deviceConnected)
            {
                timeoutTimer.Enabled = false;

                                                  // Cycle over the baudrates list
                if (tryBaudIndex >= bauds.Length)
                {
                    tryBaudIndex = 0;
                    tryPortIndex++;
                }
                                                  // Cycle over the ports list every baudrates list cycle
                if (tryPortIndex >= ports.Length)
                {
                    commLabel.Text = "No device.";
                    tryPortIndex = 0;
                }

                try
                {
                    port = ports[tryPortIndex];
                    baud = bauds[tryBaudIndex];
                    Console.WriteLine(string.Format("Probing port {0} at {1} bps...", port, baud));
                    if (SP != null && SP.IsOpen) SP.Close();
                    SP = new SerialPort(port, baud, Parity.None, 8);
                    SP.Open();

                                                  // If the device is present and physically connected,
                                                  // it pings the host software in regular intervals
                                                  // and if those data are received, the connection
                                                  // will be established and held.
                    SP.DataReceived += SP_DataReceived;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                                                  // In case the device doesn't respond in time,
                                                  // prepare to try the next port in the list
                tryBaudIndex++;
            }
            else
            {
                timeoutTimer.Enabled = true;
                indicateConnection();
            }
        }

                                                  // Precondition incoming data.
        private byte SP_ReadLine()
        {
            try
            {
                string line = Regex.Replace( SP.ReadLine(), @"[^0-9a-f]+", "");
                if (line != "") return Convert.ToByte( line );
                else return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }

                                                  // Ingress and digest serial data coming from the USB device.
                                                  // This one is a simple device, so only ping packets
                                                  // are arriving.
        private void SP_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while (SP.IsOpen && SP.BytesToRead >= 8 && SP_ReadLine() == 0xFF)
            {
                byte inByteExtra = SP_ReadLine();
                byte major = SP_ReadLine();
                byte minor = SP_ReadLine();
                byte command = SP_ReadLine();
                byte value = SP_ReadLine();
                byte extra = SP_ReadLine();
                byte more = SP_ReadLine();
                // Console.WriteLine(string.Format("PACKET major:{0} minor:{1} command:{2} value:{3} extra:{4} more:{5}", major, minor, command, value, extra, more));

                if (inByteExtra == 0xFF)
                {
                    switch (command)
                    {
                        case 0xB0:            // ping beacon
                                              // In order to not try to communicate with other unrelated
                                              // devices on the serial bus device identifier is checked here.
                            if (value == 0xD2)
                            {
                                if (!deviceConnected)
                                {
                                    deviceConnected = true;
                                    firmwareVersionMajor = major;
                                    firmwareVersionMinor = minor;
                                    startMatrix();
                                }
                                lastPingTime = DateTime.Now;
                                initiationTimer.Stop();
                            }
                            break;
                    }
                }
            }
        }

                                                  // Sends out 8-bit data packets through the serial connection
        private void SP_SendData(byte[] data)
        {
            if (deviceConnected)
                try
                {
                    SP.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SERIAL WRITE FAIL");
                    Console.WriteLine(ex);
                    deviceConnected = false;
                    indicateConnection();
                }
        }

                                                  // Timed connection attempts cycling the known
                                                  // serial ports list.
        private void connectTimer_Tick(object sender, EventArgs e)
        {
            startCommunication();
        }

                                                  // If connection gets broken for more than 1 second, drop it.
        private void timeoutTimer_Tick(object sender, EventArgs e)
        {
            if (lastPingTime.AddMilliseconds(serialTimeout).CompareTo(DateTime.Now) < 0)
            {
                Console.WriteLine("TIMEOUT DISCONNECT");
                deviceConnected = false;
                indicateConnection();
                connectTimer.Enabled = true;
                initiationTimer.Start();
            }
        }
    }
}
