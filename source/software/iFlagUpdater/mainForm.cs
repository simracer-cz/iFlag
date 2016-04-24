using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace iFlagUpdater
{
    public partial class mainForm : Form
    {
        string updatesLevel = "";
        string updateURL = "";

        int[] windowLocation = { 0, 0 };
        string version = "";

        public mainForm(string[] args)
        {
            if (args.Length != 5)
            {
                Application.Exit();
                return;
            }

            version = args[0];
            updateURL = args[1];
            updatesLevel = args[2];
            windowLocation[0] = Convert.ToInt32(args[3]);
            windowLocation[1] = Convert.ToInt32(args[4]);

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
        }
    }
}
