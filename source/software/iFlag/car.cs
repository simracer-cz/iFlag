using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        float[] carsLapPct;
        float carLapPct;
        bool pitsOpen;
        bool[] carsOnPitRoad;
        int[] carsTrackSurface;
        int carSurface;
        int carLeftRight;
        float speed;
        float kph = 1 / 3.600F;
        bool onPitRoad;
        bool onPitExitRoad;
        bool onPitEntryRoad;
        bool inPitStall;

        private void startCar()
        {
        }

        private void getCarDetails()
        {
            carsLapPct = (float[])sdk.GetData("CarIdxLapDistPct");
            carLapPct = carsLapPct[carID];

            pitsOpen = (bool)sdk.GetData("PitsOpen");
            carsOnPitRoad = (bool[])sdk.GetData("CarIdxOnPitRoad");
            carsTrackSurface = (int[])sdk.GetData("CarIdxTrackSurface");

            carSurface = carsTrackSurface[carID];
            carLeftRight = (int)sdk.GetData("CarLeftRight");

            onPitRoad = (bool)sdk.GetData("OnPitRoad");
            onPitExitRoad = !onPitRoad && carSurface == irsdk_AproachingPits && carLapPct < 0.5;
            onPitEntryRoad = !onPitRoad && carSurface == irsdk_AproachingPits && carLapPct > 0.5;
            inPitStall = (bool)sdk.GetData("PlayerCarInPitStall");

            speed = (float)sdk.GetData("Speed");
        }
    }
}
