using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using iFlag.Properties;

namespace iFlag
{
    public partial class mainForm : Form
    {

                                                  // Stores current software updates level of involvement
        string updatesLevel = Settings.Default.Updates;
        string updateVersion = null;              // Version string of the update (if detected)
        bool updateServiceWorking = false;
        string updateChanges = "";                // Copy of the changelog
        string updateURL = "";

        private Thread updateSoftwareThread;      // To not hold up the startup, check for updates
                                                  // is done in a separate thread

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

                                                  // Checks if currently installed version match the latest
                                                  // update available on selected updates level.
                                                  // Returns `true` when software is up to date.
        private bool softwareUpdated()
        {
            return updateVersion == null
                || updateVersion == version;
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
            Console.WriteLine(info.Arguments);
            Console.WriteLine(process.ExitCode);
        }

                                                  // Runs a separate thread, which will check for app updates
        private void updateSoftware()
        {
            updateSoftwareThread = new Thread(UpdateWorkerThread);
            updateSoftwareThread.Start();  
        }

                                                  // If iFlag doesn't find the hardware within 30seconds
                                                  // it will assume, that a brand new Arduino board is plugged in
                                                  // and will activate a otherwise invisible options menu item
                                                  // allowing the user to initialize the board. Last known port
                                                  // in the list is used in that cae
        private void initiationTimer_Tick(object sender, EventArgs e)
        {
            port = ports[ports.Length - 1];
            
            if (!deviceConnected && port != "COM1")
            {
                initiationTimer.Stop();
                initiateBoardMenuItem.Visible = true;
                initiateBoardMenuItem.Text += port;
            }
            else
            {
                startCommunication();
            }
        }

        private void initiateBoardMenuItem_Click(object sender, EventArgs e)
        {
            initiateBoardMenuItem.Visible = false;
            port = ports[ports.Length - 1];
            updateFirmware();
            connectTimer.Enabled = true;
        }

                                                  // Jumps on the internet to retreive a XML version file for
                                                  // selected updates level, reads the version information inside
                                                  // and stores these data into vars for later use.
                                                  // Returns `true` when there is an update.
        private bool CheckSoftwareVersion()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(updatesURL + Profile());
                request.UserAgent = UserAgent();
                var reader = XmlReader.Create(request.GetResponse().GetResponseStream());
                reader.MoveToContent();
                string elementName = "";
                updateChanges = "";
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "iflag")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            if (reader.NodeType == XmlNodeType.Text && reader.HasValue)
                            {
                                switch (updatesLevel)
                                {
                                    case "stable":
                                        switch (elementName)
                                        {
                                            case "stable-version":
                                                updateVersion = reader.Value;
                                                updateServiceWorking = true;
                                                break;
                                            case "stable-changelog":
                                                updateChanges = reader.Value;
                                                break;
                                            case "stable-url":
                                                updateURL = reader.Value;
                                                break;
                                        }
                                        break;

                                    case "experimental":
                                        switch (elementName)
                                        {
                                            case "experimental-version":
                                                updateVersion = reader.Value;
                                                updateServiceWorking = true;
                                                break;
                                            case "stable-changelog":
                                            case "experimental-changelog":
                                                updateChanges = reader.Value + updateChanges;
                                                break;
                                            case "experimental-url":
                                                updateURL = reader.Value;
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                //if (reader != null) reader.Close();
            }
            return !softwareUpdated();
        }

                                                  // Constructs and returns a profile string
                                                  // sent along with the update request.
                                                  // Contents:
                                                  // - iFLAG version
                                                  // - updates channel setting
                                                  // - connected device USB name and port
                                                  // - device's brightness and connector settings
        private string Profile()
        {
            string[] device = {
                Regex.Replace(DeviceUSBName(port), @"[^a-zA-Z0-9.,]", ""),
                ":" + matrixLuma,
                ":" + connectorSide,
                "@" + port,
            };
            string[] profile = {
                "v=" + version,
                "u=" + updatesLevel,
                "d=" + String.Join("", device),
            };
            return "?" + String.Join("&", profile);
        }

                                                  // Returns back user agent for update request
        private string UserAgent()
        {
            string[] profile = {
                "iFLAG",
                "Arduino",
                MachineID(),
            };
            return String.Join("/", profile);
        }

                                                  // Returns back machine identifier,
                                                  // currently MD5-hashed processor ID
        private string MachineID()
        {
            foreach (ManagementObject item in new ManagementClass("Win32_Processor").GetInstances())
            {
                return MD5(item.Properties["ProcessorID"].Value.ToString());
            }
            return "";
        }

                                                  // Returns back the description of an USB device
                                                  // on a given serial port
        private string DeviceUSBName(string port)
        {
            foreach (ManagementObject item in new ManagementClass("Win32_SerialPort").GetInstances())
            {
                if (item["DeviceID"].ToString() == port)
                {
                    return item["Name"].ToString();
                }
            }
            return "";
        }

                                                  // Calculates and returns MD5 hash of the input
        private string MD5(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] originalBytes = ASCIIEncoding.Default.GetBytes(input);
            byte[] encodedBytes = md5.ComputeHash(originalBytes);

            return BitConverter.ToString(encodedBytes).Replace("-", "");
        }

