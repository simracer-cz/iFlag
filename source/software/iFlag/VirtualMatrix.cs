using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using iFlag.Properties;

namespace iFlag
{
    public partial class VirtualMatrix : Form
    {
        public Color[] COLORS = new Color[16]
        {
            Color.Black,            // 0x00 | black
            Color.White,            // 0x01 | white
            Color.Red,              // 0x02 | red
            Color.Lime,             // 0x03 | green
            Color.Blue,             // 0x04 | blue
            Color.Yellow,           // 0x05 | yellow
            Color.LightSeaGreen,    // 0x06 | teal
            Color.Fuchsia,          // 0x07 | purple
            Color.DarkOrange,       // 0x08 | orange
            Color.LightGray,        // 0x09 | dim white
            Color.Firebrick,        // 0x10 | dim red
            Color.Green,            // 0x11 | dim green
            Color.DarkBlue,         // 0x12 | dim blue
            Color.Gold,             // 0x13 | dim yellow
            Color.Teal,             // 0x14 | dim teal
            Color.Purple,           // 0x15 | dim purple
        };

        private int Xs = 8;                         // Number of matrix dots horizontally
        private int Ys = 8;                         // Number of matrix dots vertically

        static private int Pages = 2;               // Number of matrix page frames

        private byte[, ,] Matrix = new byte[2, 8, 8];

        private int DotShapeIndex = 0;              // Index of the chosen matrix dot shape
        private Bitmap[] DotShapes = new Bitmap[]   // Bank of available matrix dot shapes
        {
            global::iFlag.Properties.Resources.matrixDotCircle,
            global::iFlag.Properties.Resources.matrixDotSquare,
            global::iFlag.Properties.Resources.matrixDotDiamond,
        };

        private int DotSizeX;                       // Width of the chosen matrix dot size
        private int DotSizeY;                       // Height of the chosen matrix dot size
        private int DotSizeIndex = 0;               // Index of the chosen matrix dot sizr
        private Size[] DotSizes = new Size[]        // Bank of available dot sizes
        {
            new Size(30, 30),                       // 30 * 30px
            new Size(24, 24),                       // 24 * 24px
            new Size(20, 20),                       // 20 * 20px
        };

                                                    // Array of bitmaps representing the matrix page frames
        private Bitmap[] PageFrames = new Bitmap[Pages];

                                                    // Arrays of the physical LED dots and their masks
        private PictureBox[] MatrixLedBoxes = new PictureBox[Pages];
        private PictureBox[] MatrixMaskBoxes = new PictureBox[Pages];

        private bool IsWindowDragging;              // Flags window being mouse-dragged
        private Point LastLocation;                 // Holds last window location at the start of drag

        public VirtualMatrix()
        {
            InitializeComponent();

            DotSizeX = DotSizes[DotSizeIndex].Width;
            DotSizeY = DotSizes[DotSizeIndex].Height;

            this.BackColor = Color.Black;

            for (int f = 0; f < Pages; f++)
            {
                MatrixLedBoxes[f] = new PictureBox();
                MatrixLedBoxes[f].MouseDown += new MouseEventHandler(this.Drag_MouseDown);
                MatrixLedBoxes[f].MouseMove += new MouseEventHandler(this.Drag_MouseMove);
                MatrixLedBoxes[f].MouseUp += new MouseEventHandler(this.Drag_MouseUp);

                MatrixMaskBoxes[f] = new PictureBox();
                MatrixMaskBoxes[f].BackgroundImage = new Bitmap(DotShapes[DotShapeIndex], DotSizes[DotSizeIndex]);
                MatrixMaskBoxes[f].BackColor = Color.Transparent;
                MatrixMaskBoxes[f].MouseDown += new MouseEventHandler(this.Drag_MouseDown);
                MatrixMaskBoxes[f].MouseMove += new MouseEventHandler(this.Drag_MouseMove);
                MatrixMaskBoxes[f].MouseUp += new MouseEventHandler(this.Drag_MouseUp);

                matrixBox.Controls.Add(MatrixLedBoxes[f]);
                MatrixLedBoxes[f].Controls.Add(MatrixMaskBoxes[f]);

                PageFrames[f] = new Bitmap(Xs * DotSizeX, Ys * DotSizeY);
            }
        }

                                                    // Executes on load of the window to restore matrix
                                                    // settings from the persistent storage
        private void VirtualMatrix_Load(object sender, EventArgs e)
        {
            if (!Settings.Default.DisplayWindowLocation.IsEmpty)
            {
                this.Location = Settings.Default.DisplayWindowLocation;
            }
        }

                                                    // Executes when leaving the app to persistently store
                                                    // window's settings
        private void VirtualMatrix_Close(object sender, FormClosingEventArgs e)
        {
            Settings.Default.DisplayWindowLocation = this.Location;
            Settings.Default.Save();
        }

                                                    // Holds the window location prior the drag
        private void Drag_MouseDown(object sender, MouseEventArgs e)
        {
            IsWindowDragging = true;
            LastLocation = e.Location;
        }

                                                    // Adjusts the window location based on the drag movement
        private void Drag_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsWindowDragging)
            {
                this.Location = new Point((this.Location.X - LastLocation.X) + e.X, (this.Location.Y - LastLocation.Y) + e.Y);
                this.Update();
            }
        }

                                                    // Stop dragging the window on mouse release
        private void Drag_MouseUp(object sender, MouseEventArgs e)
        {
            IsWindowDragging = false;
        }

                                                    // Saves window location on window drag
        private void SaveLocation(object sender, EventArgs e)
        {
            Settings.Default.DisplayWindowLocation = this.Location;
            Settings.Default.Save();
        }
   }
}
