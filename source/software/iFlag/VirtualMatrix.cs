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

        private ToolTip helperTip = new ToolTip();  // Tooltip object for UI controls


        public VirtualMatrix()
        {
            InitializeComponent();

            DotSizeX = DotSizes[DotSizeIndex].Width;
            DotSizeY = DotSizes[DotSizeIndex].Height;

            this.BackColor = Color.Black;

            shapeToggle.BackColor = Color.Black;
            shapeToggle.BackgroundImage = new Bitmap(DotShapes[nextDotShapeIndex()], DotSizes[DotSizeIndex]);
            shapeToggle.Size = DotSizes[0];
            shapeToggle.MouseClick += new MouseEventHandler(this.ChangeShape);
            helperTip.SetToolTip(shapeToggle, "Cycle dot shapes");

            sizeToggle.BackColor = Color.Black;
            sizeToggle.BackgroundImage = new Bitmap(DotShapes[DotShapeIndex], DotSizes[nextDotSizeIndex()]);
            sizeToggle.Size = DotSizes[0];
            sizeToggle.MouseClick += new MouseEventHandler(this.ChangeSize);
            helperTip.SetToolTip(sizeToggle, "Cycle dot sizes");
            
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
            ChangeShape(Settings.Default.DisplayDotShape);
            ChangeSize(Settings.Default.DisplayDotSize);
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
            Settings.Default.DisplayDotShape = DotShapeIndex;
            Settings.Default.DisplayDotSize = DotSizeIndex;
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

                                                    // Mouse handler for shape change UI control
        private void ChangeShape(object sender, MouseEventArgs e)
        {
            ChangeShape(nextDotShapeIndex());
        }

                                                    // Mouse handler for size change UI control
        private void ChangeSize(object sender, MouseEventArgs e)
        {
            ChangeSize(nextDotSizeIndex());
        }

                                                    // Changes shape of all matrix dots
        private void ChangeShape(int shape)
        {
            int nextShape = shape;
            int nextSize = nextDotSizeIndex();

            for (int f = 0; f < Pages; f++)
                MatrixMaskBoxes[f].BackgroundImage = new Bitmap(DotShapes[nextShape], DotSizes[DotSizeIndex]);

            DotShapeIndex = nextShape;

            sizeToggle.BackgroundImage = new Bitmap(DotShapes[nextShape], DotSizes[nextSize]);
            shapeToggle.BackgroundImage = new Bitmap(DotShapes[nextDotShapeIndex()], DotSizes[DotSizeIndex]);
        }

                                                    // Changes size of all matrix dots
        private void ChangeSize(int size)
        {
            DotSizeIndex = size;
            DotSizeX = DotSizes[DotSizeIndex].Width;
            DotSizeY = DotSizes[DotSizeIndex].Height;

            int nextShape = nextDotShapeIndex();
            int nextSize = nextDotSizeIndex();

            for (int f = 0; f < Pages; f++)
                MatrixMaskBoxes[f].BackgroundImage = PageFrames[f] = new Bitmap(Xs * DotSizeX, Ys * DotSizeY);

            sizeToggle.BackgroundImage = new Bitmap(DotShapes[DotShapeIndex], DotSizes[nextSize]);
            shapeToggle.BackgroundImage = new Bitmap(DotShapes[nextShape], DotSizes[DotSizeIndex]);
        }

                                                    // Returns an index of the next dot shape in the cycle
        private int nextDotShapeIndex()
        {
            int index = DotShapeIndex + 1;
            if (index >= DotShapes.Length) index = 0;
            return index;
        }

                                                    // Returns an index of the next dot size in the cycle
        private int nextDotSizeIndex()
        {
            int index = DotSizeIndex + 1;
            if (index >= DotSizes.Length) index = 0;
            return index;
        }
   }
}
