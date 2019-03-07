using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        bool flagOnDisplay = false;               // Flag currently on display
        int overlaysOnDisplay = 0;

        string flagOnDisplayLabel = "";
        string overlaysOnDisplayLabel = "";

        int demoFlagIndex;                        // Current demo flag index in a list of flags to cycle through

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
        const long STARTUP_GREETING =           -90;
        const long ORIENTATION_CHECK =          -92;
        const long LUMA_CHECK =                 -93;
        
        const long NO_FLAG =                    -100;

        const int SPOTTER_OFF =                 irsdk_LROff;
        const int SPOTTER_CLEAR =               irsdk_LRClear;
        const int SPOTTER_CAR_LEFT =            irsdk_LRCarLeft;
        const int SPOTTER_CAR_RIGHT =           irsdk_LRCarRight;
        const int SPOTTER_CARS_LEFT_RIGHT =     irsdk_LRCarLeftRight;
        const int SPOTTER_CARS_LEFT =           irsdk_LR2CarsLeft;
        const int SPOTTER_CARS_RIGHT =          irsdk_LR2CarsRight;

                                                  // Overlays bit field constants
        const int if_spotterOverlay = 16;

        long clearFlag = NO_FLAG;

        private void startFlags()
        {
            CAUTION_FLAG = SAFETYCAR_FLAG;
        }

                                                  // Accepts a flag identifier number
                                                  // and matches it against a bank of known flags/signals
                                                  // and their eventual overlays
                                                  // Returns true if matched, false otherwise.
        private bool showFlag(long flagID)
        {
            int overlaysMatched = 0;
            bool broadcast = true;

            if (matchFlags(flagID))
            {
                flagOnDisplay = true;
                flagLabel.Text = flagOnDisplayLabel;
            }
            else if (clearFlag != NO_FLAG)
            {
                flagOnDisplay = false;
                broadcast = false;
                if (!clearTimer.Enabled)
                {
                    clearTimer.Start();
                    return showFlag(clearFlag);
                }
            }
            else
            {
                flagOnDisplay = false;
                matchSystemFlags(NO_FLAG);
            }

            overlaysOnDisplayLabel = "";
            overlaysMatched = matchOverlays();
            if (overlaysMatched != overlaysOnDisplay)
            {
                overlaysOnDisplay = overlaysMatched;
            }

            if (broadcast)
            {
                broadcastMatrix();
            }
            return broadcast;
        }

                                                  // Try to match the given flag ID
                                                  // with one of the flag modules
                                                  // Returns true if matched, false otherwise.
        private bool matchFlags(long flagID)
        {
            return matchSystemFlags(flagID)
                || matchStartingFlags(flagID)
                || matchRacingFlags(flagID)
                || false;
        }

                                                  // Try to match given flag ID against racing flag constants
        private bool matchRacingFlags(long flagID)
        {
            if (flagID < 0) return false;

                 if (Convert.ToBoolean(flagID & irsdk_crossed)
                  || Convert.ToBoolean(flagID & irsdk_disqualify)) return flag("Disqualified", CROSSED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellow)) return clearedFlag("Careful And No Overtaking!", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellowWaving)) return clearedFlag("Careful And No Overtaking!", WAVING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_black)) return flag("Serve Stop And Go Penalty", INVERTED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_furled)) return clearedFlag("Serve Penalty", FURLED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE, COLOR_BLACK }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_green)) return flag("Green", SIMPLE_FLAG, new byte[] { COLOR_GREEN, COLOR_GREEN }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_red)) return flag("Session Stopped", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_RED }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_blue)) return flag("Traffic Overtake Coming Up", DIAGONAL_STRIPE_FLAG, new byte[] { COLOR_BLUE, COLOR_YELLOW }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_debris)) return flag("Debris on the Track", STRIPPED_FLAG, new byte[] { COLOR_YELLOW, COLOR_RED }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_repair)) return flag("Too Much Damage", MEATBALL_FLAG, new byte[] { COLOR_BLACK, COLOR_ORANGE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_checkered)) return flag("Checkered Flag", CHECKERED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_white)) return flag("Last Lap", SIMPLE_FLAG, new byte[] { COLOR_DIM_WHITE, COLOR_BLACK }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_greenHeld)
                  || Convert.ToBoolean(flagID & irsdk_caution)) return flag("Full Course Caution", CAUTION_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_cautionWaving)) return flag("Full Course Caution", CAUTION_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW, COLOR_WHITE }, SLOW);
            // else if (Convert.ToBoolean(flagID & irsdk_startHidden)) return flag("None", SIMPLE_FLAG, new byte[] { COLOR_BLACK, COLOR_BLACK }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_oneLapToGreen)
                  && eventType == "Race") return flag("One Lap To Green", INVERTED_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
            else return false;
        }

                                                  // Try to match given flag ID against extra flag constants
                                                  // serving as a starting lights for both standing
                                                  // and rolling starts
        private bool matchStartingFlags(long flagID)
        {
            if (!this.startLightsModuleMenuItem.Checked) return false;
            if (flagID < 0) return false;

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

                                                  // Try to match the overlay modules
                                                  // Returns bit field of matches.
        private int matchOverlays()
        {
            if (!sdk.IsConnected()) return 0;

            return 0
            + matchSpotterOverlay()
            ;
        }

                                                  // Try to match signals from the spotter
        private int matchSpotterOverlay()
        {
            if (!this.spotterOverlayModuleMenuItem.Checked) return 0;

            int carLeftRight = (int)sdk.GetData("CarLeftRight");

                 if (carLeftRight == SPOTTER_CAR_LEFT || carLeftRight == SPOTTER_CARS_LEFT) return overlay(if_spotterOverlay, "Left side!", WARN_L_OVERLAY, new byte[] { COLOR_BLACK, COLOR_PURPLE });
            else if (carLeftRight == SPOTTER_CAR_RIGHT || carLeftRight == SPOTTER_CARS_RIGHT) return overlay(if_spotterOverlay, "Right side!", WARN_R_OVERLAY, new byte[] { COLOR_BLACK, COLOR_PURPLE });
            else if (carLeftRight == SPOTTER_CARS_LEFT_RIGHT) return overlay(if_spotterOverlay, "Both sides!", WARN_LR_OVERLAY, new byte[] { COLOR_BLACK, COLOR_PURPLE });
            else return 0;
        }

                                                  // Pour the specified flag into the matrix awaiting boradcast
                                                  // logging the flags with time codes into console
                                                  // (^^ this might be eventually going into a file in the future.)
        public bool flag(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            flagOnDisplayLabel = flagName;
            patternToMatrix(ref flagMatrix, pattern, color, speed);
            return true;
        }

        public bool clearedFlag(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            clearFlag = irsdk_green;
            return flag(flagName, pattern, color, speed);
        }

                                                  // Pour the specified overlay into the overlay matrix awaiting boradcast
                                                  // logging the flags with time codes into console
                                                  // (^^ this might be eventually going into a file in the future.)
        public int overlay(int overlayID, string overlayName, byte[, ,] pattern, byte[] color)
        {
            overlaysOnDisplayLabel += String.Format(" +{0}", overlayName);
            patternToMatrix(ref overlayMatrix, pattern, color);
            return overlayID;
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
        private void showSystemFlag(long flagID = ORIENTATION_CHECK, int durationSec = 2)
        {
            if (demoTimer.Enabled)
            {
                demoTimer.Stop();
                demoTimer.Start();
            }
            if (updateTimer.Enabled)
            {
                updateTimer.Stop();
            }
            showFlag(flagID);

            if (durationSec > 0)
            {
                durationTimer.Stop();
                durationTimer.Interval = durationSec * 1000;
                durationTimer.Start();
            }
            else
            {
                updateTimer.Start();
            }
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
                    getCarDetails();

                    long flagID = (long)Convert.ToInt64(sdk.GetData("SessionFlags"));

                    resetOverlay();
                    flagsEveryTick(flagID);
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
        private bool flagsEveryTick(long flagID)
        {
            return showFlag(flagID);
        }

        private void clearTimer_Tick(object sender, EventArgs e)
        {
            clearTimer.Stop();
            clearFlag = NO_FLAG;
        }

        private void durationTimer_Tick(object sender, EventArgs e)
        {
            durationTimer.Stop();
            showFlag(NO_FLAG);
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
