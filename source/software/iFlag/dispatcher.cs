using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
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

        const int WAY_BELOW_PIT_LIMIT =         -40;
        const int BELOW_PIT_LIMIT =             -41;
        const int LOW_ON_PIT_LIMIT =            -42;
        const int ON_PIT_LIMIT =                -43;
        const int HIGH_ON_PIT_LIMIT =           -44;
        const int ABOVE_PIT_LIMIT =             -45;
        const int WAY_ABOVE_PIT_LIMIT =         -46;

        const int REPAIRS_MANDATORY =           -20;
        const int REPAIRS_OPTIONAL =            -21;
        const int REPAIRS_DONE =                -22;

        long clearFlag = NO_FLAG;

        string pitSpeedMap;                       // Holds currently selected pit speed limit module map

        private void startDispatcher()
        {
        }

                                                  // Accepts a flag identifier number
                                                  // and matches it against a bank of known flags/signals
                                                  // and their eventual overlays
                                                  // Returns true if matched, false otherwise.
        public bool showFlag(long flagID)
        {
            int overlaysMatched = 0;
            bool broadcast = true;

            if (matchFlags(flagID))
            {
                flagOnDisplay = true;
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

            if (!terminating)
            {
                overlaysMatched = matchOverlays();
                if (overlaysMatched != overlaysOnDisplay)
                {
                    overlaysOnDisplay = overlaysMatched;
                }
            }

            if (broadcast)
            {
                broadcastMatrix();
            }
            return broadcast;
        }

                                                  // Advances the demo flag picking cycle
        public bool showDemoFlag()
        {
            long[] flags = demoMenuItem.Checked ? demoFlags : noDemoFlags;
            if (demoFlagIndex++ >= flags.Length) demoFlagIndex = 1;
            return showFlag(flags[demoFlagIndex - 1]);
        }

                                                  // Takes a moment to ensure a system flag
                                                  // gets displayed long enough before it gets overruled
                                                  // by timeouts
        public void showSystemFlag(long flagID = ORIENTATION_CHECK, int durationSec = 2)
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
            if (locationSpecificFlags())
            {
                return false;
            }
            else
            {
                return showFlag(flagID);
            }
        }

        private bool locationSpecificFlags()
        {
            if (inPitStall)
            {
                float repairsLeft = (float)sdk.GetData("PitRepairLeft");
                float optRepairsLeft = (float)sdk.GetData("PitOptRepairLeft");

                if (repairsLeft > 0) return showFlag(REPAIRS_MANDATORY);
                if (repairsLeft == 0 && optRepairsLeft > 0) return showFlag(REPAIRS_OPTIONAL);
                if (repairsLeft == 0 && optRepairsLeft == 0) return showFlag(REPAIRS_DONE);
            }
            if (onPitEntryRoad || onPitRoad)
            {
                speed = (float)sdk.GetData("Speed");
                if (speed > 1)
                {
                    double[] map;

                         if (pitSpeedMap == "aggressive") map = new double[]{ -1.0, -0.5,  0.0, 1.0, 1.5, 2.0 };
                    else if (pitSpeedMap == "narrow")     map = new double[]{ -1.5, -1.0, -0.5, 0.5, 1.0, 1.5 };
                    else if (pitSpeedMap == "wide")       map = new double[]{ -2.0, -1.5, -1.0, 1.0, 1.5, 2.0 };
                    else                                  map = new double[]{ -2.0, -1.5, -1.0, 0.0, 0.5, 1.0 }; // "safe"

                    float speedDelta = speed - pitSpeedLimit;

                    if (speedDelta < map[0] * kph) return showFlag(WAY_BELOW_PIT_LIMIT);
                    if (speedDelta > map[5] * kph) return showFlag(WAY_ABOVE_PIT_LIMIT);
                    if (speedDelta < map[1] * kph) return showFlag(BELOW_PIT_LIMIT);
                    if (speedDelta > map[4] * kph) return showFlag(ABOVE_PIT_LIMIT);
                    if (speedDelta < map[2] * kph) return showFlag(LOW_ON_PIT_LIMIT);
                    if (speedDelta > map[3] * kph) return showFlag(HIGH_ON_PIT_LIMIT);
                    if (speedDelta < map[3] * kph) return showFlag(ON_PIT_LIMIT);
                }
            }
            if (onPitExitRoad)
            {
                                                  // Shows blue flag warning when leaving pits with a car
                                                  // on track within 20 meters behind
                if (this.pitExitBlueModuleMenuItem.Checked)
                {
                    float distance;
                    float range = (1 / trackLength * 100); // 100 meters in track percent

                    for (int idx = 0; idx < 63; idx++)
                    if (idx != carID && idx != paceCarID)
                    {
                        distance = carLapPct - carsLapPct[idx];

                        if (distance > 0 && distance < range && carsTrackSurface[idx] == irsdk_OnTrack)
                        {
                            return showFlag(irsdk_blue);
                        }
                    }
                }
            }
            return false;
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
