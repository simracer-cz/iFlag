namespace iFlag
{
    partial class VirtualMatrix
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
            this.pageFlipTimer = new System.Windows.Forms.Timer(this.components);
            this.sizeToggle = new System.Windows.Forms.PictureBox();
            this.shapeToggle = new System.Windows.Forms.PictureBox();
            this.logoPicture = new System.Windows.Forms.PictureBox();
            this.matrixBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.sizeToggle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shapeToggle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pageFlipTimer
            // 
            this.pageFlipTimer.Enabled = true;
            this.pageFlipTimer.Interval = 100;
            this.pageFlipTimer.Tick += new System.EventHandler(this.pageFlipTimer_Tick);
            // 
            // sizeToggle
            // 
            this.sizeToggle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.sizeToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sizeToggle.Location = new System.Drawing.Point(0, 240);
            this.sizeToggle.Name = "sizeToggle";
            this.sizeToggle.Size = new System.Drawing.Size(30, 30);
            this.sizeToggle.TabIndex = 4;
            this.sizeToggle.TabStop = false;
            // 
            // shapeToggle
            // 
            this.shapeToggle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.shapeToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.shapeToggle.Location = new System.Drawing.Point(210, 240);
            this.shapeToggle.Name = "shapeToggle";
            this.shapeToggle.Size = new System.Drawing.Size(30, 30);
            this.shapeToggle.TabIndex = 3;
            this.shapeToggle.TabStop = false;
            // 
            // logoPicture
            // 
            this.logoPicture.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.logoPicture.Image = global::iFlag.Properties.Resources.matrixLogo;
            this.logoPicture.Location = new System.Drawing.Point(85, 240);
            this.logoPicture.Name = "logoPicture";
            this.logoPicture.Size = new System.Drawing.Size(69, 30);
            this.logoPicture.TabIndex = 2;
            this.logoPicture.TabStop = false;
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(0, 0);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(240, 240);
            this.matrixBox.TabIndex = 0;
            this.matrixBox.TabStop = false;
            // 
            // VirtualMatrix
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(240, 270);
            this.ControlBox = false;
            this.Controls.Add(this.sizeToggle);
            this.Controls.Add(this.shapeToggle);
            this.Controls.Add(this.logoPicture);
            this.Controls.Add(this.matrixBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VirtualMatrix";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "iFLAG Virtual Matrix";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VirtualMatrix_Close);
            this.Load += new System.EventHandler(this.VirtualMatrix_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sizeToggle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shapeToggle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Timer pageFlipTimer;
        private System.Windows.Forms.PictureBox matrixBox;
        private System.Windows.Forms.PictureBox sizeToggle;
        private System.Windows.Forms.PictureBox shapeToggle;
        private System.Windows.Forms.PictureBox logoPicture;
    }
}