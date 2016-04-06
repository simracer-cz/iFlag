using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace iFlag
{
    public partial class mainForm : Form
    {
        bool sessionDetected;
        int carID;

        private void startSession()
        {
        }

                                                  // Once per session reads the session info string
                                                  // and parses it for values of interest storing them
                                                  // into dedicated variables.
        private void getSessionDetails()
        {
            if (!sessionDetected)
            {
                string sessionInfo = sdk.GetSessionInfo();

                if (sessionInfo != "")
                {
                    sessionDetected = true;

                    Match carIdxMatch = Regex.Match(sessionInfo, @"(?<=DriverCarIdx: )\d+");
                    carID = int.Parse(carIdxMatch.Value);
                    Console.WriteLine("Car ID: {0}", carID);
                }
            }
        }
    }
}
