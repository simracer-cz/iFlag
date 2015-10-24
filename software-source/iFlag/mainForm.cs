using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using iFlag.Properties;

namespace iFlag
{
    public partial class mainForm : Form
    {
        // Version
        const byte major = 0;
        const byte minor = 50;
        String edition = "";

        bool simConnected;

        public mainForm()
        {
            InitializeComponent();

            // Put version number into the window title
            this.Text += " v" + major + "." + minor;

            // And special edition string if applicable
            if (edition != "") this.Text += edition;

            startCommunication();
            startSDK();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.Location = Settings.Default.WindowLocation;
            this.WindowState = Settings.Default.WindowState;
            this.TopMost = this.alwaysOnTopMenuItem.Checked = Settings.Default.WindowTopMost;
            restoreCommunication();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal) Settings.Default.WindowLocation = this.Location;
            Settings.Default.WindowTopMost = this.TopMost;
            storeCommunication();
            Settings.Default.Save();
        }

        private void mainForm_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal) Settings.Default.WindowLocation = this.Location;
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            optionsMenu.Show(Cursor.Position);
        }

        private void alwaysOnTopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.Default.WindowTopMost = this.TopMost = this.alwaysOnTopMenuItem.Checked;
        }

        // Visually indicates the serial connection alive state
        // by changin color of the "Matrix" indicator
        // in the lower left corner of the main form
        //
        private void indicateConnection()
        {
            if (deviceConnected)
            {
                hardwareLight.BackColor = Color.FromName("ForestGreen");
                commLabel.Text = "v" + firmwareVersionMajor + "." + firmwareVersionMinor + " @" + port;
            }
            else
            {
                hardwareLight.BackColor = Color.FromName("Red");
                commLabel.Text = "";
            }

            detectSDK();
        }

        private void indicateSimConnected(bool connected)
        {
            if (connected && !simConnected)
            {
                simLight.BackColor = Color.FromName("ForestGreen");
            }
            else if (!connected)
            {
                simLight.BackColor = Color.FromName("Red");
            }
            simConnected = connected;
        }
    }
}
