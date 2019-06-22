using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        bool flagOnDisplay = false;               // Flag currently on display
        string flagOnDisplayLabel = "";

                                                  // Subset of flags for the demo sequence
        long[] demoFlags = {
            irsdk_green,
            irsdk_checkered,
            irsdk_yellowWaving,
            irsdk_caution,
            irsdk_red,
            irsdk_white,
            irsdk_blue,
            irsdk_black,
            irsdk_debris,
            irsdk_disqualify,
            irsdk_repair,
            irsdk_furled,
        };
        long[] noDemoFlags = {
            NO_FLAG,
        };
        
        int demoFlagIndex;                        // Current demo flag index in a list of flags to cycle through

        private void startFlags()
        {
            CAUTION_FLAG = SAFETYCAR_FLAG;
        }

                                                  // Try to match the given flag ID
                                                  // with one of the flag modules
                                                  // Returns true if matched, false otherwise.
        private bool matchFlags(long flagID)
        {
            return matchSystemFlags(flagID)
                || matchStartingFlags(flagID)
                || matchPitSpeedFlags(flagID)
                || matchRacingFlags(flagID)
                || false;
        }

                                                  // Try to match given flag ID against racing flag constants
        private bool matchRacingFlags(long flagID)
        {
            if (flagID < 0) return false;

                 if (Convert.ToBoolean(flagID & irsdk_crossed)
                  || Convert.ToBoolean(flagID & irsdk_disqualify)) return flag("Disqualified", CROSSED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE, COLOR_WHITE, COLOR_WHITE, COLOR_WHITE }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellow)) return clearedFlag("Careful And No Overtaking!", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_yellowWaving)) return clearedFlag("Careful And No Overtaking!", WAVING_FLAG, new byte[] { COLOR_BLACK, COLOR_YELLOW }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_black)) return flag("Serve Stop And Go Penalty", INVERTED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_furled)) return clearedFlag("Serve Penalty", FURLED_FLAG, new byte[] { COLOR_BLACK, COLOR_WHITE, COLOR_BLACK }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_green)) return flag("Green", SIMPLE_FLAG, new byte[] { COLOR_GREEN, COLOR_GREEN }, FAST);
            else if (Convert.ToBoolean(flagID & irsdk_red)) return flag("Session Stopped", FLASHING_FLAG, new byte[] { COLOR_BLACK, COLOR_RED }, SLOW);
            else if (Convert.ToBoolean(flagID & irsdk_blue)) return flag("Faster Car Approaching", DIAGONAL_STRIPE_FLAG, new byte[] { COLOR_BLUE, COLOR_YELLOW }, SLOW);
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

                                                  // Match against pit speed limit assistant flags
                                                  // shown on pit entry and on the pit lane
        private bool matchPitSpeedFlags(long flagID)
        {
            if (!this.pitSpeedLimitModuleMenuItem.Checked) return false;

                 if (flagID == WAY_ABOVE_PIT_LIMIT) return flag("Pit speed: Way too fast!", TOO_HIGH_FLAG, new byte[] { COLOR_BLACK, COLOR_RED, COLOR_RED }, SLOW);
            else if (flagID == ABOVE_PIT_LIMIT) return flag("Pit speed: Too fast!", TOO_HIGH_FLAG, new byte[] { COLOR_BLACK, COLOR_DIM_RED, COLOR_DIM_RED }, SLOW);
            else if (flagID == HIGH_ON_PIT_LIMIT) return flag("Pit speed: Little over limit", TOO_HIGH_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN, COLOR_BLACK }, SLOW);
            else if (flagID == ON_PIT_LIMIT) return flag("Pit speed: On speed limit", ENOUGH_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN, COLOR_BLACK }, SLOW);
            else if (flagID == LOW_ON_PIT_LIMIT) return flag("Pit speed: Little below limit", TOO_LOW_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN, COLOR_BLACK }, SLOW);
            else if (flagID == BELOW_PIT_LIMIT) return flag("Pit speed: Too slow!", TOO_LOW_FLAG, new byte[] { COLOR_DIM_BLUE, COLOR_DIM_BLUE, COLOR_BLACK }, SLOW);
            else if (flagID == WAY_BELOW_PIT_LIMIT) return flag("Pit speed: Way too slow!", TOO_LOW_FLAG, new byte[] { COLOR_BLUE, COLOR_BLUE, COLOR_BLACK }, SLOW);
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

                                                  // Pour the specified flag into the flag matrix awaiting boradcast
                                                  // logging the flags with time codes into console
                                                  // (^^ this might be eventually going into a file in the future.)
        public bool flag(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            flagOnDisplayLabel = flagName;
            patternToMatrix(ref flagMatrix, pattern, color, speed);
            return true;
        }

                                                  // Same here, only when the flag ends, it will be cleared
                                                  // with green flag
        public bool clearedFlag(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            clearFlag = irsdk_green;
            return flag(flagName, pattern, color, speed);
        }
    }
}
