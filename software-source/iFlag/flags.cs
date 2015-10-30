using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        uint flagOnDisplay = 47652875;            // Currently displayed flag; initiated with "random" number
        int demoFlagIndex;                        // Current demo flag index in a list of falgs to cycle through

                                                  // Subset of flags for the demo sequence
        uint[] demoFlags = {
            irsdk_green,
            irsdk_checkered,
            irsdk_yellowWaving,
            irsdk_caution,
            irsdk_red,
            irsdk_white,
            irsdk_blue,
            irsdk_debris,
            irsdk_disqualify,
            irsdk_repair,
            irsdk_furled,
        };
        uint[] noDemoFlags = {
            NO_FLAG,
        };

                                                  // Special purpose system-level "flags"
        const uint STARTUP_GREETING = 6666;
        const uint NO_FLAG = 7777;
        const uint ORIENTATION_CHECK = 8888;

        private void startFlags()
        {
        }

                                                  // Accepts a flag identifier number
                                                  // and matches it against a bank of known flags/signals
                                                  // Returns true if matched, false otherwise.
        private bool showFlag(uint flagID)
        {
            if (flagID != flagOnDisplay)
            {
                flagOnDisplay = flagID;

                if (matchSystemFlags(flagOnDisplay)) return broadcastMatrix();
                else if (matchRacingFlags(flagOnDisplay)) return broadcastMatrix();
                else if (matchStartingFlags(flagOnDisplay)) return broadcastMatrix();
            }
            return false;
        }

                                                  // Try to match given flag ID against racing flag constants
        private bool matchRacingFlags(uint flagID)
        {
                 if (Convert.ToBoolean(flagID & irsdk_crossed)
                  || Convert.ToBoolean(flagID & irsdk_disqualify)) return flag("Disqualify flag", CROSSED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_green)) return flag("Green flag", SIMPLE_FLAG, new byte[] { COLOR_GREEN, COLOR_GREEN }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellow)) return flag("Yellow flag", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_red)) return flag("Red flag", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_RED }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_blue)) return flag("Blue flag", DIAGONAL_STRIPE_FLAG, new byte[] { COLOR_BLUE, COLOR_YELLOW }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_debris)) return flag("Debris flag", STRIPPED_FLAG, new byte[] { COLOR_YELLOW, COLOR_RED }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellowWaving)) return flag("Yellow Waving flag", WAVING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_repair)) return flag("Meat Ball flag", MEATBALL_FLAG, new byte[] { COLOR_BLACK, COLOR_ORANGE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_black)) return flag("Black flag", INVERTED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_furled)) return flag("Furled Black flag", FURLED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE, COLOR_BLACK }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_checkered)) return flag("Checkered flag", CHECKERED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_white)) return flag("White flag", SIMPLE_FLAG, new byte[] { COLOR_WHITE, COLOR_BLACK }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_greenHeld)
                  || Convert.ToBoolean(flagID & irsdk_caution)
                  || Convert.ToBoolean(flagID & irsdk_cautionWaving)) return flag("Caution flag", SAFETYCAR_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_startHidden)) return flag("None", SIMPLE_FLAG, new byte[] { COLOR_BLACK, COLOR_BLACK }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_oneLapToGreen)) return flag("One To Green", INVERTED_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
                 return false;
        }

                                                  // Try to match given flag ID against extra flag constants
                                                  // serving as a starting lights for both standing
                                                  // and rolling starts
        private bool matchStartingFlags(uint flagID)
        {
            if (!this.startLightsModuleMenuItem.Checked) return false;

                 if (Convert.ToBoolean(flagID & irsdk_startReady)) return flag("Startlights: Ready!", HALF_FLAG, new byte[] { COLOR_RED, COLOR_BLACK }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_startSet)) return flag("Startlights: Set!", SIMPLE_FLAG, new byte[] { COLOR_RED, COLOR_RED }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_startGo)) return flag("Startlights: Go!", SIMPLE_FLAG, new byte[] { COLOR_GREEN, COLOR_GREEN }, FAST);
            else return false;
        }

                                                  // Ruleset of "flags" or flag signals used for system purposes
                                                  // Returns true if flag matched, false otherwise.
        private bool matchSystemFlags(uint flagID)
        {
            if (flagID == NO_FLAG) return flag("---", SIMPLE_FLAG, new byte[] { COLOR_BLACK, COLOR_BLACK }, SLOW);
            else if (flagID == ORIENTATION_CHECK) return flag("Letter \"F\" check!", F_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
            else if (flagID == STARTUP_GREETING) return flag("Let's go!", F_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
            else return false;
        }

                                                  // Pour the specified flag into the matrix awaiting boradcast
                                                  // logging the flags with time codes into console
                                                  // (^^ this might be eventually going into a file in the future.)
        public bool flag(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            flagLabel.Text = flagName;
            Console.WriteLine(DateTime.Now + " " + flagName);
            flagToMatrix(flagName, pattern, color, speed);
            return true;
        }

                                                  // Advances the demo flag picking cycle
        private bool showDemoFlag()
        {
            uint[] flags = demoMenuItem.Checked ? demoFlags : noDemoFlags;
            if (demoFlagIndex++ >= flags.Length) demoFlagIndex = 1;
            return showFlag(flags[demoFlagIndex - 1]);
        }

                                                  // Takes a moment to ensure a system flag
                                                  // gets displayed long enough before it gets overruled
                                                  // by timeouts
        private void showSystemFlag(uint flag = ORIENTATION_CHECK)
        {
            if (demoTimer.Enabled)
            {
                demoTimer.Stop();
                demoTimer.Start();
            }
            if (updateTimer.Enabled)
            {
                updateTimer.Stop();
                updateTimer.Start();
            }
            flagOnDisplay = NO_FLAG;
            showFlag(flag);
        }

                                                  // Relentlessly queries the iRacing SDK's real time telemetry
                                                  // for flags and other data and displays the signals
                                                  // when appropriate
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (sdk.IsConnected())
                {
                    bool onTrack = !Convert.ToBoolean(sdk.GetData("IsReplayPlaying"));
                    if (onTrack)
                    {
                        uint flag = Convert.ToUInt32(sdk.GetData("SessionFlags"));
                        showFlag(flag);
                    }
                    else
                    {
                        showFlag(NO_FLAG);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FLAG UPDATE FAILED");
                Console.WriteLine(ex);
            }
        }

                                                  // Transforms the given pattern string
                                                  // into a three-dimensional byte array of color indexes
                                                  // which further get replaced by an actual color
        static private byte[,,] pattern(String flagPattern)
        {
            byte[,,] patternMatrix = new byte[2, 8, 8];
            String[] patternSplit = flagPattern.Split();
            int pixel;
            for (int frame = 0; frame < 2; frame++)
            for (int pixelX = 0; pixelX < 8; pixelX++)
            for (int pixelY = 0; pixelY < 8; pixelY++)
            {
                int pixelFrame = (frame == 1 && patternSplit.Length == 16) ? 1 : 0;
                pixel = Convert.ToInt32(patternSplit[pixelY + pixelFrame * 8][pixelX]) - 48;
                patternMatrix[frame, pixelY, pixelX] = Convert.ToByte(pixel);
            }
            return patternMatrix;
        }

                                                  // Graphical patterns, often easily reusable
                                                  // geometric primitives
        byte[,,] SIMPLE_FLAG =            pattern("00000000 00000000 00000000 00011000 00011000 00000000 00000000 00000000");
        byte[,,] SQUARE_FLAG =            pattern("00000000 00000000 00111100 00111100 00111100 00111100 00000000 00000000");
        byte[,,] HALF_FLAG =              pattern("00000000 00000000 00000000 00000000 11111111 11111111 11111111 11111111");
        byte[,,] FURLED_FLAG =            pattern("11111111 01222221 00122221 00012221 00001221 00000121 00000011 00000001 " +
                                                  "00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000");
        byte[,,] INVERTED_FLAG =          pattern("11111111 10000001 10000001 10000001 10000001 10000001 10000001 11111111 " +
                                                  "00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000");
        byte[,,] FLASHING_FLAG =          pattern("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 " +
                                                  "00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000");
        byte[,,] WAVING_FLAG =            pattern("00001111 00001111 00001111 00001111 00001111 00001111 00001111 00001111 " +
                                                  "11110000 11110000 11110000 11110000 11110000 11110000 11110000 11110000");
        byte[,,] DOUBLE_WAVING_FLAG =     pattern("11110000 11110000 11110000 11110000 00001111 00001111 00001111 00001111 " +
                                                  "00001111 00001111 00001111 00001111 11110000 11110000 11110000 11110000");
        byte[,,] CHECKERED_FLAG =         pattern("11001100 11001100 00110011 00110011 11001100 11001100 00110011 00110011 " +
                                                  "00110011 00110011 11001100 11001100 00110011 00110011 11001100 11001100");
        byte[,,] STRIPPED_FLAG =          pattern("11001100 11001100 11001100 11001100 11001100 11001100 11001100 11001100 " +
                                                  "00110011 00110011 00110011 00110011 00110011 00110011 00110011 00110011");
        byte[,,] CIRCLE_FLAG =            pattern("00111100 01122110 11222211 12222221 12222221 11222211 01122110 00111100");
        byte[,,] CROSSED_FLAG =           pattern("11000011 11100111 01111110 00111100 00111100 01111110 11100111 11000011");
        byte[,,] DIAGONAL_STRIPE_FLAG =   pattern("00000011 00000111 00001110 00011100 00111000 01110000 11100000 11000000");
        byte[,,] MEATBALL_FLAG =          pattern("00000000 00011000 00111100 01111110 01111110 00111100 00011000 00000000 " +
                                                  "00000000 00000000 00011000 00111100 00111100 00011000 00000000 00000000");
        byte[,,] SAFETYCAR_FLAG =         pattern("01110222 11112222 11002200 11102200 01112200 00112200 11112222 11100222 " +
                                                  "02220111 22221111 22001100 22201100 02221100 00221100 22221111 22200111");

        byte[,,] F_FLAG =                 pattern("11111111 11000011 11011111 11000011 11011111 11011111 11011111 11111111");
        byte[,,] STATUS_FLAG =            pattern("00000000 00000000 00000000 00000000 00000000 00000000 00000000 10000002");
        byte[,,] IRACING_LOGO_FLAG =      pattern("01111110 11333331 12111311 12211131 12211331 12211331 12211331 01111110");

    }
}
