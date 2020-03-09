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
        private int Page;                           // Currently displayed page index

                                                    // Structure physically holding the matrix data
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

                                                    // Pixels adding to the overall height of the otherwise square matrix
                                                    // making space and housing the UI controls under the matrix
        private int AdditionalHeight = 30;          

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

                                                    // Physically size the matrix and all its components
                                                    // to match the user-selected dot size
        public void SetSize()
        {
            int width = Xs * DotSizeX;
            int height = Ys * DotSizeY;
            Size matrixSize = new Size(width, height);

            this.matrixBox.Visible = logoPicture.Visible = false;
            
            this.Location = new Point(this.Location.X, this.Location.Y + (this.ClientSize.Height - height - AdditionalHeight));
            this.ClientSize = new Size(width, height + AdditionalHeight);

            matrixBox.Size = new Size(width, height);

            for (int frame = 0; frame < Pages; frame++)
            {
                MatrixMaskBoxes[frame].Size = matrixBox.Size;
                MatrixLedBoxes[frame].Size = matrixBox.Size;
                MatrixLedBoxes[frame].Image = new Bitmap(PageFrames[frame], matrixBox.Size);
            }

            sizeToggle.Location = new Point(0, height);
            logoPicture.Location = new Point(width / 2 - logoPicture.Size.Width / 2, height);
            shapeToggle.Location = new Point(width - AdditionalHeight, height);

            this.matrixBox.Visible = logoPicture.Visible = true;
        }

                                                    // Method to pass matrix array to the virtual matrix
        public void SetMatrix(byte[, ,] matrix)
        {
            Matrix = matrix;
            PaintMatrix();
        }

                                                    // Goes over each matrix dot to actually paint it
        public void PaintMatrix()
        {
            for (int f = 0; f < 2; f++)
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        PaintDot(f, x, y, Matrix[f, 8 - y - 1, x]);
        }

                                                    // Paints a single dot of the matrix by paining a representation
                                                    // of an actual RGB LED chip with its 3 independent color components
        public void PaintDot(int frame, int x, int y, int color)
        {
            using (Graphics g = Graphics.FromImage(PageFrames[frame]))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.CompositingQuality = CompositingQuality.HighSpeed;

                int X = x * DotSizeX;
                int Y = (Ys - y - 1) * DotSizeY;

                Color R, G, B;
                Color col = COLORS[color];

                int chipWidth = DotSizeX / 10;
                int chipHeight = 1;
                int chipX = X + DotSizeX / 2 - chipWidth/2;
                int chipY = Y + DotSizeY / 2 - chipHeight/2;

                g.FillRectangle(new SolidBrush(col), new Rectangle(X, Y, DotSizeX, DotSizeY));

                if (color == 0)
                {
                                                    // only faintly visible chips when black
                    R = G = B = Color.FromArgb(255, 24, 24, 24);
                }
                else
                {
                                                    // individual chips illuminated to match the color
                    R = Color.FromArgb(Math.Min(col.R, (byte)190), 255, col.R / 3, col.R / 3);
                    G = Color.FromArgb(Math.Min(col.G, (byte)190), col.G / 3, 255, col.G / 3);
                    B = Color.FromArgb(Math.Min(col.B, (byte)190), col.B / 3, col.B / 3, 255);
                }

                g.FillRectangle(new SolidBrush(R), new Rectangle(chipX, chipY - 1, chipWidth, chipHeight));
                g.FillRectangle(new SolidBrush(G), new Rectangle(chipX, chipY + 0, chipWidth, chipHeight));
                g.FillRectangle(new SolidBrush(B), new Rectangle(chipX, chipY + 1, chipWidth, chipHeight));
            }

                                                    // Place the LED mask on top of the LEDs
                                                    // once last frame of last page is painted
            if (frame == Pages - 1 && x == 7 && y == 7)
            {
                for (int f = 0; f < Pages; f++)
                {
                    MatrixLedBoxes[f].Image = PageFrames[f];
                    MatrixMaskBoxes[f].BackgroundImage = new Bitmap(DotShapes[DotShapeIndex], DotSizes[DotSizeIndex]);
                    matrixBox.Controls.SetChildIndex(MatrixLedBoxes[f], f + 1);
                }
            }
        }

                                                    // On every page flip timer tick,
                                                    // flip the page by bringing it to foreground
        private void pageFlipTimer_Tick(object sender, EventArgs e)
        {
            Page = Page+++1 < Pages ? Page : 0;
            matrixBox.Controls.SetChildIndex(MatrixLedBoxes[Page], 1);
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
                                                    // and calls for overall matrix resize
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

            PaintMatrix();
            SetSize();
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
