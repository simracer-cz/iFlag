using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace iFlagUpdater
{
    public partial class mainForm : Form
    {
        int[] windowLocation = { 0, 0 };
        string version = "";
        string updateVersion = "";

        public mainForm(string[] args)
        {
            if (args.Length != 4)
            {
                Application.Exit();
                return;
            }

            version = args[0];
            updateVersion = args[1];
            windowLocation[0] = Convert.ToInt32(args[2]);
            windowLocation[1] = Convert.ToInt32(args[3]);

            InitializeComponent();
        }

                                                  // When the window opens,
                                                  // set its location the same as caller's app
        private void mainForm_Load(object sender, EventArgs e)
        {
            System.Drawing.Point place = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Location;
            place.X += windowLocation[0];
            place.Y += windowLocation[1];
            this.Location = place;

            beforeLabel.Text = version;
            afterLabel.Text = updateVersion;
        }
    }
}
