using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        float[] carsLapPct;
        float carLapPct;
        float pitStallLapPct;
        bool pitStallDetected;
        bool[] carsOnPitRoad;
        int[] carsTrackSurface;
        int carSurface;
        float speed;
        float kph = 1 / 3.600F;
        bool onPitRoad;
        bool onPitExitRoad;
        bool onPitEntryRoad;
        bool inPitStall;
        bool hasPitted;

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
            onPitExitRoad = !onPitRoad && hasPitted && carSurface == irsdk_AproachingPits && carLapPct < 0.5;
            onPitEntryRoad = !onPitRoad && carSurface == irsdk_AproachingPits && carLapPct > 0.5;
            inPitStall = (bool)sdk.GetData("PlayerCarInPitStall");

            if (onPitRoad && !hasPitted) hasPitted = true;
            if (hasPitted && !onPitEntryRoad && !onPitRoad && !onPitExitRoad) hasPitted = false;

            if (inPitStall && !pitStallDetected)
            {
                pitStallDetected = true;
                pitStallLapPct = carLapPct;
                Console.WriteLine("Pitstall detected at {0}", pitStallLapPct);
            }

            speed = (float)sdk.GetData("Speed");
        }
    }
}
