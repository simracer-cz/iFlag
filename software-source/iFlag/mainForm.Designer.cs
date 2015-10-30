namespace iFlag
{
    partial class mainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.optionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectorTopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectorLeftMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectorRightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectorBottomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOnTopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.commLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.hardwareLight = new System.Windows.Forms.Label();
            this.timeoutTimer = new System.Windows.Forms.Timer(this.components);
            this.connectTimer = new System.Windows.Forms.Timer(this.components);
            this.simLight = new System.Windows.Forms.Label();
            this.flagLabel = new System.Windows.Forms.Label();
            this.optionsMenu.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionsMenu
            // 
            this.optionsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alwaysOnTopMenuItem,
            this.appMenuItem});
            this.optionsMenu.Name = "optionsMenu";
            this.optionsMenu.Size = new System.Drawing.Size(155, 92);
            // 
            // 
            // connectorMenuItem
            // 
            this.connectorMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectorTopMenuItem,
            this.connectorLeftMenuItem,
            this.connectorRightMenuItem,
            this.connectorBottomMenuItem});
            this.connectorMenuItem.Name = "connectorMenuItem";
            this.connectorMenuItem.Size = new System.Drawing.Size(154, 22);
            this.connectorMenuItem.Text = "USB Connector";
            this.connectorMenuItem.ToolTipText = "Configure here how is your Arduino unit assembly oriented";
            // 
            // connectorTopMenuItem
            // 
            this.connectorTopMenuItem.CheckOnClick = true;
            this.connectorTopMenuItem.Name = "connectorTopMenuItem";
            this.connectorTopMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectorTopMenuItem.Text = "Top";
            this.connectorTopMenuItem.Click += new System.EventHandler(this.connectorMenuItem_Click);
            // 
            // connectorLeftMenuItem
            // 
            this.connectorLeftMenuItem.CheckOnClick = true;
            this.connectorLeftMenuItem.Name = "connectorLeftMenuItem";
            this.connectorLeftMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectorLeftMenuItem.Text = "Left";
            this.connectorLeftMenuItem.Click += new System.EventHandler(this.connectorMenuItem_Click);
            // 
            // connectorRightMenuItem
            // 
            this.connectorRightMenuItem.CheckOnClick = true;
            this.connectorRightMenuItem.Name = "connectorRightMenuItem";
            this.connectorRightMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectorRightMenuItem.Text = "Right";
            this.connectorRightMenuItem.Click += new System.EventHandler(this.connectorMenuItem_Click);
            // 
            // connectorBottomMenuItem
            // 
            this.connectorBottomMenuItem.CheckOnClick = true;
            this.connectorBottomMenuItem.Name = "connectorBottomMenuItem";
            this.connectorBottomMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectorBottomMenuItem.Text = "Bottom";
            this.connectorBottomMenuItem.Click += new System.EventHandler(this.connectorMenuItem_Click);
            // 
            // alwaysOnTopMenuItem
            // 
            this.alwaysOnTopMenuItem.Checked = true;
            this.alwaysOnTopMenuItem.CheckOnClick = true;
            this.alwaysOnTopMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alwaysOnTopMenuItem.Name = "alwaysOnTopMenuItem";
            this.alwaysOnTopMenuItem.Size = new System.Drawing.Size(174, 22);
            this.alwaysOnTopMenuItem.Text = "Always on Top";
            this.alwaysOnTopMenuItem.ToolTipText = "Keep this window on top of other windows";
            this.alwaysOnTopMenuItem.CheckStateChanged += new System.EventHandler(this.alwaysOnTopMenuItem_CheckStateChanged);
            // 
            // appMenuItem
            // 
            this.appMenuItem.Enabled = false;
            this.appMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.appMenuItem.Name = "appMenuItem";
            this.appMenuItem.Size = new System.Drawing.Size(174, 22);
            this.appMenuItem.Text = "iFlag vX.Y";
            // 
            // optionsButton
            // 
            this.optionsButton.Location = new System.Drawing.Point(214, 8);
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Size = new System.Drawing.Size(59, 23);
            this.optionsButton.TabIndex = 1;
            this.optionsButton.Text = "Options";
            this.optionsButton.UseVisualStyleBackColor = true;
            this.optionsButton.Click += new System.EventHandler(this.optionsButton_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 38);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip.Size = new System.Drawing.Size(281, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // commLabel
            // 
            this.commLabel.Margin = new System.Windows.Forms.Padding(41, 3, 0, 2);
            this.commLabel.Name = "commLabel";
            this.commLabel.Size = new System.Drawing.Size(78, 17);
            this.commLabel.Text = "Connecting...";
            // 
            // hardwareLight
            // 
            this.hardwareLight.BackColor = System.Drawing.Color.Red;
            this.hardwareLight.ForeColor = System.Drawing.Color.White;
            this.hardwareLight.Location = new System.Drawing.Point(1, 39);
            this.hardwareLight.Name = "hardwareLight";
            this.hardwareLight.Size = new System.Drawing.Size(41, 20);
            this.hardwareLight.TabIndex = 3;
            this.hardwareLight.Text = "Matrix";
            this.hardwareLight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeoutTimer
            // 
            this.timeoutTimer.Interval = 5000;
            this.timeoutTimer.Tick += new System.EventHandler(this.timeoutTimer_Tick);
            // 
            // connectTimer
            // 
            this.connectTimer.Enabled = true;
            this.connectTimer.Interval = 3000;
            this.connectTimer.Tick += new System.EventHandler(this.connectTimer_Tick);
            // 
            // simLight
            // 
            this.simLight.BackColor = System.Drawing.Color.Red;
            this.simLight.ForeColor = System.Drawing.Color.White;
            this.simLight.Location = new System.Drawing.Point(227, 39);
            this.simLight.Name = "simLight";
            this.simLight.Size = new System.Drawing.Size(53, 20);
            this.simLight.TabIndex = 4;
            this.simLight.Text = "iRacing";
            this.simLight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flagLabel
            // 
            this.flagLabel.AutoSize = true;
            this.flagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.flagLabel.Location = new System.Drawing.Point(61, 9);
            this.flagLabel.Name = "flagLabel";
            this.flagLabel.Size = new System.Drawing.Size(39, 18);
            this.flagLabel.TabIndex = 5;
            this.flagLabel.Text = "iFlag";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 60);
            this.Controls.Add(this.flagLabel);
            this.Controls.Add(this.simLight);
            this.Controls.Add(this.hardwareLight);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.optionsButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "iFlag";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.Move += new System.EventHandler(this.mainForm_Move);
            this.optionsMenu.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip optionsMenu;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopMenuItem;
        private System.Windows.Forms.Button optionsButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel commLabel;
        private System.Windows.Forms.Label hardwareLight;
        private System.Windows.Forms.Timer timeoutTimer;
        private System.Windows.Forms.Timer connectTimer;
        private System.Windows.Forms.Label simLight;
        private System.Windows.Forms.ToolStripMenuItem connectorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectorTopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectorLeftMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectorRightMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectorBottomMenuItem;
        private System.Windows.Forms.Label flagLabel;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem;
        private System.Windows.Forms.ToolStripSeparator optionsMenuSeparator;
    }
}

