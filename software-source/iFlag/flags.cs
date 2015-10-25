using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        uint flagOnDisplay = 47652875;            // Currently displayed flag; initiated with "random" number

                                                  // Special purpose system-level "flags"
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
            }
            return false;
        }
                                                  // Ruleset of "flags" or flag signals used for system purposes
                                                  // Returns true if flag matched, false otherwise.
        private bool matchSystemFlags(uint flagID)
        {
            if (flagID == NO_FLAG) return flag("", SIMPLE_FLAG, new byte[] { COLOR_BLACK, COLOR_BLACK }, SLOW);
            else if (flagID == ORIENTATION_CHECK) return flag("Letter \"F\" check!", F_FLAG, new byte[] { COLOR_BLACK, COLOR_GREEN }, SLOW);
            else return false;
        }

                                                  // Pour the specified flag into the matrix awaiting boradcast
                                                  // logging the flags with time codes into console
                                                  // (^^ this might be eventually going into a file in the future.)
        public bool flag(string flagName, byte[, ,] pattern, byte[] color, bool speed)
        {
            Console.WriteLine(DateTime.Now + " " + flagName);
            flagToMatrix(flagName, pattern, color, speed);
            return true;
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
                                                  // Flag patterns


        byte[, ,] SIMPLE_FLAG = new byte[,,] { {
          { 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 1, 1, 0, 0, 0 },
          { 0, 0, 0, 1, 1, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0 }
        } };

        byte[, ,] F_FLAG = new byte[,,] { {
          { 1, 1, 1, 1, 1, 1, 1, 1 },
          { 1, 1, 0, 0, 0, 0, 1, 1 },
          { 1, 1, 0, 1, 1, 1, 1, 1 },
          { 1, 1, 0, 0, 0, 0, 1, 1 },
          { 1, 1, 0, 1, 1, 1, 1, 1 },
          { 1, 1, 0, 1, 1, 1, 1, 1 },
          { 1, 1, 0, 1, 1, 1, 1, 1 },
          { 1, 1, 1, 1, 1, 1, 1, 1 }
        } };

    }
}
