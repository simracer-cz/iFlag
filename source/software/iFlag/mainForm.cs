using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using iFlag.Properties;

namespace iFlag
{
    public partial class mainForm : Form
    {
        // Version
        const string version = "v0.66";

        // Embedded firmware version
        const byte firmwareMajor = 0;             // Major version number of the firmware payload
        const byte firmwareMinor = 21;            // Minor version number

                                                  // In case of special occasions releases,
                                                  // this is what holds the edition string,
                                                  // which features in the window title
        const string edition = "";

        string repositoryURL = "https://github.com/simracer-cz/iFlag";
        string forumThreadURL = "http://members.iracing.com/jforum/posts/list/0/3341549.page";
        string updateURL = "http://simracer.cz/iflag/version.xml";

        bool simConnected;
        bool greeted;                             // Whether the startup greeting has happened

        int processNo;                            // Number of the currently running order (1 to X)

        public mainForm()
        {
            processNo = Process.GetProcessesByName("iFlag").Length;

            InitializeComponent();

            this.Text = String.Format("iFLAG{1} {0}", edition, processNo > 1 ? "#" + processNo : "");
            flagLabel.Text = appMenuItem.Text = String.Format("iFlag {0}", version);

                                                  // Initialize flag modules
            startCommunication();
            startSDK();
            startMatrix();
            startFlags();
            startUpdater();
        }

                                                  // When the window opens,
                                                  // restore persistent user options.
        private void mainForm_Load(object sender, EventArgs e)
        {
            if (!Settings.Default.WindowLocation.IsEmpty)
            {
                this.Location = Settings.Default.WindowLocation;
            }
            else
            {
                System.Drawing.Point screenCenter = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Location;
                screenCenter.X += System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
                screenCenter.Y += System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
                this.Location = screenCenter;
            }
            this.WindowState = Settings.Default.WindowState;
            this.TopMost = this.alwaysOnTopMenuItem.Checked = Settings.Default.WindowTopMost;
            this.demoMenuItem.Checked = Settings.Default.DemoMode;
            this.spotterOverlayModuleMenuItem.Checked = Settings.Default.ShowSpotterOverlay;
            this.startLightsModuleMenuItem.Checked = Settings.Default.ShowStartLights;

            connectorSide = Settings.Default.UsbConnector;
            switch (connectorSide)
            {
                case 0x00: connectorBottomMenuItem.Checked = true; break;
                case 0x01: connectorRightMenuItem.Checked = true; break;
                case 0x02: connectorLeftMenuItem.Checked = true; break;
                case 0x03: connectorTopMenuItem.Checked = true; break;
            }

            matrixLuma = Settings.Default.MatrixLuma;
            switch (matrixLuma)
            {
                case 10: lowBrightnessMenuItem.Checked = true; break;
                case 25: mediumBrightnessMenuItem.Checked = true; break;
                case 60: highBrightnessMenuItem.Checked = true; break;
                case 100: fullBrightnessMenuItem.Checked = true; break;
            }

            updatesLevel = Settings.Default.Updates;
            if (updatesLevel == "stable" || updatesLevel == "experimental")
            {
                this.updatesEnabledMenuItem.Checked = true;

                if (updatesLevel == "experimental")
                {
                    this.updatesExperimentalMenuItem.Checked = true;
                }
            }

            restoreCommunication();

            if (!Settings.Default.AllowMultiple && processNo > 1)
            {
                multiFlagMessage.Show();
            }
        }

                                                  // When the window closes,
                                                  // make sure to save all persistent user options.
        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            showFlag(NO_FLAG);

            Settings.Default.WindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal) Settings.Default.WindowLocation = this.Location;
            Settings.Default.WindowTopMost = this.TopMost;
            Settings.Default.DemoMode = this.demoMenuItem.Checked;
            Settings.Default.ShowSpotterOverlay = this.spotterOverlayModuleMenuItem.Checked;
            Settings.Default.ShowStartLights = this.startLightsModuleMenuItem.Checked;
            Settings.Default.UsbConnector = connectorSide;
            Settings.Default.MatrixLuma = matrixLuma;
            Settings.Default.Updates = updatesLevel;
            Settings.Default.Save();

