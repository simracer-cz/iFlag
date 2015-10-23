using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using iFlag.Properties;

namespace iFlag
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.Location = Settings.Default.WindowLocation;
            this.WindowState = Settings.Default.WindowState;
            this.TopMost = this.alwaysOnTopMenuItem.Checked = Settings.Default.WindowTopMost;
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal) Settings.Default.WindowLocation = this.Location;
            Settings.Default.WindowTopMost = this.TopMost;
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
    }
}
