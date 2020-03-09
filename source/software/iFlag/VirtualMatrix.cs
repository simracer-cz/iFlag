using System;
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

        private int DotSizeX;                       // Width of the chosen matrix dot size
        private int DotSizeY;                       // Height of the chosen matrix dot size
        private int DotSizeIndex = 0;               // Index of the chosen matrix dot sizr
        private Size[] DotSizes = new Size[]        // Bank of available dot sizes
        {
            new Size(30, 30),                       // 30 * 30px
            new Size(24, 24),                       // 24 * 24px
            new Size(20, 20),                       // 20 * 20px
        };

        public VirtualMatrix()
        {
            InitializeComponent();

            DotSizeX = DotSizes[DotSizeIndex].Width;
            DotSizeY = DotSizes[DotSizeIndex].Height;
        }
    }
}
