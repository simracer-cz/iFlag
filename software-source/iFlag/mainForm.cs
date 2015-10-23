using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            optionsMenu.Show(Cursor.Position);
        }
    }
}
