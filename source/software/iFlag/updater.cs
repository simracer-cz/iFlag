using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
        string updateChanges = "";                // Copy of the changelog

                                                  // Current app version string `vX.Y`
        string version = string.Format("v{0}.{1}", major, minor);

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
            return updateVersion == null || version == updateVersion;
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
            XmlTextReader reader;
            try
            {
                reader = new XmlTextReader(updateURL);
                reader.MoveToContent();
                string elementName = "";
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
                                                break;
                                            case "stable-changelog":
                                                updateChanges = reader.Value;
                                                break;
                                        }
                                        break;

                                    case "experimental":
                                        switch (elementName)
                                        {
                                            case "experimental-version":
                                                updateVersion = reader.Value;
                                                break;
                                            case "stable-changelog":
                                            case "experimental-changelog":
                                                updateChanges = reader.Value + updateChanges;
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

                                                  // Handles the actual update instructions process
                                                  // after user has clicked on the "Updates Available" link.
                                                  // It presents user with a dialog detailing the versions
                                                  // and changes. When user proceeds, it then offers
                                                  // detailed instructions on how to perform the manual update.
        private void updateLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string dialogText = "";
            dialogText += string.Format("Your iFLAG will be updated to {0} (from {1})\n\n", updateVersion, version);
            dialogText += string.Format("Change log:\n{0}", updateChanges);

            if ( DialogResult.OK == MessageBox.Show( dialogText, "iFLAG Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) )
            {
                Process process = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "updater.exe";
                info.Arguments = string.Format("{0} {1} {2} {3}", version, updateVersion, this.Location.X, this.Location.Y);
                process.StartInfo = info;
                process.Start();
                Application.Exit();
            }
        }

                                                  // Asynchronously handles the software update check
                                                  // and adjusts the main UI based on its findings
        private void UpdateWorkerThread()  
        {
            if (updatesLevel != "none")
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