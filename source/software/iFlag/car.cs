using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        float[] carsLapPct;
        float carLapPct;
        bool[] carsOnPitRoad;
        int[] carsTrackSurface;
        int carSurface;
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

            carsOnPitRoad = (bool[])sdk.GetData("CarIdxOnPitRoad");
            carsTrackSurface = (int[])sdk.GetData("CarIdxTrackSurface");

            carSurface = carsTrackSurface[carID];

            onPitRoad = (bool)sdk.GetData("OnPitRoad");
            onPitExitRoad = !onPitRoad && carSurface == irsdk_AproachingPits && carLapPct < 0.5;
            onPitEntryRoad = !onPitRoad && carSurface == irsdk_AproachingPits && carLapPct > 0.5;
            inPitStall = (bool)sdk.GetData("PlayerCarInPitStall");

            speed = (float)sdk.GetData("Speed");
        }
    }
}
