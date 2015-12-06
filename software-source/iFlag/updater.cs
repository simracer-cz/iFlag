using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace iFlag
{
    public partial class mainForm : Form
    {

        private void startUpdater()
        {
        }

                                                  // Checks if expected and actual firmware versions match,
                                                  // in which case it returns `true`.
        private bool deviceUpdated()
        {
            return firmwareVersionMajor == firmwareMajor
                && firmwareVersionMinor == firmwareMinor;
        }

                                                  // Uses embedded `avrdude` tool to flash the device's memory
                                                  // with the firmware distributed along the software
        private void updateFirmware()
        {
            hardwareLight.BackColor = Color.FromName("Blue");
            commLabel.Text = "Programming with v" + firmwareMajor + "." + firmwareMinor + "...";

            SP.Close();
            deviceConnected = false;
            connectTimer.Enabled = false;
            demoTimer.Enabled = false;
            greeted = false; 
            tryPortIndex = 0;
                
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.FileName = "cmd.exe";
            info.Arguments = "/C device\\tools\\avrdude\\avrdude -Cdevice\\tools\\avrdude\\avrdude.conf -q -q -patmega328p -carduino -P" + port + " -b115200 -D -Uflash:w:device\\firmware.hex:i";
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();
            Console.WriteLine(process.ExitCode);
        }
    }
}
