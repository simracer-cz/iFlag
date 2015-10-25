using System;
using System.Windows.Forms;

using iFlag.Properties;

namespace iFlag
{
    public partial class mainForm : Form
    {
        byte[, ,] matrix = new byte[2, 8, 8];     // The physical matrix buffer to hold the flag in
        bool blinkSpeed;                          // Blinking speed of the pattern
        const bool SLOW = false;                  // Symbol of slow blinking
        const bool FAST = true;                   // Symbol of fast blinking

                                                  // On what side of the matrix does the Arduino
                                                  // USB connector sticks out. Persistent user option.
        byte connectorSide = Settings.Default.UsbConnector;

                                                  // Indexes of colors usable in flag patterns, which
                                                  // match the v0.15 firmware palette, so don't change.
        const byte COLOR_BLACK = 0;
        const byte COLOR_WHITE = 1;
        const byte COLOR_RED = 2;
        const byte COLOR_GREEN = 3;
        const byte COLOR_BLUE = 4;
        const byte COLOR_YELLOW = 5;
        const byte COLOR_TEAL = 6;
        const byte COLOR_PURPLE = 7;
        const byte COLOR_ORANGE = 8;
        const byte COLOR_DIM_WHITE = 10;
        const byte COLOR_DIM_RED = 11;
        const byte COLOR_DIM_BLUE = 12;

        private void startMatrix()
        {
        }

                                                  // Translates the flag pattern to color data
                                                  // and feeds then to the matrix buffer
                                                  // to be broadcasted right away.
        public void flagToMatrix(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            int matrixX = 0, matrixY = 0;

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                                                  // A matrix rotation is performed based on in what
                                                  // direction is Arduino USB connector sticking out
                                                  // of the hardware assembly.
                    switch (connectorSide)
                    {
                        case 0x00:                // Down
                            matrixX = 8 - y - 1;
                            matrixY = x;
                            break;
                        case 0x01:                // Right
                            matrixX = 8 - x - 1;
                            matrixY = 8 - y - 1;
                            break;
                        case 0x02:                // Left
                            matrixX = x;
                            matrixY = y;
                            break;
                        case 0x03:                // Up
                            matrixX = y;
                            matrixY = 8 - x - 1;
                            break;
                    }
                    matrixX = x;
                    matrixY = y;
                    matrix[0, x, y] = color[pattern[0, matrixX, matrixY]];
                                                  // For single-frame patterns the second frame of the matrix
                                                  // is a clone of the first one.
                    matrix[1, x, y] = color[pattern[pattern.Length >= 128 ? 1 : 0, matrixX, matrixY]];
                }

            blinkSpeed = speed;
        }
    }
}