                                                  // Handles the actual update instructions process
                                                  // after user has clicked on the "Updates Available" link.
                                                  // It presents user with a dialog detailing the versions
                                                  // and changes. When user proceeds, it then offers
                                                  // detailed instructions on how to perform the manual update.
        private void updateLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string dialogText = "";
            if (updateVersion == version)
            {
                dialogText += string.Format("Your iFLAG {0} ({1}) is up-to-date.\n\n", version, updatesLevel);
                dialogText += string.Format("Change log:\n{0}", updateChanges);
                MessageBox.Show( dialogText, "iFLAG Version", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                dialogText += string.Format("If you agree,\nyour iFLAG {0} will be updated to {1} ({2})\n\n", version, updateVersion, updatesLevel);
                dialogText += string.Format("Change log:\n{0}\n", updateChanges);
                dialogText += string.Format("Perform the update?");

                if ( DialogResult.OK == MessageBox.Show( dialogText, "Update iFLAG?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) )
                {
                    Process process = new Process();
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = "updater.exe";
                    info.Arguments = string.Format("{0} {1} {2} {3} {4} {5}", version, updateVersion, this.Location.X, this.Location.Y, updatesLevel, updateURL);
                    process.StartInfo = info;
                    process.Start();
                    Application.Exit();
                }
            }
        }

                                                  // Asynchronously handles the software update check
                                                  // and adjusts the main UI based on its findings
        private void UpdateWorkerThread()  
        {
            if (updatesLevel != "none" && updatable)
            {
                if (CheckSoftwareVersion())
                {
                    this.InvokeEx(f => f.indicateUpdatesAvailable());
                }
                else
                {
                    this.InvokeEx(f => f.indicateNoUpdates());
                }
            }
            else
            {
                this.InvokeEx(f => f.indicateUpdatesOff());
            }
        }

        private void indicateUpdatesAvailable()
        {
            updateLinkLabel.Text = "**Update available**";
            updateLinkLabel.LinkColor = Color.FromName("Gold");
            updateLinkLabel.BackColor = Color.FromName("Black");
            updateLinkLabel.Location = new Point(this.Width - updateLinkLabel.Width - 5, updateLinkLabel.Location.Y);
            updateLinkLabel.Show();
        }

        private void indicateNoUpdates()
        {
            if (!updateServiceWorking)
            {
                indicateUpdatesOff();
                return;
            }

            updateLinkLabel.Text = "Up-to-date";
            updateLinkLabel.LinkColor = Color.FromName("Gray");
            updateLinkLabel.BackColor = Color.FromName("Transparent");
            updateLinkLabel.Location = new Point(this.Width - updateLinkLabel.Width - 5, updateLinkLabel.Location.Y);
            updateLinkLabel.Show();
        }

        private void indicateUpdatesOff()
        {
            updateLinkLabel.Hide();
        }
    }
}

                                                  // This bit below is needed to overcome the thread lock-in
                                                  // and perform actions on the main Form in the main thread
public static class ISynchronizeInvokeExtensions
{
  public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
  {
    if (@this.InvokeRequired)
    {
      @this.Invoke(action, new object[] { @this });
    }
    else
    {
      action(@this);
    }
  }
}