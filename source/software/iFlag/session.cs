using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace iFlag
{
    public partial class mainForm : Form
    {
        public bool sessionDetected;
        string sessionInfo;
        string eventType;
        int carID;
        int paceCarID;
        string trackCategory;
        float trackLength;
        float pitSpeedLimit;

        int incidentCount = 0;
        int newIncidentCount = 0;

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

                    Match match, value, units;

                    match = Regex.Match(sessionInfo, @"(?<=SessionID: )\d+");
                    int sessionID = int.Parse(match.Value);
                    Console.WriteLine("SessionID: {0}", sessionID);

                    match = Regex.Match(sessionInfo, @"(?<=SubSessionID: )\d+");
                    int subSessionID = int.Parse(match.Value);
                    Console.WriteLine("SubSessionID: {0}", subSessionID);

                    match = Regex.Match(sessionInfo, @"(?<=DriverCarIdx: )\d+");
                    carID = int.Parse(match.Value);
                    Console.WriteLine("Car ID: {0}", carID);

                    match = Regex.Match(sessionInfo, @"(?<=PaceCarIdx: )[-0-9.]+");
                    paceCarID = int.Parse(match.Value);
                    Console.WriteLine("Pace car ID: {0}", paceCarID);

                    match = Regex.Match(sessionInfo, @"(?<=TrackPitSpeedLimit: )[0-9.]+ (k|m)ph");
                    value = Regex.Match(match.Value, "[0-9.]+");
                    units = Regex.Match(match.Value, "(k|m)ph");
                    pitSpeedLimit = float.Parse(value.Value, CultureInfo.InvariantCulture);
                    if (units.Value == "kph") pitSpeedLimit *= 0.277778F; // convert to m/s
                    if (units.Value == "mph") pitSpeedLimit *= 1.609344F * 0.277778F; // convert to m/s
                    Console.WriteLine("Pit speed limit: {0} ({1} m/s)", match.Value, pitSpeedLimit);

                    match = Regex.Match(sessionInfo, @"(?<=TrackLength: )[0-9.]+ (km|mi)");
                    value = Regex.Match(match.Value, "[0-9.]+");
                    units = Regex.Match(match.Value, "(km|mi)");
                    trackLength = float.Parse(value.Value, CultureInfo.InvariantCulture);
                    if (units.Value == "km") trackLength *= 1000; // convert to m
                    if (units.Value == "mi") trackLength *= 1609.344F; // convert to m
                    Console.WriteLine("Track length: {0} ({1} m)", match.Value, trackLength);

                    match = Regex.Match(sessionInfo, @"(?<=Category: )[a-zA-Z ]+");
                    trackCategory = match.Value;
                    Console.WriteLine("Track category: {0}", trackCategory);

                    match = Regex.Match(sessionInfo, @"(?<=EventType: )[a-zA-Z ]+");
                    eventType = match.Value;
                    Console.WriteLine("Event type: {0}", eventType);

                    CAUTION_FLAG = trackCategory == "Road" ? SAFETYCAR_FLAG : FLASHING_FLAG;

                    incidentCount = (int)sdk.GetData("PlayerCarTeamIncidentCount");
                }
            }
        }
    }
}