            storeCommunication();
        }

                                                  // When the window moves,
                                                  // make its new position persistent.
        private void mainForm_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal) Settings.Default.WindowLocation = this.Location;
        }

                                                  // GUI
        private void optionsButton_Click(object sender, EventArgs e)
        {
            optionsMenu.Show(Cursor.Position);
        }


        private void gitMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(repositoryURL);
        }

        private void forumThreadMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(forumThreadURL);
        }

        private void demoTimer_Tick(object sender, EventArgs e)
        {
            showDemoFlag();
        }

        private void demoMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.Default.DemoMode = this.demoMenuItem.Checked;
            demoTimer.Stop();
            demoTimer.Start();
            showDemoFlag();
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
                if (!deviceUpdated())
                {
                    updateFirmware();
                }
                else
                {
                    if (!greeted)
                    {
                        greeted = true;
                        showSystemFlag(STARTUP_GREETING);
                        demoTimer.Enabled = true;
                        updateSoftware();
                    }
                    hardwareLight.BackColor = Color.FromName("ForestGreen");
                    commLabel.Text = "v" + firmwareVersionMajor + "." + firmwareVersionMinor + " @" + port;
                }
            }
            else
            {
                hardwareLight.BackColor = Color.FromName("Red");
                commLabel.Text = "";
            }

            detectSDK();
        }

                                                  // Visually indicates a running iRacing session
                                                  // in which demo mode gets disabled and telemetry update
                                                  // gets enabled
        private void indicateSimConnected(bool connected)
        {
            if (connected && !simConnected)
            {
                simLight.BackColor = Color.FromName("ForestGreen");

                demoMenuItem.Enabled = false;
                demoTimer.Enabled = false;
                updateTimer.Enabled = true;
            }
            else if (!connected && simConnected)
            {
                simLight.BackColor = Color.FromName("Red");

                demoMenuItem.Enabled = true;
                demoTimer.Enabled = true;
                updateTimer.Enabled = false;
                sessionDetected = false;
            }
            simConnected = connected;
        }

        private void connectorMenuItem_Click(object sender, EventArgs e)
        {
            switch (((ToolStripMenuItem)sender).Name)
            {
                case "connectorBottomMenuItem": connectorSide = 0x00; break;
                case "connectorRightMenuItem":  connectorSide = 0x01; break;
                case "connectorLeftMenuItem":   connectorSide = 0x02; break;
                case "connectorTopMenuItem":    connectorSide = 0x03; break;
            }
            connectorTopMenuItem.Checked = false;
            connectorLeftMenuItem.Checked = false;
            connectorRightMenuItem.Checked = false;
            connectorBottomMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;

            Settings.Default.UsbConnector = connectorSide;
            showSystemFlag(ORIENTATION_CHECK);
        }

        private void brightnessMenuItem_Click(object sender, EventArgs e)
        {
            switch (((ToolStripMenuItem)sender).Name)
            {
                case "fullBrightnessMenuItem":    matrixLuma = 100; break;
                case "highBrightnessMenuItem":    matrixLuma = 60; break;
                case "mediumBrightnessMenuItem":  matrixLuma = 25; break;
                case "lowBrightnessMenuItem":     matrixLuma = 10; break;
            }
            fullBrightnessMenuItem.Checked = false;
            highBrightnessMenuItem.Checked = false;
            mediumBrightnessMenuItem.Checked = false;
            lowBrightnessMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;

            Settings.Default.MatrixLuma = matrixLuma;
            setMatrixLuma();
            showSystemFlag(LUMA_CHECK);
        }

        private void updatesEnabledMenuItem_Click(object sender, EventArgs e)
        {
            if (updatesEnabledMenuItem.Checked)
            {
                updatesEnabledMenuItem.Checked = false;
                updatesExperimentalMenuItem.Enabled = false;
                updatesLevel = "none";
            }
            else
            {
                updatesEnabledMenuItem.Checked = true;
                updatesExperimentalMenuItem.Enabled = true;
                updatesLevel = updatesExperimentalMenuItem.Checked ? "experimental" : "stable";
            }
            Settings.Default.Updates = updatesLevel;
            updateSoftware();
        }

        private void updatesExperimentalMenuItem_Click(object sender, EventArgs e)
        {
            if (updatesExperimentalMenuItem.Checked)
            {
                updatesExperimentalMenuItem.Checked = false;
                updatesLevel = "stable";
            }
            else
            {
                updatesExperimentalMenuItem.Checked = true;
                updatesLevel = "experimental";
            }
            Settings.Default.Updates = updatesLevel;
            updateSoftware();
        }

        private void multiFlagMessageDismissButton_Click(object sender, EventArgs e)
        {
            multiFlagMessage.Hide();
            Settings.Default.AllowMultiple = true;
            Settings.Default.Save();
        }
    }
}
