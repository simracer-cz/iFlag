using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace iFlag
{
    public partial class mainForm : Form
    {
        bool sessionDetected;
        string sessionInfo;
        int carID;
        int paceCarID;
        string trackCategory;
        float pitSpeedLimit;

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
                sessionInfo = sdk.GetSessionInfo();

                if (sessionInfo != "")
                {
                    sessionDetected = true;

                    Match sessionIDMatch = Regex.Match(sessionInfo, @"(?<=SessionID: )\d+");
                    int sessionID = int.Parse(sessionIDMatch.Value);
                    Console.WriteLine("SessionID: {0}", sessionID);

                    Match subSessionIDMatch = Regex.Match(sessionInfo, @"(?<=SubSessionID: )\d+");
                    int subSessionID = int.Parse(subSessionIDMatch.Value);
                    Console.WriteLine("SubSessionID: {0}", subSessionID);

                    Match carIdxMatch = Regex.Match(sessionInfo, @"(?<=DriverCarIdx: )\d+");
                    carID = int.Parse(carIdxMatch.Value);
                    Console.WriteLine("Car ID: {0}", carID);

                    Match paceCarIdxMatch = Regex.Match(sessionInfo, @"(?<=PaceCarIdx: )[-0-9.]+");
                    paceCarID = int.Parse(paceCarIdxMatch.Value);
                    Console.WriteLine("Pace car ID: {0}", paceCarID);

                    Match match = Regex.Match(sessionInfo, @"(?<=TrackPitSpeedLimit: )[0-9.]+ (k|m)ph");
                    Match speedMatch = Regex.Match(match.Value, "[0-9.]+");
                    Match speedUnits = Regex.Match(match.Value, "(k|m)ph");
                    pitSpeedLimit = float.Parse(speedMatch.Value, CultureInfo.InvariantCulture);
                    if (speedUnits.Value == "kph") pitSpeedLimit *= 0.277778F; // convert to m/s
                    if (speedUnits.Value == "mph") pitSpeedLimit *= 1.609344F * 0.277778F; // convert to m/s
                    Console.WriteLine("Pit speed limit: {0} ({1} m/s)", match.Value, pitSpeedLimit);

                    Match category = Regex.Match(sessionInfo, @"(?<=Category: )(Road|Oval)");
                    trackCategory = category.Value;
                    Console.WriteLine("Track category: {0}", trackCategory);

                    CAUTION_FLAG = trackCategory == "Road" ? SAFETYCAR_FLAG : FLASHING_FLAG;

                }
            }
        }
    }
}
