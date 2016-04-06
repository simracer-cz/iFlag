using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        long flagOnDisplay = 47652875;            // Currently displayed flag; initiated with "random" number
        int demoFlagIndex;                        // Current demo flag index in a list of falgs to cycle through

                                                  // Subset of flags for the demo sequence
        long[] demoFlags = {
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
        long[] noDemoFlags = {
            NO_FLAG,
        };

                                                  // Special purpose system-level "flags"
        const long STARTUP_GREETING = 6666;
        const long NO_FLAG = 7777;
        const long ORIENTATION_CHECK = 8888;
        const long LUMA_CHECK = 8887;

        long clearFlag = NO_FLAG;

        private void startFlags()
        {
            flagOnDisplay = NO_FLAG;
            CAUTION_FLAG = SAFETYCAR_FLAG;
        }

                                                  // Accepts a flag identifier number
                                                  // and matches it against a bank of known flags/signals
                                                  // Returns true if matched, false otherwise.
        private bool showFlag(long flagID)
        {
            if (flagID != flagOnDisplay)
            {
                flagOnDisplay = flagID;

                if (matchSystemFlags(flagOnDisplay)) return broadcastMatrix();
                else if (matchStartingFlags(flagOnDisplay)) return broadcastMatrix();
                else if (matchRacingFlags(flagOnDisplay)) return broadcastMatrix();
            }
            return false;
        }

                                                  // Try to match given flag ID against racing flag constants
        private bool matchRacingFlags(long flagID)
        {
                 if (Convert.ToBoolean(flagID & irsdk_crossed)
                  || Convert.ToBoolean(flagID & irsdk_disqualify)) return flag("Disqualify flag", CROSSED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellow)) return clearedFlag("Yellow flag", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellowWaving)) return clearedFlag("Yellow Waving flag", WAVING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_black)) return flag("Black flag", INVERTED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_furled)) return clearedFlag("Furled Black flag", FURLED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE, COLOR_BLACK }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_green)) return flag("Green flag", SIMPLE_FLAG, new byte[] { COLOR_GREEN, COLOR_GREEN }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_red)) return flag("Red flag", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_RED }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_blue)) return flag("Blue flag", DIAGONAL_STRIPE_FLAG, new byte[] { COLOR_BLUE, COLOR_YELLOW }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_debris)) return flag("Debris flag", STRIPPED_FLAG, new byte[] { COLOR_YELLOW, COLOR_RED }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_repair)) return flag("Meat Ball flag", MEATBALL_FLAG, new byte[] { COLOR_BLACK, COLOR_ORANGE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_checkered)) return flag("Checkered flag", CHECKERED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_white)) return flag("White flag", SIMPLE_FLAG, new byte[] { COLOR_DIM_WHITE, COLOR_BLACK }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_greenHeld)
                  || Convert.ToBoolean(flagID & irsdk_caution)
                  || Convert.ToBoolean(flagID & irsdk_cautionWaving)) return flag("Caution flag", CAUTION_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_startHidden)) return flag("None", SIMPLE_FLAG, new byte[] { COLOR_BLACK, COLOR_BLACK }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_oneLapToGreen)) return flag("One To Green", INVERTED_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
                 return false;
        }

                                                  // Try to match given flag ID against extra flag constants
                                                  // serving as a starting lights for both standing
                                                  // and rolling starts
        private bool matchStartingFlags(long flagID)
        {
            if (!this.startLightsModuleMenuItem.Checked) return false;

                 if (Convert.ToBoolean(flagID & irsdk_startReady)) return flag("Startlights: Ready!", HALF_FLAG, new byte[] { COLOR_RED, COLOR_BLACK }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_startSet)) return flag("Startlights: Set!", SIMPLE_FLAG, new byte[] { COLOR_RED, COLOR_RED }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_startGo)) return flag("Startlights: Go!", SIMPLE_FLAG, new byte[] { COLOR_GREEN, COLOR_GREEN }, FAST);
            else return false;
        }

                                                  // Ruleset of "flags" or flag signals used for system purposes
                                                  // Returns true if flag matched, false otherwise.
        private bool matchSystemFlags(long flagID)
        {
            if (flagID == NO_FLAG) return flag("---", SIMPLE_FLAG, new byte[] { COLOR_BLACK, COLOR_BLACK }, SLOW);
            else if (flagID == ORIENTATION_CHECK) return flag("Letter \"F\" check!", F_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
            else if (flagID == STARTUP_GREETING) return flag("Let's go!", F_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
            else if (flagID == LUMA_CHECK) return flag("Brightness set.", SIMPLE_FLAG, new byte[] { COLOR_WHITE, COLOR_WHITE }, SLOW);
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

        public bool clearedFlag(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            clearFlag = irsdk_green;
            return flag(flagName, pattern, color, speed);
        }

                                                  // Advances the demo flag picking cycle
        private bool showDemoFlag()
        {
            long[] flags = demoMenuItem.Checked ? demoFlags : noDemoFlags;
            if (demoFlagIndex++ >= flags.Length) demoFlagIndex = 1;
            return showFlag(flags[demoFlagIndex - 1]);
        }

                                                  // Takes a moment to ensure a system flag
                                                  // gets displayed long enough before it gets overruled
                                                  // by timeouts
        private void showSystemFlag(long flag = ORIENTATION_CHECK)
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
                    getSessionDetails();

                    bool onTrack = (bool)sdk.GetData("IsOnTrack");
                    long flag = Convert.ToInt64(sdk.GetData("SessionFlags"));

                    if (onTrack || Convert.ToBoolean(flag & irsdk_checkered))
                    {
                        flagsEveryTick(flag);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FLAG UPDATE FAILED");
                Console.WriteLine(ex);
            }
        }

                                                  // Displays the actual flag signal returning `true` when
                                                  // a change has been picked up. Also clears certain flags
                                                  // such as yellows with a green flag.
        private bool flagsEveryTick(long flag)
        {

            if (flag != irsdk_noFlag)
            {
                return showFlag(flag);
            }
            else
            {
                if (clearFlag != NO_FLAG && !clearTimer.Enabled)
                {
                    clearTimer.Start();
                }
                return showFlag(clearFlag);
            }
        }

        private void clearTimer_Tick(object sender, EventArgs e)
        {
            clearTimer.Stop();
            clearFlag = NO_FLAG;
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

    }
}
