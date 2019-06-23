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
        float toPitStallMtr;

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

            float flip = 0;
            if (pitStallLapPct - carLapPct > 0.5) flip = -1.0F;
            else if (pitStallLapPct - carLapPct < -0.5) flip = 1.0F;
            toPitStallMtr = (flip + pitStallLapPct - carLapPct) * trackLength;

            onPitRoad = (bool)sdk.GetData("OnPitRoad");
            onPitExitRoad = !onPitRoad && hasPitted && carSurface == irsdk_AproachingPits && toPitStallMtr < 0;
            onPitEntryRoad = !onPitRoad && carSurface == irsdk_AproachingPits && toPitStallMtr > 0;
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
